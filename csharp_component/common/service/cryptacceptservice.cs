/*
 * cryptacceptservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;

namespace abelkhan
{
    class cryptAcceptServerHandler : ChannelHandlerAdapter
    {
        private uint xor_key;
        public cryptAcceptServerHandler(uint _xor_key)
        {
            xor_key = _xor_key;
        }

        private cryptchannel ch = null;

        public delegate void cb_connect(cryptchannel ch);
        public event cb_connect on_connect;
        public override void ChannelActive(IChannelHandlerContext context)
        {
            log.log.trace("cryptchannel new connection!");

            ch = new cryptchannel(context);
            if (on_connect != null)
            {
                on_connect(ch);
            }
        }

        public event Action<cryptchannel> on_channel_exception;
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            Span<byte> spby = ((Span<byte>)buffer.Array).Slice(buffer.ArrayOffset, buffer.ReadableBytes);
            byte[] recv_data = spby.ToArray();

            try
            {
                ch._channel_onrecv.on_recv(recv_data);
            }
            catch (System.Exception e)
            {
                log.log.err("channel_onrecv.on_recv error:{0}!", e);
                ch.disconnect();
                if (on_channel_exception != null)
                {
                    on_channel_exception(ch);
                }
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public delegate void cb_disconnect(cryptchannel ch);
        public event cb_disconnect on_disconnect;
        public override void ExceptionCaught(IChannelHandlerContext context, System.Exception exception)
        {
            context.CloseAsync();
            
            if (on_disconnect != null)
            {
                on_disconnect(ch);
            }
        }
    }

    public class cryptacceptservice
    {
        private ushort port;
        private uint xor_key;
        private IChannel boundChannel;

        public event Action<cryptchannel> on_channel_exception;
        public cryptacceptservice(uint _xor_key, ushort _port)
        {
            port = _port;
            xor_key = _xor_key;
        }

        public delegate void cb_connect(cryptchannel ch);
        public event cb_connect on_connect;
        private void onConnect(cryptchannel ch)
        {
            if (on_connect != null)
            {
                on_connect(ch);
            }
        }

        public delegate void cb_disconnect(cryptchannel ch);
        public event cb_disconnect on_disconnect;
        private void onDisconnect(cryptchannel ch)
        {
            if (on_disconnect != null)
            {
                on_disconnect(ch);
            }
        }

        private async Task RunServerAsync()
        {
            IEventLoopGroup bossGroup;
            IEventLoopGroup workerGroup;

            var dispatcher = new DispatcherEventLoopGroup();
            bossGroup = dispatcher;
            workerGroup = new WorkerEventLoopGroup(dispatcher);

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);
                bootstrap.Channel<TcpServerChannel>();
                bootstrap
                    .Option(ChannelOption.SoBacklog, 100)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var _handle = new cryptAcceptServerHandler(xor_key);
                        _handle.on_connect += onConnect;
                        _handle.on_disconnect += onDisconnect;
                        _handle.on_channel_exception += (cryptchannel ch) => {
                            if (on_channel_exception != null)
                            {
                                on_channel_exception(ch);
                            }
                        };

                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast("accept", _handle);
                    }));
                boundChannel = await bootstrap.BindAsync(port);
            }
            finally
            {
            }
        }

        public void start()
        {
            RunServerAsync().Wait();
        }
    }
}