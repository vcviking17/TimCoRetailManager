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
    public class UserData : IUserData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;

        public UserData(IConfiguration config, ISqlDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }
        public List<UserModel> GetUserById(string Id)
        {
            //pre dependency injection
            //SqlDataAccess sql = new SqlDataAccess(_config);

            var p = new { Id = Id };

            //we have to specifiy the list type since it's a T, U
            //dynamic is a trick we can use since it's the same assembly
            //var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");

            return output;
        }
    }
}
