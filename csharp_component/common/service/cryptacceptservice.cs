/*
 * cryptacceptservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;

namespace abelkhan
{
    class cryptAcceptServerHandler : ChannelHandlerAdapter
    {
        public cryptAcceptServerHandler()
        {
        }

        private cryptchannel ch = null;

        public event Action<cryptchannel> on_connect;
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
            try
            {
                var buffer = message as IByteBuffer;
                Span<byte> spby = ((Span<byte>)buffer.Array).Slice(buffer.ArrayOffset, buffer.ReadableBytes);
                byte[] recv_data = spby.ToArray();
                ch._channel_onrecv.on_recv(recv_data);
            }
            catch (System.Exception e)
            {
                log.log.err("channel_onrecv.on_recv error:{0}!", e);
                ch.disconnect();
                on_channel_exception?.Invoke(ch);
            }
            finally
            {
                ReferenceCountUtil.Release(message);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, System.Exception exception)
        {
            context.CloseAsync();
        }
    }

    public class cryptacceptservice
    {
        private ushort port;
        private IChannel boundChannel;
        private IEventLoopGroup bossGroup;
        private IEventLoopGroup workerGroup;

        public event Action<cryptchannel> on_channel_exception;
        public cryptacceptservice(ushort _port)
        {
            port = _port;
        }

        public event Action<abelkhan.Ichannel> on_connect;
        private void onConnect(cryptchannel ch)
        {
            if (on_connect != null)
            {
                on_connect(ch);
            }
        }

        public event Action<cryptchannel> on_disconnect;
        private void onDisconnect(cryptchannel ch)
        {
            if (on_disconnect != null)
            {
                on_disconnect(ch);
            }
        }

        private async Task RunServerAsync()
        {
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
                        var _handle = new cryptAcceptServerHandler();
                        _handle.on_connect += onConnect;
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

        public async void start()
        {
            await RunServerAsync();
        }

        public async void close()
        {
            await boundChannel.CloseAsync();

            await Task.WhenAll(
                bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}