﻿using System;
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
        public void SaveSale(SaleModel saleinfo, string cashierId)
        {

            ProductData products = new ProductData();
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

            //Save the sales model - create a call to the database
            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.sp_Sale_Insert", sale, "TRMData");

            //since sale is passed by reference, the SaleId should be updated on the insert of the db
            //Get the ID from the sale model
            int salesId = sql.LoadData<int, dynamic>("dbo.sp_SaleLookup", 
                new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }, "TRMData").FirstOrDefault();
            sale.Id = salesId;

            //Finish filling in the sale detail models
            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                //save the sale detail models
                sql.SaveData("dbo.sp_SaleDetail_Insert", item, "TRMData");
            }
        }
    }
}