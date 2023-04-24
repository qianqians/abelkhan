

namespace Common
{
	public class moduleException : System.Exception
	{
		public moduleException(string _err) : base(_err)
		{
		}
	}

	public class Response
	{
	}

	public class IModule
	{
		public IModule()
        {
		}

		public Response rsp = null;
	}
}

