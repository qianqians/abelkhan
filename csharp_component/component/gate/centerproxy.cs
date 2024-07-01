using System;

namespace Gate
{
	public struct name_info
	{
		public string name;
	};

	public class CenterProxy
	{
		public bool is_reg_sucess;
		public long timetmp;

		private readonly Abelkhan.center_caller _center_caller;

		public CenterProxy(Abelkhan.Ichannel ch)
		{
			is_reg_sucess = false;
			_center_caller = new Abelkhan.center_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);

			timetmp = Service.Timerservice.Tick;
		}

		public void reg_server(name_info _name_info, Action conn_callback)
		{
			Log.Log.trace("begin connect center server");

			_center_caller.reg_server_mq("gate", "gate", _name_info.name).callBack(() =>
			{
				conn_callback.Invoke();
				Log.Log.trace("connect center server sucessed");
			}, () =>
			{
				Log.Log.trace("connect center server faild");
			}).timeout(5000, () =>
			{
				Log.Log.trace("connect center server timeout");
			});
		}

		public void reconn_reg_server(name_info _name_info, Action conn_callback)
		{
			_center_caller.reconn_reg_server_mq("gate", "gate", _name_info.name).callBack(() =>
			{
				is_reg_sucess = true;
				conn_callback.Invoke();
				Log.Log.trace("connect center server sucess!");
			}, () =>
			{
				Log.Log.trace("connect center server faild!");
			}).timeout(5000, () =>
			{
				Log.Log.trace("connect center server timeout!");
			});
		}

		public void heartbeat(uint tick)
		{
			Log.Log.trace("heartbeat center!");
			_center_caller.heartbeat(tick).callBack(() =>
			{
				Log.Log.trace("heartbeat center server sucessed");
				timetmp = Service.Timerservice.Tick;
			}, () =>
			{
				Log.Log.trace("heartbeat center server faild");
			}).timeout(5000, () =>
			{
				Log.Log.trace("heartbeat center server timeout");
			});
		}

		public void closed()
		{
			_center_caller.closed();
		}
	}
}
