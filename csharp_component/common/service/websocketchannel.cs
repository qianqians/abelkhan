/*
 * channel
 * 2020/6/2
 * qianqians
 */
using System;
using System.IO;
using System.Collections;
using Fleck;

namespace abelkhan
{

    public class websocketchannel : abelkhan.Ichannel
    {
        private IWebSocketConnection _socket;
        private object lockobj;

        public channel_onrecv _channel_onrecv;

        public event Action<websocketchannel> on_connect;
        public websocketchannel(IWebSocketConnection socket)
        {
            lockobj = new object();
            _channel_onrecv = new channel_onrecv();

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
            _socket.Close();
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