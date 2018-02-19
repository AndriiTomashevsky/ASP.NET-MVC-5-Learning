using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class AdminTests
    {
        [Test]
        public void Index_Contains_All_Products()
        {
            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(repository => repository.Products).Returns(new Product[]{
            new Product(){ProductID=1,Name="P1" },
            new Product(){ProductID=2,Name="P2" },
            new Product(){ProductID=3,Name="P3" }});

            // Arrange - create a controller
            AdminController adminController = new AdminController(mock.Object);

            // Action
            Product[] result = ((IEnumerable<Product>)adminController.Index().Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        //В методе действия Edit() нужно протестировать два аспекта поведения.Первый из них состоит в том, что мы получаем запрашиваемый товар, 
        //когда предоставляем допустимое значение идентификатора.Очевидно, необходимо удостовериться, что мы редактируем товар, который ожидали. 
        [Test]
        public void Can_Edit_Product()
        {
            //Arrange - - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(productRepository => productRepository.Products).Returns(new Product[] {
            new Product(){ProductID=1,Name="P1" },
            new Product(){ProductID=2,Name="P2" },
            new Product(){ProductID=3,Name="P3" }});

            // Arrange - create the controller
            AdminController adminController = new AdminController(mock.Object);

            //Act
            Product result1 = adminController.Edit(1).ViewData.Model as Product;
            Product result2 = adminController.Edit(2).ViewData.Model as Product;
            Product result3 = adminController.Edit(3).ViewData.Model as Product;

            //Arrange
            Assert.AreEqual(1, result1.ProductID);
            Assert.AreEqual(2, result2.ProductID);
            Assert.AreEqual(3, result3.ProductID);
        }

        //Второй аспект поведения заключается в том, что мы не должны получать товар при запросе значения идентификатора, который отсутствует 
        //в хранилище. 
        [Test]
        public void Cannot_Edit_Nonexistent_Product()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1"},
            new Product {ProductID = 2, Name = "P2"},
            new Product {ProductID = 3, Name = "P3"},
            });

            // Arrange - create the controller
            AdminController adminController = new AdminController(mock.Object);

            // Act
            Product result = (Product)adminController.Edit(4).Model;

            // Assert
            Assert.IsNull(result);
        }

    }
}
