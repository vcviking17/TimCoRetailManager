using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TimCoRetailManagerGood.Models;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;

namespace TimCoRetailManagerGood.Controllers
{
    [Authorize] //This has to be everyone since we call this to see who is logged in
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            //entity framework
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();  //this gets me all the users
                var roles = context.Roles.ToList();  //gets all the roles

                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel()
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                    foreach (var r in user.Roles)
                    {
                        //grab from the dictionary (LINQ)
                        u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                    }
                    output.Add(u);
                }
            }
            return output;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                //same as above, but convert it to the dictionary
                //lambda expression
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public void AddARole(UserRolePairModel pairing)
        {
            //from above as a template
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(pairing.UserId, pairing.RoleName);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public void RemoveARole(UserRolePairModel pairing)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            }
        }
    }
}
