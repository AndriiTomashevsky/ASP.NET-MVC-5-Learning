using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class ImageTests
    {
        //Мы должны удостовериться в том, что метод GetImage() возвращает корректный тип MIME из хранилища, и что никакие данные не возвращаются, 
        //если запрошенный идентификатор товара не существует.
        [Test]
        public void Can_Retrieve_Image_Data()
        {
            //Arrange - create Product with image data
            Product product = new Product()
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(productRepository => productRepository.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                product,
                new Product {ProductID = 3, Name = "P3"}
            }
            .AsQueryable());

            // Arrange - create the controller
            ProductController productController = new ProductController(mock.Object);

            // Act - call the GetImage action method
            ActionResult result = productController.GetImage(2);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(FileContentResult), result);
            Assert.AreEqual(product.ImageMimeType, ((FileResult)result).ContentType);
        }

        [Test]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"}
            }
            .AsQueryable());

            // Arrange - create the controller
            ProductController target = new ProductController(mock.Object);

            // Act - call the GetImage action method
            ActionResult result = target.GetImage(100);

            // Assert
            Assert.IsNull(result);
        }
    }
}
