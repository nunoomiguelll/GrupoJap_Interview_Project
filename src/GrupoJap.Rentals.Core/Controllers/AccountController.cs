using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Controllers;

[Route("account")]
public sealed class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    Translator T) : Controller
{
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Index() => LocalRedirect(HomePath);

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAfterLogin(returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(model.Email.Trim());
        if (user is not null && AdminOnly && !await userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            // Na administração, contas sem perfil Admin recebem a mesma resposta que credenciais erradas.
            user = null;
        }

        if (user is not null)
        {
            var result = await signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectAfterLogin(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, T["account.error.locked"]);
                return View(model);
            }
        }

        // Mensagem genérica: não revela se o email existe.
        ModelState.AddModelError(string.Empty, T["account.error.invalid"]);
        return View(model);
    }

    [HttpGet("register")]
    [AllowAnonymous]
    public IActionResult Register() => !AllowRegistration ? NotFound() : User.Identity?.IsAuthenticated == true
        ? LocalRedirect(HomePath)
        : View(new RegisterViewModel());

    [HttpPost("register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!AllowRegistration)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        var user = new ApplicationUser { UserName = email, Email = email, FullName = model.FullName.Trim() };
        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TranslateError(error));
            }

            return View(model);
        }

        // O registo público cria sempre utilizadores normais; o perfil Admin só é atribuído internamente.
        await userManager.AddToRoleAsync(user, AppRoles.User);
        await signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect(HomePath);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("access-denied")]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    private string HomePath => configuration["Account:HomePath"] is { Length: > 0 } path && path.StartsWith('/') ? path : "/";

    private bool AdminOnly => configuration.GetValue<bool>("Account:AdminOnly");

    private bool AllowRegistration => configuration.GetValue<bool>("Account:AllowRegistration");

    private IActionResult RedirectAfterLogin(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : LocalRedirect(HomePath);

    private string TranslateError(IdentityError error) => error.Code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => T["account.error.duplicate"],
        "PasswordTooShort" => T["password.error.too_short"],
        "PasswordRequiresDigit" => T["password.error.digit"],
        "PasswordRequiresUpper" => T["password.error.upper"],
        "PasswordRequiresLower" => T["password.error.lower"],
        "PasswordRequiresNonAlphanumeric" => T["password.error.symbol"],
        "InvalidEmail" or "InvalidUserName" => T["account.error.invalid_email"],
        _ => error.Description
    };
}
