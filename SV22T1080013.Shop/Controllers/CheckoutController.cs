using Microsoft.AspNetCore.Mvc;

namespace SV22T1080013.Shop.Controllers
{
    public class CheckoutController : Controller
    {
        /// <summary>
        /// Hiển thị màn hình thanh toán
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Nhập địa chỉ
        /// </summary>
        /// <returns></returns>
        public IActionResult Address()
        {
            return View();
        }

        /// <summary>
        /// Thanh toán đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Payment() { return View(); }

        /// <summary>
        /// Hoàn tất Order
        /// </summary>
        /// <returns></returns>
        public IActionResult Confirm() { return View(); }

    }
}
