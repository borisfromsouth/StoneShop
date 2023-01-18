using Microsoft.AspNetCore.Mvc.Rendering;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Utility;
using System.Collections.Generic;
using System.Linq;

namespace StoneShop_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public InquiryHeaderRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(InquiryHeader obj)
        {
            //var objFromDataBase = _dataBase.Category.FirstOrDefault(u => u.Id == obj.Id);  // используется метод базового класса Repository
            //var objFromDataBase = base.FirstOrDefault(u => u.Id == obj.Id);  // базовый метод + фильтр заполняется  из типа T, свойства в скобках
            //if (objFromDataBase == null)
            //{
            //    objFromDataBase.Name = obj.Name;
            //    objFromDataBase.DisplayOrder = obj.DisplayOrder;
            //}
            _dataBase.InquiryHeader.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
