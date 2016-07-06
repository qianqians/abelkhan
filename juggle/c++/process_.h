/*
 * qianqians
 * 2016/7/4
 * process.h
 */
#ifndef _process_h
#define _process_h

#include <tuple>
#include <unordered_map>
#include <list>

#include "Imodule.h"
#include "Ichannel.h"

namespace juggle {

class process {
public:
	process() {
	}

	void reg_channel(boost::shared_ptr<Ichannel> ch){
		event_set.push_back(ch);
	}

	void unreg_channel(boost::shared_ptr<Ichannel> ch)
	{
		event_set.remove(ch);
	}

	void reg_module(boost::shared_ptr<Imodule> module)
	{
		module_set.insert(std::make_pair(module->module_name, module));
	}

	void unreg_module(boost::shared_ptr<Imodule> module)
	{
		module_set.erase(module->module_name);
	}


	void poll(){
		for (auto ch : event_set) {
			while (true) {
				std::string buff;
				if (ch->pop(buff)) {
					msgpack::object_handle oh = msgpack::unpack(buff.c_str(), buff.size());
					msgpack::object o = oh.get();

					auto module_name = std::get<0>(o.as<std::tuple<std::string, std::string, msgpack::object> >());

					auto module = module_set.find(module_name);
					if (module != module_set.end()) {
						module->second->process_event(ch, o);
					}
					else {
						std::cout << "do not have a module named:" << module_name << std::endl;
					}
				}
				else {
					break;
				}
			}
		}
	}

private:
	std::list<boost::shared_ptr<Ichannel> > event_set;
	std::unordered_map<std::string, boost::shared_ptr<Imodule> > module_set;

};

}

#endif // !_process_h