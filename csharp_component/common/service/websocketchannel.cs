/*
 * channel
 * 2020/6/2
 * qianqians
 */
using System;
using System.IO;
using System.Collections;
using Fleck;

namespace Abelkhan
{

    public class WebsocketChannel : Abelkhan.Ichannel
    {
        private readonly IWebSocketConnection _socket;
        private readonly object lockobj;

        public readonly ChannelOnRecv _channel_onrecv;

        public event Action<WebsocketChannel> on_connect;
        public WebsocketChannel(IWebSocketConnection socket)
        {
            lockobj = new object();
            _channel_onrecv = new ChannelOnRecv(this);

            _socket = socket;
            _socket.OnOpen = () => {
                on_connect?.Invoke(this);
            };
            _socket.OnClose = () => {
                //on_disconnect?.Invoke(this);
            };
            _socket.OnBinary = (byte[] data) => {
                _channel_onrecv.on_recv(data);
            };
        }

        public void disconnect()
        {
            _socket.Close();
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
                _socket.Send(data);
            }
        }
    }
}