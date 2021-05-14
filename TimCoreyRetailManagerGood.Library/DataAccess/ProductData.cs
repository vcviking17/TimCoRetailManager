using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoreyRetailManagerGood.Library.Internal.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;

        public ProductData(IConfiguration config, ISqlDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }
        public List<ProductModel> GetProducts()
        {
            //before dependency injection
            //SqlDataAccess sql = new SqlDataAccess(_config);
            //dynamic is a trick we can use since it's the same assembly
            //var output = sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetAll", new { }, "TRMData");
            //with dependency injection
            var output = _sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetAll", new { }, "TRMData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            //before dependency injection
            //SqlDataAccess sql = new SqlDataAccess(_config);
            //dynamic is a trick we can use since it's the same assembly
            //var output = sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            //with dependency injection
            var output = _sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return output;
        }
    }
}
