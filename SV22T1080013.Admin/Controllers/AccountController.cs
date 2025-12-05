using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.AppCodes;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DataLayers;
using System.Threading.Tasks;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.UserName = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập email và mật khẩu");
                return View();
            }

            //Kiểm tra thông tin đăng nhập
            var userAccount = await UserAccountService.EmployeeUserAccountDB.AuthenticateAsync(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Tạo thông tin để ghi trong "giấy chứng nhận"
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //Thiết lập phiên đăng nhập 
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Được gọi khi người dùng có truy cập khi không được cấp quyền
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            try
            {

                var user = User.GetUserData();

                var isChangedPassword = await UserAccountService.EmployeeUserAccountDB.ChangePasswordAsync(int.Parse(user?.UserId), oldPassword, newPassword);

                if (!isChangedPassword)
                {
                    ModelState.AddModelError("Error", "Đổi mật khẩu không thành công.");
                }

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
