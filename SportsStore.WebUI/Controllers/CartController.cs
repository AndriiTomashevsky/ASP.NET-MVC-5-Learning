using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        IProductRepository repository;

        public CartController(IProductRepository repository)
        {
            this.repository = repository;
        }

        public RedirectToRouteResult AddToCart(int productId, string returnUrl)
        {
            Product product = repository.Products
                                        .FirstOrDefault(item => item.ProductID == productId);

            if (product != null)
            {
                GetCart().AddItem(product, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(int productId, string returnUrl)
        {
            Product product = repository.Products
                              .FirstOrDefault(item => item.ProductID == productId);

            if (product != null)
            {
                GetCart().RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        private Cart GetCart()
        {
            //retrieve an object from the session state
            Cart Cart = (Cart)Session["Cart"];

            if (Cart == null)
            {
                Cart = new Cart();

                //add an object to the session state
                Session["Cart"] = Cart;
            }
            return Cart;
        }
    }
}
