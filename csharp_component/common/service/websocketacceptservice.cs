/*
 * websocketacceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Fleck;

namespace Abelkhan
{
    public class WebsocketAcceptService
    {
        private WebSocketServer _server;

        public event Action<Abelkhan.Ichannel> on_connect;
        public WebsocketAcceptService(ushort port, bool is_ssl, string pfx)
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
                var ch = new WebsocketChannel(socket);
                ch.on_connect += on_connect;
            });
        }

    }
}