using System.ComponentModel.DataAnnotations;

namespace SV22T1080013.Shop.Models
{
    public class AuthPageViewModel
    {
        public LoginViewModel? Login { get; set; }
        public RegisterViewModel? Register { get; set; }
        public bool? IsAuth { get; set; }
    }

    public class LoginViewModel
    {
        //[Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; } = "";

        //[Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = "";
    }
    
    public class RegisterViewModel
    {
        public string FullName { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        public string ConfirmPassword {  get; set; } = "";
    }

}
