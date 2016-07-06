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
#include <boost/function.hpp>
#include <boost/bind.hpp>
#include <boost/signals2.hpp>

#include <container/optimisticque.h>

#include <msgpack.hpp>

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
						
					que.push(std::string(&tmpbuff[offset], len));

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
	
	bool pop(std::string & data) {
		return que.pop(data);
	}
	
	void senddata(char * data, int datasize) {
		int offset = s->send(boost::asio::buffer(data, datasize));
		while (offset < datasize) {
			offset += s->send(boost::asio::buffer(&data[offset], datasize - offset));
		}
	}

private:
	boost::shared_ptr<boost::asio::ip::tcp::socket> s;
	char * buff;
	size_t buflen;
	char * tmpbuff;
	size_t tmpbufflen;
	size_t tmpbufoffset;

	Fossilizid::container::optimisticque<std::string> que;

};

}

#endif // !_channel_h