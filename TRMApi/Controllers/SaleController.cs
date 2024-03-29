﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ISaleData _saleData;

        public SaleController(IConfiguration config, ISaleData saleData)
        {
            _config = config;
            _saleData = saleData;
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            //SaleData data = new SaleData(_config);
            //string userId = RequestContext.Principal.Identity.GetUserId();  old way in .NET Framework
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //data.SaveSale(sale, userId);
            _saleData.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            //bool isAdmin = RequestContext.Principal.IsInRole("Admin");
            //bool isManager = RequestContext.Principal.IsInRole("Manager");
            //if (isAdmin)
            //{
            //    //do admin stuff
            //}
            //else if (isManager)
            //{
            //    //do manager stuff
            //}

            //SaleData data = new SaleData(_config);
            //return data.GetSalesReport();
            return _saleData.GetSalesReport();
        }
    }
}
