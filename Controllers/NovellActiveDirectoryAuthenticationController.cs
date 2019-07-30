using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Services;
using Nop.Services.Stores;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Controllers
{
	public class NovellActiveDirectoryAuthenticationController : BasePluginController
	{
		private readonly NovellActiveDirectoryExternalAuthSettings _novellActiveDirectoryExternalAuthSettings;

		private readonly IExternalAuthenticationService _externalAuthenticationService;

		private readonly ILocalizationService _localizationService;

		private readonly IPermissionService _permissionService;

		private readonly ISettingService _settingService;

		private readonly ICustomerService _customerService;

		private readonly IAuthenticationService _authenticationService;

		private readonly IShoppingCartService _shoppingCartService;

		private readonly IWorkContext _workContext;

		private readonly IEventPublisher _eventPublisher;

		private readonly ICustomerActivityService _customerActivityService;

		private readonly IStaticCacheManager _cacheManager;

        private readonly INotificationService _notificationService;

        private readonly ILdapService _ldapService;

        private readonly IStoreService _storeService;

        private readonly IStoreContext _storeContext;

        private readonly IAuthenticationPluginManager _authenticationPluginManager;

        public NovellActiveDirectoryAuthenticationController(NovellActiveDirectoryExternalAuthSettings novellActiveDirectoryExternalAuthSettings, IExternalAuthenticationService externalAuthenticationService, ILocalizationService localizationService, IPermissionService permissionService, ISettingService settingService, ICustomerService customerService, IAuthenticationService authenticationService, IShoppingCartService shoppingCartService, IWorkContext workContext, IEventPublisher eventPublisher, ICustomerActivityService customerActivityService, IStaticCacheManager cacheManager, INotificationService notificationService, ILdapService ldapService, IStoreService storeService, IStoreContext storeContext, IAuthenticationPluginManager authenticationPluginManager)
		{
			_novellActiveDirectoryExternalAuthSettings = novellActiveDirectoryExternalAuthSettings;
			_externalAuthenticationService = externalAuthenticationService;
			_localizationService = localizationService;
			_permissionService = permissionService;
			_settingService = settingService;
			_customerService = customerService;
			_authenticationService = authenticationService;
			_shoppingCartService = shoppingCartService;
			_workContext = workContext;
			_eventPublisher = eventPublisher;
			_customerActivityService = customerActivityService;
			_cacheManager = cacheManager;
            _notificationService = notificationService;
            _ldapService = ldapService;
            _storeService = storeService;
            _storeContext = storeContext;
            _authenticationPluginManager = authenticationPluginManager;
        }

		[AuthorizeAdmin(false)]
		[Area("Admin")]
		public IActionResult Configure()
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
			{
				return this.AccessDeniedView();
			}
            int activeStoreScopeConfiguration = _storeContext.ActiveStoreScopeConfiguration;
            var novellActiveDirectorySettings = _settingService.LoadSetting<NovellActiveDirectoryExternalAuthSettings>(activeStoreScopeConfiguration);

            ConfigurationNovellModel configurationNovellModel = new ConfigurationNovellModel
			{
                LdapPath = novellActiveDirectorySettings.LdapPath,
				LdapUsername = novellActiveDirectorySettings.LdapUsername,
				LdapPassword = novellActiveDirectorySettings.LdapPassword,
				UseInstantLogin = novellActiveDirectorySettings.UseInstantLogin,
                SearchBase = novellActiveDirectorySettings.SearchBase,
                ContainerName = novellActiveDirectorySettings.ContainerName,
                Domain = novellActiveDirectorySettings.Domain,
                DomainDistinguishedName = novellActiveDirectorySettings.DomainDistinguishedName,
                LdapServerPort = novellActiveDirectorySettings.LdapServerPort,
                UseSSL = novellActiveDirectorySettings.UseSSL,
            };
            //configurationNovellModel.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
            //foreach (var store in _storeService.GetAllStores())
            //    configurationNovellModel.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });
            return this.View("~/Plugins/ExternalAuth.NovellActiveDirectory/Views/Configure.cshtml", (object)configurationNovellModel);
		}

		[HttpPost]
		[AdminAntiForgery(false)]
		[AuthorizeAdmin(false)]
		[Area("Admin")]
		public IActionResult Configure(ConfigurationNovellModel novellModel)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
			{
				return this.AccessDeniedView();
			}
			if (!this.ModelState.IsValid)
			{
				return Configure();
			}
            int activeStoreScopeConfiguration = _storeContext.ActiveStoreScopeConfiguration;
     
            _novellActiveDirectoryExternalAuthSettings.LdapPath = novellModel.LdapPath;
			_novellActiveDirectoryExternalAuthSettings.LdapUsername = novellModel.LdapUsername;
			_novellActiveDirectoryExternalAuthSettings.LdapPassword = novellModel.LdapPassword;
			_novellActiveDirectoryExternalAuthSettings.UseInstantLogin = novellModel.UseInstantLogin;
            _novellActiveDirectoryExternalAuthSettings.SearchBase = novellModel.SearchBase;
            _novellActiveDirectoryExternalAuthSettings.ContainerName = novellModel.ContainerName;
            _novellActiveDirectoryExternalAuthSettings.Domain = novellModel.Domain;
            _novellActiveDirectoryExternalAuthSettings.DomainDistinguishedName = novellModel.DomainDistinguishedName;
            _novellActiveDirectoryExternalAuthSettings.LdapServerPort = novellModel.LdapServerPort;
            _novellActiveDirectoryExternalAuthSettings.UseSSL = novellModel.UseSSL;
            int num = (this._storeService.GetAllStores(true).Count > 1) ? activeStoreScopeConfiguration : 0;
            _settingService.SaveSetting<NovellActiveDirectoryExternalAuthSettings>(_novellActiveDirectoryExternalAuthSettings, num);
            _settingService.ClearCache();
            //_cacheManager.Clear();
            this._customerActivityService.InsertActivity("EditNovellActiveDirectoryExternalAuthSettings", "Edit Novell Active Directory External Auth Settings", null);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

			return Configure();
		}

        public IActionResult SignIn(SignInViewModel model, string returnUrl)
        {
     
           // bool flag = !this._externalAuthenticationService.ExternalAuthenticationMethodIsAvailable("ExternalAuth.NovellActiveDirectory");
            var flag = _authenticationPluginManager
                .IsPluginActive("ExternalAuth.NovellActiveDirectory", _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            if (!flag)
            {
                throw new NopException("Novell Active Directory authentication module cannot be loaded");
            }
            bool flag2 = string.IsNullOrEmpty(this._novellActiveDirectoryExternalAuthSettings.LdapPath);
            if (flag2)
            {
                throw new NopException("Novell Active Directory authentication module not configured");
            }
            //string currentUser = base.User.Identity.Name;
            
            //bool flag3 = currentUser == null;
            IActionResult result2;
            if (string.IsNullOrEmpty(model.AdUserName))
            {
                ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.WindowsUserNotAvailable"));
                result2 = new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
                {
                    ReturnUrl = returnUrl
                } : null);
            }
            else
            {
                //string currentUserName = currentUser.Substring(currentUser.IndexOf("\\") + 1);
                string email = string.Empty;
                LdapUser ldapUser = null;
                try
                {
                    ldapUser = this._ldapService.GetUserByUserName(model.AdUserName);
                    if (null==ldapUser)
                    {
                        ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.UserNotFound"));
                        return new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
                        {
                            ReturnUrl = returnUrl
                        } : null);

                    }
                    
                }
                catch (Exception e)
                {
                    ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError : "+e));
                    return new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
                    {
                        ReturnUrl = returnUrl
                    } : null);
                }
          
             
                //bool flag6 = this._ldapService.Authenticate(ldapUser.DistinguishedName, model.AdPassword);
                //if (flag6)
                //{
                //    ExternalAuthenticationParameters authenticationParameters = new ExternalAuthenticationParameters
                //    {
                //        ProviderSystemName = "ExternalAuth.NovellActiveDirectory",
                //        AccessToken = Guid.NewGuid().ToString(),
                //        Email = ldapUser.Email,
                //        ExternalIdentifier = ldapUser.Email,
                //        ExternalDisplayIdentifier = ldapUser.Email
                //    };
                //    result2 = this._externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
                //}
                //else
                //{
                    
                //    ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError"));
                //    result2 = new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
                //    {
                //        ReturnUrl = returnUrl
                //    } : null);
                //}
                try
                {
                    bool flag6 = this._ldapService.Authenticate(ldapUser.DistinguishedName, model.AdPassword);
                    if (flag6)
                    {
                        ExternalAuthenticationParameters authenticationParameters = new ExternalAuthenticationParameters
                        {
                            ProviderSystemName = "ExternalAuth.NovellActiveDirectory",
                            AccessToken = Guid.NewGuid().ToString(),
                            Email = ldapUser.Email,
                            ExternalIdentifier = ldapUser.Email,
                            ExternalDisplayIdentifier = ldapUser.Email
                        };
                        return this._externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
                    }
                }
                catch (Exception e)
                {
                    ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError : "+"auth " + e));
                    return new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
                    {
                        ReturnUrl = returnUrl
                    } : null);
                }
            }

           
            ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError"));
            result2 = new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
            {
                ReturnUrl = returnUrl
            } : null);
            return result2;
        }
  //      [Obsolete("System.DirectoryServices.AccountManagement is not supported on this platform.")]
  //      public IActionResult Login(string returnUrl)
		//{
  //          bool flag = !this._externalAuthenticationService.ExternalAuthenticationMethodIsAvailable("ExternalAuth.NovellActiveDirectory");
  //          if (flag)
  //          {
  //              throw new NopException("Novell Active Directory authentication module cannot be loaded");
  //          }
  //          bool flag2 = string.IsNullOrEmpty(this._novellActiveDirectoryExternalAuthSettings.LdapPath);
  //          if (flag2)
  //          {
  //              throw new NopException("Novell Active Directory authentication module not configured");
  //          }
  //          string currentUser = base.User.Identity.Name;
  //          bool flag3 = currentUser == null;
  //          IActionResult result2;
  //          if (flag3)
  //          {
  //              ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.WindowsUserNotAvailable"));
  //              result2 = new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
  //              {
  //                  ReturnUrl = returnUrl
  //              } : null);
  //          }
  //          else
  //          {
  //              string servername = string.Empty;
  //              string email = string.Empty;
  //              try
  //              {
  //                  bool flag4 = !string.IsNullOrEmpty(this._novellActiveDirectoryExternalAuthSettings.LdapUsername);
  //                  if (flag4)
  //                  {
  //                      using (PrincipalContext context = new PrincipalContext(ContextType.Domain, this._novellActiveDirectoryExternalAuthSettings.LdapPath, this._novellActiveDirectoryExternalAuthSettings.LdapUsername, this._novellActiveDirectoryExternalAuthSettings.LdapPassword))
  //                      {
  //                          servername = context.ConnectedServer;
  //                      }
  //                  }
  //                  else
  //                  {
  //                      using (PrincipalContext context2 = new PrincipalContext(ContextType.Domain, this._novellActiveDirectoryExternalAuthSettings.LdapPath))
  //                      {
  //                          servername = context2.ConnectedServer;
  //                      }
  //                  }
  //              }
  //              catch (Exception e)
  //              {
  //                  ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.LdapError"));
  //                  return new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
  //                  {
  //                      ReturnUrl = returnUrl
  //                  } : null);
  //              }
  //              string filter = string.Format("(&(objectClass=user)(samaccountname={0}))", currentUser.Substring(currentUser.IndexOf("\\") + 1));
  //              string[] propertiesToLoad = new string[]
  //              {
  //          "mail"
  //              };
  //              try
  //              {
  //                  using (DirectoryEntry directoryEntry = new DirectoryEntry("GC://" + servername))
  //                  {
  //                      using (DirectorySearcher ds = new DirectorySearcher(directoryEntry, filter, propertiesToLoad))
  //                      {
  //                          SearchResult result = ds.FindOne();
  //                          bool flag5 = result != null;
  //                          if (flag5)
  //                          {
  //                              email = result.Properties["mail"][0].ToString();
  //                          }
  //                      }
  //                  }
  //              }
  //              catch (Exception)
  //              {
  //                  ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.UserNotFound"));
  //                  return new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
  //                  {
  //                      ReturnUrl = returnUrl
  //                  } : null);
  //              }
  //              bool flag6 = string.IsNullOrEmpty(email);
  //              if (flag6)
  //              {
  //                  ExternalAuthorizerHelper.AddErrorsToDisplay(this._localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.MailNotFound"));
  //                  result2 = new RedirectToActionResult("Login", "Customer", (!string.IsNullOrEmpty(returnUrl)) ? new
  //                  {
  //                      ReturnUrl = returnUrl
  //                  } : null);
  //              }
  //              else
  //              {
  //                  ExternalAuthenticationParameters authenticationParameters = new ExternalAuthenticationParameters
  //                  {
  //                      ProviderSystemName = "ExternalAuth.NovellActiveDirectory",
  //                      AccessToken = Guid.NewGuid().ToString(),
  //                      Email = email,
  //                      ExternalIdentifier = email,
  //                      ExternalDisplayIdentifier = email
  //                  };
  //                  result2 = this._externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
  //              }
  //          }
  //          return result2;
  //      }
	}
}
