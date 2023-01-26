using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Models.ViewModels;
using StoneShop_Utility;
using StoneShop_Utility.BrainTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StoneShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepository, IUserRepository userRepository, IInquiryHeaderRepository inquiryHeaderRepository,
                              IInquiryDetailRepository inquiryDetailRepository, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
                              IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository, IBrainTreeGate brainTreeGate)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _brainTreeGate = brainTreeGate;
        }

        public IActionResult Index()  // список всех товаров
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);  // получаем данные корзины из сессии
            }

            List<int> prodInCart = shoppingCartList.Select(u => u.ProductId).ToList();  // получаем список id товаров
            IEnumerable<Product> productListTemp = _productRepository.GetAll(u => prodInCart.Contains(u.Id));  // получаем список продуктов по списку id-шников
            IList<Product> productList = new List<Product>();

            foreach (var cartObj in shoppingCartList)
            {
                Product productTemp = productListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                productTemp.TempSqFt = cartObj.SqFt;  // берем количество из корзины
                productList.Add(productTemp);
            }

            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> productList)  // повторная обработка товаров с учетом изменения их количества
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in productList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set<List<ShoppingCart>>(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction("Index");
        }

        public IActionResult Summary()
        {
            User user;

            if (User.IsInRole(WebConstants.AdminRole)) // либо сам заказ создаешь, либо чужой обрабатываешь
            {
                if(HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)  // SessionInquiryId - константа номера обрабатываемого заказа
                {
                    // чужой заказ на обработке
                    // берем заказ на обработку (из Inquriy в Корзину)
                    InquiryHeader inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                    user = new User()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else  // свой заказ
                {
                    user = new User();
                }

                var gateway = _brainTreeGate.GetGateway(); // данные для получения токена
                var clientToken = gateway.ClientToken.Generate(); // получение токена клиента
                ViewBag.ClientToken = clientToken; // токен используется только тут для представления
            }
            else // обычный пользователь
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                // var user = User.FindFirstValue(ClaimTypes.Name);
                
                user = _userRepository.FirstOrDefault(u => u.Id == claim.Value);
            }

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);  // получаем данные козины из сессии
            }

            List<int> prodInCart = shoppingCartList.Select(u => u.ProductId).ToList();  // получаем id-шники продуктов в корзине
            IEnumerable<Product> productList = _productRepository.GetAll(u => prodInCart.Contains(u.Id));  // получаем список продуктов по списку id-шников


            ProductUserVM productUserVM = new ProductUserVM()
            {
                User = user//,
                //ProductList = productList.ToList() // нету количества товаров
            };

            foreach (var cartObj in shoppingCartList)
            {
                Product productTemp = _productRepository.FirstOrDefault(u => u.Id == cartObj.ProductId);
                productTemp.TempSqFt = cartObj.SqFt;
                productUserVM.ProductList.Add(productTemp);
            }

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(/*ProductUserVM productUserVM*/)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            if (User.IsInRole(WebConstants.AdminRole))
            {
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,  //  id текущего пользователя
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempSqFt*x.Price),
                    City = ProductUserVM.User.City,
                    Street = ProductUserVM.User.Street,
                    State = ProductUserVM.User.State,
                    PostCode = ProductUserVM.User.PostCode,
                    FullName = ProductUserVM.User.FullName,
                    Email = ProductUserVM.User.Email,
                    PhoneNumber = ProductUserVM.User.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstants.StatusPending
                };
                _orderHeaderRepository.Add(orderHeader);
                _orderHeaderRepository.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        ProductId = product.Id,
                        Sqft = product.TempSqFt,
                        PricePerSqFt = product.Price
                    };
                    _orderDetailRepository.Add(orderDetail);
                }
                _orderDetailRepository.Save();
                return RedirectToAction("InquiryConfirmation", new { id = orderHeader.Id });
            }
            else
            { 
                // create inquiry
                string pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates" +  
                                    Path.DirectorySeparatorChar.ToString() + "Inquiry.html";  // путь к шаблону email-сообшщения

                var subject = "New Inquiry";
                string HtmlBody = "";

                using (StreamReader reader = System.IO.File.OpenText(pathToTemplate))  // загружаем код шаблона в тело сообщения
                {
                    HtmlBody = reader.ReadToEnd();
                }

                StringBuilder  productListSB = new StringBuilder();
                foreach(var prod in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name:{prod.Name} <span syle='font-size:14px;'> (ID: {prod.Id})</span><br />");  // добавляем строки с товарами 
                }

                // по сути в HtmlBody есть скобки {} поэтому получается подставка агрументов под номера 
                string messageBody = string.Format(HtmlBody, ProductUserVM.User.FullName, ProductUserVM.User.PhoneNumber, ProductUserVM.User.Email, productListSB.ToString());
                await _emailSender.SendEmailAsync(WebConstants.AdminEmail, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    UserId = claim.Value,
                    FullName = ProductUserVM.User.FullName,
                    PhoneNumber = ProductUserVM.User.PhoneNumber,
                    Email = ProductUserVM.User.Email,
                    InquiryDate = DateTime.Now
                };

                _inquiryHeaderRepository.Add(inquiryHeader);
                _inquiryHeaderRepository.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id
                    };
                    _inquiryDetailRepository.Add(inquiryDetail);
                }
                _inquiryDetailRepository.Save();
                TempData[WebConstants.Success] = "Order successfully created";
            }
            return RedirectToAction("InquiryConfirmation");
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id);
            HttpContext.Session.Clear();

            return View(orderHeader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in productList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction("Index");
        }
    }
}
