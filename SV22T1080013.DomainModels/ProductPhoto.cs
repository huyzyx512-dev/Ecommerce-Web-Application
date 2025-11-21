using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080013.DomainModels
{
    /// <summary>
    /// Ảnh sản phẩm
    /// </summary>
    public class ProductPhoto
    {
        /// <summary>
        /// Mã ảnh sản phẩm
        /// </summary>
        public long PhotoID { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Đường dẫn ảnh
        /// </summary>
        public string Photo { get; set; } = "";
        /// <summary>
        /// Mô tả ảnh
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Thứ tự hiển thị ảnh
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Cho biết ảnh có ẩn hay không?
        /// </summary>
        public bool IsHidden { get; set; }
    }
}
