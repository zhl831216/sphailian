using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Common.Models;
using Microsoft.SharePoint.Client;
using CreateGroupSiteMicrosoftGraph.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using CreateGroupSiteMicrosoftGraph.Models;

namespace HaiLianFunctionApp
{
    public static class GroupSiteCreator
    {
        [FunctionName("GroupSiteCreator")]
        public static void Run([QueueTrigger("changeshappened", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");


            var changeInfo = JsonConvert.DeserializeObject<Common.Models.ChangeInfo>(myQueueItem);


            ClientContext ctx = Common.Helpers.ContextHelper.GetSPContext("https://folkuniversitetetsp2016.sharepoint.com" + changeInfo.SiteUrl).Result;

            // changeInfo.ListId is  list id
            List list = ctx.Web.Lists.GetById(new Guid(changeInfo.ListId));
            ListItem changedItem = list.GetItemById(changeInfo.ItemId);
            ctx.Load(changedItem,item=> item["Title"], item => item["SiteUrl"], item => item["Owner"]);
            ctx.ExecuteQuery();

            string title = changedItem["Title"].ToString();
            string owner = changedItem["Owner"].ToString();

            GroupHelper.CreateUnifedGroup(title, "groupsite1","groupsite1", owner);

            //create queue if not exists
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);
            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("provisionqueue");
            queue.CreateIfNotExists();

            GroupSiteUrl siteUrl = new GroupSiteUrl() { url = changeInfo.SiteUrl };            
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(siteUrl)));
            log.Info("Item was updated id = " + changeInfo.SiteUrl);

        }
        
    }
    
}
