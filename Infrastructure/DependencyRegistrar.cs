using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Services;
using Nop.Services.Authentication;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Infrastructure
{
	public class DependencyRegistrar : IDependencyRegistrar
	{
		public int Order => 1;

		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			builder.RegisterType<NovellInheritedCookieAuthenticationService>().As<IAuthenticationService>();
            builder.RegisterType<LdapService>().As<ILdapService>();
        }
	}
}
