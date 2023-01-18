using Microsoft.AspNetCore.Mvc.Rendering;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Utility;
using System.Collections.Generic;
using System.Linq;

namespace StoneShop_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public InquiryDetailRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(InquiryDetail obj)
        {
            //var objFromDataBase = _dataBase.Category.FirstOrDefault(u => u.Id == obj.Id);  // используется метод базового класса Repository
            //var objFromDataBase = base.FirstOrDefault(u => u.Id == obj.Id);  // базовый метод + фильтр заполняется  из типа T, свойства в скобках
            //if (objFromDataBase == null)
            //{
            //    objFromDataBase.Name = obj.Name;
            //    objFromDataBase.DisplayOrder = obj.DisplayOrder;
            //}
            _dataBase.InquiryDetail.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
