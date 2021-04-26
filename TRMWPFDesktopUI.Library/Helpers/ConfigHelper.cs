using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMWPFDesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            decimal output = 0;

            string rateText = ConfigurationManager.AppSettings["taxRate"];
            //need to have a valid number in app.config  Any bug should shut the system down.
            //output = double.Parse(rateText); //This will throw an exception without a valid number
            bool isValidTaxRate = decimal.TryParse(rateText, out output); //did the tryparse work or not?
            if (isValidTaxRate == false)
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            return output;
        }
    }
}
