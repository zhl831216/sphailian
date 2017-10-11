using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public class GroupHelper
    {

        public static async Task<string> CreateGroup(string groupName, string desc, string nickname)
        {
            var newGroup = new NewGroup();
            newGroup.description = desc;
            newGroup.displayName = groupName;
            newGroup.groupTypes = new List<string>();
            newGroup.groupTypes.Add("Unified");
            newGroup.mailEnabled = true;
            newGroup.securityEnabled = false;
            newGroup.mailNickname = nickname;

            string accessToken = ContextHelper.GetGraphAccessToken();

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
                    return responseString;
                }
                else
                {
                    // Something went wrong...
                    throw new Exception(await response.Content.ReadAsStringAsync());

                }
            }
        }

        public static async Task<List<UnifiedGroup>> GetGroups()
        {
            string accessToken = ContextHelper.GetGraphAccessToken();

            string requestUrl = "https://graph.microsoft.com/v1.0/groups";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //request.Content = new StringContent(JsonConvert.SerializeObject(subscription),
            //    Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponseModel<UnifiedGroup>>(responseString).Value;
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