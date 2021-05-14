using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.Internal.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IProductData _productData;

        public ProductController(IConfiguration config, IProductData productData)
        {
            _config = config;
            _productData = productData;
        }

        [HttpGet]
        // GET api/values
        public List<ProductModel> Get()
        {
            //before dependency injection
            //ProductData data = new ProductData(_config);
            //return data.GetProducts();

            //after dependency injection
            return _productData.GetProducts();
        }
    }
}
