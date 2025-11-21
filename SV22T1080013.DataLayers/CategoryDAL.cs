using Azure;
using Dapper;
using SV22T1080013.DomainModels;
using System.Data;

namespace SV22T1080013.DataLayers
{
    public class CategoryDAL : BaseDAL
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        // Tim kiem va hien thi theo phan trang
        public async Task<IEnumerable<Category>> GetCategoriesAsync(int page = 1, int pageSize = 0, string searchValue = "")
        {
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 10;
            searchValue = $"%{searchValue}%";

            #region declare SQL string
            using var connection = await OpenConnectionAsync();
            string sql = @"WITH ctg AS
                                (
                                    SELECT    *,
                                            ROW_NUMBER() OVER(ORDER BY CategoryName) AS RowNumber
                                    FROM    Categories
                                    WHERE   (@searchValue = N'' OR  @searchValue IS NULL) OR CategoryName LIKE @searchValue
                                )
                                SELECT * FROM ctg
                                WHERE    (@pageSize = 0)
                                    OR    (RowNumber BETWEEN (@page - 1)* @pageSize + 1 AND @page * @pageSize)
                                ORDER BY RowNumber;";
            var parameters = new
            {
                page,
                pageSize,
                searchValue
            };
            #endregion

            return await connection.QueryAsync<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Bo sung loai hang
        /// </summary>
        /// <param name="data">Dữ liệu loại hàng</param>
        /// <returns>Mã loại hàng</returns>
        public async Task<int> AddCategoryAsync(Category data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"INSERT INTO Categories (CategoryName, Description)
                               VALUES(@CategoryName, @Description);
                               SELECT SCOPE_IDENTITY();";
            var parameters = new
            {
                data.CategoryName,
                data.Description
            };
            int categoryId = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
            return categoryId;
        }

        /// <summary>
        /// Cập nhật loại hàng
        /// </summary>
        /// <param name="data">dữ liệu loại hàng</param>
        /// <returns></returns>
        public async Task<bool> UpdateCategoryAsync(Category data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"UPDATE Categories
                               SET CategoryName = @CategoryName,
                                   Description = @Description
                               WHERE CategoryID = @CategoryID;";
            var parameters = new
            {
                data.CategoryID,
                data.CategoryName,
                data.Description
            };
            int rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa loại hàng
        /// </summary>
        /// <param name="categoryId">Mã loại hàng</param>
        /// <returns></returns>
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"DELETE FROM Categories WHERE CategoryID = @CategoryID;";
            var parameters = new
            {
                CategoryID = categoryId
            };
            int rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.Text);
            return rowsAffected > 0;
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            #region Declare SQL string
            using var connection = await OpenConnectionAsync();
            string sql = @"select * from Categories where CategoryID = @CategoryID";
            var parameters = new
            {
                CategoryID = id
            };
            #endregion

            return await connection.QueryFirstOrDefaultAsync<Category>(sql: sql, param: parameters, commandType: CommandType.Text);
        }

        public async Task<int> CountRow(string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            using var connection = await OpenConnectionAsync();
            string sql = "SELECT COUNT(*) FROM Categories WHERE CategoryName LIKE @searchValue";
            var parameters = new { searchValue };
            return await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: System.Data.CommandType.Text);
        }
    }
}
