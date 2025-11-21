using Dapper;
using SV22T1080013.DomainModels;
using System.Data.SqlTypes;

namespace SV22T1080013.DataLayers
{
    public class EmployeeDAL : BaseDAL
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhân viên dưới dạng phân trang
        /// </summary>
        /// <param name="page">Số trang muốn lấy</param>
        /// <param name="pageSize">Số dòng muốn lấy nếu không có lấy hết</param>
        /// <param name="searchValue">Tên của nhân viên</param>
        /// <returns></returns>
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(int page = 1, int pageSize = 0, string searchValue = "")
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 6;
            searchValue = $"%{searchValue}%";
            using var connection = await OpenConnectionAsync();
            string sql = @"WITH emp AS
                                (
                                    SELECT    *,
                                            ROW_NUMBER() OVER (ORDER BY FullName) AS RowNumber
                                    FROM    Employees
                                    WHERE   (@searchValue = N'' OR @searchValue IS NULL) OR FullName LIKE @searchValue
                                )
                                SELECT * FROM emp
                                WHERE    (@PageSize = 0)
                                    OR    (RowNumber BETWEEN (@page - 1)* @pageSize + 1 AND @page * @pageSize)
                                ORDER BY RowNumber;";
            var parameters = new
            {
                page,
                pageSize,
                searchValue
            };
            return await connection.QueryAsync<Employee>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Bổ sung nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(Employee data)
        {
            using (var connection = await OpenConnectionAsync())
            {
                string sql = @"INSERT INTO Employees (FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
                               VALUES(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);
                               SELECT SCOPE_IDENTITY();";
                var parameters = new
                {
                    data.FullName,
                    data.BirthDate,
                    data.Address,
                    data.Phone,
                    data.Email,
                    data.Photo,
                    data.IsWorking
                };
                int employeeId = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                return employeeId;
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="data">Dữ liệu nhân viên</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Employee data)
        {
            try
            {
                using var connection = await OpenConnectionAsync();
                string sql = @"UPDATE Employees
                               SET FullName = @FullName,
                                   BirthDate = @BirthDate,
                                   Address = @Address,
                                   Phone = @Phone,
                                   Email = @Email,
                                   Photo = @Photo,
                                   IsWorking = @IsWorking
                               WHERE EmployeeID = @EmployeeID";

                var parameters = new
                {
                    data.FullName,
                    data.BirthDate,
                    data.Address,
                    data.Phone,
                    data.Email,
                    data.Photo,
                    data.IsWorking,
                    data.EmployeeID,
                };
                int rowsAffected = await connection.ExecuteAsync(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            var parameters = new { EmployeeID = employeeId };
            int rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0; // <-- True if deleting is success
        }

        /// <summary>
        /// Lấy thông tin nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";
            var parameters = new { EmployeeID = employeeId };
            return await connection.QueryFirstOrDefaultAsync<Employee>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Số lượng nhân viên tìm được theo từ khóa
        /// </summary>
        /// <param name="searchValue">Tên của nhân viên</param>
        /// <returns></returns>
        public async Task<int> CountAsync(string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            using (var connection = await OpenConnectionAsync())
            {
                string sql = "SELECT COUNT(*) FROM Employees WHERE FullName LIKE @searchValue";
                var parameters = new { searchValue };
                return await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<bool> InUseAsync(int id = 0)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"IF EXISTS(select EmployeeID from Orders where EmployeeID = @id)
                                SELECT 1
	                        ELSE
                                SELECT 0;";
            var parameters = new
            {
                id
            };

            return await connection.ExecuteScalarAsync<bool>(sql, parameters, commandType: System.Data.CommandType.Text); // <-- True if have
        }
    }
}
