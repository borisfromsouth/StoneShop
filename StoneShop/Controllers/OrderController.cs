using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Models.ViewModels;
using StoneShop_Utility;
using StoneShop_Utility.BrainTree;
using System.Collections.Generic;
using System.Linq;

namespace StoneShop.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGate _brainTreeGate;


        [BindProperty]
		public OrderListVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository, IBrainTreeGate brainTreeGate)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _brainTreeGate = brainTreeGate;
        }

        public IActionResult Index()
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHeaderList = _orderHeaderRepository.GetAll(),
                StatusList = WebConstants.listStatus.ToList().Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            return View(orderListVM);
        }

        //public IActionResult ConvertToCart(int id)
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult Details(int id)
        //{
        //    OrderVM = new OrderVM()
        //    {
        //        OrderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == id),
        //        OrderDetail = _orderDetailRepository.GetAll(u => u.OrderHeaderId == id, includeProperties:"Product")
        //    };
        //    return View(OrderVM);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Details() // админ перемещает заказ в корзину
        //{
        //    List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        //    OrderVM.OrderDetail = _orderDetailRepository.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);

        //    foreach (var detail in OrderVM.OrderDetail)
        //    {
        //        ShoppingCart shoppingCart = new ShoppingCart()
        //        {
        //            ProductId = detail.ProductId
        //        };
        //        shoppingCartList.Add(shoppingCart);
        //    }
        //    HttpContext.Session.Clear();
        //    HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
        //    HttpContext.Session.Set(WebConstants.SessionInquiryId, OrderVM.OrderHeader.Id);  // если значение 0 то в данной сессии не было работы с Inquiry
        //    return RedirectToAction("Index", "Cart");
        //}

        //[HttpPost]
        //public IActionResult Delete() // благодаря [BindProperty] можно не передавать напрямую модель из предсталения 
        //{
        //    OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
        //    IEnumerable<OrderDetail> orderDetails = _orderDetailRepository.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);

        //    _orderDetailRepository.RemoveRange(orderDetails);
        //    _orderHeaderRepository.Remove(orderHeader);
        //    _orderHeaderRepository.Save();  // не важно какой применять, сохранится все 

        //    return RedirectToAction("Index");
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetOrderList()
        {
            return Json(new { data = _orderHeaderRepository.GetAll() });
        }

        #endregion
    }
}
