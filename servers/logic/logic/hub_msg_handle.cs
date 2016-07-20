using System;
namespace logic
{
	public class hub_msg_handle
	{
		public hub_msg_handle(common.modulemanager _modulemanager_)
		{
			_modulemanager = _modulemanager_;
		}

		public void reg_logic_sucess_and_notify_hub_nominate(String hub_name)
		{
			Console.WriteLine("connect hub server sucess");
			logic.hubs.reg_hub(hub_name, juggle.Imodule.current_ch);
		}

		public void hub_call_logic_mothed(String module, String func, String argvs)
		{
			_modulemanager.process_module_mothed(module, func, argvs);
		}

		private common.modulemanager _modulemanager;
	}
}

