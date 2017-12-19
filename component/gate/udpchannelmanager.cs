using System;
using System.Collections.Generic;

namespace gate
{
    public class udpchannelmanager
    {
        public udpchannelmanager()
        {
            ch_refresh_table = new Dictionary<juggle.Ichannel, long>();
        }

        public void add_udpchannel(juggle.Ichannel ch)
        {
            ch_refresh_table.Add(ch, service.timerservice.Tick);
        }

        public void refresh_udpchannel(juggle.Ichannel ch)
        {
            if (ch_refresh_table.ContainsKey(ch))
            {
                ch_refresh_table[ch] = service.timerservice.Tick;
            }
        }

        public void tick_udpchannel(Int64 tick)
        {
            var remove = new List<juggle.Ichannel>();
            foreach(var item in ch_refresh_table)
            {
                if ((tick - item.Value) > 30 * 1000)
                {
                    remove.Add(item.Key);
                }
            }
            foreach (var ch in remove)
            {
                ch_refresh_table.Remove(ch);
                ch.disconnect();
            }

            gate.timer.addticktime(10 * 1000, tick_udpchannel);
        }

        private Dictionary<juggle.Ichannel, Int64> ch_refresh_table;
    }
}
