using StoneShop_DataAccess.Repository.IRepository;
using StoneShop_Models;

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
            _dataBase.InquiryDetail.Update(obj);  // родной метод ApplicationDbContext, обновляет всю запись
        }
    }
}
