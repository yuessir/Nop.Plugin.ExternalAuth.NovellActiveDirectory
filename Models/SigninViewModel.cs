using System.ComponentModel.DataAnnotations;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Validators;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models
{
    public class SignInViewModel : BaseNopModel
    {
        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapPassword")]
        public string AdPassword { get; set; }
        
            [NopResourceDisplayName("Plugins.ExternalAuth.NovellActiveDirectory.LdapUsername")]
        public string AdUserName { get; set; }

    }
}
