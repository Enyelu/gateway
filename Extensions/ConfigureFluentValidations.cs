using FluentValidation;
using FluentValidation.AspNetCore;
using gateway.api.FluentValidations;

namespace gateway.api.Extensions
{
    public static class ConfigureFluentValidations
    {
        public static void InjectFluentValidations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestDtoValidator>();

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });
            services.AddFluentValidationClientsideAdapters();
        }
    }
}