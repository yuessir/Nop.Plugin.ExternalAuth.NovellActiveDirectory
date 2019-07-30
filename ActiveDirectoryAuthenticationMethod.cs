using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System.Collections.Generic;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory
{
	public class ActiveDirectoryAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod, IPlugin, IWidgetPlugin
    {
		private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

		public ActiveDirectoryAuthenticationMethod(ISettingService settingService, IWebHelper webHelper, ILocalizationService localizationService)
		{
			_settingService = settingService;
			_webHelper = webHelper;
            _localizationService = localizationService;

        }

		public override string GetConfigurationPageUrl()
		{
			return $"{_webHelper.GetStoreLocation((bool?)null)}Admin/NovellActiveDirectoryAuthentication/Configure";
		}

        public bool HideInWidgetList => false;
        //      public void GetPublicViewComponent(out string viewComponentName)
        //{
        //	viewComponentName = "ActiveDirectoryAuthentication";
        //}

        //public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        //{
        //	viewComponentName = "WidgetsActiveDirectoryAuthentication";
        //}

        public string GetPublicViewComponentName()
        {
            return "ActiveDirectoryAuthentication";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsActiveDirectoryAuthentication";
        }

        public IList<string> GetWidgetZones()
		{
			return new List<string>
			{
				"header_links_before"
			};
		}

		public override void Install()
		{
			_settingService.SaveSetting<NovellActiveDirectoryExternalAuthSettings>(new NovellActiveDirectoryExternalAuthSettings(), 0);
            _localizationService.AddOrUpdatePluginLocaleResource( "Plugins.ExternalAuth.NovellActiveDirectory.LDAPPath", "AD Path", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LDAPPath.Hint", "Enter the path to your active directory like 'yourdirectory'", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername", "AD username", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername.Hint", "If needed, enter the username to connect to AD", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword", "AD password", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword.Hint", "If needed, enter the password to connect to AD", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapConnectError", "Could not connect to AD server", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.UserNotFound", "Windows user not found in AD", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.MailNotFound", "No email found for Windows user", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.WindowsUserNotAvailable", "Windows user not available. Please check if windows authentication is enabled and forwardWindowsAuthToken is set to true in web.config!", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.InstantLogin", "Instant login", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.InstantLogin.Hint", "Password-free logins", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError", "Cannot establish connection to active directory!", (string)null);
			_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.fields.LdapPath.Required", "AD path cannot be empty", (string)null);
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LDAPPath", "AD Pfad", "de-DE");
            

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.SearchBase", "Search Base");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.ContainerName", "Container Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.Domain", "Domain");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.DomainDistinguishedName", "Domain Distinguished Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapServerPort", "Ldap Server Port");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.UseSSL", "Use SSL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword.Required", "Please enter your AD password");
            base.Install();
		}

		public override void Uninstall()
		{
			_settingService.DeleteSetting<NovellActiveDirectoryExternalAuthSettings>();
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LDAPPath");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LDAPPath.Hint");
			_localizationService.DeletePluginLocaleResource( "Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername.Hint");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword.Hint");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapConnectError");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.UserNotFound");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.MailNotFound");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.WindowsUserNotAvailable");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.InstantLogin");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.InstantLogin.Hint");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError");
			_localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.fields.LdapPath.Required");

            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.SearchBase");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.ContainerName");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.Domain");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.DomainDistinguishedName");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapServerPort");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.UseSSL");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.UseSSL");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword.Required");
            base.Uninstall();
		}
	}
}
