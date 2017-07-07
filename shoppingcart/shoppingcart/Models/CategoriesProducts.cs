using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoppingcart.Models
{
    public class CategoriesProducts
    {
        public string activeCategory { get; set; }
        public List<Category> categories { get; set; }
        public PagedList.IPagedList<Product> products { get; set; }
    }
}