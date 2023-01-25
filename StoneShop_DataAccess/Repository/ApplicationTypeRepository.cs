using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

namespace StoneShop_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public ApplicationTypeRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(ApplicationType obj)
        {
            var objFromDataBase = base.FirstOrDefault(u => u.Id == obj.Id);  // базовый метод + фильтр заполняется  из типа T, свойства в скобках
            if (objFromDataBase == null)
            {
                objFromDataBase.Name = obj.Name;
            }
        }
    }
}
