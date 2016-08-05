/*
 * small_hash_map.h
 *  Created on: 2013-1-22
 *      Author: qianqians
 * small_hash_map
 */
#ifndef _small_hash_map_H
#define _small_hash_map_H

#include <string>
#include <map>

#include <functional>
#include <atomic>
#include <shared_mutex>

namespace Fossilizid{
namespace container{

#define mask 1023
#define rehashmask 8191

#define upgradlock 65536 

template <typename K, typename V, typename _Allocator = std::allocator<V> >
class small_hash_map {
private:
	struct node {
		V value;
		std::shared_mutex _mu;
	};
		
	typedef typename _Allocator::template rebind<std::pair<K, node> >::other _map_node_alloc;

	typedef std::pair<K, node*> value_type;
	typedef std::map<K, node*, std::less<K>, _map_node_alloc> _map;
	
	struct bucket {
		std::atomic<_map * > _hash_bucket;
		std::shared_mutex _mu;
	};

	typedef typename _Allocator::template rebind<_map>::other _map_alloc_;
	typedef typename _Allocator::template rebind<node>::other _node_alloc_;
	typedef typename _Allocator::template rebind<bucket>::other _bucket_alloc_;

public:
	small_hash_map(){
		for(int i = 0; i < mask; i++){
			_hash_array[i]._hash_bucket.store(0);
		}
	}

	~small_hash_map(){
		for(unsigned int i = 0; i < mask; i++){
			_map * _map_ = _hash_array[i]._hash_bucket.load();
			if (_map_ != 0){
				put_map(_map_);
			}
		}
	}

	/*
	 * iterates the map
	 */
	void for_each(std::function<void(V var) > handle ){
		for(int i = 0; i < mask; i++){
			std::shared_lock<std::shared_mutex> shared_lock(_hash_array[i]._mu);
			_map * _map_ = (_map *)_hash_array[i]._hash_bucket.load();
			if (_map_ != 0){
				for(_map::iterator iter = _map_->begin(); iter != _map_->end(); iter++){
					std::shared_lock<std::shared_mutex> lock(iter->second->_mu);
					handle(iter->second->value); 
				}
			}
		}
	}

	/*
	 * set a element (key == key) value = value
	 */
	bool set(K key, V value){
		unsigned int hash_value = hash(key, mask);
		bucket * _bucket = (bucket *)&_hash_array[hash_value];
		
		std::shared_lock<std::shared_mutex> lock(_bucket->_mu);
		
		_map * _map_ = (_map *)_bucket->_hash_bucket.load();
		_map::iterator iter = _map_->find(key);
		if (iter == _map_->end()){
			return false;
		}
		
		std::unique_lock<std::shared_mutex> unique_lock(iter->second->_mu);
		
		iter->second->value = value;

		return true;
	}
		
	/*
	 * insert a element to hash_map if key not exist return false
	 */
	void insert(K key, V value){
		unsigned int hash_value = hash(key, mask);
		bucket * _bucket = (bucket *)&_hash_array[hash_value];
		
		std::upgrade_lock<std::shared_mutex> lock(_bucket->_mu);

		_map * _map_ = (_map *)_bucket->_hash_bucket.load();
		if (_map_ == 0){
			_map_ = (_map *)_bucket->_hash_bucket.load();

			std::unique_lock<std::shared_mutex> unique_lock(std::move(lock));
			if (_map_ == 0){
				_map_ = get_map();
				_bucket->_hash_bucket.store(_map_);
			}
		}
			
		_map::iterator iter = _map_->find(key);
		if (iter != _map_->end()){
			std::unique_lock<std::shared_mutex> unique_lock(iter->second->_mu);
			iter->second->value = value;
		}else{
			std::unique_lock<std::shared_mutex> unique_lock(std::move(lock));

			node * _node = get_node();
			_node->value = value;
			_map_->insert(std::make_pair(key, _node));
		}

		_size++;
	}

	/*
	 * find a element for key == key from hash_map if key not exist return false
	 */
	bool search(K key, V &value){
		unsigned int hash_value = hash(key, mask);
		bucket * _bucket = (bucket *)&_hash_array[hash_value];
		
		std::shared_lock<std::shared_mutex> lock(_bucket->_mu);
		
		_map * _map_ = (_map *)_bucket->_hash_bucket.load();
		if (_map_ == 0){
			return false;
		}

		_map::iterator iter = _map_->find(key);
		if (iter == _map_->end()){
			return false;
		}
		
		std::shared_lock<std::shared_mutex> shared_lock(iter->second->_mu);
		value = iter->second->value;

		return true;
	}

