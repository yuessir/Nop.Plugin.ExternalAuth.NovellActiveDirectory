using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using SessionExtensions = Nop.Core.Http.Extensions.SessionExtensions;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Services
{
	public class NovellInheritedCookieAuthenticationService : CookieAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public NovellInheritedCookieAuthenticationService(CustomerSettings customerSettings, ICustomerService customerService, IHttpContextAccessor httpContextAccessor)
			: base(customerSettings, customerService, httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public override void SignOut()
		{
			base.SignOut();
			_httpContextAccessor.HttpContext.Session.Set<bool>("NovellLogout", true);
		}
	}
}
