# Nop.Plugin.ExternalAuth.NovellActiveDirectory
LDAP login Auth for nopCommerce 


credit by @brechtb86 and project https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard

The plugin use LDAP directly to check the identity without setting the "Windows Authentication" in IIS.

If you want to integrate into IIS,should do setting as following:
1. Enable Windows authentication on your webserver(IIS)for nopcommerce application.
2. Change forwardWindowsAuthToken in web.config to true.
