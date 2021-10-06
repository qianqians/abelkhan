/*
 * enetchannel
 * qianqians
 * 2020/6/4
 */
using System;
using System.IO;
using System.Collections;

namespace abelkhan
{
    public class enetchannel : abelkhan.Ichannel
    {
        private ENet.Host host;
        private ENet.Peer peer;
        public channel_onrecv _channel_onrecv;

        public enetchannel(ENet.Host _host, ENet.Peer _peer)
        {
            host = _host;
            peer = _peer;

            _channel_onrecv = new channel_onrecv();
        }

        public void onrecv(ENet.Packet packet)
        {
            byte[] data = new byte[packet.Length];
            packet.CopyTo(data);

            _channel_onrecv.on_recv(data);
        }

        public ArrayList pop()
        {
            if (_channel_onrecv.que.Count > 0)
            {
                return _channel_onrecv.que.Dequeue();
            }
            return null;
        }

        public void disconnect()
        {
            peer.Disconnect(0);
        }

        public void send(byte[] data)
        {
            ENet.Packet packet = default(ENet.Packet);
            packet.Create(data);

            peer.Send(0, ref packet);
            host.Flush();
        }
    }
}