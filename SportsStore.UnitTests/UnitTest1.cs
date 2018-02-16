using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        //Модульное тестирование средства разбиения на страницы можно провести, создав имитированное хранилище, внедрив его 
        //в конструктор класса ProductController и вызвав метод List(), чтобы запросить конкретную страницу. Затем полученные
        //объекты Product можно сравнить с теми, которые ожидались от тестовых данных в имитированной реализации.
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            ISetup<IProductRepository, IEnumerable<Product>> setup =
            mock.Setup((IProductRepository item) => item.Products);

            setup.Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object) { productsPerPage = 3 };

            //Act
            //Получаем данные, возвращаемые методом контроллера.
            ViewResult viewResult = controller.List(2);

            IEnumerable<Product> result = (IEnumerable<Product>)viewResult.Model;

            //Assert
            //Выполняется проверка, являются ли эти данные ожидаемыми.
            Product[] array = result.ToArray();
            Assert.IsTrue(array.Length == 2);
            Assert.AreEqual(array[0].Name, "P4");
            Assert.AreEqual(array[1].Name, "P5");
        }
    }
}
