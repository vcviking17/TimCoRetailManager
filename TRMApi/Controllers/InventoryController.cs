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
        private readonly IInventoryData _inventoryData;

        public InventoryController(IConfiguration config, IInventoryData inventoryData)
        {
            _config = config;
            _inventoryData = inventoryData;
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> Get()
        {
            //pre-dependency injection
            //InventoryData data = new InventoryData(_config);
            //return data.Getinventory();
            return _inventoryData.Getinventory(); //with dependency injection
        }

        [Authorize(Roles = "Admin")] //Can buy products add to inventory--maybe another role later on
        [HttpPost]
        public void Post(InventoryModel item)
        {
            //pre-dependency injection
            //InventoryData data = new InventoryData(_config);
            //data.SaveInventoryReocrd(item);
            _inventoryData.SaveInventoryReocrd(item); //with dependency injection
        }
    }
}
