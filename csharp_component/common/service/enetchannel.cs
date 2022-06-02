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
        private object lockobj;
        public channel_onrecv _channel_onrecv;

        public enetchannel(ENetHost _host, ENetPeer _peer)
        {
            host = _host;
            peer = _peer;
            lockobj = new object();

            _channel_onrecv = new channel_onrecv(this);
        }

        public void onrecv(ENetPacket packet)
        {
            _channel_onrecv.on_recv(packet.Data.ToArray());
        }

        public void disconnect()
        {
            peer.Disconnect(0);
        }
        
        public bool is_xor_key_crypt()
        {
            return false;
        }

        public void normal_send_crypt(byte[] data)
        {
        }

        public void send(byte[] data)
        {
            lock (lockobj)
            {
                peer.Send(0, data, ENetPacketFlags.Reliable);
            }
        }
    }
}