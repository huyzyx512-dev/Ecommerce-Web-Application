using Microsoft.AspNetCore.Mvc;

namespace SV22T1080013.Shop.Controllers
{
    public class CartController : Controller
    {
        /// <summary>
        /// Hiển thị giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Thêm mặt hàng vào giỏ hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public IActionResult Add(int productId, int quantity)
        {
            return View();
        }

        /// <summary>
        /// Xóa mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult Remove(int productId)
        {
            return View();
        }

        /// <summary>
        /// Cập nhật số lượng mặt hàng trong giỏ hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public IActionResult Update(int productId, int quantity)
        {
            return View();
        }
    }
}
