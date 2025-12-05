using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private const int PAGESIZE = 6;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<PaginationSearchCondition>(EMPLOYEE_SEARCH_CONDITION);
            condition ??= new PaginationSearchCondition()
            {
                Page = 1,
                PageSize = PAGESIZE,
                SearchValue = ""
            };
            return View(condition);
        }

        public async Task<IActionResult> Search(PaginationSearchCondition condition)
        {
            var data = await CommonDataService.EmployeeDB.GetEmployeesAsync(condition.Page,condition.PageSize, condition.SearchValue);
            var rowCount = await CommonDataService.EmployeeDB.CountAsync(condition.SearchValue);

            var model = new PaginationSearchResult<Employee>
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);

            return View(model);
        }


        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var employee = await CommonDataService.EmployeeDB.GetEmployeeByIdAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            var model = new EmployeeEditModel()
            {
                EmployeeID = employee.EmployeeID,
                FullName = employee.FullName,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                Email = employee.Email,
                Phone = employee.Phone,
                Photo = employee.Photo,
                IsWorking = employee.IsWorking,
            };
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            var model = new EmployeeEditModel()
            {
                EmployeeID = 0,
                Photo = "nophoto.jpg"
            };
            return View("Edit", model);
        }

        //TODO: xử lý hàm cập nhật, thêm, và xóa của nhân viên
        [HttpPost]
        public async Task<IActionResult> SaveData(EmployeeEditModel model)
        {
            //Nếu có ảnh thì upload ảnh lên và lấy tên file ảnh mới upload cho Photo
            if (model.UploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{model.UploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WWWRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadPhoto.CopyToAsync(stream);
                }
                model.Photo = fileName;
            }

            Employee data = new Employee()
            {
                EmployeeID = model.EmployeeID,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                Photo = model.Photo,
                IsWorking = model.IsWorking
            };

            if (data.EmployeeID == 0)
            {
                await CommonDataService.EmployeeDB.AddAsync(data);
            }
            else
            {
                await CommonDataService.EmployeeDB.UpdateAsync(data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                // Tiến hành xóa nhân viên
                await CommonDataService.EmployeeDB.DeleteEmployeeAsync(id);
                // Trả về
                return RedirectToAction("Index");
            }
            else
            {
                // Tìm nhân viên theo mã
                var model = await CommonDataService.EmployeeDB.GetEmployeeByIdAsync(id);
                return View(model);
            }
        }
    }
}
