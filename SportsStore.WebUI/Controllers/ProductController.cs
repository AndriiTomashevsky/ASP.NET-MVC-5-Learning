using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        IProductRepository productRepository;
        public int productsPerPage = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        // GET: Product
        public ViewResult List(int page = 1)
        {
            IEnumerable<Product> model = productRepository.Products
                .OrderBy(item => item.ProductID)
                .Skip((page - 1) * productsPerPage)
                .Take(productsPerPage);

            return View(model);
        }
    }
}