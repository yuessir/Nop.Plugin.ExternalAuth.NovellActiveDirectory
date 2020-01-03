using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Extensions;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models;
using Novell.Directory.Ldap;
using LdapEntry = Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models.LdapEntry;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Services
{
    public class LdapService : ILdapService
    {
        private readonly NovellActiveDirectoryExternalAuthSettings _ldapSettings;

        private readonly string[] _attributes =
        {
            "objectSid", "objectGUID", "objectCategory", "objectClass", "memberOf", "name", "cn", "distinguishedName",
            "sAMAccountName", "sAMAccountName", "userPrincipalName", "displayName", "givenName", "sn", "description",
            "telephoneNumber", "mail", "streetAddress", "postalCode", "l", "st", "co", "c"
        };

        public LdapService(NovellActiveDirectoryExternalAuthSettings ldapSettings)
        {
            this._ldapSettings = ldapSettings;
        }

        private ILdapConnection GetConnection()
        {

            var ldapConnection = new LdapConnection() { SecureSocketLayer = this._ldapSettings.UseSSL };

            //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 (adata) for secure    
            ldapConnection.Connect(this._ldapSettings.LdapPath,Convert.ToInt32(this._ldapSettings.LdapServerPort));
            //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
            ldapConnection.Bind(this._ldapSettings.LdapUsername, this._ldapSettings.LdapPassword);
         
            return ldapConnection;
        }

        public ICollection<Models.LdapEntry> GetGroups(string groupName, bool getChildGroups = false)
        {
            var groups = new Collection<Models.LdapEntry>();

            var filter = $"(&(objectClass=group)(cn={groupName}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._ldapSettings.SearchBase,
                    LdapConnection.ScopeSub,
                    filter,
                    this._attributes,
                    false,
                    null);

                LdapMessage message;
                foreach (var entry in search)
                {
                    groups.Add(this.CreateEntryFromAttributes(entry.Dn, entry.GetAttributeSet()));

                    if (!getChildGroups)
                    {
                        continue;
                    }

                    foreach (var child in this.GetChildren<Models.LdapEntry>(string.Empty, entry.Dn))
                    {
                        groups.Add(child);
                    }
                }
              
            }

            return groups;
        }

        public ICollection<LdapUser> GetAllUsers()
        {
            return this.GetUsersInGroups(null);
        }

        public ICollection<LdapUser> GetUsersInGroup(string group)
        {
            return this.GetUsersInGroups(this.GetGroups(group));
        }

        public ICollection<LdapUser> GetUsersInGroups(ICollection<Models.LdapEntry> groups)
        {
            var users = new Collection<LdapUser>();

            if (groups == null || !groups.Any())
            {
                users.AddRange(this.GetChildren<LdapUser>(this._ldapSettings.SearchBase));
            }
            else
            {
                foreach (var group in groups)
                {
                    users.AddRange(this.GetChildren<LdapUser>(this._ldapSettings.SearchBase, @group.DistinguishedName));
                }
            }

            return users;
        }

        public ICollection<LdapUser> GetUsersByEmailAddress(string emailAddress)
        {
            var users = new Collection<LdapUser>();

            var filter = $"(&(objectClass=user)(mail={emailAddress}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._ldapSettings.SearchBase,
                    LdapConnection.ScopeSub,
                    filter,
                    this._attributes,
                    false, null);

                LdapMessage message;
                foreach (var entry in search)
                {
                    users.Add(this.CreateUserFromAttributes(this._ldapSettings.SearchBase,
                        entry.GetAttributeSet()));
                }
               
            }

            return users;
        }

        public LdapUser GetUserByUserName(string userName)
        {
            LdapUser user = null;

           var filter = $"(&(objectClass=user)(name={userName}))";
            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    this._ldapSettings.SearchBase,
                    LdapConnection.ScopeSub,
                    filter,
                    this._attributes,
                    false,
                    null);

                LdapMessage message;
                if (search.HasMore())
                {
                    foreach (var entry in search)
                    {
                        user = this.CreateUserFromAttributes(this._ldapSettings.SearchBase, entry.GetAttributeSet());
                    }
                }
               
            }

            return user;
        }

        public LdapUser GetAdministrator()
        {
            var name = this._ldapSettings.LdapUsername.Substring(
                this._ldapSettings.LdapUsername.IndexOf("\\", StringComparison.Ordinal) != -1
                    ? this._ldapSettings.LdapUsername.IndexOf("\\", StringComparison.Ordinal) + 1
                    : 0);

            return this.GetUserByUserName(name);
        }

        public void AddUser(LdapUser user, string password)
        {
            var dn = $"CN={user.FirstName} {user.LastName},{this._ldapSettings.ContainerName}";

            var attributeSet = new LdapAttributeSet
            {
                new LdapAttribute("instanceType", "4"),
                new LdapAttribute("objectCategory", $"CN=Person,CN=Schema,CN=Configuration,{this._ldapSettings.DomainDistinguishedName}"),
                new LdapAttribute("objectClass", new[] {"top", "person", "organizationalPerson", "user"}),
                new LdapAttribute("name", user.UserName),
                new LdapAttribute("cn", $"{user.FirstName} {user.LastName}"),
                new LdapAttribute("sAMAccountName", user.UserName),
                new LdapAttribute("userPrincipalName", user.UserName),
                new LdapAttribute("unicodePwd", Convert.ToBase64String(Encoding.Unicode.GetBytes($"\"{user.Password}\""))),
                new LdapAttribute("userAccountControl", user.MustChangePasswordOnNextLogon ? "544" : "512"),
                new LdapAttribute("givenName", user.FirstName),
                new LdapAttribute("sn", user.LastName),
                new LdapAttribute("mail", user.EmailAddress)
            };

            if (user.DisplayName != null)
            {
                attributeSet.Add(new LdapAttribute("displayName", user.DisplayName));
            }

            if (user.Description != null)
            {
                attributeSet.Add(new LdapAttribute("description", user.Description));
            }
            if (user.Phone != null)
            {
                attributeSet.Add(new LdapAttribute("telephoneNumber", user.Phone));
            }
            if (user.Address?.Street != null)
            {
                attributeSet.Add(new LdapAttribute("streetAddress", user.Address.Street));
            }
            if (user.Address?.City != null)
            {
                attributeSet.Add(new LdapAttribute("l", user.Address.City));
            }
            if (user.Address?.PostalCode != null)
            {
                attributeSet.Add(new LdapAttribute("postalCode", user.Address.PostalCode));
            }
            if (user.Address?.StateName != null)
            {
                attributeSet.Add(new LdapAttribute("st", user.Address.StateName));
            }
            if (user.Address?.CountryName != null)
            {
                attributeSet.Add(new LdapAttribute("co", user.Address.CountryName));
            }
            if (user.Address?.CountryCode != null)
            {
                attributeSet.Add(new LdapAttribute("c", user.Address.CountryCode));
            }
            
            var newEntry = new Novell.Directory.Ldap.LdapEntry(dn, attributeSet);

            using (var ldapConnection = this.GetConnection())
            {
                ldapConnection.Add(newEntry);
            }
        }

        public void DeleteUser(string distinguishedName)
        {
            using (var ldapConnection = this.GetConnection())
            {
                ldapConnection.Delete(distinguishedName);
            }
        }


        public bool Authenticate(string distinguishedName, string password)
        {
            using (var ldapConnection = new LdapConnection() { SecureSocketLayer = true })
            {
                ldapConnection.Connect(this._ldapSettings.LdapPath,Convert.ToInt32(this._ldapSettings.LdapServerPort));

                try
                {
                    ldapConnection.Bind(distinguishedName, password);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private ICollection<T> GetChildren<T>(string searchBase, string groupDistinguishedName = null)
            where T : ILdapEntry, new()
        {
            var entries = new Collection<T>();

            var objectCategory = "*";
            var objectClass = "*";

            if (typeof(T) == typeof(Models.LdapEntry))
            {
                objectClass = "group";
                objectCategory = "group";

                entries = this.GetChildren(this._ldapSettings.SearchBase, groupDistinguishedName, objectCategory, objectClass)
                    .Cast<T>().ToCollection();

            }

            if (typeof(T) == typeof(LdapUser))
            {
                objectCategory = "person";
                objectClass = "user";

                entries = this.GetChildren(this._ldapSettings.SearchBase, null, objectCategory, objectClass).Cast<T>()
                    .ToCollection();

            }

            return entries;
        }

        private ICollection<ILdapEntry> GetChildren(string searchBase, string groupDistinguishedName = null,
            string objectCategory = "*", string objectClass = "*")
        {
            var allChildren = new Collection<ILdapEntry>();

            var filter = string.IsNullOrEmpty(groupDistinguishedName)
                ? $"(&(objectCategory={objectCategory})(objectClass={objectClass}))"
                : $"(&(objectCategory={objectCategory})(objectClass={objectClass})(memberOf={groupDistinguishedName}))";

            using (var ldapConnection = this.GetConnection())
            {
                var search = ldapConnection.Search(
                    searchBase,
                    LdapConnection.ScopeSub,
                    filter,
                    this._attributes,
                    false,
                    null);

                LdapMessage message;
                if (search.HasMore())
                {
                    foreach (var entry in search)
                    {

                        if (objectClass == "group")
                        {
                            allChildren.Add(this.CreateEntryFromAttributes(entry.Dn, entry.GetAttributeSet()));

                            foreach (var child in this.GetChildren(string.Empty, entry.Dn, objectCategory, objectClass))
                            {
                                allChildren.Add(child);
                            }
                        }

                        if (objectClass == "user")
                        {
                            allChildren.Add(this.CreateUserFromAttributes(entry.Dn, entry.GetAttributeSet()));
                        }
                    }
                }
            
            }

            return allChildren;
        }

        private LdapUser CreateUserFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            var ldapUser = new LdapUser
            {
                ObjectSid = attributeSet.GetAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.GetAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.GetAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.GetAttribute("objectClass")?.StringValue,
                IsDomainAdmin = attributeSet.GetAttribute("memberOf") != null && attributeSet.GetAttribute("memberOf").StringValueArray.Contains("CN=Domain Admins," + this._ldapSettings.SearchBase),
                MemberOf = attributeSet.GetAttribute("memberOf")?.StringValueArray,
                CommonName = attributeSet.GetAttribute("cn")?.StringValue,
                UserName = attributeSet.GetAttribute("name")?.StringValue,
                SamAccountName = attributeSet.GetAttribute("sAMAccountName")?.StringValue,
                UserPrincipalName = attributeSet.GetAttribute("userPrincipalName")?.StringValue,
                Name = attributeSet.GetAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.GetAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                DisplayName = attributeSet.GetAttribute("displayName")?.StringValue,
                FirstName = attributeSet.GetAttribute("givenName")?.StringValue,
                LastName = attributeSet.GetAttribute("sn")?.StringValue,
                Description = attributeSet.GetAttribute("description")?.StringValue,
                Phone = attributeSet.GetAttribute("telephoneNumber")?.StringValue,
                EmailAddress = attributeSet.GetAttribute("mail")?.StringValue,
                Address = new LdapAddress
                {
                    Street = attributeSet.GetAttribute("streetAddress")?.StringValue,
                    City = attributeSet.GetAttribute("l")?.StringValue,
                    PostalCode = attributeSet.GetAttribute("postalCode")?.StringValue,
                    StateName = attributeSet.GetAttribute("st")?.StringValue,
                    CountryName = attributeSet.GetAttribute("co")?.StringValue,
                    CountryCode = attributeSet.GetAttribute("c")?.StringValue
                },
            
                SamAccountType = int.Parse(attributeSet.GetAttribute("sAMAccountType")?.StringValue ?? "0")
            };
          
            return ldapUser;
        }

        private DateTime GetPasswordExpiredDate(string stringValue)
        {
            if (long.TryParse(stringValue,out long tickValue))
            {
                return DateTime.FromFileTime(tickValue);
            }
            return new DateTime();
            
        }

        private Models.LdapEntry CreateEntryFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            return new Models.LdapEntry
            {
                ObjectSid = attributeSet.GetAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.GetAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.GetAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.GetAttribute("objectClass")?.StringValue,
                CommonName = attributeSet.GetAttribute("cn")?.StringValue,
                Name = attributeSet.GetAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.GetAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                SamAccountName = attributeSet.GetAttribute("sAMAccountName")?.StringValue,
                SamAccountType = int.Parse(attributeSet.GetAttribute("sAMAccountType")?.StringValue ?? "0"),
            };
        }

        private SecurityIdentifier GetDomainSid()
        {
            var administratorAcount = new NTAccount(this._ldapSettings.Domain, "administrator");
            var administratorSId = (SecurityIdentifier)administratorAcount.Translate(typeof(SecurityIdentifier));
            return administratorSId.AccountDomainSid;
        }
    }
}
