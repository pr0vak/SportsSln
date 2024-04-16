using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using Xunit;

namespace SportsStore.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void CanUseRepository()
        {
            // Организация 
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product { ProductId = 1, Name = "P1" },
                new Product { ProductId = 2, Name = "P2" }
            }).AsQueryable);
            HomeController controller = new HomeController(mock.Object);

            // Действия
            ProductsListViewModel result = controller.Index(null).ViewData.Model as ProductsListViewModel;
            Product[] prodArray = result.Products.ToArray();

            // Утверждения
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void CanPaginate()
        {
            // Организация 
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] 
            {
                new Product { ProductId = 1, Name = "P1" },
                new Product { ProductId = 2, Name = "P2" },
                new Product { ProductId = 3, Name = "P3" },
                new Product { ProductId = 4, Name = "P4" },
                new Product { ProductId = 5, Name = "P5" }
            }).AsQueryable);
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            // Действие
            ProductsListViewModel result = controller.Index(null, 2).ViewData.Model as ProductsListViewModel;

            // Утверждение
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void CanSendPaginationViewModel()
        {
            // Организация
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] 
            {
                new Product { ProductId = 1, Name = "P1" },
                new Product { ProductId = 2, Name = "P2" },
                new Product { ProductId = 3, Name = "P3" },
                new Product { ProductId = 4, Name = "P4" },
                new Product { ProductId = 5, Name = "P5" }
            }).AsQueryable);
            HomeController controller = new HomeController(mock.Object) { PageSize = 3 };

            // Действие
            ProductsListViewModel result = controller.Index(null, 2).ViewData.Model
                as ProductsListViewModel;
            PagingInfo pageInfo = result.PagingInfo;

            // Утверждение
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void CanFilterProducts()
        {
            // Организация
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductId = 1, Name = "P1", Category = "Cat1" },
                new Product { ProductId = 2, Name = "P2", Category = "Cat2" },
                new Product { ProductId = 3, Name = "P3", Category = "Cat1" },
                new Product { ProductId = 4, Name = "P4", Category = "Cat2" },
                new Product { ProductId = 5, Name = "P5", Category = "Cat3" }
            }).AsQueryable);
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            // Действия
            Product[] result = (controller.Index("Cat2", 1).ViewData.Model as ProductsListViewModel)
                .Products.ToArray();

            // Утверждения
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [Fact]
        public void GenerateCategorySpecificProductCount()
        {
            // Организация
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductId = 1, Name = "P1", Category = "Cat1" },
                new Product { ProductId = 2, Name = "P2", Category = "Cat2" },
                new Product { ProductId = 3, Name = "P3", Category = "Cat1" },
                new Product { ProductId = 4, Name = "P4", Category = "Cat2" },
                new Product { ProductId = 5, Name = "P5", Category = "Cat3" }
            }).AsQueryable);
            HomeController target = new HomeController(mock.Object);
            target.PageSize = 3;
            Func<ViewResult, ProductsListViewModel> GetModel = result => 
                result?.ViewData?.Model as ProductsListViewModel;

            // Действие
            int? res1 = GetModel(target.Index("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.Index("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.Index("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.Index(null))?.PagingInfo.TotalItems;

            // Утверждение
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}