namespace SV22T1080013.Admin.Models
{
    /// <summary>
    /// Dữ liệu trả về cho các API dưới dạng JSON
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Trả về 1 nếu thành công, 0 nếu không thành công
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Chuỗi thông báo kết quả hoặc lý do không thành công/lỗi
        /// </summary>
        public String Message { get; set; } = "";
        /// <summary>
        /// Dữ liệu trả về (nếu có)
        /// </summary>
        public object? Data { get; set; } = null;
    }
}
