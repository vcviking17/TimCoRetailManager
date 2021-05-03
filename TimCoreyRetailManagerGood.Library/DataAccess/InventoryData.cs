﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.Internal.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoreyRetailManagerGood.Library.DataAccess
{
    public class InventoryData
    {
        public List<InventoryModel> Getinventory()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.sp_inventory_GetAll", new { }, "TRMData");

            return output;
        }

        public void SaveInventoryReocrd(InventoryModel item)
        {
            SqlDataAccess sql = new SqlDataAccess();

            sql.SaveData("dbo.sp_InventoryInsert", item, "TRMData");
            //don't have to speicify type since I'm passing the item and it knows from that. 
        }
    }
}
