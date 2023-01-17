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
        //private readonly ApplicationDbContext _dataBase;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _categoryRepository.GetAll();
            return View(objList);
        }

        // операция GET показывает формочку
        public IActionResult Create()
        {
            return View();
        }

        // операция Post возвращает данные
        [HttpPost]
        [ValidateAntiForgeryToken] // аттрибут-токен для защиты данных 
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(obj); // добавление записи
                _categoryRepository.Save();     // созранение в БД
                return RedirectToAction("Index");  // возвращаемся на страниццу со всеми записями
            }
            return View(obj);
            
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var obj = _categoryRepository.Find(id.GetValueOrDefault());
            if (obj == null) return NotFound();

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
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var obj = _categoryRepository.Find(id.GetValueOrDefault());
            if (obj == null) return NotFound();

            _categoryRepository.Remove(obj);
            _categoryRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
