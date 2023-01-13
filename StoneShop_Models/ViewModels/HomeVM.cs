using System.Collections.Generic;

namespace StoneShop_Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Category> Categorys { get; set; }

    }
}
