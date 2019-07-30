using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory
{
	public class NovellActiveDirectoryExternalAuthSettings : ISettings
	{
		public string LdapPath
		{
			get;
			set;
		}

		public string LdapUsername
		{
			get;
			set;
		}

		public string LdapPassword
		{
			get;
			set;
		}

		public bool UseInstantLogin
		{
			get;
			set;
		}
        public string SearchBase
        {
            get;
            set;
        }
        public string ContainerName
        {
            get;
            set;
        }
        public string Domain
        {
            get;
            set;
        }
        public string DomainDistinguishedName
        {
            get;
            set;
        }
        public string LdapServerPort
        {
            get;
            set;
        }
        public bool UseSSL
        {
            get;
            set;
        }
    }
}
