using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Linq.Expressions;
using Nop.Plugin.ExternalAuth.NovellActiveDirectory.Models;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Validators
{
	public class ConfigurationValidator : BaseNopValidator<ConfigurationNovellModel>
	{
		public ConfigurationValidator(ILocalizationService localizationService)
		{
			DefaultValidatorOptions.WithMessage<ConfigurationNovellModel, string>(DefaultValidatorExtensions.NotEmpty<ConfigurationNovellModel, string>(base.RuleFor<string>((Expression<Func<ConfigurationNovellModel, string>>)((ConfigurationNovellModel x) => x.LdapPath))), localizationService.GetResource("Plugins.ExternalAuth.NovellActiveDirectory.fields.LdapPath.Required"));
		}
	}
}
