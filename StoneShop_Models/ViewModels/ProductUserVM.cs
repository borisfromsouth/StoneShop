using System.Collections.Generic;

namespace StoneShop_Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList = new List<Product>();
        }

        public User User { get; set; }

        public IList<Product> ProductList { get; set; }
    }
}
