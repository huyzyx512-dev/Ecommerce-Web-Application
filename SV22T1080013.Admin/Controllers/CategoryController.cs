using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private const int PAGESIZE = 10;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<PaginationSearchCondition>(CATEGORY_SEARCH_CONDITION);
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
            var data = await CommonDataService.CategoryDB.GetCategoriesAsync(condition.Page, condition.PageSize, condition.SearchValue);
            var rowCount = await CommonDataService.CategoryDB.CountRow(condition.SearchValue);
            var model = new PaginationSearchResult<Category>
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới một loại hàng";
            var model = new Category() { CategoryID = 0 };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật một loại hàng";
            var model = await CommonDataService.CategoryDB.GetCategoryById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(Category data)
        {
            if (data.CategoryID == 0)
            {
                // Add
                await CommonDataService.CategoryDB.AddCategoryAsync(data);
                return RedirectToAction("Index");
            }
            else
            {
                // Update
                await CommonDataService.CategoryDB.UpdateCategoryAsync(data);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CommonDataService.CategoryDB.DeleteCategoryAsync(id);
                return RedirectToAction("Index");
            }
            else
            {
                var model = await CommonDataService.CategoryDB.GetCategoryById(id);
                return View(model);
            }

        }
    }
}
