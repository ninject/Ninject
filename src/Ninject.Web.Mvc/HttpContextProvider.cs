using System;
using System.Web;
using Ninject.Activation;

namespace Ninject.Web.Mvc
{
	public class HttpContextProvider : Provider<HttpContext>
	{
		protected override HttpContext CreateInstance(IContext context)
		{
			return HttpContext.Current;
		}
	}
}