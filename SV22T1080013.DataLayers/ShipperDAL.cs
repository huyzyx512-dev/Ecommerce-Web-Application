using Dapper;
using SV22T1080013.DomainModels;

namespace SV22T1080013.DataLayers
{
    public class ShipperDAL(string connectionString) : BaseDAL(connectionString)
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách shipper dưới dạng phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Shipper>> ListAsync(int page = 1, int pageSize = 0, string searchValue = "")
        {
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using var connection = await OpenConnectionAsync();
            var sql = @"WITH cte AS
                            (
                                SELECT    *,
                                        ROW_NUMBER() OVER(ORDER BY ShipperName) AS RowNumber
                                FROM    Shippers
                                WHERE    (@searchValue = N'' OR @searchValue is NULL) OR ShipperName LIKE @searchValue
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
            return await connection.QueryAsync<Shipper>(sql, parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lấy thông tin shipper theo mã
        /// </summary>
        /// <param name="id">Mã người giao hàng</param>
        /// <returns></returns>
        public async Task<Shipper?> GetAsync(int id)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var sql = "SELECT * FROM Shippers WHERE ShipperID = @id";
                var parameters = new
                {
                    id
                };
                return await connection.QueryFirstOrDefaultAsync<Shipper>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
        }

        /// <summary>
        /// Bổ sung người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(Shipper data)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var sql = @"INSERT INTO Shippers(ShipperName, Phone)
                            VALUES(@ShipperName, @Phone);
                            SELECT SCOPE_IDENTITY();";
                var parameters = new
                {
                    data.ShipperName,
                    data.Phone
                };
                int shipperId = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                return shipperId;
            }
        }

        /// <summary>
        /// Cập nhật người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Shipper data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"UPDATE Shippers
                            SET ShipperName = @ShipperName,
                                Phone = @Phone
                            WHERE ShipperID = @ShipperID";
            var parameters = new
            {
                data.ShipperID,
                data.ShipperName,
                data.Phone
            };
            int rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa người giao hàng
        /// </summary>
        /// <param name="id">Mã người giao hàng</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            var sql = "DELETE FROM Shippers WHERE ShipperID = @id";
            var parameters = new
            {
                id
            };
            int rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Số lượng người giao hàng theo tìm kiếm
        /// </summary>
        /// <param name="searchValue">Tên người giao hàng</param>
        /// <returns></returns>
        public async Task<int> CountAsync(string searchValue = "")
        {
            searchValue = "%" + searchValue + "%";
            using var connection = await OpenConnectionAsync();
            var sql = "SELECT COUNT(*) FROM Shippers WHERE ShipperName LIKE @searchValue";
            var parameters = new
            {
                searchValue
            };
            int count = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
            return count;
        }

        /// <summary>
        /// Kiểm tra người giao hàng có đang được sử dụng hay không
        /// </summary>
        /// <param name="id">Mã người giao hàng</param>
        /// <returns></returns>
        public async Task<bool> InUsedAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            var sql = "SELECT CASE WHEN EXISTS(SELECT * FROM Orders WHERE ShipperID = @id) THEN 1 ELSE 0 END";
            var parameters = new
            {
                id
            };
            int inUsed = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
            return inUsed > 0;
        }
    }
}
