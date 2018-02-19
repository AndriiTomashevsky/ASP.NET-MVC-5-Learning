using System;
using System.Collections.Generic;
using System.Linq;
using SportsStore.Domain.Entities;
using NUnit.Framework;
using SportsStore.WebUI.Controllers;
using Moq;
using SportsStore.Domain.Abstract;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class CartTests
    {
        //Добавление элемента в корзину.При самом первом добавлении в корзину объекта Game должен быть добавлен новый экземпляр CartLine.
        //Ниже показан тестовый метод, включая определение класса модульного тестирования
        [Test]
        public void Can_Add_New_Lines()
        {
            //Arrange - create some test products
            Product product1 = new Product { ProductID = 1, Name = "P1" };
            Product product2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create new cart
            Cart cart = new Cart();

            //Act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 2);

            CartLine[] result = ((List<CartLine>)cart.Lines).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, product1);
            Assert.AreEqual(result[1].Product, product2);
        }

        //Однако если пользователь уже добавил объект Poduct в корзину, необходимо увеличить количество в соответствующем экземпляре CartLine, 
        //а не создавать новый.
        [Test]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange - create some test products
            Product product1 = new Product { ProductID = 1, Name = "P1" };
            Product product2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create new cart
            Cart cart = new Cart();

            //Act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            cart.AddItem(product1, 10);
            //CartLine[] result = ((List<CartLine>)cart.Lines).Where(cartLine=>cartLine.Product.ProductID==1).ToArray();
            var result = (from item in cart.Lines
                          orderby item.Product.ProductID
                          select item).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        //Мы также должны проверить, что пользователи имеют возможность менять свое решение и удалять товары из корзины.Это средство 
        //реализовано в виде метода RemoveLine(). Ниже приведен необходимый тестовый метод: 
        [Test]
        public void Can_Remove_Line()
        {
            //Arrange - create some test products
            Product product1 = new Product() { ProductID = 1, Name = "P1" };
            Product product2 = new Product() { ProductID = 2, Name = "P2" };
            Product product3 = new Product() { ProductID = 3, Name = "P3" };

            //Arrange - create a new cart
            Cart cart = new Cart();

            //Arrange - add some products to the cart
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 3);
            cart.AddItem(product3, 5);
            cart.AddItem(product2, 1);

            //Act
            cart.RemoveLine(product2);

            //Assert
            Assert.AreEqual(cart.Lines.Where(item => item.Product == product2).Count(), 0);
            Assert.AreEqual(cart.Lines.ToArray().Length, 2);
        }

        //Следующее проверяемое поведение касается возможности вычисления общей стоимости элементов в корзине:
        [Test]
        public void Calculate_Cart_Total()
        {
            //Arrange - create some test products
            Product product1 = new Product() { ProductID = 1, Name = "P1", Price = 100M };
            Product product2 = new Product() { ProductID = 2, Name = "P2", Price = 50M };

            //Arrange - create a new cart
            Cart cart = new Cart();

            //Arrange - add some products to the cart
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            cart.AddItem(product1, 3);

            //Act
            decimal result = cart.ComputeTotalValue();

            //Assert
            Assert.AreEqual(result, 450);
        }

        //Мы должны удостовериться, что в результате очистки корзины ее содержимое корректно удаляется.
        [Test]
        public void Can_Clear_Contents()
        {
            // Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - create a new cart
            Cart cart = new Cart();

            // Arrange - add some items
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            // Act - reset the cart
            cart.Clear();

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 0);
        }
    }
}