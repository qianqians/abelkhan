/*
 * qianqians
 * 2016/7/4
 * process.h
 */
#ifndef _process_h
#define _process_h

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


	void poll(uint64_t tick){
		for (auto ch : event_set) {
			while (true) {
				auto _event = ch->pop();

				if (_event == nullptr) {
					break;
				}

				auto module_name = std::get<0>(_event.as<std::tuple<std::string, std::string, msgpack::object> >());

				auto module = module_set.find(module_name);
				if (module != module_set.end()){
					module->second->process_event(ch, _event);
				}
				else {
					std::cout << "do not have a module named:" << module_name << std::endl;
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