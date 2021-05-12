using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData(_config);
            return data.Getinventory();
        }

        [Authorize(Roles = "Admin")] //Can buy products add to inventory--maybe another role later on
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(_config);
            data.SaveInventoryReocrd(item);
        }
    }
}
