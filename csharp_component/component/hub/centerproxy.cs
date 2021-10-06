﻿using System;

namespace hub
{
	public class centerproxy
    {
        private abelkhan.center_caller _center_caller;

        public centerproxy(abelkhan.Ichannel ch)
		{
			is_reg_center_sucess = false;

            _center_caller = new abelkhan.center_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

		public void reg_hub(String ip, ushort port, String name)
        {
            log.log.trace("begin connect center server");

            _center_caller.reg_server("hub", ip, port, name).callBack(() =>
            {
                log.log.trace("connect center server sucessed");
            }, () =>
            {
                log.log.trace("connect center server faild");
            }).timeout(5 * 1000, () =>
            {
                log.log.trace("connect center server timeout");
            });
		}

        public void heartbeat()
        {
            _center_caller.heartbeat();
        }

        public void closed()
        {
            _center_caller.closed();
        }

		public bool is_reg_center_sucess;

    }
}

