using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Common.Models;

namespace HaiLianFunctionApp
{
    public static class ChangeValidator
    {
        [FunctionName("ChangeValidator")]
        public static void Run([QueueTrigger("eventhappened", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            ConfigureBindingRedirects();

            var notification = JsonConvert.DeserializeObject<Common.Models.NotificationModel>(myQueueItem);


            ClientContext ctx = Common.Helpers.ContextHelper.GetSPContext("https://folkuniversitetetsp2016.sharepoint.com" + notification.SiteUrl).Result;

            // notification.Resource is our list id
            List list = ctx.Web.Lists.GetById(new Guid(notification.Resource));

            string currentChangeTokenstr = list.GetPropertyBagValueString("customlistchangetoken", null);
            ChangeToken currentChangeToken = null;
            if (currentChangeTokenstr != null)
            {
                currentChangeToken = new ChangeToken();

                currentChangeToken.StringValue = currentChangeTokenstr;
            }

            ctx.Load(list, l => l.CurrentChangeToken);
            ctx.ExecuteQuery();

            ChangeQuery q = new ChangeQuery(false, false);
            q.Update = true;
            q.Add = true;
            q.Item = true;

            q.DeleteObject = true;
            q.ChangeTokenStart = currentChangeToken;
            q.ChangeTokenEnd = list.CurrentChangeToken;

            ChangeCollection changes = list.GetChanges(q);
            ctx.Load(changes);
            ctx.ExecuteQueryRetry();

            //create queue if not exists
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);
            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("changehappened");
            queue.CreateIfNotExists();

           foreach (var change in changes)
            {
                if (change.ChangeType == ChangeType.Add)
                {
                    ChangeItem itemChange = change as ChangeItem;
                    ChangeInfo changeInfo = new ChangeInfo() {SiteUrl= notification.SiteUrl, ListId= notification.Resource, ItemId= itemChange.ItemId };                  
                    queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(changeInfo)));                   
                    log.Info("Item was added id = " + itemChange.ItemId);

                }
                if (change.ChangeType == ChangeType.DeleteObject)
                {
                    ChangeItem itemChange = change as ChangeItem;
                    ChangeInfo changeInfo = new ChangeInfo() { SiteUrl = notification.SiteUrl, ListId = notification.Resource, ItemId = itemChange.ItemId };
                    queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(changeInfo)));
                    log.Info("Item was deleted id = " + itemChange.ItemId);
                }
                if (change.ChangeType == ChangeType.Update)
                {
                    ChangeItem itemChange = change as ChangeItem;
                    ChangeInfo changeInfo = new ChangeInfo() { SiteUrl = notification.SiteUrl, ListId = notification.Resource, ItemId = itemChange.ItemId };
                    queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(changeInfo)));
                    log.Info("Item was updated id = " + itemChange.ItemId);
                }
            }
            list.SetPropertyBagValue("customlistchangetoken", q.ChangeTokenEnd.StringValue);
        }

        public static void ConfigureBindingRedirects()
        {
            var config = Environment.GetEnvironmentVariable("BindingRedirects");
            var redirects = JsonConvert.DeserializeObject<List<BindingRedirect>>(config);
            redirects.ForEach(RedirectAssembly);
        }

        public class BindingRedirect
        {
            public string ShortName { get; set; }
            public string PublicKeyToken { get; set; }
            public string RedirectToVersion { get; set; }
        }

        public static void RedirectAssembly(BindingRedirect bindingRedirect)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

                if (requestedAssembly.Name != bindingRedirect.ShortName)
                {
                    return null;
                }

                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + bindingRedirect.PublicKeyToken)
                    .GetPublicKeyToken();
                requestedAssembly.Version = new Version(bindingRedirect.RedirectToVersion);
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

    }
}
    

