// devolver un pdf: https://learn.microsoft.com/es-es/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.physicalfile?view=aspnetcore-8.0#microsoft-aspnetcore-mvc-controllerbase-physicalfile(system-string-system-string)

using Microsoft.AspNetCore.Mvc;
using Proyecto1_EliasCorderoLeon.Models;
using System.Diagnostics;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Manual()
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "Manual", "Manual.pdf");

            return PhysicalFile(ruta, "application/pdf");
        }
    }
}
