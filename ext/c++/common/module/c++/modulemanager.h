/*
 * modulemanager
 *
 *  Created on: 2016-10-6
 *      Author: qianqians
 */
#ifndef _modulemanager_h
#define _modulemanager_h

#include <map>
#include <memory>
#include <string>

#include <boost/any.hpp>

#include "module.h"

namespace common
{

class modulemanager
{
public:
    modulemanager()
    {
    }
        
    void add_module(std::string module_name, std::shared_ptr<imodule> _module)
    {
        modules.insert(std::make_pair(module_name, _module));
    }
        
    void process_module_mothed(std::string module_name, std::string func_name, std::shared_ptr<std::vector<boost::any> > argvs)
    {
        if (modules.find(module_name) != modules.end())
        {
            std::shared_ptr<imodule> _module = modules[module_name];
            
            _module->process_method(func_name, argvs);
        }
        else
        {
            std::cout << "do not have a module named " << module_name << std::endl;
        }
    }
    
private:
    std::map<std::string, std::shared_ptr<imodule> > modules;
    
};
    
}

#endif //_client_h
