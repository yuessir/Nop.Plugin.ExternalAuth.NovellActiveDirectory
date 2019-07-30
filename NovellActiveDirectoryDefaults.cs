namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory
{
	public static class NovellActiveDirectoryDefaults
	{
		public const string ProviderSystemName = "ExternalAuth.NovellActiveDirectory";

		public static string AuthenticationScheme => "NovellActiveDirectory";

		public static string ClaimsIssuer => "NovellActiveDirectory";
	}
}
