using Microsoft.AspNetCore.Mvc.Rendering;
using StoneShop_Models;
using System.Collections.Generic;

namespace StoneShop_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        IEnumerable<SelectListItem> GetAllDropdownLists(string obj);
    }
}
