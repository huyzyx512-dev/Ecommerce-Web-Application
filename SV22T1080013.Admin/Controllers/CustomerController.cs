using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private const int PAGESIZE = 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";

        public IActionResult Index()
        {
            // Nếu trong session có lưu điều kiện tìm kiếm thì sử dụng lại điều kiện đó.
            // Ngược lại, thì tạo điều kiện tìm kiếm mặc định
            var condition = ApplicationContext.GetSessionData<PaginationSearchCondition>(CUSTOMER_SEARCH_CONDITION);
            if (condition == null)
            {
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
            var data = await CommonDataService.CustomerDB.ListAsync(condition.Page, condition.PageSize, condition.SearchValue);
            var rowCount = await CommonDataService.CustomerDB.CountAsync(condition.SearchValue);
            var model = new PaginationSearchResult<Customer>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            // lưu lại điều kiện tìm kiếm vào trong Session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);

            return View(model);
        }

        public async Task<IActionResult> Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = await CommonDataService.CustomerDB.GetCustomerByIdAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            try
            {
                ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng mới" : "Cập nhật thông tin khách hàng";

                #region  Kiểm tra dữ liệu đầu vào 
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống"); // <-- ModelState lưu trữ các thông báo lỗi
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
                if (string.IsNullOrWhiteSpace(data.Address))
                    ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
                if (string.IsNullOrWhiteSpace(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Chọn tỉnh/thành");
                #endregion


                #region Thông báo lỗi và yêu cầu nhập lại dữ liệu nếu có trường hợp không hợp lệ
                if (!ModelState.IsValid)
                    return View("Edit", data);
                #endregion


                if (data.CustomerID == 0)
                {
                    await CommonDataService.CustomerDB.AddAsync(data);
                }
                else
                {
                    await CommonDataService.CustomerDB.UpdateAsync(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Edit"); ;
            }
        }

        public IActionResult Create()
        {
            // TODO: Validation inputs , control errors
            ViewBag.Title = "Bổ sung khách hàng mới";
            var model = new Customer()
            {
                CustomerID = 0
            };

            return View("Edit", model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CommonDataService.CustomerDB.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            else
            {
                var model = await CommonDataService.CustomerDB.GetCustomerByIdAsync(id);
                if (model == null)
                {
                    return RedirectToAction("Index");

                }
                return View(model);
            }
        }
    }
}
