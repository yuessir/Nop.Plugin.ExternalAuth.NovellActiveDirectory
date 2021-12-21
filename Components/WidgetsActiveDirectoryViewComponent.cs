using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Components
{
	[ViewComponent(Name = "WidgetsActiveDirectoryAuthentication")]
	public class WidgetsActiveDirectoryViewComponent : NopViewComponent
	{
		private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly NovellActiveDirectoryExternalAuthSettings _novellActiveDirectoryExternalAuthSettings;

		public WidgetsActiveDirectoryViewComponent(ICustomerService customerService, IWorkContext workContext, NovellActiveDirectoryExternalAuthSettings novellActiveDirectoryExternalAuthSettings)
		{
			_workContext = workContext;
			_novellActiveDirectoryExternalAuthSettings = novellActiveDirectoryExternalAuthSettings;
            _customerService = customerService;

        }

		public IViewComponentResult Invoke()
		{
            var flag = _novellActiveDirectoryExternalAuthSettings.UseInstantLogin &&
                       !_customerService.IsRegistered(_workContext.CurrentCustomer) &&
                       !HttpContext.Session.Get<bool>("NovellLogout");

            return View("~/Plugins/ExternalAuth.NovellActiveDirectory/Views/WidgetPublicInfo.cshtml", flag);
        }
	}
}
