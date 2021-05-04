using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoRetailManagerGood.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            string userId = RequestContext.Principal.Identity.GetUserId();

            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            bool isAdmin = RequestContext.Principal.IsInRole("Admin");
            bool isManager = RequestContext.Principal.IsInRole("Manager");
            if (isAdmin)
            {
                //do admin stuff
            }
            else if (isManager)
            {
                //do manager stuff
            }

            SaleData data = new SaleData();
            return data.GetSalesReport();
        }
    }
}
