namespace SV22T1080013.Admin.Models
{
    /// <summary>
    /// Lớp thể hiện trả về khi gọi api
    /// </summary>
    public class ApiResult
    {
        public int Code { get; set; }
        public String Message { get; set; } = "";
        public object? Data { get; set; } = null;

        public static ApiResult ResultSuccess(String message, object data)
        {
            return new ApiResult { Code = 1, Message = message, Data = data };
        }
        public static ApiResult ResultFailed(String message)
        {
            return new ApiResult { Code = 0, Message = message};
        }
    }
}
