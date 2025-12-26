using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV22T1080013.Shop.AppCodes
{
    /// <summary>
    /// Thông tin tài khoản người dùng được lưu trong phiên đăng nhập (cookie)
    /// </summary>
    public class WebUserData
    {
        public string? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactName { get; set; }
        public string? Province { get; set; }
        public string? Address {  get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Photo { get; set; }

        /// <summary>
        /// Thông tin người dùng dưới dạng danh sách các Claim
        /// </summary>
        /// <returns></returns>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims =
                [
                    new Claim(nameof(UserId), UserId ?? ""),
                    new Claim(nameof(CustomerName), CustomerName ?? ""),
                    new Claim(nameof(ContactName), ContactName ?? ""),
                    new Claim(nameof(Province), Province ?? ""),
                    new Claim(nameof(Address), Address ?? ""),
                    new Claim(nameof(Phone), Phone ?? ""),
                    new Claim(nameof(Email), Email ?? ""),
                    new Claim(nameof(Photo), Photo ?? ""),
                ];
               
                return claims;
            }
        }

        /// <summary>
        /// Tạo Principal dựa trên thông tin của người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            var claimIdentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);
            return claimPrincipal;
        }
    }

    /// <summary>
    /// Định nghĩa tên của các role sử dụng trong phân quyền chức năng cho người dùng
    /// </summary>
    public class WebUserRoles
    {
        public const string Administrator = "admin";
        public const string Employee = "employee";
        public const string Customer = "customer";
    }

    /// <summary>
    /// Extension các phương thức cho các đối tượng liên quan đến xác thực tài khoản người dùng
    /// </summary>
    public static class WebUserExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;

                var userData = new WebUserData();

                userData.UserId = principal.FindFirstValue(nameof(userData.UserId));
                userData.ContactName = principal.FindFirstValue(nameof(userData.ContactName));
                userData.CustomerName = principal.FindFirstValue(nameof(userData.CustomerName));
                userData.Email = principal.FindFirstValue(nameof(userData.Email));
                userData.Province= principal.FindFirstValue(nameof(userData.Province));
                userData.Phone= principal.FindFirstValue(nameof(userData.Phone));
                userData.Address = principal.FindFirstValue(nameof(userData.Address));
                userData.Photo = principal.FindFirstValue(nameof(userData.Photo));

                return userData;
            }
            catch
            {
                return null;
            }

        }
    }
}
