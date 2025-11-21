using Dapper;
using SV22T1080013.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 
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
        public Task<bool> UpdateAsync(Product data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ProductAttribute>> ListAttributesAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool DeleteAttribute(long attributeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<ProductPhoto>> ListPhotosAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            throw new NotImplementedException();
        }
    }
}
