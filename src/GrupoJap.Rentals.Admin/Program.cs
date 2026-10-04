using GrupoJap.Rentals.Infrastructure;
using GrupoJap.Rentals.Models;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddCookieTempDataProvider(options => options.Cookie.Name = ".GrupoJap.Admin.TempData")
    .AddDataAnnotationsLocalization();
builder.Services.AddRentalsCore(builder.Configuration, "Admin");

// Toda a aplicação é reservada a administradores; só o login e o acesso negado são anónimos.
var adminOnly = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(AppRoles.Admin).Build();
builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(adminOnly)
    .SetFallbackPolicy(adminOnly);

var app = builder.Build();

await app.SeedRentalsAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRentalsUploads();
app.UseRentalsLocalization(multilingual: false);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
