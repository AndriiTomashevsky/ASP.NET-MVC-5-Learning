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
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

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
            ViewResult viewResult = controller.List(null, 2);

            ProductsListViewModel result = (ProductsListViewModel)viewResult.Model;

            //Assert
            //Выполняется проверка, являются ли эти данные ожидаемыми.
            Product[] array = result.Products.ToArray();
            Assert.IsTrue(array.Length == 2);
            Assert.AreEqual(array[0].Name, "P4");
            Assert.AreEqual(array[1].Name, "P5");
        }

        //Чтобы протестировать вспомогательный метод PageLinks(), мы вызываем метод с тестовыми данными и сравниваем результаты
        //с ожидаемой HTML-разметкой.
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange - define an HTML helper - we need to do this in order to apply the extension method
            HtmlHelper htmlHelper = null;

            //Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalProducts = 28,
                ProductsPerPage = 10
            };

            //Arrange - set up the delegate using a lambda experession
            //Func<int, string> pageUrl = (int i) => "Page" + i;
            string pageUrl(int i) => "Page" + i;

            //Act
            MvcHtmlString result = htmlHelper.PageLinks(pagingInfo, pageUrl);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                 + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                 + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                 result.ToString());
        }

        //Необходимо удостовериться, что контроллер отправляет представлению правильную информацию о разбиении на страницы.
        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            ISetup<IProductRepository, IEnumerable<Product>> setup =
                mock.Setup((IProductRepository productRepository) => productRepository.Products);

            setup.Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object) { productsPerPage = 3 };

            //Act
            ViewResult viewResult = controller.List(null, 2);
            ProductsListViewModel result = (ProductsListViewModel)viewResult.Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ProductsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalProducts, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        //Нам необходим модульный тест для проверки функциональности фильтрации по категории, чтобы удостовериться в том, 
        //что фильтр может корректно генерировать сведения о товарах указанной категории.
        [TestMethod]
        public void Can_Filter_Products()
        {
            //Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            ISetup<IProductRepository, IEnumerable<Product>> setup = mock.Setup(item => item.Products);
            setup.Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            // Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object)
            {
                productsPerPage = 3
            };

            // Action
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        //Модульное тестирование: генерация списка категорий.
        //Цель заключается в создании списка, который отсортирован в алфавитном порядке и не содержит дубликатов.Для этого проще всего 
        //построить тестовые данные, которые имеют дублированные категории и не отсортированы должным образом, передать их в NavController 
        //и установить утверждение, что данные будут соответствующим образом очищены.
        //Внутри теста создается имитированная реализация хранилища, которая содержит повторяющиеся категории и категории, не отсортированные 
        //в алфавитном порядке.Затем определяется утверждение о том, что дубликаты будут удалены и алфавитный порядок восстановлен.

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange
            //-create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(item => item.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            });

            //Arrange-create the controller
            NavController navController = new NavController(mock.Object);

            //Act - get the set of categories
            string[] results = ((IEnumerable<string>)navController.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        //Модульное тестирование: сообщение о выбранной категории.
        //Для выполнения проверки того, что метод действия Menu() корректно добавил детали о выбранной категории, в модульном тесте 
        //можно прочитать значение свойства ViewBag, которое доступно через класс ViewResult.
        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //Arrange 
            //- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(productRepository => productRepository.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            });

            //Arrange-create the controller
            NavController navController = new NavController(mock.Object);

            //Arrange-define the category to selected
            string categoryToSelected = "Apples";

            //Action
            string result = navController.Menu(categoryToSelected).ViewBag.SelectedCategory;
        }

        //Модульное тестирование: счетчик товаров определенной категории.
        //Протестировать возможность генерации корректных счетчиков товаров для различных категорий можно очень просто - необходимо создать 
        //имитированное хранилище, которое содержит известные данные в диапазоне категорий, и затем вызывать метод действия List(), запрашивая 
        //каждую категорию по очереди.
        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(repository => repository.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            //Arrange - create controller and make the ProductsPerPage 3 items
            ProductController productController = new ProductController(mock.Object)
            {
                productsPerPage = 3
            };

            //Action - test the product counts for different categories
            int result1 = ((ProductsListViewModel)productController.List("Cat1").Model).PagingInfo.TotalProducts;
            int result2 = ((ProductsListViewModel)productController.List("Cat2").Model).PagingInfo.TotalProducts;
            int result3 = ((ProductsListViewModel)productController.List("Cat3").Model).PagingInfo.TotalProducts;
            int resultAll = ((ProductsListViewModel)productController.List(null).Model).PagingInfo.TotalProducts;

            //Assert
            Assert.AreEqual(result1, 2);
            Assert.AreEqual(result2, 2);
            Assert.AreEqual(result3, 1);
            Assert.AreEqual(resultAll, 5);
        }
    }
}
