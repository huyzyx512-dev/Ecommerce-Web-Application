using SV22T1080013.DataLayers;

namespace SV22T1080013.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly ProductDAL productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        public static ProductDAL ProductDB => productDB;
    }
}
