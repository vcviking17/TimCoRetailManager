using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoreyRetailManagerGood.Library.Internal.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration _config;

        public ProductData(IConfiguration config )
        {
            _config = config;
        }
        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);
                        
            //dynamic is a trick we can use since it's the same assembly
            var output = sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetAll", new { }, "TRMData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            //dynamic is a trick we can use since it's the same assembly
            var output = sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return output;
        }
    }
}
