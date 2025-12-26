using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080013.Admin.AppCodes;
using SV22T1080013.Admin.Models;
using SV22T1080013.BusinessLayers;
using SV22T1080013.DomainModels;
using System.Threading.Tasks;

namespace SV22T1080013.Admin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGESIZE_PRODUCT = 4;
        private const int PAGESIZE_ORDER = 20;
        private const string CART = "CART";
        private const string PRODUCT_SEARCH_FOR_SALE = "ProductSearchForSale";
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCodition";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchCondition>(ORDER_SEARCH_CONDITION);
            condition ??= new OrderSearchCondition()
            {
                Page = 1,
                PageSize = PAGESIZE_ORDER,
                SearchValue = "",
                StatusID = 0,
                DateRange = string.Format(
                    "{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                    DateTime.Today.AddYears(-2),
                    DateTime.Today.AddDays(1)
                )
            };
            return View(condition);
        }

        public async Task<IActionResult> Search(OrderSearchCondition condition)
        {
            var data = await OrderDataService.OrderDB.ListAsync(
                condition.Page,
                condition.PageSize,
                condition.StatusID,
                condition.FromDate,
                condition.ToDate,
                condition.SearchValue
            );

            var rowCount = await OrderDataService.OrderDB.CountAsync(
                condition.StatusID,
                condition.FromDate,
                condition.ToDate,
                condition.SearchValue
            );

            var model = new PaginationSearchResult<Order>()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data,
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await OrderDataService.OrderDB.GetAsync(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            return View(order);
        }

        public async Task<IActionResult> ItemsOrder(int orderID)
        {
            var items = await OrderDataService.OrderDB.ListDetailsAsync(orderID);
            return View(items);
        }

        /// <summary>
        /// Chuyển giao đơn hàng cho đơn vị giao hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Shipping(int id)
        {
            var order = await OrderDataService.OrderDB.GetAsync(id);
            if (order == null) return NotFound();

            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> Shipping(int orderId, int shipperID)
        {
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(orderId);
                if (order == null)
                    return Json(ApiResult.ResultFailed("Không tìm thấy đơn hàng"));

                // Kiểm tra trạng thái hợp lệ (ví dụ chỉ được chuyển giao khi đã ACCEPTED)
                if (order.Status != Constants.ORDER_ACCEPTED && order.Status != Constants.ORDER_SHIPPING)
                    return Json(ApiResult.ResultFailed("Đơn hàng chưa được duyệt, không thể chuyển giao hàng"));

                if (shipperID <= 0)
                    return Json(ApiResult.ResultFailed("Vui lòng chọn người giao hàng"));

                // Cập nhật thông tin shipper và trạng thái
                order.ShipperID = shipperID;
                order.ShippedTime = DateTime.Now;
                order.Status = Constants.ORDER_SHIPPING; // hoặc trạng thái tương ứng

                await OrderDataService.OrderDB.UpdateAsync(order);

                return Json(ApiResult.ResultSuccess("Chuyển giao hàng thành công"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(ApiResult.ResultFailed("Lỗi hệ thống khi chuyển giao hàng"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDetail(OrderDetail item)
        {
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(item.OrderID);
                if (order != null && order.Status == Constants.ORDER_INIT)
                {
                    if (string.IsNullOrWhiteSpace(item.OrderID.ToString()))
                        return Json(ApiResult.ResultFailed("Lỗi id đơn hàng khi UpdateDetail"));
                    if (string.IsNullOrWhiteSpace(item.ProductID.ToString()))
                        return Json(ApiResult.ResultFailed("Lỗi id mặt hàng khi UpdateDetail"));
                    if (item.Quantity < 1)
                        return Json(ApiResult.ResultFailed("Lỗi số lượng mặt hàng khi UpdateDetail"));
                    if (item.SalePrice < 0)
                        return Json(ApiResult.ResultFailed("Lỗi giá bán mặt hàng khi UpdateDetail"));

                    var result = await OrderDataService.OrderDB.SaveDetailAsync(item.OrderID, item.ProductID, item.Quantity, item.SalePrice);

                    if (result == false) return Json(ApiResult.ResultFailed("Cập nhật chi tiết đơn hàng không thành công"));

                    return Json(ApiResult.ResultSuccess("Cập nhật chi tiết đơn hàng thành công", result));
                }
                return Json(ApiResult.ResultFailed("Không thể cập nhật chi tiết đơn hàng ở trạng thái này"));
            }
            catch (Exception ex)
            {
                return Json(ApiResult.ResultSuccess("Lỗi hệ thống", ex));
            }
        }

        /// <summary>
        /// Duyệt đơn hàng mới tạo
        /// </summary>
        /// <param name="id">OrderId</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                // Trước khi khởi tạo có thể xóa đơn hàng, chỉnh sửa chi tiết đơn hàng, ở trạng thái Init có thể duyệt đơn hàng, hủy đơn hàng
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order == null)
                {
                    return Json(ApiResult.ResultFailed("Không tìm thấy đơn hàng"));
                }

                // Trước khi cập nhật kiểm tra mức độ trạng thái đơn hàng 
                if (order.Status != Constants.ORDER_INIT)
                {
                    return Json(ApiResult.ResultFailed("Trạng thái đơn hàng hiện tại không thể duyệt, Vui lòng kiểm tra lại"));
                }

                // Thay đổi trạng thái đơn hàng và thời điểm duyệt đơn
                order.AcceptTime = DateTime.Now;
                order.Status = Constants.ORDER_ACCEPTED;

                await OrderDataService.OrderDB.UpdateAsync(order);

                return Json(ApiResult.ResultSuccess("Duyệt đơn hàng thành công"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed($"Lỗi hệ thống"));
            }
        }

        /// <summary>
        /// Xác nhận đơn hàng giao hoàn thành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Finish(int id)
        {
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order == null)
                {
                    return Json(ApiResult.ResultFailed("Không tìm thấy đơn hàng"));
                }

                // Trước khi cập nhật kiểm tra mức độ trạng thái đơn hàng 
                if (order.Status != Constants.ORDER_SHIPPING)
                {
                    return Json(ApiResult.ResultFailed("Không thể xác nhận hoàn tất đơn hàng khi chưa chuyển giao hàng"));
                }

                // Thay đổi trạng thái đơn hàng và thời điểm duyệt đơn
                order.FinishedTime = DateTime.Now;
                order.Status = Constants.ORDER_FINISHED;

                await OrderDataService.OrderDB.UpdateAsync(order);

                return Json(ApiResult.ResultSuccess("Xác nhận hoàn tất đơn hàng thành công"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed($"Lỗi hệ thống"));
            }
        }

        /// <summary>
        /// Hủy đơn hàng 
        /// </summary>
        /// <param name="id">OrderId</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            // Accepted, Shipping 
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order == null)
                {
                    return Json(ApiResult.ResultFailed("Không tìm thấy đơn hàng"));
                }

                // Trước khi cập nhật kiểm tra mức độ trạng thái đơn hàng order.Status != Constants.ORDER_INIT || order.Status != Constants.ORDER_ACCEPTED || order.Status == Constants.ORDER_REJECTED
                if (order.Status == Constants.ORDER_INIT || order.Status == Constants.ORDER_FINISHED || order.Status == Constants.ORDER_REJECTED || order.Status == Constants.ORDER_CANCEL)
                {
                    return Json(ApiResult.ResultFailed("Không thể hủy đơn hàng ở trạng thái này"));
                }

                // Thay đổi trạng thái đơn hàng và thời điểm duyệt đơn
                order.Status = Constants.ORDER_CANCEL;

                await OrderDataService.OrderDB.UpdateAsync(order);

                return Json(ApiResult.ResultSuccess("Hủy đơn hàng thành công"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed($"Lỗi hệ thống"));
            }
        }

        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="id">OrderId</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            // Inited, Accepted
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order == null)
                {
                    return Json(ApiResult.ResultFailed("Không tìm thấy đơn hàng"));
                }

                // Trước khi cập nhật kiểm tra mức độ trạng thái đơn hàng 
                if (order.Status == Constants.ORDER_SHIPPING || order.Status == Constants.ORDER_FINISHED || order.Status == Constants.ORDER_REJECTED || order.Status == Constants.ORDER_CANCEL)
                {
                    return Json(ApiResult.ResultFailed("Không thể từ chối đơn hàng ở trạng thái này"));
                }

                // Thay đổi trạng thái đơn hàng và thời điểm duyệt đơn
                order.Status = Constants.ORDER_REJECTED;

                await OrderDataService.OrderDB.UpdateAsync(order);

                return Json(ApiResult.ResultSuccess("Từ chối đơn hàng thành công"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed($"Lỗi hệ thống"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order != null && order.Status == Constants.ORDER_INIT)
                {
                    var deleteOrder = await OrderDataService.OrderDB.DeleteAsync(id);
                    if (deleteOrder)
                        return Json(ApiResult.ResultSuccess("Xóa đơn hàng thành công", null));
                }
                return Json(ApiResult.ResultFailed("Không thể xóa đơn hàng ở trạng thái này"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed("Xóa đơn hàng không thành công"));
            }
        }

        public async Task<IActionResult> EditDetail(int id, int productId = 0)
        {
            // id = orderId
            //var order = await OrderDataService.OrderDB.GetAsync(id);
            var itemOrderDetail = await OrderDataService.OrderDB.GetDetailAsync(id, productId);

            return View(itemOrderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDetail(int id, int productId = 0)
        {
            try
            {
                var order = await OrderDataService.OrderDB.GetAsync(id);
                if (order != null && order.Status == Constants.ORDER_INIT)
                {
                    await OrderDataService.OrderDB.DeleteDetailAsync(id, productId);
                    return Json(ApiResult.ResultSuccess("Xóa chi tiết đơn hàng thành công"));
                }
                return Json(ApiResult.ResultFailed("Không thể xóa đơn hàng ở trạng thái này"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ApiResult.ResultFailed("Xóa chi tiết đơn hàng không thành công"));
            }
        }

        /// <summary>
        /// Xóa sản phẩm trong giỏ hàng
        /// </summary>
        /// <param name="id">Mã sản phẩm</param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id)
        {
            var items = GetSessionCart();
            var item = items.Find(i => i.ProductID == id);
            if (item != null)
            {
                items.Remove(item);
                ApplicationContext.SetSessionData(CART, items);
                return Json(new ApiResult { Code = 1 });
            }
            return PartialView("GetCart", items);
        }

        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CART);
            return View("GetCart", new List<OrderDetail>()); // trả về giỏ rỗng
        }

        /// <summary>
        /// Tạo đơn hàng từ danh sách session cart 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            try
            {
                var employeeId = User.GetUserData()?.UserId;
                if (customerID == 0)
                    return Json(ApiResult.ResultFailed("Vui lòng chọn khách hàng"));

                if (string.IsNullOrWhiteSpace(employeeId))
                    return Json(ApiResult.ResultFailed("Người tạo đơn hàng không tồn tại"));

                if (string.IsNullOrWhiteSpace(deliveryProvince))
                    return Json(ApiResult.ResultFailed("Vui lòng chọn tỉnh/thành giao hàng"));

                if (string.IsNullOrWhiteSpace(deliveryAddress))
                    return Json(ApiResult.ResultFailed("Vui lòng nhập địa chị giao hàng"));

                // Insert new Order
                Order order = new()
                {
                    CustomerID = customerID,
                    DeliveryProvince = deliveryProvince,
                    DeliveryAddress = deliveryAddress,
                    EmployeeID = Convert.ToInt32(employeeId),
                    Status = Constants.ORDER_INIT
                };

                int orderId = await OrderDataService.OrderDB.AddAsync(order); // return OrderId mới tạo

                if (orderId > 0 && GetSessionCart().Count > 0)
                {
                    // Insert list OrderDetail
                    foreach (var item in GetSessionCart())
                    {
                        await OrderDataService.OrderDB.SaveDetailAsync(orderId, item.ProductID, item.Quantity, item.SalePrice);
                    }
                }
                return Json(ApiResult.ResultSuccess("Thêm mới đơn hàng thành công", orderId));
            }
            catch (Exception ex)
            {
                return Json(ApiResult.ResultFailed(ex.Message));
            }
        }

        public IActionResult Create()
        {
            ProductSearchCondition? condition = ApplicationContext.GetSessionData<ProductSearchCondition>(PRODUCT_SEARCH_FOR_SALE);
            condition ??= new ProductSearchCondition()
            {
                Page = 1,
                PageSize = PAGESIZE_PRODUCT,
                SearchValue = "",
                CategoryID = 0,
                SupplierID = 0,
                MaxPrice = 0,
                MinPrice = 0,
            };
            return View(condition);
        }

        /// <summary>
        /// Tìm kiếm sản phầm có phân trang 
        /// </summary>
        /// <param name="condition">Thông tin search sản phẩm, nếu trống thì trả về toàn bộ sản phẩm có phân trang</param>
        /// <returns></returns>
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

        /// <summary>
        /// Lấy danh sách sản phẩm trong session giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCart()
        {
            return View(GetSessionCart());
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <param name="data">Chi tiết mặt hàng</param>
        /// <returns></returns>
        public IActionResult AddToCart(OrderDetail data)
        {
            if (data.Quantity < 1)
                return Json(new ApiResult() { Code = 0, Message = "Số lượng không hợp lệ" });
            if (data.SalePrice < 0)
                return Json(new ApiResult() { Code = 0, Message = "Giá bán không hợp lệ" });

            AddSessionCart(data);
            return Json(new ApiResult() { Code = 1, Message = "Thêm mặt hàng thành công" });
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
