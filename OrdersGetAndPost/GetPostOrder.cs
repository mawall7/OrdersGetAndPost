using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using GetPostOrders;

namespace OrdersGetAndPost
{
    public static class GetPostOrder
    {
        public static string Message{get; set;}
        public static Order Order { get; set; }
        
        [FunctionName("GetOrder")]
        public static async Task<IActionResult> FunctionGet(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            

            //string path = "C:/Users/matte/source/repos/OrdersGetAndPost/OrdersGetAndPost/local.settings.json";
            string OrderAsJson = default;

            if (name == "112y08aD2_x8")
            {
                string constring = "DefaultEndpointsProtocol=https;AccountName=ordermessagestorage;AccountKey=BqBrvm2YaRGDs1rUCJws/fJakkE8E5OJaOH2y4PXr4/xVjI1wgUOEfTizwgB1NJcVQFn2qiKshBi+AStaavRWg==;EndpointSuffix=core.windows.net";
                

                QueueClient queueClient = new QueueClient(constring, "queue1");
               
                if (queueClient.Exists())
                {
                    //PeekedMessage[] m = queueClient.PeekMessages();
                    //Message = m[0].Body.ToString();

                    //QueueMessage message = await queueClient.ReceiveMessageAsync();
                    PeekedMessage message = await queueClient.PeekMessageAsync();

                    if (message != null)
                    {
                        //queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                        //Message = message.;
                        Order = message.Body.ToObjectFromJson<Order>();
                        OrderAsJson = System.Text.Json.JsonSerializer.Serialize(Order, typeof(Order));

                    }
                    else return new BadRequestResult ();
                        
                }
                
            }
            return new OkObjectResult(OrderAsJson);//return new OkObjectResult(Message);
        }

        [FunctionName("PostOrder")]
        public static async Task<IActionResult> FunctionPost(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (name == "112y08aD2_x8")
            {

                string constring = "DefaultEndpointsProtocol=https;AccountName=ordermessagestorage;AccountKey=BqBrvm2YaRGDs1rUCJws/fJakkE8E5OJaOH2y4PXr4/xVjI1wgUOEfTizwgB1NJcVQFn2qiKshBi+AStaavRWg==;EndpointSuffix=core.windows.net";

                QueueClient queueClient = new QueueClient(constring, "queue1");
                //queueClient.Delete();
                queueClient.CreateIfNotExists();
                if (queueClient.Exists())
                {
                    //PeekedMessage[] m = queueClient.PeekMessages();
                    // Message = m[0].Body.ToString();
                    try
                    {
                        await queueClient.SendMessageAsync(requestBody);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(e.Message);
                    }
                }
                
                return new OkObjectResult(requestBody);
            }


            else
            {
                return new BadRequestObjectResult("Bad Request 404, Order not found");
            };
        }


    }

}
