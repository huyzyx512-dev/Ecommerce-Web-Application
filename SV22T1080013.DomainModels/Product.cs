using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080013.DomainModels
{
    /// <summary>
    /// Sản phẩm
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; } = "";
        /// <summary>
        /// Mô tả sản phẩm
        /// </summary>
        public string ProductDescription { get; set; } = "";
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public int SupplierID { get; set; }
        /// <summary>
        /// Mã loại sản phẩm
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; } = "";
        /// <summary>
        /// Giá bán sản phẩm
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Đường dẫn ảnh đại diện cho sản phẩm
        /// </summary>
        public string Photo { get; set; } = "";
        /// <summary>
        /// Cho biết sản phẩm có đang bán hay không?
        /// </summary>
        public bool IsSelling { get; set; }
    }
}
