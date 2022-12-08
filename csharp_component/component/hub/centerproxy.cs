using System;
using System.Threading.Tasks;

namespace hub
{
	public class centerproxy
    {
        private abelkhan.center_caller _center_caller;

        public centerproxy(abelkhan.Ichannel ch)
		{
			is_reg_center_sucess = false;
            _ch = ch;
            _center_caller = new abelkhan.center_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void reg_hub(Action callback)
        {
            log.log.trace("begin connect center server");

            _center_caller.reg_server_mq("hub", hub.type, hub.name).callBack(() =>
            {
                callback.Invoke();
                log.log.trace("connect center server sucessed");
            }, () =>
            {
                log.log.trace("connect center server faild");
            }).timeout(5000, () =>
            {
                log.log.trace("connect center server timeout");
            });
        }

        public Task<bool> reconn_reg_hub()
        {
            log.log.trace("begin connect center server");

            var task_ret = new TaskCompletionSource<bool>();

            _center_caller.reconn_reg_server_mq("hub", hub.type, hub.name).callBack(() => {
                log.log.trace("reconnect center server sucessed");
                task_ret.SetResult(true);
            }, () => {
                log.log.err("reconnect center server faild");
                task_ret.SetResult(false);
            }).timeout(5000, () => {
                log.log.err("reconnect center server timeout");
                task_ret.SetResult(false);
            });

            return task_ret.Task;
        }

        public void heartbeat()
        {
            _center_caller.heartbeat(hub.tick).callBack(() => {
                log.log.trace("heartbeat center server sucessed");

                timetmp = service.timerservice.Tick;

            }, () => {
                log.log.err("heartbeat center server faild");
            }).timeout(5000, () => {
                log.log.err("heartbeat center server timeout");
            });
            log.log.trace("begin heartbeath center server tick:{0}!", service.timerservice.Tick);
        }

        public void closed()
        {
            _center_caller.closed();
        }

        public long timetmp = service.timerservice.Tick;
        public bool is_reg_center_sucess;
        public abelkhan.Ichannel _ch;

    }
}

