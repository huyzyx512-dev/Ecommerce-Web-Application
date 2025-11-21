using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;
using System.Threading.Tasks;

namespace SV22T1080013.Admin.Controllers
{
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH_CONDITION = "ProductSeachCondition";
        private const int PAGESIZE = 10;

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchCondition>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchCondition()
                {
                    Page = 1,
                    PageSize = PAGESIZE,
                    SearchValue = ""
                };

            }
            return View(condition);
            //return View();
        }

        public async Task<IActionResult> Search(ProductSearchCondition condition)
        {
            var data = await ProductDataService.ProductDB.ListAsync(
                page: condition.Page,
                pageSize: condition.PageSize,
                searchValue: condition.SearchValue,
                categoryID: condition.CategoryID,
                supplierID: condition.SupplierID,
                minPrice: condition.MinPrice,
                maxPrice: condition.MaxPrice
            );

            var rowCount = await ProductDataService.ProductDB.CountAsync(
                searchValue: condition.SearchValue,
                categoryID: condition.CategoryID,
                supplierID: condition.SupplierID,
                minPrice: condition.MinPrice,
                maxPrice: condition.MaxPrice
            );

            var model = new PaginationSearchResult<Product>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            // Lưu vào session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return View(model);
        }
        //public async Task<IActionResult> Search(ProductSearchCondition condition)
        //{
        //    // Dữ liệu mặt hàng được phân trang
        //    var data = await ProductDataService.ProductDB.ListAsync(condition.Page, condition.PageSize, condition.SearchValue, condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
        //    // Tổng số lượng mặt hàng
        //    var rowCount = await ProductDataService.ProductDB.CountAsync(condition.SearchValue, condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
        //    // Tạo PaginationProductResult
        //    var model = new PaginationSearchResult<Product>()
        //    {
        //        Page = condition.Page,
        //        PageSize = condition.PageSize,
        //        SearchValue = condition.SearchValue,
        //        RowCount = rowCount,
        //        Data = data
        //    };

        //    ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

        //    return View(model);
        //}

        public IActionResult Create()
        {
            return View("Edit");
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public IActionResult SaveData()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }


        public IActionResult Photo(int id, string method = "", int photoId = 0)
        {
            switch (method.ToLower())
            {
                case "add":
                    ViewBag.Method = "add";
                    return View();
                case "edit":
                    ViewBag.Method = "edit";
                    ViewBag.PhotoId = photoId;
                    return View();
                case "delete":
                    // TODO: Xoá ảnh (hoàn thành đoạn code này ngày mai)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }
        }

        public IActionResult Attribute(int id, string method = "", int attributeId = 0)
        {
            switch (method.ToLower())
            {
                case "add":
                    ViewBag.Method = "add";
                    return View();
                case "edit":
                    ViewBag.Method = "edit";
                    ViewBag.PhotoId = attributeId;
                    return View();
                case "delete":
                    //TODO: Xoá thuộc tính
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }
        }

    }
}
