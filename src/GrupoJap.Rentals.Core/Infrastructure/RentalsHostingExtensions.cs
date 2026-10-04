using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace GrupoJap.Rentals.Infrastructure;

/// <summary>Configuração da aplicação de administração: base de dados, Identity, traduções e uploads.</summary>
public static class RentalsHostingExtensions
{
    /// <param name="appName">Prefixo usado nos cookies de sessão.</param>
    public static IServiceCollection AddRentalsCore(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = $".GrupoJap.{appName}.Auth";
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/account/access-denied";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        services.AddScoped<AvatarStorage>();
        services.AddScoped<RentalAvailabilityService>();

        // Traduções: guardadas na base de dados e usadas pelas vistas (@T["chave"]) e pelas DataAnnotations.
        services.AddSingleton<TranslationStore>();
        services.AddSingleton<Translator>();
        services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, TranslationLocalizerFactory>());
        return services;
    }

    /// <summary>Garante perfis, administrador inicial e dados de demonstração (todos idempotentes).</summary>
    public static async Task SeedRentalsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        await DemoDataSeeder.SeedAsync(scope.ServiceProvider);
        await TranslationSeeder.SeedAsync(scope.ServiceProvider);
    }

    /// <summary>
    /// Define o idioma de cada pedido a partir do cookie de idioma (<see cref="LanguageCookieName"/>), escolhido no
    /// seletor do painel. Sem escolha, usa o português, que é o idioma base.
    /// </summary>
    public static IApplicationBuilder UseRentalsLocalization(this WebApplication app)
    {
        var cultures = SiteLanguages.All.Select(language => new CultureInfo(language.Culture)).ToList();

        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(SiteLanguages.DefaultLanguage.Culture),
            SupportedCultures = cultures,
            SupportedUICultures = cultures
        };

        options.RequestCultureProviders.Clear();
        options.RequestCultureProviders.Add(new CookieRequestCultureProvider { CookieName = LanguageCookieName });

        return app.UseRequestLocalization(options);
    }

    public const string LanguageCookieName = ".GrupoJap.Language";

    /// <summary>Serve /uploads a partir da pasta partilhada (Uploads:Path).</summary>
    public static IApplicationBuilder UseRentalsUploads(this WebApplication app)
    {
        var root = AvatarStorage.ResolveUploadsRoot(app.Environment, app.Configuration);
        Directory.CreateDirectory(root);
        return app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(root),
            RequestPath = "/uploads"
        });
    }
}
