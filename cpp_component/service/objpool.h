/*
 * objpool.h
 *
 *  Created on: 2022-6-2
 *      Author: qianqians
 */
#ifndef _objpool_h
#define _objpool_h

#include <memory>
#include <ringque.h>

namespace service {

template<class T>
class objpool {
public:
	void recycle(std::shared_ptr<T> obj) {
		_pool.push(obj);
	}

	template<typename... Args>
	std::shared_ptr<T> make_obj(Args... p) {
		std::shared_ptr<T> _obj;
		if (_pool.pop(_obj)) {
			_obj->~T();
			new (_obj.get()) T(p...);
			new (&_obj) std::shared_ptr<T>(_obj.get());
			return _obj;
		}

		return std::make_shared<T>(p...);
	}

private:
	concurrent::ringque<std::shared_ptr<T> > _pool;
};

}

#endif //_objpool_h
