using Microsoft.AspNetCore.Mvc;

namespace SV22T1080013.Admin.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }

        public IActionResult Shipping()
        {
            return View();
        }

        public IActionResult UpdateDetail(int id)
        {
            return View();
        }

        public IActionResult Accept(int id)
        {
            return View();
        }

        public IActionResult Finish(int id)
        {
            return View();
        }

        public IActionResult Cancel(int id)
        {
            return View();
        }

        public IActionResult Reject(int id)
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult EditDetail(int id, int productId = 0)
        {
            return View();
        }

        public IActionResult DeleteDetail(int id, int productId = 0)
        {
            return View();
        }

        public IActionResult RemoveFromCart(int id)
        {
            return View();
        }

        public IActionResult ClearCart()
        {
            return View();
        }

        public IActionResult Init()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
