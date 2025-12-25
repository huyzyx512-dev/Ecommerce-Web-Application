using Microsoft.AspNetCore.Mvc;

namespace SV22T1080013.Shop.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Hiển thị profile user
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Hiển thị trang đăng nhập
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Thực hiện đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string username, string password) { return View(); }

        /// <summary>
        /// Hiển thị trang đăng ký
        /// </summary>
        /// <returns></returns>
        public IActionResult Register() { return View(); }

        /// <summary>
        /// Thực hiện đăng ký
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register(string username, string password, string displayName, string phone) { return View(); }

        /// <summary>
        /// Thực hiện đăng xuất tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Logout() {  return View(); }

        /// <summary>
        /// Hiển thị page profile người dùng
        /// </summary>
        /// <returns></returns>
        public IActionResult Profile() { return View(); }
        
        /// <summary>
        /// Hiển thị page địa chỉ giao hàng người dùng
        /// </summary>
        /// <returns></returns>
        public IActionResult Address() { return View(); }
        
        /// <summary>
        /// Hiển thị page phương thức thanh toán
        /// </summary>
        /// <returns></returns>
        public IActionResult Payments() { return View(); }


    }
}
