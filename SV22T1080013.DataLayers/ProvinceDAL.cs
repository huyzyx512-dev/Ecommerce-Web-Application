using Dapper;
using SV22T1080013.DomainModels;

namespace SV22T1080013.DataLayers
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến tỉnh thành
    /// </summary>
    public class ProvinceDAL : BaseDAL
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }
 
        public async Task<IEnumerable<Province>> ListAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var sql = "SELECT * FROM Provinces ORDER BY ProvinceName";
                return await connection.QueryAsync<Province>(sql);
            }
        }
    }
}
