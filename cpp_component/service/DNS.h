/*
 * DNS.h
 *
 *  Created on: 2022-1-10
 *      Author: qianqians
 */
#ifndef _DNS_h
#define _DNS_h

#include <string>
#include <ares.h>

#include <spdlog/spdlog.h>

namespace service {

#define IP_LEN 32

struct IpList {
    char host[64] = {0};
    char ip[10][IP_LEN] = {0};
    int count = 0;
};

inline void dns_callback(void* arg, int status, int timeouts, struct hostent* hptr)  //ares  ������ɣ�����DNS��������Ϣ
{
    IpList* ips = (IpList*)arg;
    if (ips == NULL) return;
    if (status == ARES_SUCCESS) {
        strncpy(ips->host, hptr->h_name, sizeof(ips->host));
        spdlog::trace("h_name:{0}", hptr->h_name);
        char** pptr = hptr->h_addr_list;
        for (int i = 0; *pptr != NULL && i < 10; pptr++, ++i) {
            inet_ntop(hptr->h_addrtype, *pptr, ips->ip[ips->count++], IP_LEN);
            spdlog::trace("ip:{0}", ips->ip[ips->count]);
        }
    }
    else {
        spdlog::trace("dns_callback faild status:{0}!", status);
    }
}

inline void dns_resolve(const char* host, IpList& ips)
{
    ares_channel channel;    // ����һ��ares_channel

    int res;
    if ((res = ares_init(&channel)) != ARES_SUCCESS) {     // ares ��channel ���г�ʼ��
        spdlog::trace("ares_init faild!");
        return;
    }
    //get host by name
    ares_gethostbyname(channel, host, AF_INET, dns_callback, (void*)(&ips));

    int nfds;
    fd_set readers, writers;
    timeval tv, * tvp;
    while (true)
    {
        FD_ZERO(&readers);
        FD_ZERO(&writers);
        nfds = ares_fds(channel, &readers, &writers);   //��ȡares channelʹ�õ�FD  
        if (nfds == 0) {
            break;
        }
        tvp = ares_timeout(channel, NULL, &tv);
        select(nfds, &readers, &writers, NULL, tvp);   //��ares��SOCKET FD �����¼�ѭ��  
        ares_process(channel, &readers, &writers);  // ���¼����� ����ares ����
    }
    ares_destroy(channel);
}

inline std::string DNS(std::string host) {


    IpList ips;
    do {
        memset(&ips, 0, sizeof(ips));
        dns_resolve(host.c_str(), ips);
    } while (0);

    return ips.ip[0];
}

}

#endif //_DNS_h
