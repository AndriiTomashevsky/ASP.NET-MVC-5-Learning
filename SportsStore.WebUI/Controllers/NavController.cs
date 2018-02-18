using SportsStore.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        IProductRepository productRepository;

        public NavController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        // GET: Nav
        public PartialViewResult Menu()
        {
            IEnumerable<string> categories = productRepository.Products
                .Select(item => item.Category)
                .Distinct()
                .OrderBy(value => value);

            return PartialView(categories);
        }
    }
}