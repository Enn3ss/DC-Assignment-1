using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Web.Http;
using Newtonsoft.Json;
using Registry.Models;
using ServiceProvider.Models;

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
        public string Publish(List<ServiceDescription> dataList, int token) // Saves service description into description.txt
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            StatusData status;

            if (foob.Validate(token).Equals("not validated")) // If token invalid return denied status
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                return JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            try
            {
                if (File.Exists(descriptionFilePath)) // Check if file exists and if there is JSON data, combine it
                {
                    jsonContent = File.ReadAllText(descriptionFilePath);
                    List<ServiceDescription> currDataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent);
                    dataList.AddRange(currDataList); // Gets a list of JSON data and then combines it with dataList
                }

                jsonContent = JsonConvert.SerializeObject(dataList.ToArray(), Formatting.Indented);
                File.WriteAllText(descriptionFilePath, jsonContent); // Rewrite the entire file with new list
                status = new StatusData
                {
                    Status = "successful"
                };
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                status = new StatusData
                {
                    Status = "unsuccessful"
                };
            }
            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        [Route("search")]
        [HttpPost]
        public string Search(string search, int token) // Returns service info based on 'search' string
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            List<ServiceDescription> dataList;
            List<ServiceDescription> newDataList;

            if (foob.Validate(token).Equals("validated"))
            {
                jsonContent = File.ReadAllText(descriptionFilePath);
                dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection
                newDataList = new List<ServiceDescription>();

                foreach (ServiceDescription data in dataList) // Searches collection for service info that matches 'search'
                {
                    if (data.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        data.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        newDataList.Add(data);
                    }
                }

                jsonContent = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented); // Return matches
            }
            else // Else if token is invalid
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonContent = JsonConvert.SerializeObject(error, Formatting.Indented); // Return denied status
            }

            return jsonContent;
        }

        [Route("allservices")]
        [HttpPost]
        public string AllServices(int token) // Returns all services saved in description.txt in JSON format
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;

            if (foob.Validate(token).Equals("validated")) // If token is valid (found in token.txt)
            {            
                jsonContent = File.ReadAllText(descriptionFilePath); // Return all services
            }
            else // Else if token is invalid
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonContent = JsonConvert.SerializeObject(error, Formatting.Indented); // Return denied status
            }

            return jsonContent;
        }

        [Route("unpublish")]
        [HttpPost]
        public string Unpublish(string serviceEndpoint, int token) // Given a serviceEndpoint, remove the service description from description.txt
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            string status;
            List<ServiceDescription> dataList;
            List<ServiceDescription> newDataList;
            ErrorData error = new ErrorData();

            if (foob.Validate(token).Equals("validated")) // If token is valid (found in token.txt)
            {
                jsonContent = File.ReadAllText(descriptionFilePath);
                dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a list
                newDataList = new List<ServiceDescription>();

                foreach (ServiceDescription data in dataList)
                {
                    if (!data.APIEndpoint.Equals(serviceEndpoint)) // If serviceEndpoint matches APIEndpoint then do not add to new list
                    {
                        newDataList.Add(data);
                    }
                }

                jsonContent = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented); // Serialize jsonContent into new list
                File.WriteAllText(descriptionFilePath, jsonContent); // Rewrite description.txt with newDataList in JSON format

                error.Status = "Approved";
                error.Reason = "Authentication Authorised";

                status = JsonConvert.SerializeObject(error, Formatting.Indented);
            }
            else // Else if token is invalid
            {
                error.Status = "Denied";
                error.Reason = "Authentication Error";

                status = JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            return status;
        }
    }
}
