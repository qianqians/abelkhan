/*
 * qianqians
 * 2016/7/4
 * channel.h
 */
#ifndef _channel_h
#define _channel_h

#include <sstream>

#include <boost/asio.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/shared_array.hpp>
#include <boost/function.hpp>
#include <boost/bind.hpp>
#include <boost/signals2.hpp>

#include <container/optimisticque.h>

#include <JsonParser.h>

#include <Ichannel.h>

namespace service {

class channel : public juggle::Ichannel, public boost::enable_shared_from_this<channel> {
public:
	channel(boost::shared_ptr<boost::asio::ip::tcp::socket> _s) {
		s = _s;

		buflen = 8 * 1024;
		buff = new char[buflen];

		tmpbuff = new char[buflen];
		tmpbufflen = buflen;
		tmpbufoffset = 0;

		s->async_receive(
			boost::asio::buffer(buff, buflen), 
			boost::bind(&channel::onRead, this, 
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}

	~channel() {
	}

	boost::signals2::signal<void(boost::shared_ptr<channel>)> sigdisconn;

	void onRead(const boost::system::error_code & error, size_t bytes_transferred) {
		if (!error){
			while (tmpbufflen < (tmpbufoffset + bytes_transferred)) {
				tmpbufflen = 2 * tmpbufflen;
				char * tmp = new char[tmpbufflen];
				memcpy_s(tmp, tmpbufflen, tmpbuff, tmpbufoffset);
				delete[] tmpbuff;
				tmpbuff = tmp;
			}
			memcpy_s(&tmpbuff[tmpbufoffset], tmpbufflen - tmpbufoffset, buff, bytes_transferred);
			tmpbufoffset += bytes_transferred;

			int offset = 0;
			do
			{
				size_t len = ((int)tmpbuff[offset + 0]) | ((int)tmpbuff[offset + 1]) << 8 | ((int)tmpbuff[offset + 2]) << 16 | ((int)tmpbuff[offset + 3]) << 24;

				if (len <= (tmpbufoffset - 4))
				{
					tmpbufoffset -= len + 4;
					offset += 4;
							
					boost::any object;
					Fossilizid::JsonParse::unpacker(object, std::string(&tmpbuff[offset], &tmpbuff[offset + len]));
					boost::shared_ptr<std::vector<boost::any> > o = boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >(object);
							
					que.push(o);

					offset += len;
				}
				else
				{
					memcpy_s(tmpbuff, tmpbufflen, &tmpbuff[offset], tmpbufoffset);

					break;
				}

				if (tmpbufoffset == 0) {
					break;
				}

			} while (true);

			s->async_receive(
				boost::asio::buffer(buff, buflen),
				boost::bind(&channel::onRead, this,
					boost::asio::placeholders::error,
					boost::asio::placeholders::bytes_transferred));
		}
		else {
			sigdisconn(shared_from_this());
		}
	}
	
	bool pop(boost::shared_ptr<std::vector<boost::any> >  & data) {
		return que.pop(data);
	}
	
	void push(boost::shared_ptr<std::vector<boost::any> > data) {
		auto buf = Fossilizid::JsonParse::pack(data);

		size_t len = buf.size();
		char * _data = new char[len + 4];
		_data[0] = len & 0xff;
		_data[1] = len >> 8 & 0xff;
		_data[2] = len >> 16 & 0xff;
		_data[3] = len >> 24 & 0xff;
		memcpy_s(&_data[4], len, buf.c_str(), buf.size());
		size_t datasize = len + 4;

		int offset = s->send(boost::asio::buffer(_data, datasize));
		while (offset < buf.size()) {
			offset += s->send(boost::asio::buffer(&_data[offset], datasize - offset));
		}

		delete[] _data;
	}

private:
	boost::shared_ptr<boost::asio::ip::tcp::socket> s;
	char * buff;
	size_t buflen;
	char * tmpbuff;
	size_t tmpbufflen;
	size_t tmpbufoffset;

	Fossilizid::container::optimisticque<boost::shared_ptr<std::vector<boost::any> > > que;

};

}

#endif // !_channel_h