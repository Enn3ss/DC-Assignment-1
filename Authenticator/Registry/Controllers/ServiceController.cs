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
                File.WriteAllText(descriptionFilePath, jsonContent);
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
        public string Search(string search)
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent = File.ReadAllText(descriptionFilePath);
            List<ServiceDescription> test = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent);


            string allDescriptions = File.ReadAllText(descriptionFilePath);
            string[] descriptions = allDescriptions.Split(',');
            string serviceInfo;

            foreach (string desc in descriptions)
            {
                if (desc.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //serviceInfo = jsonConcat(serviceInfo, desc);
                }
            }

            return "";
        }
    }
}
