using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Shop.Models;
using System.Diagnostics;

namespace SV22T1080013.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị Homepage
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
      
    }
}
