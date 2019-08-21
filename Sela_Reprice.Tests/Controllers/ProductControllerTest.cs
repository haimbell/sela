using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sela_Reprice;
using Sela_Reprice.Controllers;
using Sela_Reprice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sela_Reprice.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        /// <summary>
        /// 50 Parallel request with 50 different products
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestA()
        {
            // Arrange
            ProductController controller = new ProductController(new FileSystemRepository(new FileSystemTestFactory(), new MemoryCacheService()));
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            List<dynamic> results = new List<dynamic>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                Task task = Task.Run(async () =>
                {
                    var productUpdate = new ProductUpdateModel() { Price = i * 2.1, Id = Guid.NewGuid() };
                    var httpActionResult = controller.Reprice(productUpdate);
                    var actionResult = await httpActionResult.ExecuteAsync(new CancellationToken());
                    results.Add(new { statusCode = actionResult.StatusCode });
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Thread.Sleep(3000);
            foreach (dynamic result in results)
            {
                //ProductUpdateModel productUpdateModel = result.product;
                HttpStatusCode statusCode = result.statusCode;
                Assert.AreEqual(HttpStatusCode.Accepted, statusCode);
            }
            // Assert
        }

        /// <summary>
        /// 100 Parallel request with 50 different products
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestB()
        {
            // Arrange
            ProductController controller = new ProductController(new FileSystemRepository(new FileSystemTestFactory(), new MemoryCacheService()));
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            List<ProductUpdateModel> products = new List<ProductUpdateModel>();
            for (int i = 0; i < 50; i++)
            {
                var productUpdate = new ProductUpdateModel() { Price = i * 2.1, Id = Guid.NewGuid() };
                products.Add(productUpdate);
            }

            products = products.OrderBy(x => Guid.NewGuid()).ToList();
            products.AddRange(products);


            List<dynamic> results = new List<dynamic>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var productUpdate = products[i];
                Task task = Task.Run(async () =>
                {
                    var httpActionResult = controller.Reprice(productUpdate);
                    var actionResult = await httpActionResult.ExecuteAsync(new CancellationToken());
                    results.Add(new { statusCode = actionResult.StatusCode });
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Thread.Sleep(3000);
            foreach (dynamic result in results)
            {
                //ProductUpdateModel productUpdateModel = result.product;
                HttpStatusCode statusCode = result.statusCode;
                Assert.AreEqual(HttpStatusCode.Accepted, statusCode);
            }
            // Assert
        }

        /// <summary>
        /// 100 Parallel request with 50 different products
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestC()
        {
            // Arrange
            ProductController controller = new ProductController(new FileSystemRepository(new FileSystemTestFactory(), new MemoryCacheService()));
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            List<ProductUpdateModel> products = new List<ProductUpdateModel>();
            for (int i = 0; i < 4; i++)
            {
                var productUpdate = new ProductUpdateModel() { Price = i * 2.1, Id = Guid.NewGuid() };
                products.Add(productUpdate);
            }

            products = products.OrderBy(x => Guid.NewGuid()).ToList();
            products.AddRange(products);


            List<dynamic> results = new List<dynamic>();
            for (int i = 0; i < 100; i++)
            {
                var productUpdate = products[i];
                var httpActionResult = controller.Reprice(productUpdate);
                var actionResult = await httpActionResult.ExecuteAsync(new CancellationToken());
                results.Add(new { statusCode = actionResult.StatusCode });
                Thread.Sleep(500);
            }

            Thread.Sleep(3000);
            foreach (dynamic result in results)
            {
                //ProductUpdateModel productUpdateModel = result.product;
                HttpStatusCode statusCode = result.statusCode;
                Assert.AreEqual(HttpStatusCode.Accepted, statusCode);
            }
            // Assert
        }

    }
}
