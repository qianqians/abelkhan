using System;

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

		public void reg_hub(String host, ushort port, String sub_type)
        {
            log.log.trace("begin connect center server");

            _center_caller.reg_server("hub", sub_type, hub.name, host, port).callBack((uint serial_num, string name) =>
            {
                hub.name = name;
                hub.serial = serial_num;
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
            log.log.trace("begin heartbeath center server tick:{0}!", service.timerservice.Tick);
        }

        public void closed()
        {
            _center_caller.closed();
        }

		public bool is_reg_center_sucess;

    }
}

