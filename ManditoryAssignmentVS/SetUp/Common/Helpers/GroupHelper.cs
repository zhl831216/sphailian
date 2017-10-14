using CreateGroupSiteMicrosoftGraph.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Common.Models;

namespace CreateGroupSiteMicrosoftGraph.Helpers
{
    public class GroupHelper
    {
        public static string GetGraphAccessToken()
        {


            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string clientSecret = ConfigurationManager.AppSettings["clientSecret"];


            AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/folkuniversitetetsp2016.onmicrosoft.com/oauth2/token");

            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            AuthenticationResult authResult = authContext.AcquireTokenAsync("https://graph.microsoft.com/", creds).Result;

            return authResult.AccessToken;

        }


        public static void CreateUnifedGroup(string groupName, string desc, string nickname, string ownerEmail)
        {
            string accessToken = GetGraphAccessToken();
            var newGroup = CreateGroup(groupName, desc, nickname, accessToken).Result;
            var userId = GetUserId(accessToken, ownerEmail).Result;
            var isSuccess = AddOwnerToGroup(newGroup, userId, accessToken).Result;
        }


        private static async Task<bool> AddOwnerToGroup(UnifiedGroup group, User user, string accessToken)
        {

            string requestUrl = "https://graph.microsoft.com/v1.0/groups/" + group.id + "/owners/$ref";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var content = "{\"@odata.id\": \"https://graph.microsoft.com/v1.0/users/" + user.id + "\"}"; //JsonConvert.SerializeObject(user);

            request.Content = new StringContent(content,
                    Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    //string responseString = await response.Content.ReadAsStringAsync();

                    return true;


                }
                else
                {
                    // Something went wrong...
                    throw new Exception(await response.Content.ReadAsStringAsync());

                }
            }
        }


        private static async Task<UnifiedGroup> CreateGroup(string groupName, string desc, string nickname, string accessToken)
        {
            var newGroup = new NewGroup();
            newGroup.Description = desc;
            newGroup.DisplayName = groupName;
            newGroup.GroupTypes = new List<string>();
            newGroup.GroupTypes.Add("Unified");
            newGroup.MailEnabled = true;
            newGroup.SecurityEnabled = false;
            newGroup.MailNickname = nickname;



            string requestUrl = "https://graph.microsoft.com/v1.0/groups";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = new StringContent(JsonConvert.SerializeObject(newGroup),
                Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    UnifiedGroup group = JsonConvert.DeserializeObject<UnifiedGroup>(responseString);
                    return group;


                }
                else
                {
                    // Something went wrong...
                    throw new Exception(await response.Content.ReadAsStringAsync());

                }
            }
        }




        private static async Task<User> GetUserId(string accessToken, string userLogin)
        {
            string endpoint = "https://graph.microsoft.com/v1.0/users/" + userLogin + "?$select=id";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    User user = JsonConvert.DeserializeObject<User>(responseString);
                    return user;
                }
                else
                {
                    // Something went wrong...
                    throw new Exception(await response.Content.ReadAsStringAsync());

                }
            }

        }

        

        
        }
    }
