using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

namespace StoneShop_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public CategoryRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(Category obj)
        {
            //var objFromDataBase = _dataBase.Category.FirstOrDefault(u => u.Id == obj.Id);  // используется метод базового класса Repository
            var objFromDataBase = base.FirstOrDefault(u => u.Id == obj.Id);  // базовый метод + фильтр заполняется  из типа T, свойства в скобках
            if (objFromDataBase == null)
            {
                objFromDataBase.Name = obj.Name;
                objFromDataBase.DisplayOrder = obj.DisplayOrder;
            }
        }
    }
}
