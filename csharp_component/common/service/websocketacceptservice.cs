/*
 * websocketacceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Fleck;

namespace Abelkhan
{
    public class WebsocketAcceptService
    {
        private readonly WebSocketServer _server;

        public event Action<Abelkhan.Ichannel> on_connect;
        public WebsocketAcceptService(ushort port, bool is_ssl, string pfx, string password)
        {
            if (!is_ssl)
            {
                _server = new WebSocketServer(string.Format("ws://0.0.0.0:{0}", port));
            }
            else
            {
                _server = new WebSocketServer(string.Format("wss://0.0.0.0:{0}", port))
                {
                    Certificate = new X509Certificate2(pfx, password),
                    EnabledSslProtocols = SslProtocols.Tls12,
                    RestartAfterListenError = true,
                };
                _server.ListenerSocket.NoDelay = true;
            }

            _server.Start(socket =>
            {
                var ch = new WebsocketChannel(socket);
                ch.on_connect += on_connect;
            });

            FleckLog.LogAction = (level, message, ex) => {
                switch (level)
                {
                    case LogLevel.Debug:
                        Log.Log.debug(message, ex);
                        break;
                    case LogLevel.Error:
                        Log.Log.err(message, ex);
                        break;
                    case LogLevel.Warn:
                        Log.Log.warn(message, ex);
                        break;
                    default:
                        Log.Log.info(message, ex);
                        break;
                }
            };
        }

    }
}