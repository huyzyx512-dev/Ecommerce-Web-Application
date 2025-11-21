using Microsoft.AspNetCore.Mvc.Rendering;
using SV22T1080013.BusinessLayers;
using System.Threading.Tasks;

namespace SV22T1080013.Admin
{
    public static class SelectListHelper
    {
        /// <summary>
        /// Danh sách các tỉnh thành dùng thẻ select
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SelectListItem>> Provinces()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "-- Chọn Tỉnh/Thành --" });
            foreach(var item in await CommonDataService.ProvinceDB.ListAsync())
            {
                list.Add(new SelectListItem() { Value = item.ProvinceName, Text=item.ProvinceName }); 
            }

            return list;
        }
        /// <summary>
        /// Danh sách các loại hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SelectListItem>> Categories()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "-- Chọn loại hàng --" });
            foreach (var item in await CommonDataService.CategoryDB.GetCategoriesAsync())
            {
                list.Add(new SelectListItem() { Value = item.CategoryID.ToString(), Text = item.CategoryName });
            }

            return list;
        }
        /// <summary>
        /// Danh sách các nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SelectListItem>> Suppliers()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "-- Chọn nhà cung cấp --" });
            foreach (var item in await CommonDataService.SupplierDB.ListAsync())
            {
                list.Add(new SelectListItem() { Value = item.SupplierID.ToString(), Text = item.SupplierName });
            }

            return list;
        }
    }
}
