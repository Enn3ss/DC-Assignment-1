using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Web.Http;
using Newtonsoft.Json;
using Registry.Models;

namespace Registry.Controllers
{
    [RoutePrefix("api/service")]
    public class ServiceController : ApiController
    {
        private readonly Authenticator.IAuthenticatorServer foob;
        
        public ServiceController()
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
        }

        [Route("publish")]
        [HttpPost]
        public StatusData Publish(PublishData publishData) // Saves service description into description.txt
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            StatusData statusData = new StatusData();
            List<ServiceDescription> currDataList = new List<ServiceDescription>();
            
            if (foob.Validate(publishData.Token).Equals("not validated")) // If token invalid return denied status
            {
                statusData.Status = "Denied";
                statusData.Reason = "Authentication Error";
                
                return statusData;
            }
            
            try
            {
                if (File.Exists(descriptionFilePath)) // Check if file exists and if there is JSON data, combine it
                {
                    jsonContent = File.ReadAllText(descriptionFilePath);
                    currDataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent);
                }

                ServiceDescription serviceDescription = new ServiceDescription
                {
                    Name = publishData.Name,
                    Description = publishData.Description,
                    APIEndpoint = publishData.APIEndpoint,
                    NumberOfOperands = publishData.NumberOfOperands,
                    OperandType = publishData.OperandType
                };
                currDataList.Add(serviceDescription);
                jsonContent = JsonConvert.SerializeObject(currDataList.ToArray(), Formatting.Indented);
                File.WriteAllText(descriptionFilePath, jsonContent); // Rewrite the entire file with new list

                statusData.Status = "Successful";
                statusData.Reason = "File Created/Found";
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                statusData.Status = "Unsuccessful";
                statusData.Reason = "IO Exception";
            }

            return statusData;
        }

        [Route("search")]
        [HttpPost]
        public StatusData Search(SearchData searchData) // Returns service info based on 'search' string
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            List<ServiceDescription> dataList, newDataList;
            StatusData statusData = new StatusData();

            if (foob.Validate(searchData.Token).Equals("validated"))
            {
                jsonContent = File.ReadAllText(descriptionFilePath);
                dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection
                newDataList = new List<ServiceDescription>();

                foreach (ServiceDescription data in dataList) // Searches collection for service info that matches 'search'
                {
                    if (data.Name.IndexOf(searchData.Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        data.Description.IndexOf(searchData.Search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        newDataList.Add(data);
                    }
                }

                statusData.Status = "Successful";
                statusData.Data = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented); // Return matches
            }
            else // Else if token is invalid
            {
                statusData.Status = "Denied";
                statusData.Reason = "Authentication Error";
            }

            return statusData;
        }

        [Route("allservices")]
        [HttpPost]
        public StatusData AllServices(int token) // Returns all services saved in description.txt in JSON format
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            StatusData statusData = new StatusData();

            if (foob.Validate(token).Equals("validated")) // If token is valid (found in token.txt)
            {
                statusData.Status = "Successful";
                statusData.Reason = "File Found";
                statusData.Data = File.ReadAllText(descriptionFilePath); // Return all services in statusData *might need try/catch*
            }
            else // Else if token is invalid
            {
                statusData.Status = "Denied";
                statusData.Reason = "Authentication Error";
            }

            return statusData;
        }

        [Route("unpublish")]
        [HttpPost]
        public StatusData Unpublish(UnpublishData unpublishData) // Given a serviceEndpoint, remove the service description from description.txt
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            List<ServiceDescription> dataList, newDataList;
            StatusData statusData = new StatusData();

            if (foob.Validate(unpublishData.Token).Equals("validated")) // If token is valid (found in token.txt)
            {
                jsonContent = File.ReadAllText(descriptionFilePath);
                dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a list
                newDataList = new List<ServiceDescription>();

                foreach (ServiceDescription data in dataList)
                {
                    if (!data.APIEndpoint.Equals(unpublishData.ServiceEndpoint)) // If serviceEndpoint matches APIEndpoint then do not add to new list
                    {
                        newDataList.Add(data);
                    }
                }

                jsonContent = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented); // Serialize jsonContent into new list
                File.WriteAllText(descriptionFilePath, jsonContent); // Rewrite description.txt with newDataList in JSON format

                statusData.Status = "Successful";
                statusData.Reason = "Service Endpoint found and removed from description.txt";
            }
            else // Else if token is invalid
            {
                statusData.Status = "Denied";
                statusData.Reason = "Authentication Error";
            }

            return statusData;
        }
    }
}
