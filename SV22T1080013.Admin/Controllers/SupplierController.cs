using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private const int PAGESIZE = 10;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<PaginationSearchCondition>(SUPPLIER_SEARCH_CONDITION);
            if (condition == null) { 
                condition = new PaginationSearchCondition()
                {
                    Page = 1,
                    PageSize = PAGESIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public async Task<IActionResult> Search(PaginationSearchCondition condition)
        {
            // get suppliers page 
            var data = await CommonDataService.SupplierDB.ListAsync(condition.Page, condition.PageSize,condition.SearchValue);
            // get total supplier
            var rowCount = await CommonDataService.SupplierDB.CountAsync(condition.SearchValue);
            // 
            var model = new PaginationSearchResult<Supplier>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại session
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp mới";
            var model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Supplier supplier)
        {
            //TODO: Kiểm tra và soát lỗi với chức năng bổ sung/cập nhật Nhà cung cáp
            try
            {
                #region Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                    ModelState.AddModelError(nameof(supplier.SupplierName), "Tên nhà cung cấp không được để trống");
                if (string.IsNullOrWhiteSpace(supplier.ContactName))
                    ModelState.AddModelError(nameof(supplier.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrWhiteSpace(supplier.Phone))
                    ModelState.AddModelError(nameof(supplier.Phone), "Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(supplier.Email))
                    ModelState.AddModelError(nameof(supplier.Email), "Email không được để trống");
                if (string.IsNullOrWhiteSpace(supplier.Address))
                    ModelState.AddModelError(nameof(supplier.Address), "Địa chỉ không được để trống");
                if (string.IsNullOrWhiteSpace(supplier.Province))
                    ModelState.AddModelError(nameof(supplier.Province), "Chọn tỉnh/thành");
                #endregion


                #region Hiển thị thông báo lỗi nếu có lỗi
                if (!ModelState.IsValid)
                    return View("Edit",supplier);
                #endregion

                if (supplier.SupplierID == 0)
                {
                    await CommonDataService.SupplierDB.AddAsync(supplier);
                }
                else
                {
                    await CommonDataService.SupplierDB.UpdateAsync(supplier);
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return View("Edit");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            // return model 
            var model = await CommonDataService.SupplierDB.GetAsync(id);
            if (model != null)
            {
                RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CommonDataService.SupplierDB.DeleteAsync(id);
                return RedirectToAction("Index");

            }
            else
            {
                var model = await CommonDataService.SupplierDB.GetAsync(id);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }

                return View(model);
            }
        }

    }
}
