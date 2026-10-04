using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoJap.Rentals.Tests.Controllers;

/// <summary>
/// Monta os controllers do painel como num pedido real: base de dados em memória (nova em cada teste),
/// traduções reais (catálogo) e TempData. <see cref="Validate"/> reproduz a validação que o MVC faz antes da action.
/// </summary>
internal sealed class ControllerTestContext : IDisposable
{
    private readonly ServiceProvider _root;
    private readonly IServiceScope _scope;

    public ControllerTestContext()
    {
        var databaseName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddScoped<RentalAvailabilityService>();
        services.AddSingleton<TranslationStore>();
        services.AddSingleton<Translator>();

        _root = services.BuildServiceProvider();
        _scope = _root.CreateScope();
    }

    public IServiceProvider Services => _scope.ServiceProvider;

    /// <summary>O contexto usado pelos controllers (o mesmo de um pedido).</summary>
    public ApplicationDbContext Db => Services.GetRequiredService<ApplicationDbContext>();

    public Translator T => Services.GetRequiredService<Translator>();

    public RentalAvailabilityService Availability => Services.GetRequiredService<RentalAvailabilityService>();

    /// <summary>Grava dados de partida num contexto à parte, para o controller começar sem nada em memória.</summary>
    public void Seed(params object[] entities)
    {
        using var scope = _root.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.AddRange(entities);
        db.SaveChanges();
    }

    /// <summary>Lê o estado gravado num contexto novo (como faria o pedido seguinte).</summary>
    public T Query<T>(Func<ApplicationDbContext, T> query)
    {
        using var scope = _root.CreateScope();
        return query(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }

    public TController Setup<TController>(TController controller) where TController : Controller
    {
        var httpContext = new DefaultHttpContext { RequestServices = Services };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.TempData = new TempDataDictionary(httpContext, new NullTempDataProvider());
        controller.Url = new FakeUrlHelper(controller.ControllerContext);
        return controller;
    }

    /// <summary>Valida o modelo como o MVC (atributos e, se passarem, <see cref="IValidatableObject"/>) e copia os erros para o ModelState.</summary>
    public void Validate(Controller controller, object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model, Services, items: null), results, validateAllProperties: true);
        foreach (var result in results)
        {
            foreach (var member in result.MemberNames.DefaultIfEmpty(string.Empty))
            {
                controller.ModelState.AddModelError(member, result.ErrorMessage ?? string.Empty);
            }
        }
    }

    public void Dispose()
    {
        _scope.Dispose();
        _root.Dispose();
    }

    /// <summary>Gera URLs simples (/controller/action) e aceita como locais só os caminhos que começam por "/".</summary>
    private sealed class FakeUrlHelper(ActionContext actionContext) : IUrlHelper
    {
        public ActionContext ActionContext { get; } = actionContext;

        public string? Action(UrlActionContext context) => $"/{context.Controller}/{context.Action}";

        public string? Content(string? contentPath) => contentPath;

        public bool IsLocalUrl(string? url) => !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\");

        public string? Link(string? routeName, object? values) => null;

        public string? RouteUrl(UrlRouteContext routeContext) => null;
    }

    private sealed class NullTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}
