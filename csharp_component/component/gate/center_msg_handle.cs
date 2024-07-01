using Abelkhan;
using System;

namespace Gate
{

	public class center_msg_handle
	{
		private readonly GateService _gate_service;
		private readonly Service.Timerservice _timerservice;

		private readonly Abelkhan.center_call_server_module _center_call_server_module;

		public center_msg_handle(GateService gate_service_, Service.Timerservice timerservice_)
		{
			_gate_service = gate_service_;
			_timerservice = timerservice_;

			_center_call_server_module = new Abelkhan.center_call_server_module(Abelkhan.ModuleMgrHandle._modulemng);
			_center_call_server_module.on_close_server += on_close_server;
			_center_call_server_module.on_console_close_server += console_close_server;
			_center_call_server_module.on_svr_be_closed += svr_be_closed;
			_center_call_server_module.on_take_over_svr += take_over_svr;
		}

		private void on_close_server()
		{
			_timerservice.addticktime(5000, (long tick) =>
			{
				_gate_service.on_close_server();
			});
		}

		private void console_close_server(string svr_type, string svr_name)
		{
			if (svr_type == "gate" && svr_name == _gate_service.gate_name_info.name)
			{
				_timerservice.addticktime(5000, (long tick) =>
				{
					_gate_service.on_close_server();
				});
			}
		}

		private void svr_be_closed(string svr_type, string svr_name)
		{
		}

		private void take_over_svr(string svr_name)
		{
			Log.Log.info("gate not support take_over_svr svr_name:{0}", svr_name);
		}

	}

}
