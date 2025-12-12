using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Controllers
{
    /// <summary>
    /// Chứa các api trong hệ thống
    /// </summary>
    [Authorize]
    public class ApiController : Controller
    {
        public async Task<IActionResult> Customer(int id)
        {
            var data = await CommonDataService.CustomerDB.GetCustomerByIdAsync(id);

            if (data == null)
            {
                return Json(new Customer());
            }
            return Json(data);
        }


    }
}
