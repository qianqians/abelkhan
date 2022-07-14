/*
 * enetservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net;
using System.Threading.Tasks;
using ENet.Managed;

namespace abelkhan
{
    public class enetservice
    {
        private ENetHost host;
        private Dictionary<UInt64, enetchannel> back_conns;
        private Dictionary<UInt64, enetchannel> conns;
        private Dictionary<UInt64, Action<enetchannel> > conn_cbs;
        private Task run_t;
        private bool run_flag = true;

        public event Action<abelkhan.Ichannel> on_connect;

        public enetservice(string _host, ushort port)
        {
            var listenEndPoint = new IPEndPoint(Dns.GetHostAddresses(_host)[0], port);
            host = new ENetHost(listenEndPoint, 2048, 1);

            back_conns = new Dictionary<UInt64, enetchannel>();
            conns = new Dictionary<UInt64, enetchannel>();
            conn_cbs = new Dictionary<UInt64, Action<enetchannel> >();
        }

        private void poll()
        {
            var Event = host.Service(TimeSpan.FromMilliseconds(10));

            switch (Event.Type)
            {
                case ENetEventType.None:
                    break ;

                case ENetEventType.Connect:
                    {
                        var ep = Event.Peer.GetRemoteEndPoint();

                        var ip_bytes = ep.Address.GetAddressBytes();
                        var ip_addr = (UInt64)(ip_bytes[0] | ip_bytes[1] << 8 | ip_bytes[2] << 16 | ip_bytes[3] << 24);
                        var peerHandle = ip_addr << 32 | (UInt64)((UInt32)ep.Port);

                        log.log.trace("enetservice poll raddr:{0}", peerHandle);
                        if (!back_conns.TryGetValue(peerHandle, out enetchannel ch))
                        {
                            ch = new enetchannel(host, Event.Peer);
                            back_conns.Add(peerHandle, ch);

                            on_connect?.Invoke(ch);
                        }
                        else
                        {
                            back_conns.Remove(peerHandle);
                        }

                        if (conn_cbs.Remove(peerHandle, out Action<enetchannel> cb))
                        {
                            cb(ch);
                        }

                        if (conns.ContainsKey(peerHandle))
                        {
                            conns[peerHandle] = ch;
                        }
                        else
                        {
                            conns.Add(peerHandle, ch);
                        }
                    }
                    break;

                case ENetEventType.Disconnect:
                    break;

                case ENetEventType.Receive:
                    {
                        var ep = Event.Peer.GetRemoteEndPoint();

                        var ip_bytes = ep.Address.GetAddressBytes();
                        var ip_addr = (UInt64)(ip_bytes[0] | ip_bytes[1] << 8 | ip_bytes[2] << 16 | ip_bytes[3] << 24);
                        var peerHandle = ip_addr << 32 | (UInt64)((UInt32)ep.Port);

                        if (conns.TryGetValue(peerHandle, out enetchannel ch))
                        {
                            ch.onrecv(Event.Packet);
                        }
                        Event.Packet.Destroy();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            host.Flush();
        }

        public void start()
        {
            run_t = new Task(()=>
            {
                while (run_flag)
                {
                    poll();
                }
            });
            run_t.Start();
        }

        public void stop()
        {
            run_flag = false;
        }

        public void connect(string ulr_host, ushort port, Action<enetchannel> cb)
        {
            log.log.trace("enet connect host:{0}, port:{1}!", ulr_host, port);

            IPEndPoint connectEndPoint = new IPEndPoint(Dns.GetHostAddresses(ulr_host)[0], port);

            var ip_bytes = connectEndPoint.Address.GetAddressBytes();
            var ip_addr = (UInt64)(ip_bytes[0] | ip_bytes[1] << 8 | ip_bytes[2] << 16 | ip_bytes[3] << 24);
            var peerHandle = ip_addr << 32 | (UInt64)((UInt32)connectEndPoint.Port);

            conn_cbs.Add(peerHandle, cb);

            var peer = host.Connect(connectEndPoint, 1, 0);
        }
    }
}