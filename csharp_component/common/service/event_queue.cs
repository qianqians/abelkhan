/*
 * acceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace abelkhan
{
    public class event_queue
    {
        public static ConcurrentQueue<Tuple<Ichannel, ArrayList> > msgQue = new ConcurrentQueue<Tuple<Ichannel, ArrayList> >();
    }
}