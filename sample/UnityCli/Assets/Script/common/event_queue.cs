/*
 * acceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Abelkhan
{
    public class EventQueue
    {
        public readonly static ConcurrentQueue<Tuple<Ichannel, ArrayList> > msgQue = new();
    }
}