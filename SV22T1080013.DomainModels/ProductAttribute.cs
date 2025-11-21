using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080013.DomainModels
{
    /// <summary>
    /// Thuộc tính sản phẩm
    /// </summary>
    public class ProductAttribute
    {
        /// <summary>
        /// Mã thuộc tính sản phẩm
        /// </summary>
        public long AttributeID { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Tên thuộc tính
        /// </summary>
        public string AttributeName { get; set; } = "";
        /// <summary>
        /// Giá trị thuộc tính
        /// </summary>
        public string AttributeValue { get; set; } = "";
        /// <summary>
        /// Thứ tự hiển thị thuộc tính
        /// </summary>
        public int DisplayOrder { get; set; } 
    }
}
