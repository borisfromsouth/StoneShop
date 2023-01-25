using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

namespace StoneShop_DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dataBase;

        public OrderDetailRepository(ApplicationDbContext dataBase) : base(dataBase)
        {
            _dataBase = dataBase;
        }

        public void Update(OrderDetail obj)
        {
            _dataBase.OrderDetail.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
