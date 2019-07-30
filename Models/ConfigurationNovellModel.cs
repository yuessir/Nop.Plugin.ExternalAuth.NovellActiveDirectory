using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Validators;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models
{
	public class ConfigurationNovellModel : BaseNopModel
	{
        public ConfigurationNovellModel()
        {
           // AvailableStores = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapPath")]
		public string LdapPath
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername")]
		public string LdapUsername
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword")]
		public string LdapPassword
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.InstantLogin")]
		public bool UseInstantLogin
		{
			get;
			set;
		}

        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.SearchBase")]
        public string SearchBase
        {
            get;
            set;
        }
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.ContainerName")]
        public string ContainerName
        {
            get;
            set;
        }
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.Domain")]
        public string Domain
        {
            get;
            set;
        }
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.DomainDistinguishedName")]
        public string DomainDistinguishedName
        {
            get;
            set;
        }
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapServerPort")]
        public string LdapServerPort
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.UseSSL")]
        public bool UseSSL
        {
            get;
            set;
        }
        //public List<SelectListItem> AvailableStores { get; set; }
        //[NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Store")]
        //public int StoreId { get; set; }
        //public string StoreName { get; set; }
    }

}
