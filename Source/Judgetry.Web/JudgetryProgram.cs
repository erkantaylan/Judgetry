using Judgetry.Core.Framework;
using Judgetry.Web.Components;
using Judgetry.Web.Components.Account;
using Judgetry.Web.Database;
using Judgetry.Web.Database.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Judgetry.Web;

public class JudgetryProgram
{
    public static void Main(string[] args)
    {
        var microApp = new MicroApp(args);

        microApp.RegisterApiDefaults();

        microApp.Register(
            builder =>
            {
                builder.Services
                       .AddRazorComponents()
                       .AddInteractiveServerComponents();

                builder.Services.AddCascadingAuthenticationState();
                builder.Services.AddScoped<IdentityUserAccessor>();
                builder.Services.AddScoped<IdentityRedirectManager>();
                builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

                builder.Services
                       .AddAuthentication(
                            options =>
                            {
                                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                            })
                       .AddIdentityCookies();

                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                builder.Services
                       .AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
                       .AddEntityFrameworkStores<JudgetryDbContext>()
                       .AddSignInManager()
                       .AddDefaultTokenProviders();

                builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();
            },
            app =>
            {
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                }

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<App>()
                   .AddInteractiveServerRenderMode();

                // Add additional endpoints required by the Identity /Account Razor components.
                app.MapAdditionalIdentityEndpoints();
            });

        microApp.RegisterBuilder(
            builder =>
            {
                builder.AddNpgsqlDbContext<JudgetryDbContext>(
                    "cs-judgetry",
                    configureDbContextOptions: dbOptions => dbOptions.EnableDetailedErrors()
                                                                     .EnableSensitiveDataLogging());
                builder.Services.AddMigration<JudgetryDbContext, JudgetrySeeds>();
                //builder.Services.AddMigration<JudgetryDbContext>();
                builder.Services.AddScoped<UnitOfWork>();
            });

        microApp.Run();
    }
}
