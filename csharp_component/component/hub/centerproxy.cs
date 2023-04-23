using System;
using System.Threading.Tasks;

namespace hub
{
	public class Centerproxy
    {
        private readonly abelkhan.center_caller _center_caller;

        public Centerproxy(abelkhan.Ichannel ch)
		{
			is_reg_center_sucess = false;
            _ch = ch;
            _center_caller = new abelkhan.center_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void reg_hub(Action callback)
        {
            log.Log.trace("begin connect center server");

            _center_caller.reg_server_mq("hub", Hub.type, Hub.name).callBack(() =>
            {
                callback.Invoke();
                log.Log.trace("connect center server sucessed");
            }, () =>
            {
                log.Log.trace("connect center server faild");
            }).timeout(5000, () =>
            {
                log.Log.trace("connect center server timeout");
            });
        }

        public Task<bool> reconn_reg_hub()
        {
            log.Log.trace("begin connect center server");

            var task_ret = new TaskCompletionSource<bool>();

            _center_caller.reconn_reg_server_mq("hub", Hub.type, Hub.name).callBack(() => {
                log.Log.trace("reconnect center server sucessed");
                task_ret.SetResult(true);
            }, () => {
                log.Log.err("reconnect center server faild");
                task_ret.SetResult(false);
            }).timeout(5000, () => {
                log.Log.err("reconnect center server timeout");
                task_ret.SetResult(false);
            });

            return task_ret.Task;
        }

        public void heartbeat()
        {
            _center_caller.heartbeat(Hub.tick).callBack(() => {
                log.Log.trace("heartbeat center server sucessed");

                timetmp = service.Timerservice.Tick;

            }, () => {
                log.Log.err("heartbeat center server faild");
            }).timeout(5000, () => {
                log.Log.err("heartbeat center server timeout");
            });
            log.Log.trace("begin heartbeath center server tick:{0}!", service.Timerservice.Tick);
        }

        public void closed()
        {
            _center_caller.closed();
        }

        public long timetmp = service.Timerservice.Tick;
        public bool is_reg_center_sucess;
        public readonly abelkhan.Ichannel _ch;

    }
}

