using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.BusinessLayers;
using SV22T1080013.Shop.AppCodes;
using SV22T1080013.Shop.Models;
using System.Threading.Tasks;

namespace SV22T1080013.Shop.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Hiển thị page order của user
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //TODO: Thêm page Theo dõi trạng thái xử lý của đơn hàng.
            return View();
        }

        /// <summary>
        /// Hiển thị trang đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.ActiveTab = "login";
            return View(new AuthPageViewModel());
        }

        /// <summary>
        /// Thực hiện đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(AuthPageViewModel request)
        {
            ViewBag.ActiveTab = "login";

            if (string.IsNullOrWhiteSpace(request.Login?.Email))
                ModelState.AddModelError("Login.Email", "Email không được để trống");

            if (string.IsNullOrWhiteSpace(request.Login?.Password))
                ModelState.AddModelError("Login.Password", "Mật khẩu không được để trống");

            if (!ModelState.IsValid)
                return View(request);

            var user = await UserAccountService.CustomerUserAccountDB
                .AuthenticateAsync(request.Login.Email, request.Login.Password);

            if (user == null)
            {
                ModelState.AddModelError("Login.Password", "Email hoặc mật khẩu không đúng");
                return View(request);
            }

            WebUserData cus = new()
            {
                UserId = user.CustomerID.ToString(),
                ContactName = user.ContactName,
                CustomerName = user.CustomerName,
                Province = user.Province,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone
                //TODO: Photo
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                cus.CreatePrincipal()
            );

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Hiển thị trang đăng ký
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register(bool registered = false)
        {
            ViewBag.ActiveTab = "register";
            ModelState.Clear(); // Xóa hết lỗi cũ
            return View("Login", new AuthPageViewModel
            {
                IsAuth = registered
            });
        }

        /// <summary>
        /// Thực hiện đăng ký
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(AuthPageViewModel request)
        {
            ViewBag.ActiveTab = "register";

            if (request?.Register == null)
            {
                ModelState.AddModelError("", "Dữ liệu đăng ký không hợp lệ.");
                return View("Login", request ?? new AuthPageViewModel());
            }

            // Validate
            if (string.IsNullOrWhiteSpace(request.Register.FullName))
                ModelState.AddModelError("Register.FullName", "Vui lòng nhập tên người dùng");

            if (string.IsNullOrWhiteSpace(request.Register.Email))
                ModelState.AddModelError("Register.Email", "Vui lòng nhập địa chỉ email");

            if (string.IsNullOrWhiteSpace(request.Register.Password))
                ModelState.AddModelError("Register.Password", "Vui lòng nhập mật khẩu");

            if (!string.IsNullOrWhiteSpace(request.Register.Password) &&
                request.Register.Password.Length < 6)
                ModelState.AddModelError("Register.Password", "Mật khẩu phải có ít nhất 6 ký tự");

            if (string.IsNullOrWhiteSpace(request.Register.ConfirmPassword))
                ModelState.AddModelError("Register.ConfirmPassword", "Vui lòng nhập xác nhận mật khẩu");

            if (!string.IsNullOrWhiteSpace(request.Register.Password) &&
                !string.IsNullOrWhiteSpace(request.Register.ConfirmPassword) &&
                request.Register.Password != request.Register.ConfirmPassword)
                ModelState.AddModelError("Register.ConfirmPassword", "Mật khẩu xác nhận không đúng");

            if (!ModelState.IsValid)
                return View("Login", request);

            // HASH PASSWORD
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                request.Register.Password,
                workFactor: 11 // 10–12 là hợp lý
            );

            // Register DB
            var success = await UserAccountService.customerUserAccountDB
                .RegisterAsync(
                    request.Register.FullName,
                    request.Register.Email,
                    hashedPassword
                );

            if (!success)
            {
                ModelState.AddModelError("Register.Email", "Email đã tồn tại");
                return View("Login", request);
            }

            // Thành công
            return RedirectToAction("Login", new { registered = true });
        }

        /// <summary>
        /// Thực hiện đăng xuất tài khoản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout() 
        {
            ViewBag.ActiveTab = "login";
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Hiển thị page profile người dùng
        /// </summary>
        /// <returns></returns>
        public IActionResult Profile()
        {
            // Thêm đổi mật khẩu cho customer
            return View();
        }

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

        //TODO: thêm page add new payment

        // GET: Hiển thị partial view chứa form add address
        public IActionResult AddNewAddress()
        {
            return View(); // Trả về partial view
        }

        // GET: Edit một address cụ thể
        public IActionResult EditAddress(int id)
        {
            // Lấy data address theo id...
            var model = new AddressViewModel { };
            return PartialView("_AddAddressForm", model); // Có thể dùng chung form
        }
    }
}
