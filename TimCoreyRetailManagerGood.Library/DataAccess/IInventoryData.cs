using System.Collections.Generic;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoreyRetailManagerGood.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> Getinventory();
        void SaveInventoryReocrd(InventoryModel item);
    }
}