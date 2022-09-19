using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using Registry.Models;

namespace Registry.Controllers
{
    [RoutePrefix("api/service")]
    public class ServiceController : ApiController
    {
        [Route("publish")]
        [HttpPost]
        public string Publish(List<ServiceDescription> dataList) // Saves service description into description.txt
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent;
            StatusData status;

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
        public string Search(string search) // Returns service info based on 'search' string
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent = File.ReadAllText(descriptionFilePath);
            List<ServiceDescription> dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection
            List<ServiceDescription> newDataList = new List<ServiceDescription>();

            foreach (ServiceDescription data in dataList) // Searches collection for service info that matches 'search'
            {
                if(data.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   data.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    newDataList.Add(data);
                }
            }

            return JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented);
        }

        [Route("allservices")]
        [HttpPost]
        public string AllServices() // Returns all services saved in description.txt in JSON format
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            //string jsonContent = File.ReadAllText(descriptionFilePath);
            //List<ServiceDescription> dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection

            //return JsonConvert.SerializeObject(dataList.ToArray(), Formatting.Indented);
            return File.ReadAllText(descriptionFilePath);
        }

        [Route("unpublish")]
        [HttpPost]
        public void Unpublish(string serviceEndpoint)
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent = File.ReadAllText(descriptionFilePath);
            List<ServiceDescription> dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection
            List<ServiceDescription> newDataList = new List<ServiceDescription>();

            foreach (ServiceDescription data in dataList)
            {
                if (!data.APIEndpoint.Equals(serviceEndpoint)) // If serviceEndpoint matches APIEndpoint then do not add to new list
                {
                    newDataList.Add(data);
                }
            }

            jsonContent = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented);
            File.WriteAllText(descriptionFilePath, jsonContent); // Rewrite description.txt with newDataList in JSON format
        }
    }
}
