/*
 * acceptservice
 * 2020/6/2
 * qianqians
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
    class AcceptServerHandler : ChannelHandlerAdapter
    {
        private channel ch = null;

        public event Action<channel> on_connect;
        public override void ChannelActive(IChannelHandlerContext context)
        {
            log.log.trace("new connection!");

            ch = new channel(context);
            if (on_connect != null)
            {
                on_connect(ch);
            }
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {            
            var buffer = message as IByteBuffer;
            Span<byte> spby = ((Span<byte>)buffer.Array).Slice(buffer.ArrayOffset, buffer.ReadableBytes);
            byte[] recv_data = spby.ToArray();

            ch._channel_onrecv.on_recv(recv_data);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public event Action<channel> on_disconnect;
        public override void ExceptionCaught(IChannelHandlerContext context, System.Exception exception)
        {
            log.log.trace("ExceptionCaught connection!");

            context.CloseAsync();
            if (on_disconnect != null)
            {
                on_disconnect(ch);
            }
        }
    }

    public class acceptservice
    {
        private ushort port;
        private IChannel boundChannel;

        public acceptservice(ushort _port)
        {
            port = _port;
        }

        public event Action<channel> on_connect;
        private void onConnect(channel ch)
        {
            if (on_connect != null)
            {
                on_connect(ch);
            }
        }

        public event Action<channel> on_disconnect;
        private void onDisconnect(channel ch)
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
                        var _handle = new AcceptServerHandler();
                        _handle.on_connect += onConnect;
                        _handle.on_disconnect += onDisconnect;

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