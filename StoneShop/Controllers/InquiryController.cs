using Microsoft.AspNetCore.Mvc;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using System.Collections.Generic;

namespace StoneShop.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailRepository inquiryDetailRepository)
        {
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
        }

        public IActionResult Index()
        {
            //IEnumerable<InquiryHeader> items = _inquiryHeaderRepository.FirstOrDefault();
            return View(/*items*/);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }

        #endregion
    }
}
