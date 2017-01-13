/*
 * client.h
 *
 *  Created on: 2016-10-6
 *      Author: qianqians
 */
#ifndef _client_h
#define _client_h

#include <string>
#include <memory>

#include <boost/signals2.hpp>
#include <boost/any.hpp>
#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/thread.hpp>

#include "gate_call_clientmodule.h"
#include "client_call_gatecaller.h"
#include "connectnetworkservice.h"
#include "juggleservice.h"
#include "service.h"
#include "timerservice.h"
#include "config.h"
#include "JsonParser.h"
#include "module.h"
#include "modulemanager.h"

namespace client
{

class client
{
public:
    client()
    {
        uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
        
        int64_t tick = clock();
        int64_t tickcount = 0;
        timer = std::make_shared<service::timerservice>(tick);
        
        modules = std::make_shared<common::modulemanager>();
        
        _process = std::make_shared<juggle::process>();
        _gate_call_client = std::make_shared<module::gate_call_client>();
        _gate_call_client->sigack_connect_serverhandle.connect(std::bind(&client::on_ack_connect_server, this, std::placeholders::_1));
        _gate_call_client->sigcall_clienthandle.connect(std::bind(&client::on_call_client, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        _process->reg_module(_gate_call_client);
        
        _conn = std::make_shared<service::connectnetworkservice>(_process);
        
        _juggleservice = std::make_shared<service::juggleservice>();
        _juggleservice->add_process(_process);
    }
    
    void heartbeats(int64_t tick)
    {
        _client_call_gate->heartbeats(tick);
        
        timer->addticktime(tick + 30 * 1000, std::bind(&client::heartbeats, this, std::placeholders::_1));
    }
    
    void on_ack_connect_server(std::string result)
    {
        onConnectServerHandle(result);
    }
    
    void on_call_client(std::string module_name, std::string func_name, std::shared_ptr<std::vector<boost::any> > argvs)
    {
        modules->process_module_mothed(module_name, func_name, argvs);
    }
    
    bool connect_server(std::string ip, short port, int64_t tick)
    {
        try
        {
            auto ch = _conn->connect(ip, port);
            _client_call_gate = std::make_shared<caller::client_call_gate>(ch);
            _client_call_gate->connect_server(uuid, tick);
            
            timer->addticktime(timer->ticktime + 30*1000, std::bind(&client::heartbeats, this, std::placeholders::_1));
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }
    
    void cancle_server()
    {
        _client_call_gate->cancle_server();
    }
    
    void call_logic(std::string module_name, std::string func_name, std::shared_ptr<std::vector<boost::any> > _argvs)
    {
        _client_call_gate->forward_client_call_logic(module_name, func_name, _argvs);
    }
    
    void poll(int64_t tick)
    {
        _juggleservice->poll(tick);
        timer->poll(tick);
    }
    
public:
    std::string uuid;
    std::shared_ptr<common::modulemanager> modules;
    std::shared_ptr<service::timerservice> timer;
    
    boost::signals2::signal<void(std::string)> onConnectServerHandle;

private:
    std::shared_ptr<service::connectnetworkservice> _conn;
    std::shared_ptr<juggle::process> _process;
    std::shared_ptr<service::juggleservice> _juggleservice;
    std::shared_ptr<module::gate_call_client> _gate_call_client;
    std::shared_ptr<caller::client_call_gate> _client_call_gate;

};

}


#endif //_client_h
