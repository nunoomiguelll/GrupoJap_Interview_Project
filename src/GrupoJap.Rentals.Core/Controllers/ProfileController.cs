using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

[Authorize]
[Route("profile")]
public sealed class ProfileController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext dbContext,
    AvatarStorage avatarStorage,
    Translator T) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        return View(await BuildViewModelAsync(user));
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        string? newPhotoPath = null;
        if (model.Photo is { Length: > 0 })
        {
            var (path, error) = await avatarStorage.SaveAsync(model.Photo);
            if (error is not null)
            {
                ModelState.AddModelError(nameof(model.Photo), T[error]);
            }
            else
            {
                newPhotoPath = path;
            }
        }

        if (!ModelState.IsValid)
        {
            // Descarta uma foto já gravada se o resto do formulário falhou.
            avatarStorage.Delete(newPhotoPath);
            await FillReadOnlyAsync(model, user);
            return View(model);
        }

        user.FullName = model.FullName.Trim();
        user.JobTitle = NullIfBlank(model.JobTitle);
        user.FavoriteBrand = NullIfBlank(model.FavoriteBrand);
        user.Bio = NullIfBlank(model.Bio);

        var phone = NullIfBlank(model.PhoneNumber);
        if (phone != user.PhoneNumber)
        {
            var phoneResult = await userManager.SetPhoneNumberAsync(user, phone);
            if (!phoneResult.Succeeded)
            {
                avatarStorage.Delete(newPhotoPath);
                foreach (var error in phoneResult.Errors)
                {
                    ModelState.AddModelError(nameof(model.PhoneNumber), error.Description);
                }

                await FillReadOnlyAsync(model, user);
                return View(model);
            }
        }

        var oldPhotoPath = user.PhotoPath;
        if (newPhotoPath is not null)
        {
            user.PhotoPath = newPhotoPath;
        }
        else if (model.RemovePhoto)
        {
            user.PhotoPath = null;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            avatarStorage.Delete(newPhotoPath);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await FillReadOnlyAsync(model, user);
            return View(model);
        }

        if (user.PhotoPath != oldPhotoPath)
        {
            avatarStorage.Delete(oldPhotoPath);
        }

        TempData["SuccessMessage"] = T["profile.saved"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("password")]
    public IActionResult Password() => View(new ChangePasswordViewModel());

    [HttpPost("password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Password(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TranslateError(error));
            }

            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["SuccessMessage"] = T["password.changed"].Value;
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProfileViewModel> BuildViewModelAsync(ApplicationUser user)
    {
        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            FavoriteBrand = user.FavoriteBrand,
            Bio = user.Bio
        };

        await FillReadOnlyAsync(model, user);
        return model;
    }

    private async Task FillReadOnlyAsync(ProfileViewModel model, ApplicationUser user)
    {
        model.Email = user.Email ?? string.Empty;
        model.PhotoPath = user.PhotoPath;
        model.Initial = user.Initial;
        model.RoleLabel = T[User.IsInRole(AppRoles.Admin) ? "profile.role_admin" : "profile.role_user"];
        model.BrandSuggestions = await dbContext.Vehicles
            .AsNoTracking()
            .Select(v => v.Brand)
            .Distinct()
            .OrderBy(brand => brand)
            .ToListAsync();
    }

    private static string? NullIfBlank(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private string TranslateError(IdentityError error) => error.Code switch
    {
        "PasswordMismatch" => T["password.error.current_wrong"],
        "PasswordTooShort" => T["password.error.too_short"],
        "PasswordRequiresDigit" => T["password.error.digit"],
        "PasswordRequiresUpper" => T["password.error.upper"],
        "PasswordRequiresLower" => T["password.error.lower"],
        "PasswordRequiresNonAlphanumeric" => T["password.error.symbol"],
        _ => error.Description
    };
}
