using LopuhDC.Data.Remote.Models;
using System;

namespace LopuhDC.Domain.Contexts
{
    public class ProductContext
    {
        public Product Product { get; set; }
        public event Action<Product> ProductAdded;
        public void AddProductNotification( Product product)
        {
            ProductAdded?.Invoke(product);
        }
    }
}
