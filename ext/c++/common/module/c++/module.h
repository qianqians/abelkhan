/*
 * module
 *
 *  Created on: 2016-10-6
 *      Author: qianqians
 */
#ifndef _module_h
#define _module_h

#include <map>
#include <string>
#include <functional>
#include <vector>

#include <boost/any.hpp>

namespace common
{

class imodule
{
public:
    imodule(){
    }
    
    void reg_func(std::string func_name, std::function<void(std::shared_ptr<std::vector<boost::any> > ) > func)
    {
        maps.insert(std::make_pair(func_name, func));
    }
    
    void process_method(std::string func_name, std::shared_ptr<std::vector<boost::any> > argvs)
    {
        if (maps.find(func_name) != maps.end()){
            maps[func_name](argvs);
        }
    }
    
private:
    std::map<std::string, std::function<void(std::shared_ptr<std::vector<boost::any> > ) > maps;
    
    
};

#pragma waining(disable:4503)

}


#endif //_client_h
