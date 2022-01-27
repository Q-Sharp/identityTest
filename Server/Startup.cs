using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Kvno.Bff.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "__Host-X-XSRF-TOKEN";
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddHttpClient();
            services.AddOptions();

            var openIDConnectSettings = Configuration.GetSection("OpenIDConnectSettings");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddCookie()
          .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
          {
              options.Authority = "https://demo.duendesoftware.com";

              options.ClientId = "interactive.confidential";
              options.ClientSecret = "secret";
              options.ResponseType = "code";
              options.ResponseMode = "query";

              options.Scope.Clear();
              options.Scope.Add("openid");
              options.Scope.Add("profile");
              options.Scope.Add("api");
              options.Scope.Add("offline_access");

              options.MapInboundClaims = false;
              options.GetClaimsFromUserInfoEndpoint = true;
              options.SaveTokens = true;
          });

            services.AddControllersWithViews(options =>
                 options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddRazorPages().AddMvcOptions(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSecurityHeaders(
                SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
                    Configuration["OpenIDConnectSettings:Authority"]));

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
