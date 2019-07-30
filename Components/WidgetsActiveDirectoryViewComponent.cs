using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Components
{
	[ViewComponent(Name = "WidgetsActiveDirectoryAuthentication")]
	public class WidgetsActiveDirectoryViewComponent : NopViewComponent
	{
		private readonly IWorkContext _workContext;

		private readonly NovellActiveDirectoryExternalAuthSettings _novellActiveDirectoryExternalAuthSettings;

		public WidgetsActiveDirectoryViewComponent(IWorkContext workContext, NovellActiveDirectoryExternalAuthSettings novellActiveDirectoryExternalAuthSettings)
		{
			_workContext = workContext;
			_novellActiveDirectoryExternalAuthSettings = novellActiveDirectoryExternalAuthSettings;
		}

		public IViewComponentResult Invoke()
		{
			bool flag = false;
			if (_novellActiveDirectoryExternalAuthSettings.UseInstantLogin && !_workContext.CurrentCustomer.IsRegistered(true) && !this.HttpContext.Session.Get<bool>("NovellLogout"))
			{
				flag = true;
			}
			return this.View<bool>("~/Plugins/ExternalAuth.NovellActiveDirectory/Views/WidgetPublicInfo.cshtml", flag);
		}
	}
}
