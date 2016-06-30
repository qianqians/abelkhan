/*
 *  qianqians
 *  2014-10-5
 */
#include "../../third_party/json/json_protocol.h"
#include "../../remoteq/remote_queue.h"

int main(){
	Fossilizid::remoteq::QUEUE que = Fossilizid::remoteq::queue();
	Fossilizid::remoteq::ENDPOINT ep = Fossilizid::remoteq::endpoint("127.0.0.1", 4567);
	Fossilizid::remoteq::ACCEPTOR acceptor = Fossilizid::remoteq::reliable::acceptor(que, ep);

	while (1){
		Fossilizid::remoteq::EVENT ev = Fossilizid::remoteq::queue(que);

		if (ev.type == Fossilizid::remoteq::event_type_reliable_accept){
			Fossilizid::remoteq::CHANNEL ch = Fossilizid::remoteq::reliable::accept(ev.handle.acp);
			
			Json::Value value;
			value["test"] = "ok";
			Fossilizid::remoteq::reliable::push(ch, value, Fossilizid::json_parser::json_to_buf);
		}
		else if (ev.type == Fossilizid::remoteq::event_type_reliable_recv){
			Json::Value ret;
			if (Fossilizid::remoteq::reliable::pop(ev.handle.ch, ret, Fossilizid::json_parser::buf_to_json)){
				printf("ret=%s\n", ret["ret"].asString().c_str());
				break;
			}
		}

		Sleep(1);
	}

	return 0;
}