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
        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess();
                        
            //dynamic is a trick we can use since it's the same assembly
            var output = sql.LoadData<ProductModel, dynamic>("dbo.sp_Product_GetAll", new { }, "TRMData");

            return output;
        }
    }
}
