/*
 * enetservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using ENet;

namespace abelkhan
{
    public class enetservice
    {
        private ENet.Host host;
        private Dictionary<string, enetchannel> conns;
        private Dictionary<string, Action<enetchannel> > conn_cbs;

        public delegate void cb_connect(enetchannel ch);
        public event cb_connect on_connect;

        public enetservice(string ip, ushort port)
        {
            host = new Host();
            var address = new Address();
            address.Port = port;
            host.Create(address, 2048);

            conns = new Dictionary<string, enetchannel>();
            conn_cbs = new Dictionary<string, Action<enetchannel> >();
        }

        public void poll()
        {
            Event ev;
            bool polled = false;

            while (!polled)
            {
                if (host.CheckEvents(out ev) <= 0)
                {
                    if (host.Service(15, out ev) <= 0)
                    {
                        break;
                    }

                    polled = true;
                }

                switch (ev.Type)
                {
                case EventType.None:
                    break;

                case EventType.Connect:
                    {
                        var raddr = ev.Peer.IP + ":" + ev.Peer.Port.ToString();
                        log.log.trace("enetservice poll raddr:{0}", raddr);
                        if (!conns.TryGetValue(raddr, out enetchannel ch))
                        {
                            ch = new enetchannel(host, ev.Peer);
                            conns[raddr] = ch;
                        }

                        if (conn_cbs.Remove(raddr, out Action<enetchannel> cb))
                        {
                            cb(ch);
                        }
                        else{
                            if (on_connect != null)
                            {
                                on_connect(ch);
                            }
                        }
                    }
                    break;

                case EventType.Disconnect:
                    break;

                case EventType.Timeout:
                    break;

                case EventType.Receive:
                    {
                        var raddr = ev.Peer.IP + ":" + ev.Peer.Port.ToString();
                        if (conns.TryGetValue(raddr, out enetchannel ch))
                        {
                            ch.onrecv(ev.Packet);
                        }
                        ev.Packet.Dispose();
                    }
                    break;
                }
            }
        }

        public void connect(string ip, ushort port, Action<enetchannel> cb)
        {
            var raddr = ip + ":" + port.ToString();
            conn_cbs.Add(raddr, cb);

            var address = new Address();
            address.SetIP(ip);
            address.Port = port;
            host.Connect(address);
        }
    }
}