using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        List<CartLine> list = new List<CartLine>();

        //add an item to the cart
        public void AddItem(Product product, int quantity)
        {
            CartLine cartLine = list
                .Where(cl => cl.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if (cartLine == null)
            {
                list.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                cartLine.Quantity += quantity;
            }
        }

        //remove a previously added item from the cart
        public void RemoveLine(Product product)
        {
            list.RemoveAll(cartLine => cartLine.Product.ProductID == product.ProductID);
        }

        //calculate the total cost of the items in the cart
        public decimal ComputeTotalValue()
        {
            return list.Sum(cartLine => cartLine.Product.Price * cartLine.Quantity);
        }

        //reset the cart by removing all of the items
        public void Clear()
        {
            list.Clear();
        }

        public IEnumerable<CartLine> Lines { get { return list; } }
    }

    public class CartLine
    {
        public Product Product { get; set; }   // product selected by the customer
        public int Quantity { get; set; }      //quantity the user wants to buy
    }
}
