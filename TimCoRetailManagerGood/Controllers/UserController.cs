using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoRetailManagerGood.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {   
        [HttpGet]
        public UserModel GetById()
        {
            //get the information about the user
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            //The model is from the data access.
            //We will have a different display model for the API that
            //may have different display for example. 
            //We don't want display models in data access models. 
            return data.GetUserById(userId).First();
            
        }
    }
}
