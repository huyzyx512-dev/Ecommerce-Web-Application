namespace SV22T1080013.BusinessLayers
{
    /// <summary>
    /// Dùng để khởi tạo và lưu các thông tin cấu hình cho tâng nghiệp vụ
    /// </summary>
    public static class Configuration
    {
        private static string connectionString = "";

        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }

        /// <summary>
        /// Chuỗi tham số kết nối đến csdl
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }
    }
}
