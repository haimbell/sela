using Core;
using Sela_Reprice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Hosting;
using System.Web.Http;

namespace Sela_Reprice.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [LimitConcurrentRequestAttribute(Limit = 2)]
        [Route("reprice")]
        [HttpPost]
        public IHttpActionResult Reprice([FromBody]ProductUpdateModel product)
        {
            _productRepository.Set(new Product()
            {
                Id = product.Id,
                Price = product.Price,
                Created = DateTime.Now,

            });
            return StatusCode(HttpStatusCode.Accepted);
        }


        [LimitConcurrentRequestAttribute(Limit = 50)]
        [Route("{id}/price")]
        public IHttpActionResult Get(Guid id)
        {
            var product = _productRepository.Get(id);
            return Ok(product);
        }

        [Route("query")]
        public IHttpActionResult Query(Guid productId, DateTime from, DateTime to, int pageSize, int pageNumber)
        {
            var product = _productRepository.All();
            var page = product.Where(x => x.Created >= from && x.Created <= to).Skip(pageSize * pageNumber)
                .Take(pageSize);

            return Ok(page);
        }
    }
}
