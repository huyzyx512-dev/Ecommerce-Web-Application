using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.AppCodes;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;
using System.Reflection;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
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

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    try
                    {
                        // 1. Lấy thông tin sản phẩm để biết tên file ảnh
                        var product = await ProductDataService.ProductDB.GetAsync(id);
                        var photos = await ProductDataService.ProductDB.ListPhotosAsync(id);
                        if (product == null)
                        {
                            return NotFound();
                        }

                        // 2. Xóa toàn bộ ảnh liên quan trong CSDL (ProductPhoto + ProductAttribute + Product)
                        var deleted = await ProductDataService.ProductDB.DeleteAsync(id);
                        if (!deleted)
                        {
                            TempData["Error"] = "Không thể xóa sản phẩm (có thể đang được sử dụng trong đơn hàng).";
                            return RedirectToAction("Index");
                        }

                        // 3. XÓA FILE ẢNH CHÍNH TRÊN Ổ ĐĨA (nếu tồn tại và không phải ảnh mặc định)
                        if (!string.IsNullOrWhiteSpace(product.Photo) &&
                            product.Photo.Trim() != "nophoto.png" &&
                            product.Photo.Trim() != "no-photo.png")
                        {
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                                       "wwwroot", "images", "products", product.Photo.Trim());

                            if (System.IO.File.Exists(filePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(filePath);
                                    // Nếu bạn có thumbnail thì xóa luôn ở đây
                                    // var thumbPath = Path.Combine(... "thumb", product.Photo);
                                    // if (File.Exists(thumbPath)) File.Delete(thumbPath);
                                }
                                catch (Exception ex)
                                {
                                    // Ghi log nếu cần, nhưng không làm hỏng flow xóa CSDL
                                    // _logger.LogWarning(ex, "Không thể xóa file ảnh: {FilePath}", filePath);
                                }
                            }
                        }

                        // 4. (Tùy chọn) XÓA TẤT CẢ ẢNH TRONG ProductPhoto TRÊN Ổ ĐĨA
                        foreach (var photo in photos)
                        {
                            if (string.IsNullOrWhiteSpace(photo.Photo) ||
                                photo.Photo.Trim() == "nophoto.png")
                                continue;

                            var photoPath = Path.Combine(Directory.GetCurrentDirectory(),
                                                        "wwwroot", "images", "products", photo.Photo.Trim());

                            if (System.IO.File.Exists(photoPath))
                            {
                                try { System.IO.File.Delete(photoPath); }
                                catch { /* bỏ qua lỗi file */ }
                            }
                        }

                        TempData["Success"] = $"Đã xóa sản phẩm \"{product.ProductName}\" và các file ảnh thành công!";
                        return RedirectToAction("Index", "Product");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
                        return RedirectToAction("Index", "Product");
                    }
                }
                else
                {
                    var model = await ProductDataService.ProductDB.GetAsync(id);
                    if (model == null)
                    {
                        return RedirectToAction("Product");
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Product");
            }
        }


        public async Task<IActionResult> Photo(int id, string method = "", int photoId = 0)
        {
            switch (method.ToLower())
            {
                case "add":
                    var model = new ProductPhotoEditModel()
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.png"
                    };
                    ViewBag.Method = "add";
                    return View(model);
                case "edit":
                    ViewBag.Method = "edit";
                    ViewBag.PhotoId = photoId;
                    var modelUp = await ProductDataService.ProductDB.GetPhotoAsync(photoId);
                    if (modelUp == null)
                    {
                        return RedirectToAction("Edit", new { id });
                    }
                    var photoUp = new ProductPhotoEditModel()
                    {
                        PhotoID = modelUp.PhotoID,
                        Photo = modelUp.Photo,
                        Description = modelUp.Description,
                        DisplayOrder = modelUp.DisplayOrder,
                        IsHidden = modelUp.IsHidden,
                        ProductID = id
                    };
                    return View(photoUp);
                case "delete":
                    var deleted = await ProductDataService.ProductDB.DeletePhotoAsync(photoId);

                    if (Request.Query["ajax"].ToString() == "true")
                    {
                        return Json(new { success = deleted });
                    }
                    return RedirectToAction("Edit", new { id });
                default:
                    return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDataPhoto(ProductPhotoEditModel model)
        {
            try
            {
                if (model.UpLoadPhoto == null && model.Photo == null)
                    ModelState.AddModelError(nameof(model.UpLoadPhoto), "Vui lòng chọn ảnh");
                if (string.IsNullOrWhiteSpace(model.Description))
                    ModelState.AddModelError(nameof(model.Description), "Mô tả/Tiêu đề không thể bỏ trống");
                if (model.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị không thể bỏ trống");

                if (!ModelState.IsValid) return View("Photo", model);

                //Nếu có ảnh thì upload ảnh lên và lấy tên file ảnh mới upload cho Photo
                if (model.UpLoadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{model.UpLoadPhoto.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WWWRootPath, @"images\products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UpLoadPhoto.CopyToAsync(stream);
                    }
                    model.Photo = fileName;
                }

                var photo = new ProductPhoto()
                {
                    ProductID = model.ProductID,
                    Photo = model.Photo,
                    Description = model.Description,
                    DisplayOrder = model.DisplayOrder,
                    PhotoID = model.PhotoID,
                    IsHidden = model.IsHidden,
                };

                //TODO: Fix lỗi Add, Update Attribute
                if (model.PhotoID == 0)
                {
                    // Add
                    await ProductDataService.ProductDB.AddPhotoAsync(photo);
                }
                else
                {
                    // Edit
                    await ProductDataService.ProductDB.UpdatePhotoAsync(photo);
                }
                return RedirectToAction("Edit", new { id = photo.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Edit", new { id = model.ProductID });
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
                        return RedirectToAction("Edit", new { id });
                    }
                    ViewBag.PhotoId = attributeId;
                    return View(attributeUd);
                case "delete":
                    var deleted = await ProductDataService.ProductDB.DeleteAttribute(attributeId);

                    if (Request.Query["ajax"].ToString() == "true")
                    {
                        return Json(new { success = deleted });
                    }
                    return RedirectToAction("Edit", new { id });
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
