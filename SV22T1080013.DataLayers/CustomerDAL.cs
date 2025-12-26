using BCrypt.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1080013.DomainModels;


namespace SV22T1080013.DataLayers
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến khách hàng
    /// </summary>
    /// <remarks>
    /// Ctor
    /// </remarks>
    /// <param name="connectionString"></param>
    public class CustomerDAL(string connectionString) : BaseDAL(connectionString)
    {

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang (Nếu pageSize=0 thì không phân trang)</param>
        /// <param name="searchValue">Tên khách hàng cần tìm (Nếu rỗng thì lấy toàn bộ)</param>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> ListAsync(int page = 1, int pageSize = 0, string searchValue = "")
        {
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using var connection = await OpenConnectionAsync();
            var sql = @"WITH cte AS
                        (
                            SELECT *,
                                   ROW_NUMBER() OVER (ORDER BY CustomerName) AS RowNumber
                            FROM Customers
                            WHERE
                                (@searchValue = N'' OR @searchValue IS NULL)
                                OR (CustomerName LIKE N'%' + @searchValue + N'%'
                                    OR ContactName LIKE N'%' + @searchValue + N'%')
                        )
                        SELECT *
                        FROM cte
                        WHERE RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize
                        ORDER BY RowNumber;";
            var parameters = new
            {
                page,
                pageSize,
                searchValue
            };
            // Thực thi câu lệnh SQL
            return await connection.QueryAsync<Customer>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Customer>> ListUserAsync()
        {
            using var connection = await OpenConnectionAsync();
            string sql = "SELECT * FROM Customers ORDER BY CustomerName ASC";

            return await connection.QueryAsync<Customer>(sql);
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id">Mã khách hàng</param>
        /// <returns></returns>
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "SELECT * FROM Customers WHERE CustomerID = @id";
            var parameters = new { id };
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Bổ sung khách hàng vào CSDL
        /// </summary>
        /// <param name="data">Đối tượng khách hàng cần bổ sung</param>
        /// <returns>Mã khách hàng</returns>
        public async Task<int> AddAsync(Customer data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"INSERT INTO Customers 
                            (CustomerName, ContactName, Address, Province, Phone, Email, IsLocked)
                            VALUES(@CustomerName, @ContactName, @Address, @Province, @Phone, @Email, @IsLocked);
                            SELECT SCOPE_IDENTITY();";
            // Thực thi câu lệnh SQL
            return await connection.ExecuteScalarAsync<int>(sql, data, commandType: System.Data.CommandType.Text);
        }


        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public async Task<bool> UpdateAsync(Customer data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"UPDATE Customers 
                            SET
                                CustomerName = @CustomerName,
                                ContactName = @ContactName,
                                Address = @Address,
                                Province = @Province,
                                Phone = @Phone,
                                Email = @Email,
                                IsLocked = @IsLocked
                            WHERE CustomerID = @CustomerID";
            // Thực thi câu lệnh SQL
            int rowsAffected = await connection.ExecuteAsync(sql, data, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa 1 khách hàng khỏi CSDL
        /// </summary>
        /// <param name="id">Ma khach hang</param>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "DELETE FROM Customers WHERE CustomerId = @id";
            var parameters = new { id };

            return (await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text)) > 0;
        }

        /// <summary>
        /// Đếm tổng số khách hàng
        /// </summary>
        public async Task<int> CountAsync(string searchValue = "")
        {
            searchValue = "%" + searchValue + "%";
            using var connection = await OpenConnectionAsync();
            var sql = @"SELECT    COUNT(*)
                            FROM    Customers
                            WHERE    CustomerName LIKE @searchValue OR ContactName LIKE @searchValue";
            var parameters = new
            {
                searchValue
            };
            // Thực thi câu lệnh SQL
            return await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Kiểm tra khách hàng có đơn hàng hay không
        /// </summary>
        public async Task<bool> InUsedAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"IF EXISTS(SELECT 1 FROM Orders WHERE CustomerID = @id)
                                    SELECT 1
                                ELSE
                                    SELECT 0;";

            var parameters = new { id };

            return await connection.ExecuteScalarAsync<bool>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập và mật khẩu.
        /// Nếu hợp lệ trả về thông tin của tài khoản, ngược lại thì trả về null
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Customer?> AuthenticateAsync(string email, string password)
        {
            using var conn = await OpenConnectionAsync();

            var user = await conn.QuerySingleOrDefaultAsync<Customer>(
                "SELECT * FROM Customers WHERE Email = @Email AND IsLocked = 0",
                new { Email = email });

            if (user == null)
                return null;

            // VERIFY HASH
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return user;
        }


        /// <summary>
        /// Đăng ký người dùng mới 
        /// </summary>
        /// <param name="fullName">CustomerName</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            using var connection = await OpenConnectionAsync();

            var sql = @"INSERT INTO Customers(CustomerName, ContactName, Email, Password, IsLocked)
                VALUES (@CustomerName, @CustomerName, @Email, @Password, 0);";

            try
            {
                await connection.ExecuteAsync(sql, new
                {
                    CustomerName = fullName,
                    Email = email,
                    Password = password
                });

                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Email đã tồn tại
                return false;
            }
        }


        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> ChangePasswordAsync(int userID, string oldPassword, string newPassword)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"UPDATE	Employees 
                        SET		Password = @newPassword 
                        WHERE	EmployeeID = @userID AND Password = @oldPassword";
            var parameters = new { userID, oldPassword, newPassword };
            return (await connection.ExecuteAsync(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)) > 0;
        }
    }
}
