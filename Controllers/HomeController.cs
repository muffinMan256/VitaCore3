using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitaCore.Models;

namespace VitaCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Features()
        {
            return View(new ContactFormModel());
        }

        //[HttpPost]
        //public IActionResult Features(ContactFormModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Handle form submission (send email, save to DB, etc.)
        //        return RedirectToAction("ContactConfirmation");
        //    }
        //    return View(model);
        //}

        public IActionResult About()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
