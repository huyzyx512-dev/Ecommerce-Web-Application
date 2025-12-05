using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string CART = "CART";
        private const string PRODUCT_SEARCH_FOR_SALE = "ProductSearchForSale";
        private const int PAGESIZE = 4;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }

        public IActionResult Shipping()
        {
            return View();
        }

        public IActionResult UpdateDetail(int id)
        {
            return View();
        }

        public IActionResult Accept(int id)
        {
            return View();
        }

        public IActionResult Finish(int id)
        {
            return View();
        }

        public IActionResult Cancel(int id)
        {
            return View();
        }

        public IActionResult Reject(int id)
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult EditDetail(int id, int productId = 0)
        {
            return View();
        }

        public IActionResult DeleteDetail(int id, int productId = 0)
        {
            return View();
        }

        public IActionResult RemoveFromCart(int id)
        {
            return View();
        }

        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CART);
            return PartialView("GetCart", new List<OrderDetail>()); // trả về giỏ rỗng
        }
            
        public IActionResult Init()
        {
            return View();
        }

        public IActionResult Create()
        {
            ProductSearchCondition? condition = ApplicationContext.GetSessionData<ProductSearchCondition>(PRODUCT_SEARCH_FOR_SALE);
            condition ??= new ProductSearchCondition()
            {
                Page = 1,
                PageSize = PAGESIZE,
                SearchValue = "",
                CategoryID = 0,
                SupplierID = 0,
                MaxPrice = 0,
                MinPrice = 0,
            };
            return View(condition);
        }

        public async Task<IActionResult> SearchProduct(ProductSearchCondition condition)
        {
            if (condition == null)
            {
                return Content("Yêu cầu không hợp lệ");
            }
            var model = new PaginationSearchProductResult<Product>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = await ProductDataService.ProductDB.CountAsync(condition.SearchValue, condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice),
                Data = await ProductDataService.ProductDB.ListAsync(condition.Page, condition.PageSize, condition.SearchValue, condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice)
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_FOR_SALE, condition);
            return View(model);
        }

        public IActionResult GetCart()
        {
            return View(GetSessionCart());
        }

        public IActionResult AddToCart(OrderDetail data)
        {
            AddSessionCart(data);
            return View("GetCart", GetSessionCart());
        }
        /// <summary>
        /// Lấy giỏ hàng trong Session
        /// </summary>
        /// <returns></returns>
        public List<OrderDetail> GetSessionCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetail>>(CART);

            cart ??= new List<OrderDetail>();
            return cart;
        }
        /// <summary>
        /// Thêm hàng vào giỏ trong session
        /// </summary>
        /// <param name="data"></param>
        public void AddSessionCart(OrderDetail data)
        {
            var cart = GetSessionCart();
            var existOrderDetail = cart.Find(m => m.ProductID == data.ProductID);
            if (existOrderDetail == null)
            {
                cart.Add(data);
            }
            else
            {
                existOrderDetail.Quantity += data.Quantity;
                existOrderDetail.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(CART, cart);
        }
    }
}
