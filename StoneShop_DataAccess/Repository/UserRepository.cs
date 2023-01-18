using Microsoft.AspNetCore.Mvc.Rendering;
using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;
using StoneShop_Utility;
using System.Collections.Generic;
using System.Linq;

namespace StoneShop_DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public UserRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public IEnumerable<SelectListItem> GetAllDropdownLists(string obj)
        {
            if (obj == WebConstants.CategoryName)
            {
                return _dataBase.Category.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            if (obj == WebConstants.ApplicationTypeName)
            {
                return _dataBase.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return null;
        }

        public void Update(User obj)
        {
            //var objFromDataBase = _dataBase.Category.FirstOrDefault(u => u.Id == obj.Id);  // используется метод базового класса Repository
            //var objFromDataBase = base.FirstOrDefault(u => u.Id == obj.Id);  // базовый метод + фильтр заполняется  из типа T, свойства в скобках
            //if (objFromDataBase == null)
            //{
            //    objFromDataBase.Name = obj.Name;
            //    objFromDataBase.DisplayOrder = obj.DisplayOrder;
            //}
            _dataBase.User.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
