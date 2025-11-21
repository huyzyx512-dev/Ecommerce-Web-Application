using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Buffers;

namespace SV22T1080013.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const int PAGESIZE = 10;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<PaginationSearchCondition>(SHIPPER_SEARCH_CONDITION);
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
            var data = await CommonDataService.ShipperDB.ListAsync(condition.Page, condition.PageSize, condition.SearchValue);
            var rowCount = await CommonDataService.ShipperDB.CountAsync(condition.SearchValue);

            var model = new PaginationSearchResult<Shipper>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            // save session
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await CommonDataService.ShipperDB.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Shipper shipper)
        {
            if (shipper.ShipperID == 0)
            {
                await CommonDataService.ShipperDB.AddAsync(shipper);
            }
            else
            {
                await CommonDataService.ShipperDB.UpdateAsync(shipper);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {

            return View();
        }
    }
}
