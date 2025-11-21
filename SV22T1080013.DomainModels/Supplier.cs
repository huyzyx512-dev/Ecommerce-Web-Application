using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080013.DomainModels
{
    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public int SupplierID { get; set; }
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; } = "";
        /// <summary>
        /// Tên giao dịch
        /// </summary>
        public string ContactName { get; set; } = "";
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public string Province { get; set; } = "";
        /// <summary>
        /// Địa chỉ nhà cung cấp
        /// </summary>
        public string Address { get; set; } = "";
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; } = "";
        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public string Email { get; set; } = ""; 
    }
}
