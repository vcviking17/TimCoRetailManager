using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimCoreyRetailManagerGood.Library.Models
{
    public class SaleModel
    {
        public List<SaleDetailModel> SaleDetails { get; set; }
        //not initializing the list since I want to know if it's NULL
    }
}