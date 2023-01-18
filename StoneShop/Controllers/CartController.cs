using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using StoneShop_DataAccess;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Models.ViewModels;
using StoneShop_Utility;
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

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepository, IUserRepository userRepository, IInquiryHeaderRepository inquiryHeaderRepository,
                                IInquiryDetailRepository inquiryDetailRepository, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        public IActionResult Index()  // список всех товаров
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);  // получаем данные в сессии
            }

            List<int> prodInCart = shoppingCartList.Select(u => u.ProductId).ToList();  // получаем только данные определенного поля 
            IEnumerable<Product> productList = _productRepository.GetAll(u => prodInCart.Contains(u.Id));  // получаем список продуктов по списку id-шников в корзине


            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction("Summary");
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);  // получаем данные в сессии
            }

            List<int> prodInCart = shoppingCartList.Select(u => u.ProductId).ToList();  // получаем только данные определенного поля 
            IEnumerable<Product> productList = _productRepository.GetAll(u => prodInCart.Contains(u.Id));  // получаем список продуктов по списку id-шников в корзине


            ProductUserVM productUserVM = new ProductUserVM()
            {
                User = _userRepository.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = productList.ToList()
            };

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates" +  
                                    Path.DirectorySeparatorChar.ToString() + "Inquiry.html";  // путь к шаблону email-сообшщения

            var subject = "New Inquiry";
            string HtmlBody = "";

            using (StreamReader reader = System.IO.File.OpenText(pathToTemplate))  // загружаем код шаблона в тело сообщения
            {
                HtmlBody = reader.ReadToEnd();
            }

            StringBuilder  productListSB = new StringBuilder();
            foreach(var prod in productUserVM.ProductList)
            {
                productListSB.Append($" - Name:{prod.Name} <span syle='font-size:14px;'> (ID: {prod.Id})</span><br />");  // добавляем строки с товарами 
            }

            // по сути в HtmlBody есть скобки {} поэтому получается подставка агрументов под номера 
            string messageBody = string.Format(HtmlBody, productUserVM.User.FullName, productUserVM.User.PhoneNumber, productUserVM.User.Email, productListSB.ToString());
            await _emailSender.SendEmailAsync(WebConstants.AdminEmail, subject, messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                UserId = claim.Value,
                FullName = productUserVM.User.FullName,
                PhoneNumber = productUserVM.User.PhoneNumber,
                Email = productUserVM.User.Email,
                InquiryDate = System.DateTime.Now
            };

            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.Save();

            foreach (var product in productUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = product.Id
                };
                _inquiryDetailRepository.Add(inquiryDetail);
            }
            _inquiryDetailRepository.Save();
            return RedirectToAction("InquiryConfiguration");
        }

        public IActionResult InquiryConfiguration()
        {
            HttpContext.Session.Clear();

            return View();
        }
    }
}
