using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimCoreyRetailManagerGood.Library.Internal.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoRetailManagerGood.Controllers
{
    [Authorize]
    public class ProductController : ApiController
    {
        [HttpGet]
        // GET api/values
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();

            return data.GetProducts();
        }
    }
}
