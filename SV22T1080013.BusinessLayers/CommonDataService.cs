using SV22T1080013.DataLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng giao tiếp và xử lý dữ liệu chung (Supplier, Customer, Shipper, Employee, Category, Province)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ProvinceDAL provinceDB;
        private static readonly SupplierDAL supplierDB;
        private static readonly ShipperDAL shipperDB;
        private static readonly CustomerDAL customerDB;
        private static readonly EmployeeDAL employeeDB;
        private static readonly CategoryDAL categoryDB;

        /// <summary>
        /// (Trong c#, ctor của lớp static có đặc điểm gì?)
        /// </summary>
        static CommonDataService()
        {
            provinceDB = new ProvinceDAL(Configuration.ConnectionString);
            supplierDB = new SupplierDAL(Configuration.ConnectionString);
            shipperDB = new ShipperDAL(Configuration.ConnectionString);
            customerDB = new CustomerDAL(Configuration.ConnectionString);
            employeeDB = new EmployeeDAL(Configuration.ConnectionString);
            categoryDB = new DataLayers.CategoryDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Dữ liệu tỉnh thành
        /// </summary>
        public static ProvinceDAL ProvinceDB => provinceDB;
        /// <summary>
        /// Dữ liệu nhà cung cấp
        /// </summary>
        public static SupplierDAL SupplierDB => supplierDB;
        /// <summary>
        /// Dữ liệu đơn vị vận chuyển
        /// </summary>
        public static ShipperDAL ShipperDB => shipperDB;
        /// <summary>
        /// Dữ liệu Khách hàng
        /// </summary>
        public static CustomerDAL CustomerDB => customerDB;
        /// <summary>
        /// Dữ liệu nhân viên
        /// </summary>
        public static EmployeeDAL EmployeeDB => employeeDB;
        /// <summary>
        /// Dữ liệu loại hàng
        /// </summary>
        public static CategoryDAL CategoryDB => categoryDB;
    }
}
