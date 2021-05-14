using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.Internal.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoreyRetailManagerGood.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;

        public InventoryData(IConfiguration config, ISqlDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }
        public List<InventoryModel> Getinventory()
        {
            //pre-dependency injection
            //SqlDataAccess sql = new SqlDataAccess(_config);
            //var output = sql.LoadData<InventoryModel, dynamic>("dbo.sp_inventory_GetAll", new { }, "TRMData");

            //using dependency injection
            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.sp_inventory_GetAll", new { }, "TRMData");

            return output;
        }

        public void SaveInventoryReocrd(InventoryModel item)
        {
            //SqlDataAccess sql = new SqlDataAccess(_config);
            //sql.SaveData("dbo.sp_InventoryInsert", item, "TRMData");
            //don't have to speicify type since I'm passing the item and it knows from that. 

            //using dependency injection
            _sql.SaveData("dbo.sp_InventoryInsert", item, "TRMData");
        }
    }
}
