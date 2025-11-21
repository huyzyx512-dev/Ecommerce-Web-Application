using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080013.DomainModels
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Mã nhân viên
        /// </summary>
        public int EmployeeID { get; set; }
        /// <summary>
        /// Họ và tên nhân viên
        /// </summary>
        public string FullName { get; set; } = "";
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Địa chỉ nơi ở
        /// </summary>
        public string Address { get; set; } = "";
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; } = "";
        /// <summary>
        /// Email liên hệ và dùng để đăng nhập
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string Photo { get; set; } = "";
        /// <summary>
        /// Cho biết nhân viên có đang làm việc hay không?
        /// </summary>
        public bool IsWorking { get; set; }
    }
}
