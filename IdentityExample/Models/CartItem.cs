using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class CartItem
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }

    public class Cart
    {
        List<CartItem> cartItems;


        public Cart()
        {
            cartItems = new List<CartItem>();
        }

        public List<CartItem> CartItems => cartItems;


        public void AddItem(Product product, int count)
        {
            CartItem cartItem = cartItems.FirstOrDefault(item => item.Product.Id == product.Id);
            if (cartItem != null)
                cartItem.Quantity += count;
            else
                cartItems.Add(new CartItem { Product = product, Quantity = count });
        }
        public void RemoveItem(Product product)
        {
            cartItems.RemoveAll(t => t.Product.Id == product.Id);
        }

        public void Clear()
        {
            cartItems.Clear();
            
        }

        public int GetTotalSum()
        {
            int totalSum = 0;
            foreach(CartItem item in cartItems)
                totalSum += item.Product.PriceWithDiscount != 0 ? (int)item.Product.PriceWithDiscount * item.Quantity : (int)item.Product.Price * item.Quantity;
            return totalSum;
        }

        public void AddItems(IEnumerable<CartItem> items)
        {
            cartItems = items.ToList();
        }
    }
}
