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
    public class SaleData
    {
        private readonly IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }
        public void SaveSale(SaleModel saleinfo, string cashierId)
        {

            ProductData products = new ProductData(_config);
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            //TODO:  Make this SOLID/DRY/Better
            //Start filling in the sale detail models we will save to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            foreach (var item in saleinfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //Get the information about this product via database call
                var productInfo = products.GetProductById(item.ProductId);

                //if it's not in the database
                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                //How do we do tax in SalesViewModel
                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }
                
                details.Add(detail);
            }
            //Create the Sale Model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            //with the using, the Dispose will be called at the end automatically
            //all calls inside will be made together
            using (SqlDataAccess sql = new SqlDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction("TRMData");
                    //Save the sales model - create a call to the database
                    sql.SaveDataInTransaction("dbo.sp_Sale_Insert", sale);

                    //Get the ID from the sale model
                    int salesId = sql.LoadDataInTransaction<int, dynamic>("dbo.sp_SaleLookup",
                        new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();
                    sale.Id = salesId;

                    //Finish filling in the sale detail models
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        //save the sale detail models
                        sql.SaveDataInTransaction("dbo.sp_SaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch //only need this for debugging to see the message.  (Exception ex)
                {
                    sql.RollbackTransaction();
                    throw;  //this will throw the original exception and not one we define
                }                
            }

            /////as it was before transaction start
            //Save the sales model - create a call to the database
            //SqlDataAccess sql = new SqlDataAccess();
            //sql.SaveData("dbo.sp_Sale_Insert", sale, "TRMData");

            ////Get the ID from the sale model
            //int salesId = sql.LoadData<int, dynamic>("dbo.sp_SaleLookup", 
            //    new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }, "TRMData").FirstOrDefault();
            //sale.Id = salesId;

            ////Finish filling in the sale detail models
            //foreach (var item in details)
            //{
            //    item.SaleId = sale.Id;
            //    //save the sale detail models
            //    sql.SaveData("dbo.sp_SaleDetail_Insert", item, "TRMData");
            //}
            /////as it was before transaction end
        }

        public List<SaleReportModel> GetSalesReport()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.sp_Sale_SaleReport", new { }, "TRMData");

            return output;
        }
    }
}
