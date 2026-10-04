using GrupoJap.Rentals.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddCookieTempDataProvider(options => options.Cookie.Name = ".GrupoJap.Web.TempData")
    .AddDataAnnotationsLocalization();
builder.Services.AddRentalsCore(builder.Configuration, "Web");

var app = builder.Build();

await app.SeedRentalsAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRentalsUploads();
app.UseRentalsLocalization(multilingual: true);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers().WithStaticAssets();

app.Run();
