using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
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
            ProductsListViewModel model = new ProductsListViewModel()
            {
                Products = productRepository.Products
                    .OrderBy(item => item.ProductID)
                    .Skip((page - 1) * productsPerPage)
                    .Take(productsPerPage),

                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ProductsPerPage = productsPerPage,
                    TotalProducts = productRepository.Products.Count()
                }
            };

            return View(model);
        }
    }
}