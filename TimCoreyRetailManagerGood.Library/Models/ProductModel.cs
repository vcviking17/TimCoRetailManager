using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimCoreyRetailManagerGood.Library.Models
{
    public class ProductModel
    {
        //For API documentation
        /// <summary>
        /// The uniqe identifier for a given product
        /// </summary>

        //This will basically be the columns in the Prodcut table
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        public int QuantityInStock { get; set; }
    }
}
