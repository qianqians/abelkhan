/*
 * enetchannel
 * qianqians
 * 2020/6/4
 */
using System;
using System.IO;
using System.Collections;
using ENet.Managed;

namespace abelkhan
{
    public class enetchannel : abelkhan.Ichannel
    {
        private ENetHost host;
        private ENetPeer peer;
        public channel_onrecv _channel_onrecv;

        public enetchannel(ENetHost _host, ENetPeer _peer)
        {
            host = _host;
            peer = _peer;

            _channel_onrecv = new channel_onrecv();
        }

        public void onrecv(ENetPacket packet)
        {
            _channel_onrecv.on_recv(packet.Data.ToArray());
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
            peer.Send(0, data, ENetPacketFlags.Reliable);
        }
    }
}