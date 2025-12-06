namespace SV22T1080013.Admin.Models
{
    /// <summary>
    /// Biểu diễn cho dữ liệu đầu ra khi tìm kiếm dưới dạng phân trang (ViewModel)
    /// </summary>
    public class PaginationSearchResult<T>
    {
        /// <summary>
        /// Trang được hiển thị
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Giá trị tìm kiếm
        /// </summary>
        public string SearchValue { get; set; }
        /// <summary>
        /// Số dòng dữ liệu tìm kiếm được 
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Số trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize <= 0)
                    return 1;
                int p = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    p += 1;
                return p;
            }
        }

        /// <summary>
        /// Danh sách dữ liệu truy vấn được 
        /// </summary>
        public required IEnumerable<T> Data { get; set; }
    }

    public class OrderSearchResult<T> : PaginationSearchResult<T>
    {
        public int StatusID { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

    }
}
