using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TRMApi.Controllers
{
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)            
        {
            //we won't actually need grant_type, but the original one does. For compatibility
            if (await IsValidUserNameAndPassword(username, password))
            {
                //generate token and return in the same format as the the .NET Framework project
                //So we don't have to rewrite all that. 
                return new ObjectResult(await GenerateToken(username));
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<bool> IsValidUserNameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username); //username is email address
            return await _userManager.CheckPasswordAsync(user, password); //is password valid?
        }

        //this token will be signed, not encrypted
        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username); //username is email address
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };  //list of all roles for a given user 

            //claims is part of JWT (JSON web token)
            var claims = new List<Claim> //claim is a key values type
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                    //nbf = not valid before a certain date/time
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
                    //exp = expiration (1 day out in this example.
            };

            //no matter who we are and how many roles, it adds that to the claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            //create the token
            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        //this is the encoding key into UTF8 to sign the token.
                        //This is a secret key and someone else can encode a new claim using it.  
                        //We will make it into a longer, random string stored in Azure Key Vault or something similar
                        //This is only for testing purposes
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKeyIsSecretSoDoNotTell")),
                        SecurityAlgorithms.HmacSha256)),  //for signing credentials 
                new JwtPayload(claims));  //claims is defined above. 

            //return it in the same format as we were returning in the .NET Framework project
            var output = new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token), //create the string from our toke
                UserName = username
            };

            return output;
        }
    }
}
