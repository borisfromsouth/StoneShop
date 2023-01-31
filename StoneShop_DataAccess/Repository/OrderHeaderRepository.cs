using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

namespace StoneShop_DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public OrderHeaderRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(OrderHeader obj)
        {
            _dataBase.OrderHeader.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
