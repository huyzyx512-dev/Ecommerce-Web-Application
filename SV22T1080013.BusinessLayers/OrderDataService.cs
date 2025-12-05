using SV22T1080013.DataLayers.SQLServer;

namespace SV22T1080013.BusinessLayers
{
    /// <summary>
    /// Các chức năng tác nghiệp liên quan đến đơn hàng
    /// </summary>
    public class OrderDataService
    {
        private static readonly OrderDAL orderDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static OrderDataService()
        {
            orderDB = new OrderDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        public static OrderDAL OrderDB => orderDB;
    }
}
