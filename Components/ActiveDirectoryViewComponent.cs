using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Components
{
	[ViewComponent(Name = "ActiveDirectoryAuthentication")]
	public class ActiveDirectoryViewComponent : NopViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return this.View("~/Plugins/ExternalAuth.NovellActiveDirectory/Views/PublicInfo.cshtml",new SignInViewModel());
		}

		public ActiveDirectoryViewComponent()
		{
		}
	}
}
