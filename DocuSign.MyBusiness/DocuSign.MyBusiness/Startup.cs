using System;
using System.Threading.Tasks;
using DocuSign.MyBusiness.Domain.Admin.Services;
using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using DocuSign.MyBusiness.Domain.Common.Services;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Domain.CustomQuote.Services;
using DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces;
using DocuSign.MyBusiness.Domain.EmploymentContract.Services;
using DocuSign.MyBusiness.Domain.EmploymentContract.Services.Interfaces;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Services;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces;
using DocuSign.MyBusiness.Hubs;
using DocuSign.MyBusiness.Infrustructure.Extensions;
using DocuSign.MyBusiness.Infrustructure.Services;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocuSign.MyBusiness
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITemplateBuilder, LocalTemplateBuilder>();
            services.AddHttpClient<DocuSignApiProvider>();
            services.AddScoped<IDocuSignApiProvider, DocuSignApiProvider>();
            services.AddScoped<ISettingsRepository, SessionSettingsRepository>();
            services.AddScoped<IAccountRepository, ClaimsAccountRepository>();
            services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
            services.AddScoped<IDocuSignClientsFactory, DocuSignClientsFactory>();
            services.AddScoped<ITestAccountConnectionSettingsRepository, AppSettingsTestAccountConnectionSettingsRepository>();
            services.AddSingleton<IEventsRepository, InMemoryEventsRepository>();
            services.AddSingleton<IAppConfiguration, AppSettingsConfiguration>();

            services.AddScoped<IEmploymentContractEnvelopeService, EmploymentContractEnvelopeService>();
            services.AddScoped<IEmploymentContractEnvelopeBuilder, EmploymentContractEnvelopeBuilder>();

            services.AddScoped<ITermsAndConditionsEnvelopeService, TermsAndConditionsEnvelopeService>();
            services.AddScoped<ITermsAndConditionsEnvelopeBuilder, TermsAndConditionsEnvelopeBuilder>();

            services.AddScoped<ICustomerProfileEnvelopeService, CustomerProfileEnvelopeService>();
            services.AddScoped<ICustomerProfileEnvelopeBuilder, CustomerProfileEnvelopeBuilder>();

            services.AddScoped<ICustomQuoteEnvelopeService, CustomQuoteEnvelopeService>();
            services.AddScoped<ICustomQuoteEnvelopeBuilder, CustomQuoteEnvelopeBuilder>();



            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/dist";
                });

            services.AddAuthentication(options =>
            {
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
            {
                config.Cookie.Name = "UserLoginCookie";
                config.Cookie.HttpOnly = true;
                config.Cookie.SameSite = SameSiteMode.Lax;
                config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            });

            services.AddControllers().AddNewtonsoftJson();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/myapp-{Date}.txt");
            app.UseSession();
            app.ConfigureDocuSignExceptionHandling(loggerFactory);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<EventsHub>("/events-hub");
            });
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
