using System;

namespace dbproxy
{
	public class centerproxy
	{
		public centerproxy(abelkhan.Ichannel ch)
		{
			is_reg_sucess = false;
			_center_caller = new abelkhan.center_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void reg_dbproxy(String host, short port)
		{
            log.log.trace("begin connect center server");

			_center_caller.reg_server("dbproxy", "dbproxy", dbproxy.name, host, (ushort)port).callBack((uint serial, string name) =>{
				log.log.trace("connect center server sucessed");
			}, ()=> {
				log.log.err("connect center server faild");
			}).timeout(5*1000, ()=> {
				log.log.err("connect center server timeout");
			});
		}

		public void heartbeath()
        {
			_center_caller.heartbeat();
			log.log.trace("begin heartbeath center server tick:{0}!", service.timerservice.Tick);
		}

		public bool is_reg_sucess;

		private abelkhan.center_caller _center_caller;
	}
}

