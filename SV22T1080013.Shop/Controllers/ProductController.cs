using Microsoft.AspNetCore.Mvc;

namespace SV22T1080013.Shop.Controllers
{
    public class ProductController : Controller
    {
        /// <summary>
        /// List product
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
