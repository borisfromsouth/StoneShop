using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneShop_DataAccess;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Utility;
using System.Collections.Generic;

namespace StoneShop.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _categoryRepository.GetAll();
            return View(objList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // аттрибут-токен для защиты данных 
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(obj); // добавление записи
                _categoryRepository.Save();     // созранение в БД
                TempData[WebConstants.Success] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating category";
            return View(obj);
            
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
            {
                TempData[WebConstants.Error] = "Category not found";
                return NotFound();
            }
            

            var obj = _categoryRepository.Find(id.GetValueOrDefault());
            if (obj == null) 
            {
                TempData[WebConstants.Error] = "Category not found";
                return NotFound();
            } 

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(obj);
                _categoryRepository.Save();
                TempData[WebConstants.Success] = "Category successfully changed";
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while edit category";
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                TempData[WebConstants.Error] = "Category not found";
                return NotFound();
            }
            
            var obj = _categoryRepository.Find(id.GetValueOrDefault());
            if (obj == null) 
            {
                TempData[WebConstants.Error] = "Category not found";
                return NotFound();
            } 

            _categoryRepository.Remove(obj);
            _categoryRepository.Save();
            TempData[WebConstants.Success] = "Category successfully delete";
            return RedirectToAction("Index");
        }
    }
}
