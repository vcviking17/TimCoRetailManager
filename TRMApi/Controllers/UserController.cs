using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TimCoreyRetailManagerGood.Library.DataAccess;
using TimCoreyRetailManagerGood.Library.Models;
using TRMApi.Data;
using TRMApi.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> usermanger, 
            IConfiguration config, IUserData userData, ILogger<UserController> logger)
        {
            _context = context;
            _userManager = usermanger;
            _config = config;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            //get the information about the user
            //string userId = RequestContext.Principal.Identity.GetUserId();  //string userId = RequestContext.Principal.Identity.GetUserId();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //pre dependency injection
            //UserData data = new UserData(_config);

            //The model is from the data access.
            //We will have a different display model for the API that
            //may have different display for example. 
            //We don't want display models in data access models. 
            //return data.GetUserById(userId).First();
            return _userData.GetUserById(userId).First();

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            //entity framework
            //using (var context = new ApplicationDbContext())             
            //{
                //var userStore = new UserStore<ApplicationUser>(_context);
                //var userManager = new UserManager<ApplicationUser>(userStore);

            //var users = userManager.Users.ToList();  //this gets me all the users
            var users = _context.Users.ToList();  //this gets me all the users
            //var roles = _context.Roles.ToList();  //gets all the roles
            //use LINQ to get the user roles from the database
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };


                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel()
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                //foreach (var r in user.Roles)
                //{
                //    //grab from the dictionary (LINQ)
                //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                //}

                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key => key.RoleId, val => val.Name);


                    output.Add(u);
                }
            //}
            return output;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            //using (var context = new ApplicationDbContext())
            //{
                //same as above, but convert it to the dictionary
                //lambda expression
                var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            //}
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        //public void AddARole(UserRolePairModel pairing)
        public async Task AddARole(UserRolePairModel pairing)
        {
            //from above as a template
            //using (var context = new ApplicationDbContext())
            //{
            //var userStore = new UserStore<ApplicationUser>(_context);
            //var userManager = new UserManager<ApplicationUser>(userStore);

            //userManager.AddToRoleAsync(pairing.UserId, pairing.RoleName);

            //}
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  //from GetByUserId
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);
            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                loggedInUserId, user.Id, pairing.RoleName); 
            //use the structured logging to make searching by user easier for example; not string interpolation

            await _userManager.AddToRoleAsync(user, pairing.RoleName);

            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            //using (var context = new ApplicationDbContext())
            //{
            //    var userStore = new UserStore<ApplicationUser>(context);
            //    var userManager = new UserManager<ApplicationUser>(userStore);

            //    userManager.RemoveFromRoleAsync(pairing.UserId, pairing.RoleName);
            //}

            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  //from GetByUserId
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
                loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
        }
    }
}
