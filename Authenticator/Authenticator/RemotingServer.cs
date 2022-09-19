using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Newtonsoft.Json;

namespace Authenticator
{
    internal class RemotingServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Remoting Server");
            ServiceHost host; // Actual host service system
            NetTcpBinding tcp = new NetTcpBinding(); // Represents a tcp/ip binding in the Windows network stack

            host = new ServiceHost(typeof(AuthenticatorServer)); // Bind server to the implementation of IAuthenticatorServer
            host.AddServiceEndpoint(typeof(IAuthenticatorServer), tcp, "net.tcp://localhost/AuthenticationService"); // Present the interface to the client
            host.Open(); // Open the host for business
            Console.WriteLine("System Online");
            Unpublish("http://localhost:56066/AddThreeNumbers");
            Console.ReadLine();
            host.Close(); // Closing the host once we are done
        }
        public static void Unpublish(string serviceEndpoint)
        {
            string descriptionFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\description.txt";
            // string descriptionFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string jsonContent = File.ReadAllText(descriptionFilePath);
            List<ServiceDescription> dataList = JsonConvert.DeserializeObject<List<ServiceDescription>>(jsonContent); // Deserializing jsonContent into a collection
            List<ServiceDescription> newDataList = new List<ServiceDescription>();

            foreach (ServiceDescription data in dataList)
            {
                if (!data.APIEndpoint.Equals(serviceEndpoint))
                {
                    newDataList.Add(data);
                }
            }

            jsonContent = JsonConvert.SerializeObject(newDataList.ToArray(), Formatting.Indented);
            File.WriteAllText(descriptionFilePath, jsonContent);
        }

        public class ServiceDescription
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string APIEndpoint { get; set; }
            public int NumberOfOperands { get; set; }
            public string OperandType { get; set; }
        }
    }
}
