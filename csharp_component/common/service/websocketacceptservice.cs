/*
 * websocketacceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Fleck;

namespace abelkhan
{
    public class websocketacceptservice
    {
        private WebSocketServer _server;

        public event Action<websocketchannel> on_connect;
        //public event Action<websocketchannel> on_disconnect;
        public websocketacceptservice(ushort port, bool is_ssl, string pfx)
        {
            if (!is_ssl)
            {
                _server = new WebSocketServer(string.Format("ws://0.0.0.0:{0}", port));
            }
            else
            {
                _server = new WebSocketServer(string.Format("wss://0.0.0.0:{0}", port));
                _server.Certificate = new X509Certificate2(pfx);
            }

            _server.Start(socket =>
            {
                var ch = new websocketchannel(socket);
                ch.on_connect += on_connect;
                //ch.on_disconnect += on_disconnect;
            });
        }

    }
}