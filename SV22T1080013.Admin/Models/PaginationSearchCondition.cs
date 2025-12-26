namespace SV22T1080013.Admin.Models
{
    /// <summary>
    /// Đầu vào sử dụng cho tìm kiếm và phân trang dữ liệu
    /// </summary>
    public class PaginationSearchCondition
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Giá trị cần tìm
        /// </summary>
        public string SearchValue { get; set; } = "";
    }

    public class ProductSearchCondition : PaginationSearchCondition
    {
        /// <summary>
        /// Mã loại hàng cần tìm
        /// </summary>
        public int CategoryID { get; set; } = 0;
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public int SupplierID { get; set; } = 0;
        /// <summary>
        /// Giá thấp nhất để tìm kiếm sản phẩm
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// Giá cao nhất để tìm kiếm sản phẩm
        /// </summary>
        public decimal MaxPrice { get; set; }
        /// <summary>
        /// Danh sách loại hàng
        /// </summary>
        public int ProductID { get; set; }
    }

    public class OrderSearchCondition : PaginationSearchCondition
    {
        public int StatusID { get; set; } 

        public string DateRange { get; set; } = "";

        public DateTime FromDate
        {
            get
            {
                string[] values = DateRange.Split('-');
                DateTime d = DateTime.Parse(values[0].Trim());
                return d;
            }
        }

        public DateTime ToDate
        {
            get
            {
                string[] values = DateRange.Split('-');
                DateTime d = DateTime.Parse(values[1].Trim());
                return d;
            }
        }
    }
}
