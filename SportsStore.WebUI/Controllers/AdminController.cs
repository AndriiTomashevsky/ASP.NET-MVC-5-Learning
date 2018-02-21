using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        IProductRepository productRepository;

        public AdminController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public ViewResult Index()
        {
            return View(productRepository.Products);
        }

        public ViewResult Edit(int productId)
        {
            Product product = productRepository.Products
                .FirstOrDefault(item => item.ProductID == productId);

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.SaveProduct(product);
                TempData["message"] = string.Format($"{product.Name} has been Saved");

                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(product);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deleteProduct = productRepository.DeleteProduct(productId);

            if (deleteProduct != null)
            {
                TempData["message"] = string.Format($"{deleteProduct.Name} was deleted");
            }

            return RedirectToAction("Index");
        }
    }
}
