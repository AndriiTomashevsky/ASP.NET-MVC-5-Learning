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
        public ViewResult List(string category, int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel()
            {
                Products = productRepository.Products
                    .Where(item => category == null || item.Category == category)
                    .OrderBy(item => item.ProductID)
                    .Skip((page - 1) * productsPerPage)
                    .Take(productsPerPage),

                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ProductsPerPage = productsPerPage,
                    TotalProducts = category == null ?
                        productRepository.Products.Count() :
                        productRepository.Products.Where(item => item.Category == category).Count()
                },
                CurrentCategory = category
            };

            return View(model);
        }

        public FileContentResult GetImage(int productId)
        {
            Product product = productRepository.Products
                .FirstOrDefault(item => item.ProductID == productId);

            if (product != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}