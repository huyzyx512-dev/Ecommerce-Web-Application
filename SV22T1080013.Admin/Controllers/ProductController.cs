using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;
using System.Reflection;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public IActionResult Create()
        {
            var product = new ProductEditModel()
            {
                ProductID = 0,
                Photo = "nophoto.png"
            };
            return View("Edit", product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await ProductDataService.ProductDB.GetAsync(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ProductEditModel()
            {
                ProductID = product.ProductID,
                Photo = product.Photo,
                CategoryID = product.CategoryID,
                SupplierID = product.SupplierID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                IsSelling = product.IsSelling,
                Price = product.Price,
                Unit = product.Unit,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(ProductEditModel model)
        {
            try
            {
                ViewBag.Title = model.ProductID == 0 ? ViewBag.Title = "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
                //Nếu có ảnh thì upload ảnh lên và lấy tên file ảnh mới upload cho Photo
                if (model.UploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{model.UploadPhoto.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WWWRootPath, @"images\products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadPhoto.CopyToAsync(stream);
                    }
                    model.Photo = fileName;
                }

                #region  Kiểm tra dữ liệu đầu vào 
                if (string.IsNullOrWhiteSpace(model.ProductName))
                    ModelState.AddModelError(nameof(model.ProductName), "Tên sản phẩm không được để trống"); // <-- ModelState lưu trữ các thông báo lỗi
                if (string.IsNullOrWhiteSpace(model.Unit))
                    ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống");
                if (model.Price == 0)
                    ModelState.AddModelError(nameof(model.Price), "Giá không được để trống");
                if (model.CategoryID == 0)
                    ModelState.AddModelError(nameof(model.CategoryID), "Chọn loại hàng");
                if (model.SupplierID == 0)
                    ModelState.AddModelError(nameof(model.SupplierID), "Chọn nhà cung cấp");
                #endregion

                if (!ModelState.IsValid) return View("Edit", model);

                var product = new Product()
                {
                    ProductID = model.ProductID,
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    CategoryID = model.CategoryID,
                    SupplierID = model.SupplierID,
                    Photo = model.Photo,
                    Unit = model.Unit,
                    Price = model.Price,
                    IsSelling = model.IsSelling,
                };

                if (product.ProductID == 0)
                {
                    await ProductDataService.ProductDB.AddAsync(product);
                }
                else
                {
                    await ProductDataService.ProductDB.UpdateAsync(product);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Edit");
            }
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

        public async Task<IActionResult> Attribute(int id, string method = "", int attributeId = 0)
        {
            switch (method.ToLower())
            {
                case "add":
                    var attribute = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = id
                    };
                    ViewBag.Method = "add";
                    return View(attribute);
                case "edit":
                    ViewBag.Method = "edit";
                    var attributeUd = await ProductDataService.ProductDB.GetAttributeAsync(attributeId);
                    if (attributeUd == null)
                    {
                        return Redirect($"/Product/Edit/{id}");
                    }
                    ViewBag.PhotoId = attributeId;
                    return View(attributeUd);
                case "delete":
                    //TODO: Xoá thuộc tính
                    await ProductDataService.ProductDB.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDataAttribute(ProductAttribute model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.AttributeName))
                    ModelState.AddModelError(nameof(model.AttributeName), "Tên thuộc tính không thể bỏ trống");
                if (string.IsNullOrWhiteSpace(model.AttributeValue))
                    ModelState.AddModelError(nameof(model.AttributeValue), "Giá trị không thể bỏ trống");
                if (model.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị không thể bỏ trống");

                if (!ModelState.IsValid) return View("Attribute", model);

                //TODO: Fix lỗi Add, Update Attribute 
                if (model.AttributeID == 0)
                {
                    // Add
                    await ProductDataService.ProductDB.AddAttributeAsync(model);
                }
                else
                {
                    // Edit
                    await ProductDataService.ProductDB.UpdateAttributeAsync(model);
                }
                return RedirectToAction("Edit", new { id = model.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Edit", new { id = model.ProductID });
            }
        }
    }
}
