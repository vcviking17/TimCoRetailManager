using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TRMWPFDesktopUI.Library.Models;

namespace TRMWPFDesktopUI.Library.Api
{
    public class UserEndpoint : IUserEndpoint
    {
        private readonly IAPIHelper _apiHelper;
        public UserEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UserModel>> GetAll()
        {
            //the headers were already created in APIHelper on the GetLoggedInUerInfo call, so we don't need to do that again
            //the token needs to be available for every API call.
            //_apiClient.DefaultRequestHeaders.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //send "credentials" with every API call
            //_apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllUsers"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UserModel>>();
                    return result;
                    //once the singleton is saved here, it's saved everywhere.
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<Dictionary<string, string>> GetAllRoles()
        {
            //the headers were already created in APIHelper on the GetLoggedInUerInfo call, so we don't need to do that again
            //the token needs to be available for every API call.
            //_apiClient.DefaultRequestHeaders.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //send "credentials" with every API call
            //_apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllRoles"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync <Dictionary<string, string>> ();
                    return result;
                    //once the singleton is saved here, it's saved everywhere.
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            //the headers were already created in APIHelper on the GetLoggedInUerInfo call, so we don't need to do that again
            //the token needs to be available for every API call.
            //_apiClient.DefaultRequestHeaders.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //send "credentials" with every API call
            //_apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/AddRole", data))
            {
                if (response.IsSuccessStatusCode == false)                
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            //the headers were already created in APIHelper on the GetLoggedInUerInfo call, so we don't need to do that again
            //the token needs to be available for every API call.
            //_apiClient.DefaultRequestHeaders.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Clear();
            //_apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //send "credentials" with every API call
            //_apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/RemoveRole", data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
