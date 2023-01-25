using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

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
            _dataBase.InquiryHeader.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
