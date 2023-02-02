using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoneShop_DataAccess;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Models.ViewModels;
using StoneShop_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StoneShop.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnviroment)
        {
            _productRepository = productRepository;
            _webHostEnviroment = webHostEnviroment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _productRepository.GetAll(includeProperties: "Category,ApplicationType");  // добавляем дополнительные нужные нам таблицы 

            return View(objList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)  // Upsert - общий метод для создания и редактирования
        {
           ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _productRepository.GetAllDropdownLists(WebConstants.CategoryName),
                ApplicationTypeSelectList = _productRepository.GetAllDropdownLists(WebConstants.ApplicationTypeName)
            };

            if (id == null) 
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _productRepository.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    TempData[WebConstants.Error] = "Product not found";
                    return NotFound();
                }

                return View(productVM);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken] // аттрибут-токен для защиты данных 
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;  // получаем файлы из формы ( поле имеет тип type="file" )
                string webRootPath = _webHostEnviroment.WebRootPath; // путь и папке wwwroot

                if (productVM.Product.Id == 0)
                {
                    // creating
                    string upload = webRootPath + WebConstants.ImagePath;  // путь до папки с картинками
                    string fileName = Guid.NewGuid().ToString();  // уникальный идентификатор
                    string extension = Path.GetExtension(files[0].FileName);  // расширение файла

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);  // запись в папку с картинками
                    }

                    productVM.Product.Image = fileName + extension;
                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    // updating
                    var objFromDb = _productRepository.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);  // получаем старую запись из БД; AsNoTracking() отключает отслеживание сущности
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _productRepository.Update(productVM.Product);
                }

                _productRepository.Save();
                TempData[WebConstants.Success] = "Create/update product successfully done";
                return RedirectToAction("Index");
            }

            // при невалидности модели список категорий не возвращается, так как мы вообще передаем только текущее значение

            productVM.CategorySelectList = _productRepository.GetAllDropdownLists(WebConstants.CategoryName);
            productVM.ApplicationTypeSelectList = _productRepository.GetAllDropdownLists(WebConstants.ApplicationTypeName);
            return View(productVM);

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) 
            {
                TempData[WebConstants.Error] = "Product not found";
                return NotFound();
            } 

            var obj = _productRepository.Find(id.GetValueOrDefault());
            if (obj == null) 
            {
                TempData[WebConstants.Error] = "Product not found";
                return NotFound();
            } 

            string webRootPath = _webHostEnviroment.WebRootPath;
            string upload = webRootPath + WebConstants.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productRepository.Remove(obj);
            _productRepository.Save();
            TempData[WebConstants.Success] = "Product created successfully";
            return RedirectToAction("Index");
        }
    }
}
