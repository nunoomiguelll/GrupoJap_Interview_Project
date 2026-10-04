using System.Diagnostics;
using GrupoJap.Rentals.Models;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Controllers;

public sealed class ErrorController : Controller
{
    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
