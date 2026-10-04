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

/// <summary>Configuração comum às duas aplicações (site público e administração): base de dados, Identity e uploads.</summary>
public static class RentalsHostingExtensions
{
    /// <param name="appName">Prefixo usado nos cookies, para que o site e a administração tenham sessões independentes.</param>
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
        services.AddScoped<BookingService>();

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
    /// Define o idioma de cada pedido. Com <paramref name="multilingual"/> (site público), usa o cookie de idioma
    /// e depois o idioma do browser; caso contrário (administração), fica sempre em português.
    /// </summary>
    public static IApplicationBuilder UseRentalsLocalization(this WebApplication app, bool multilingual)
    {
        var cultures = (multilingual ? SiteLanguages.All : SiteLanguages.All.Where(l => l.Code == SiteLanguages.Default))
            .Select(language => new CultureInfo(language.Culture))
            .ToList();

        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(cultures[0]),
            SupportedCultures = cultures,
            SupportedUICultures = cultures,
            FallBackToParentCultures = true,
            FallBackToParentUICultures = true
        };

        if (!multilingual)
        {
            options.RequestCultureProviders.Clear();
        }
        else
        {
            // "en-US" ou "es-MX" no browser correspondem a en-GB e es-ES.
            options.RequestCultureProviders.Insert(2, new LanguageCodeCultureProvider());
            options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().Single().CookieName = LanguageCookieName;
        }

        return app.UseRequestLocalization(options);
    }

    public const string LanguageCookieName = ".GrupoJap.Language";

    /// <summary>Associa o idioma do browser (só as duas letras) à cultura suportada desse idioma.</summary>
    private sealed class LanguageCodeCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var header = httpContext.Request.GetTypedHeaders().AcceptLanguage;
            var match = header
                .OrderByDescending(value => value.Quality ?? 1)
                .Select(value => value.Value.Value?.Split('-')[0].ToLowerInvariant())
                .Select(code => SiteLanguages.All.FirstOrDefault(language => language.Code == code))
                .FirstOrDefault(language => language is not null);

            return Task.FromResult(match is null ? null : new ProviderCultureResult(match.Culture));
        }
    }

    /// <summary>Serve /uploads a partir da pasta partilhada (Uploads:Path), comum ao site e à administração.</summary>
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
