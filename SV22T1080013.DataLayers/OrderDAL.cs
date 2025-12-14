using Dapper;
using SV22T1080013.DataLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.DataLayers.SQLServer
{
    /// <summary>
    /// Các chức năng xử lý dữ liệu liên quan đến đơn hàng và nội dung của đơn hàng
    /// </summary>
    public class OrderDAL : BaseDAL
    {
        /// <summary>
        /// Kết nối với csdl
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối</param>
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Lấy danh sách đơn hàng có phân trang
        /// </summary>
        /// <param name="page">giá trị trang cần tìm</param>
        /// <param name="pageSize">số lượng order trong 1 trang</param>
        /// <param name="status">trạng thái đơn hàng</param>
        /// <param name="fromTime">thời điểm đơn hàng được giao</param>
        /// <param name="toTime">thời điểm đơn hàng giao hoàn thành</param>
        /// <param name="searchValue">Tên nhân viên, khách hàng, người giao</param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> ListAsync(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            using var connection = await OpenConnectionAsync();
            var sql = @"with cte as
                        (
                            select  row_number() over(order by o.OrderTime desc) as RowNumber,
                                    o.*,
                                    c.CustomerName,
                                    c.ContactName as CustomerContactName,
                                    c.Address as CustomerAddress,
                                    c.Phone as CustomerPhone,
                                    c.Email as CustomerEmail,
                                    e.FullName as EmployeeName,
                                    s.ShipperName,
                                    s.Phone as ShipperPhone        
                            from    Orders as o
                                    left join Customers as c on o.CustomerID = c.CustomerID
                                    left join Employees as e on o.EmployeeID = e.EmployeeID
                                    left join Shippers as s on o.ShipperID = s.ShipperID
                            where   (@Status = 0 or o.Status = @Status)
                                and (@FromTime is null or o.OrderTime >= @FromTime)
                                and (@ToTime is null or o.OrderTime <= @ToTime)
                                and (c.CustomerName like @SearchValue or e.FullName like @SearchValue or s.ShipperName like @SearchValue)
                        )
                        select * from cte 
                        where (@PageSize = 0) or (RowNumber between (@Page - 1) * @PageSize + 1 and @Page * @PageSize)
                        order by RowNumber";
            var parameters = new
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                FromTime = fromTime,
                ToTime = toTime,
                SearchValue = searchValue
            };
            return await connection.QueryAsync<Order>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// Đếm số lượng đơn hàng
        /// </summary>
        /// <param name="status">trạng thái đơn hàng</param>
        /// <param name="fromTime">thời điểm đơn hàng được giao</param>
        /// <param name="toTime">thời điểm đơn hàng giao hoàn thành</param>
        /// <param name="searchValue">Tên nhân viên, khách hàng, người giao</param>
        /// <returns></returns>
        public async Task<int> CountAsync(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            using var connection = await OpenConnectionAsync();
            var sql = @"select  count(*)       
                        from    Orders as o
                                left join Customers as c on o.CustomerID = c.CustomerID
                                left join Employees as e on o.EmployeeID = e.EmployeeID
                                left join Shippers as s on o.ShipperID = s.ShipperID
                        where   (@Status = 0 or o.Status = @Status)
                            and (@FromTime is null or o.OrderTime >= @FromTime)
                            and (@ToTime is null or o.OrderTime <= @ToTime)
                            and (c.CustomerName like @SearchValue or e.FullName like @SearchValue or s.ShipperName like @SearchValue)";
            var parameters = new
            {
                status,
                fromTime,
                toTime,
                SearchValue = searchValue ?? ""
            };
            return await connection.ExecuteScalarAsync<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public async Task<Order?> GetAsync(int orderID)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"select o.*,
                                c.CustomerName,
                                c.ContactName as CustomerContactName,
                                c.Address as CustomerAddress,
                                c.Phone as CustomerPhone,
                                c.Email as CustomerEmail,
                                e.FullName as EmployeeName,
                                s.ShipperName,
                                s.Phone as ShipperPhone        
                        from    Orders as o
                                left join Customers as c on o.CustomerID = c.CustomerID
                                left join Employees as e on o.EmployeeID = e.EmployeeID
                                left join Shippers as s on o.ShipperID = s.ShipperID
                        where   o.OrderID = @OrderID";
            var parameters = new
            {
                orderID
            };
            return await connection.QueryFirstOrDefaultAsync<Order>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(Order data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"insert into Orders(CustomerId, OrderTime, DeliveryProvince, DeliveryAddress, EmployeeID, Status)
                            values(@CustomerID, getdate(), @DeliveryProvince, @DeliveryAddress, @EmployeeID, @Status);
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql: sql, param: data, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Order data)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"update Orders
                        set CustomerID = @CustomerID,
                            OrderTime = @OrderTime,
                            DeliveryProvince = @DeliveryProvince,
                            DeliveryAddress = @DeliveryAddress,
                            EmployeeID = @EmployeeID,
                            AcceptTime = @AcceptTime,
                            ShipperID = @ShipperID,
                            ShippedTime = @ShippedTime,
                            FinishedTime = @FinishedTime,
                            Status = @Status
                        where OrderID = @OrderID";
            return (await connection.ExecuteAsync(sql: sql, param: data, commandType: System.Data.CommandType.Text)) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int orderID)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"delete from OrderDetails where OrderID = @OrderID;
                        delete from Orders where OrderID = @OrderID";
            var parameters = new { orderID };
            return (await connection.ExecuteAsync(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDetail>> ListDetailsAsync(int orderID)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"select  od.*, p.ProductName, p.Photo, p.Unit
                        from    OrderDetails as od
                                join Products as p on od.ProductID = p.ProductID
                        where od.OrderID = @OrderID";
            var parameters = new { orderID };
            return await connection.QueryAsync<OrderDetail>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<OrderDetail?> GetDetailAsync(int orderID, int productID)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"select  od.*, p.ProductName, p.Photo, p.Unit
                            from    OrderDetails as od
                                    join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID and od.ProductID = @ProductID";
            var parameters = new { orderID, productID };
            return await connection.QueryFirstOrDefaultAsync<OrderDetail>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>
        public async Task<bool> SaveDetailAsync(int orderID, int productID, int quantity, decimal salePrice)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"if exists(select * from OrderDetails where OrderID = @OrderID and ProductID = @ProductID)
                                update OrderDetails 
                                set Quantity = @Quantity, SalePrice = @SalePrice 
                                where OrderID = @OrderID and ProductID = @ProductID
                            else
                                insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice) 
                                values(@OrderID, @ProductID, @Quantity, @SalePrice)";
            var parameters = new
            {
                orderID,
                productID,
                quantity,
                salePrice
            };
            return (await connection.ExecuteAsync(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            using var connection = await OpenConnectionAsync();
            var sql = @"delete from OrderDetails where OrderID = @OrderID and ProductID = @ProductID";
            var parameters = new
            {
                orderID,
                productID
            };
            return (await connection.ExecuteAsync(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)) > 0;
        }
    }
}