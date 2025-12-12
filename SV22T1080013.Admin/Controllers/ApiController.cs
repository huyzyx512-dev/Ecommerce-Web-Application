using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using SV22T1080013.Admin.Models;
>>>>>>> fix-checkout
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Controllers
{
    /// <summary>
<<<<<<< HEAD
    /// Chứa các api trong hệ thống
=======
    /// Chứa các api của hệ thống
>>>>>>> fix-checkout
    /// </summary>
    [Authorize]
    public class ApiController : Controller
    {
        public async Task<IActionResult> Customer(int id)
        {
            var data = await CommonDataService.CustomerDB.GetCustomerByIdAsync(id);

            if (data == null)
            {
<<<<<<< HEAD
                return Json(new Customer());
            }
            return Json(data);
        }


=======
                return Json(ApiResult.ResultFailed("Lấy dữ liệu không thành công"));
            }
            return Json(ApiResult.ResultSuccess("Lấy dữ liệu thành công", data));
        }
>>>>>>> fix-checkout
    }
}
