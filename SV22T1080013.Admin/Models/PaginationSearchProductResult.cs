using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Models
{
    public class PaginationSearchProductResult<T> : PaginationSearchResult<T>
    {
        // Mã loại hàng, mã nhà cung cấp, giá min, giá max
        public int CategoryID { get; set; } 
        public int SupplierID { get; set; } 
        public decimal MinPrice { get; set; } 
        public decimal MaxPrice { get; set; }
        /// <summary>
        /// Danh sách loại hàng
        /// </summary>
        public IEnumerable<Category> Categories { get; set; }
        /// <summary>
        /// Danh sách nhà cung cấp
        /// </summary>
        public IEnumerable<Supplier> Suppliers { get; set; }
    }
}
