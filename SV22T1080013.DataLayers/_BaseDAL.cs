using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace SV22T1080013.DataLayers
{
    /// <summary>
    /// Lớp cơ sở cho các lớp xử lý dữ liệu trên CSDL SQL Server
    /// </summary>
    public abstract class BaseDAL
    {
        protected string connectionString;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString">Chuỗi tham số kết nối đến CSDL</param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Mở kết nối đến CSDL
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
        /// <summary>
        /// Mở kết nối đến CSDL (bất đồng bộ)
        /// </summary>
        /// <returns></returns>
        protected async Task<SqlConnection> OpenConnectionAsync()
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
    }
}
