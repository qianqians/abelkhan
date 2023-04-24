using System;
using System.Threading.Tasks;

namespace Hub
{
	public class CenterProxy
    {
        private readonly Abelkhan.center_caller _center_caller;

        public CenterProxy(Abelkhan.Ichannel ch)
		{
			is_reg_center_sucess = false;
            _ch = ch;
            _center_caller = new Abelkhan.center_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
        }

        public void reg_hub(Action callback)
        {
            Log.Log.trace("begin connect center server");

            _center_caller.reg_server_mq("hub", Hub.type, Hub.name).callBack(() =>
            {
                callback.Invoke();
                Log.Log.trace("connect center server sucessed");
            }, () =>
            {
                Log.Log.trace("connect center server faild");
            }).timeout(5000, () =>
            {
                Log.Log.trace("connect center server timeout");
            });
        }

        public Task<bool> reconn_reg_hub()
        {
            Log.Log.trace("begin connect center server");

            var task_ret = new TaskCompletionSource<bool>();

            _center_caller.reconn_reg_server_mq("hub", Hub.type, Hub.name).callBack(() => {
                Log.Log.trace("reconnect center server sucessed");
                task_ret.SetResult(true);
            }, () => {
                Log.Log.err("reconnect center server faild");
                task_ret.SetResult(false);
            }).timeout(5000, () => {
                Log.Log.err("reconnect center server timeout");
                task_ret.SetResult(false);
            });

            return task_ret.Task;
        }

        public void heartbeat()
        {
            _center_caller.heartbeat(Hub.tick).callBack(() => {
                Log.Log.trace("heartbeat center server sucessed");

                timetmp = Service.Timerservice.Tick;

            }, () => {
                Log.Log.err("heartbeat center server faild");
            }).timeout(5000, () => {
                Log.Log.err("heartbeat center server timeout");
            });
            Log.Log.trace("begin heartbeath center server tick:{0}!", Service.Timerservice.Tick);
        }

        public void closed()
        {
            _center_caller.closed();
        }

        public long timetmp = Service.Timerservice.Tick;
        public bool is_reg_center_sucess;
        public readonly Abelkhan.Ichannel _ch;

    }
}

