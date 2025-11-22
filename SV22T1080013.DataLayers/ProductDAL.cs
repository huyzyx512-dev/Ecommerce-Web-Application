using Dapper;
using SV22T1080013.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV22T1080013.DataLayers
{
    public class ProductDAL : BaseDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng mỗi trang (0 thì không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu lấy toàn bộ)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu lấy tất cả loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp (0 nếu lấy của tất cả nhà cung cấp)</param>
        /// <param name="minPrice">Giá nhỏ nhất (0 nếu không hạn chế nhỏ nhất)</param>
        /// <param name="maxPrice">Giá lớn nhất (0 nếu không hạn chế giá lớn nhất)</param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> ListAsync(int page = 1, int pageSize = 0,
                                    string searchValue = "", int categoryID = 0, int supplierID = 0,
                                    decimal minPrice = 0, decimal maxPrice = 0)
        {
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;
            searchValue = $"%{searchValue}%";

            using var connection = await OpenConnectionAsync();
            var sql = @"with cte as
                        (
                            select  *,
                                    row_number() over(order by ProductName) as RowNumber
                            from    Products 
                            where   (ProductName like @SearchValue)
                                and (@CategoryID = 0 or CategoryID = @CategoryID)
                                and (@SupplierID = 0 or SupplierId = @SupplierID)
                                and (Price >= @MinPrice)
                                and (@MaxPrice <= 0 or Price <= @MaxPrice)
                        )
                        select * from cte 
                        where   (@PageSize = 0) 
                            or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)";
            var parameters = new
            {
                Page = page,
                PageSize = pageSize,
                SearchValue = searchValue ?? "",
                CategoryID = categoryID,
                SupplierID = supplierID,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            return await connection.QueryAsync<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Tổng số lượng mặt hàng
        /// </summary>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu lấy toàn bộ)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu lấy tất cả loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp (0 nếu lấy của tất cả nhà cung cấp)</param>
        /// <param name="minPrice">Giá nhỏ nhất (0 nếu không hạn chế nhỏ nhất)</param>
        /// <param name="maxPrice">Giá lớn nhất (0 nếu không hạn chế giá lớn nhất)</param>
        /// <returns></returns>
        public async Task<int> CountAsync(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            searchValue = $"%{searchValue}%";
            using var connection = await OpenConnectionAsync();
            var sql = @"select  count(*)
                            from    Products 
                            where   (@SearchValue = N'' or ProductName like @SearchValue)
                                and (@CategoryID = 0 or CategoryID = @CategoryID)
                                and (@SupplierID = 0 or SupplierId = @SupplierID)
                                and (Price >= @MinPrice)
                                and (@MaxPrice <= 0 or Price <= @MaxPrice)";
            var parameters = new
            {
                SearchValue = searchValue ?? "",
                CategoryID = categoryID,
                SupplierID = supplierID,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            return await connection.ExecuteScalarAsync<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Tìm mặt hàng theo mã
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Product?> GetAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"SELECT * FROM Products WHERE ProductId = @id";
            var parameters = new
            {
                id
            };
            return await connection.QueryFirstOrDefaultAsync<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Thêm mới product
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> AddAsync(Product data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"INSERT INTO Products(ProductName, ProductDescription, CategoryID, SupplierID, Unit, Price, IsSelling, Photo)
                        VALUES (@ProductName,@ProductDescription,@CategoryID,@SupplierID,@Unit,@Price,@IsSelling,@Photo);
                        SELECT SCOPE_IDENTITY();";
            var parameters = new
            {
                data.ProductName,
                data.ProductDescription,
                data.CategoryID,
                data.SupplierID,
                data.Unit,
                data.Price,
                data.IsSelling,
                data.Photo
            };
            return await connection.ExecuteScalarAsync<int>(sql: sql, param: data, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //public async Task<int> UpdateAsync(Product data)
        //{
        //    using var connection = await OpenConnectionAsync();
        //    string sql = @"UPDATE Products
        //                    SET ProductName = @ProductName,
        //                     ProductDescription = @ProductDescription,
        //                     CategoryID = @CategoryID,
        //                     SupplierID = @SupplierID,
        //                     Unit = @Unit,
        //                     Price = @Price,
        //                     IsSelling = @IsSelling,
        //                     Photo = @Photo
        //                    WHERE ProductID = @ProductID;
        //                    SELECT SCOPE_IDENTITY();";
        //    return await connection.ExecuteScalarAsync<int>(sql: sql, param: data, commandType: System.Data.CommandType.Text);
        //}


        public async Task<int> UpdateAsync(Product data)
        {
            using var connection = await OpenConnectionAsync();

            return await connection.ExecuteScalarAsync<int>(
                sql: "Product_Update",
                param: new
                {
                    data.ProductID,
                    data.ProductName,
                    data.ProductDescription,
                    data.CategoryID,
                    data.SupplierID,
                    data.Unit,
                    data.Price,
                    data.IsSelling,
                    data.Photo
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Xóa photo product
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> InUsedAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Danh sách thuộc tính của sản phẩm
        /// </summary>
        /// <param name="id">Mã sản phẩm</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ProductAttribute>> ListAttributesAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"SELECT * FROM ProductAttributes 
                            WHERE ProductID = @id ORDER BY DisplayOrder";
            return await connection.QueryAsync<ProductAttribute>(sql, new { id }, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lấy thuộc tính theo mã 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ProductAttribute?> GetAttributeAsync(int attributeID)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
            return await connection.QueryFirstOrDefaultAsync<ProductAttribute>(sql, new { attributeID }, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Thêm thuộc tính mới 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> AddAttributeAsync(ProductAttribute data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder)
                            SELECT SCOPE_IDENTITY();";
            var parameter = new
            {
                data.ProductID,
                data.AttributeName,
                data.AttributeValue,
                data.DisplayOrder
            };
            return await connection.ExecuteScalarAsync<int>(sql, parameter, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Chỉnh sửa thuộc tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> UpdateAttributeAsync(ProductAttribute data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"UPDATE ProductAttributes
                            SET AttributeName = @AttributeName, AttributeValue= @AttributeValue, DisplayOrder = @DisplayOrder  
                            WHERE AttributeID = @AttributeID
                            SELECT SCOPE_IDENTITY();";
            var parameter = new
            {
                data.AttributeID,
                data.AttributeName,
                data.AttributeValue,
                data.DisplayOrder
            };
            return await connection.ExecuteScalarAsync<int>(sql, parameter, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteAttribute(int attributeID)
        {
            using var connection = await OpenConnectionAsync();
            string sql = "DELETE FROM ProductAttributes WHERE AttributeID = @attributeID";
            return await connection.ExecuteAsync(sql, new { attributeID }, commandType: System.Data.CommandType.Text) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ProductPhoto>> ListPhotosAsync(int id)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"SELECT * FROM ProductPhotos WHERE ProductID = @id
                            ORDER BY DisplayOrder";
            return await connection.QueryAsync<ProductPhoto>(sql, new { id }, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ProductPhoto?> GetPhotoAsync(int photoID)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"SELECT * FROM ProductPhotos WHERE PhotoID = @photoID";
            return await connection.QueryFirstOrDefaultAsync<ProductPhoto>(sql, new { photoID }, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Thêm mới thuộc tính ảnh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> AddPhotoAsync(ProductPhoto data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            VALUES (@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden)
                            SELECT SCOPE_IDENTITY();";
            var parameter = new
            {
                data.ProductID,
                data.Photo,
                data.Description,
                data.DisplayOrder,
                data.IsHidden
            };
            return await connection.ExecuteScalarAsync<int>(sql, parameter, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Cập nhật thuộc tính ảnh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"UPDATE ProductPhotos
                            SET ProductID=@ProductID,
                                Photo=@Photo,
                                Description=@Description,
                                DisplayOrder=@DisplayOrder,
                                IsHidden=@IsHidden
                            WHERE PhotoID = @PhotoID";
            return await connection.ExecuteAsync(sql, data, commandType: System.Data.CommandType.Text) > 0;
        }

        /// <summary>
        /// Xóa ảnh khỏi ảnh phụ
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            using var connection = await OpenConnectionAsync();
            string sql = @"DELETE FROM ProductPhotos
                            WHERE PhotoID = @PhotoID";
            return await connection.ExecuteAsync(sql, new { photoID }, commandType: System.Data.CommandType.Text) > 0;
        }
    }
}
