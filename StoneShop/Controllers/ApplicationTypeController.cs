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
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _applicationTypeRepository;

        public ApplicationTypeController(IApplicationTypeRepository applicationTypeRepository)
        {
            _applicationTypeRepository = applicationTypeRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _applicationTypeRepository.GetAll();
            return View(objList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepository.Add(obj);
                _applicationTypeRepository.Save();
                TempData[WebConstants.Success] = "Application type created successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating application type";
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
                TempData[WebConstants.Error] = "Application type chose error";
            }
            var obj = _applicationTypeRepository.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
                TempData[WebConstants.Error] = "Application type not found";
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepository.Update(obj);
                _applicationTypeRepository.Save();
                TempData[WebConstants.Success] = "Application type change successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Application type change error";
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            var obj = _applicationTypeRepository.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Application type not found";
                return NotFound();
            }
            _applicationTypeRepository.Remove(obj);
            _applicationTypeRepository.Save();
            TempData[WebConstants.Success] = "Application type successfully delete";
            return RedirectToAction("Index");
        }
    }
}
