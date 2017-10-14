using Common.Helpers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            //ClientContext ctx = Common.Helpers.ContextHelper.GetSPContext("https://folkuniversitetetsp2016.sharepoint.com/sites/hailian/").Result;

            //ctx.Load(ctx.Web);
            //ctx.ExecuteQuery();

            //Console.WriteLine(ctx.Web.Title);


            //string webhookurl = "https://davidfa.azurewebsites.net/api/WebHookEndPoint?req=cw5TPHNRsc6iZe5Q0V3TzMXN4XgMxb6F/HP4XEcpPxhQI5AzPAZcwg==";


            //WebhookSubscription subscription = ctx.Web.GetListByTitle("CustomList").AddWebhookSubscription(webhookurl, DateTime.Now.AddMonths(5));



            //var status = GroupHelper.CreateGroup("csharp", "csharpdesc", "csharp").Result;
            //var groups = GroupHelper.GetGroups().Result;

            // you may need to store the subscription id in the future in order to renew it when needed
            // Console.WriteLine("subscription = " + subscription.Id.ToString());
            Console.WriteLine("done");
            Console.ReadLine();

        }

        
    }
}