	/*
	 * delete a element from hash_map if key not exist return false
	 */
	bool erase(K key){
		unsigned int hash_value = hash(key, mask);
		bucket * _bucket = (bucket *)&_hash_array[hash_value];
		
		std::upgrade_lock<std::shared_mutex> lock(_bucket->_mu);
		
		_map * _map_ = (_map *)_bucket->_hash_bucket.load();
		if (_map_ == 0){
			return false;
		}

		_map::iterator iter = _map_->find(key);
		if (iter == _map_->end()){
			return false;
		}
		
		std::unique_lock<std::shared_mutex> unique_lock(std::move(lock));

		_map_->erase(iter);

		_size--;

		return true;
	}

	/*
	 * get hash_map size
	 */
	unsigned int size(){
		return _size.load();
	}

private:
	unsigned int hash(char * skey, unsigned int mod){
		unsigned int hash = 5831;
		unsigned int slen = strlen(skey);
		for(unsigned int i = 0; i < slen; i++){
			hash += (hash<<5) + hash + *skey++;
		}

		return hash%mod;
	}

	unsigned int hash(std::string & strkey, unsigned int mod){
		unsigned int hash = 5831;
		for(unsigned int i = 0; i < strkey.size(); i++){
			hash += (hash<<5) + hash + (unsigned int)strkey.at(i);
		}

		return hash%mod;
	}

	unsigned int hash(wchar_t * wskey, unsigned int mod){
		unsigned int hash = 5831;
		unsigned int slen = wcslen(wskey);
		for(unsigned int i = 0; i < slen; i++){
			hash += (hash<<5) + hash + (unsigned int)*wskey++;
		}

		return hash%mod;
	}

	unsigned int hash(std::wstring & wstrkey, unsigned int mod){
		unsigned int hash = 5831;
		for(unsigned int i = 0; i < wstrkey.size(); i++){
			hash += (hash<<5) + hash + (unsigned int)wstrkey.at(i);
		}

		return hash%mod;
	}

	unsigned int hash(int32_t key, int mod){
		key += ~(key<<15);
		key ^= key>>10;
		key += (key<<3);
		key ^= key>>6;
		key += ~(key<<11);
		key ^= key>>16;

		return key%mod;
	}

	unsigned int hash(uint32_t key, int mod){
		key += ~(key<<15);
		key ^= key>>10;
		key += (key<<3);
		key ^= key>>6;
		key += ~(key<<11);
		key ^= key>>16;

		return key%mod;
	}

	unsigned int hash(int64_t key, int mod){
		return key%mod;
	}

	unsigned int hash(uint64_t key, int mod){
		return key%mod;
	}

private:	
	node * get_node(){
		node * _node = _node_alloc.allocate(1);
		::new (_node) node();

		return _node;
	}

	void put_node(node * _node_){
		_node_alloc.destroy(_node_);
		_node_alloc.deallocate(_node_, 1);
	}

	_map * get_map(){
		_map * _map_ = _map_alloc.allocate(1);
		::new (_map_) _map();

		return _map_;
	}

	void put_map(_map * _map_){
		_map_->~_map();
		_map_alloc.deallocate(_map_, 1);
	}

	bucket * get_bucket(unsigned int count){
		bucket * _bucket = _bucket_alloc.allocate(count);
		bucket * _tmpbucket = _bucket;
		for (int i = 0; i < count; i++)
		{
			::new (_tmpbucket++) bucket();
		}

		return _bucket;
	}

	void put_bucket(bucket * _bucket, unsigned int count){
		bucket * _tmpbucket = _bucket;
		for (unsigned int i = 0; i < count; i++)
		{
			_bucket_alloc.destroy(_tmpbucket++);
		}
		_bucket_alloc.deallocate(_bucket, count);
	}

private:
	bucket _hash_array[mask];
	std::atomic_uint64_t _size;

	_node_alloc_ _node_alloc;
	_map_alloc_ _map_alloc;
	_bucket_alloc_ _bucket_alloc;

};	
	
} //container
} //Fossilizid

#endif //_small_hash_map_H