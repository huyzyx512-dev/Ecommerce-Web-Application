using Dapper;
using SV22T1080013.DomainModels;

namespace SV22T1080013.DataLayers
{
    public class SupplierDAL : BaseDAL
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Tim kiếm và lấy danh sách nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Supplier>> ListAsync(int page = 1, int pageSize = 0, string searchValue = "")
        {
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using var connection = await OpenConnectionAsync();
            var sql = @"WITH cte AS
                            (
                                SELECT    *,
                                        ROW_NUMBER() OVER(ORDER BY SupplierName) AS RowNumber
                                FROM    Suppliers
                                WHERE    
                                        (@searchValue = N'' OR @searchValue IS NULL)
                                        OR (SupplierName LIKE @searchValue
                                            OR ContactName LIKE  @searchValue)
                            )
                            SELECT * FROM cte
                            WHERE (@pageSize = 0)
                                OR RowNumber BETWEEN (@page - 1)* @pageSize + 1 AND @page * @pageSize
                            ORDER BY RowNumber;";
            var parameters = new
            {
                page,
                pageSize,
                searchValue
            };
            // Thực thi câu lệnh SQL
            return await connection.QueryAsync<Supplier>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lấy thông tin nhà cung cấp theo mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Supplier?> GetAsync(int id)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var sql = "SELECT * FROM Suppliers WHERE SupplierID = @id";
                var parameters = new
                {
                    id
                };
                return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
        }

        /// <summary>
        /// Bo sung nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(Supplier data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"INSERT INTO Suppliers (SupplierName, ContactName, Province, Address, Phone, Email)
                               VALUES(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                               SELECT SCOPE_IDENTITY();";
            var parameters = new
            {
                data.SupplierName,
                data.ContactName,
                data.Province,
                data.Address,
                data.Phone,
                data.Email
            };
            int supplierId = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
            return supplierId;
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Supplier data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"UPDATE Suppliers
                               SET SupplierName = @SupplierName,
                                   ContactName = @ContactName,
                                   Province = @Province,
                                   Address = @Address,
                                   Phone = @Phone,
                                   Email = @Email
                               WHERE SupplierID = @SupplierID";
            int rowsAffected = await connection.ExecuteAsync(sql, data, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "DELETE FROM Suppliers WHERE SupplierID = @id";
            var parameters = new { id };
            int rowsAffected = await connection.ExecuteAsync(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Đếm số nhà cung
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(string searchValue = "")
        {
            //if (searchValue != "")
            searchValue = "%" + searchValue + "%";
            using var connection = await OpenConnectionAsync();
            string sql = "SELECT COUNT(*) FROM Suppliers WHERE SupplierName LIKE @searchValue";
            var parameters = new
            {
                searchValue
            };
            return await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Nhà cung cấp đã được sử dụng chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> InUsedAsync(int id)
        {
            using (var connection = await OpenConnectionAsync())
            {
                string sql = "SELECT COUNT(*) FROM Products WHERE SupplierID = @id";
                var parameters = new
                {
                    id
                };
                int count = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                return count > 0;
            }
        }

    }
}
