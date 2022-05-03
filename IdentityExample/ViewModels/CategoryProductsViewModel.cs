using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class CategoryProductsViewModel
    {
        public List<Product> PopularProducts { get; set; }

        public IQueryable<Product> AllProductsWithCategory { get; set; }

        public List<Product> FilteredProducts { get; set; }
        
        public Filter Filter { get; set; }

        public List<Manufacturer> Manufacturers;

        public List<Photo> Photos { get; set; }

        public string CurrentCategory { get; set; }

        public Category Category { get; set; }

        public List<Category> ChildCategories { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public List<int> ManufacturerIds { get; set; } = new List<int>();

        public int[] Ids { get; set; }

        public List<Manufacturer>Test { get; set; }
    }

    public class Manufacturers
    {
        List<Manufacturer> manufacturers;


        public Manufacturers()
        {
            manufacturers = new List<Manufacturer>();
        }

        public List<Manufacturer> SelectedManufacturers => manufacturers;


        public void AddItem(Manufacturer man)
        {
            Manufacturer manufacturer = manufacturers.FirstOrDefault(item => item.Id == man.Id);
            if (!manufacturers.Contains(manufacturer))
                manufacturers.Add(man);
            else
                manufacturers.Remove(man);
        }
    }
}
