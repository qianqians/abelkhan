using System;
using System.Threading.Tasks;

namespace dbproxy
{
	public class centerproxy
	{
		public centerproxy(abelkhan.Ichannel ch)
		{
			is_reg_sucess = false;
			_ch = ch;
			_center_caller = new abelkhan.center_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void reg_dbproxy(String host, ushort port)
		{
            log.log.trace("begin connect center server");

			_center_caller.reg_server("dbproxy", dbproxy.name, host, port).callBack(() =>{
				log.log.trace("connect center server sucessed");
			}, ()=> {
				log.log.err("connect center server faild");
			}).timeout(5*1000, ()=> {
				log.log.err("connect center server timeout");
			});
		}

		public Task<bool> reconn_reg_dbproxy(String host, ushort port)
		{
			log.log.trace("begin connect center server");

			var task_ret = new TaskCompletionSource<bool>();

			_center_caller.reconn_reg_server("dbproxy", dbproxy.name, host, port).callBack(() => {
				log.log.trace("reconnect center server sucessed");
				task_ret.SetResult(true);
			}, () => {
				log.log.err("reconnect center server faild");
				task_ret.SetResult(false);
			}).timeout(5 * 1000, () => {
				log.log.err("reconnect center server timeout");
				task_ret.SetResult(false);
			});

			return task_ret.Task;
		}

		public void heartbeath()
        {
            _center_caller.heartbeat(dbproxy.tick).callBack(() => {
				log.log.trace("heartbeat center server sucessed");

				timetmp = service.timerservice.Tick;

			}, () => {
				log.log.err("heartbeat center server faild");
			}).timeout(5*1000, () => {
				log.log.err("heartbeat center server timeout");
			});
			log.log.trace("begin heartbeath center server tick:{0}!", service.timerservice.Tick);
		}

		public bool is_reg_sucess;
		public long timetmp = service.timerservice.Tick;
		public abelkhan.Ichannel _ch;
		private abelkhan.center_caller _center_caller;
	}
}

