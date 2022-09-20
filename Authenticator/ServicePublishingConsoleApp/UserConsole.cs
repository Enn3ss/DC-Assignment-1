using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace ServicePublishingConsoleApp
{
    internal class UserConsole
    {
        public static Authenticator.IAuthenticatorServer foob;
        public static RestClient client;

        static void Main(string[] args)
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
            client = new RestClient("http://localhost:63273/");
            Menu();
        }

        public static void Menu()
        {
            int menuOption = 0;
            int token = -1; // -1 is an invalid token
            string menuStr = "Please select an operation:" +
                             "\n> 1. Register" +
                             "\n> 2. Login" +
                             "\n> 3. Publish Service" +
                             "\n> 4. Unpublish Service" +
                             "\n> 5. Exit";

            do
            {
                Console.WriteLine(menuStr);
                menuOption = int.Parse(Console.ReadLine());

                switch (menuOption)
                {
                    case 1:
                        Register();
                        break;
                    case 2:
                        token = Login();
                        break;
                    case 3:
                        Publish(token);
                        break;
                }
            }
            while (menuOption != 5);
        }

        public static void Register()
        {
            string name, password, status;

            Console.WriteLine("You have selected ---> 1. Register");
            Console.Write("Enter a name: ");
            name = Console.ReadLine();

            Console.Write("Enter a password: ");
            password = Console.ReadLine();

            status = foob.Register(name, password);
            Console.WriteLine("Register Status: " + status);
        }

        public static int Login()
        {
            string name, password;
            int token;

            Console.WriteLine("You have selected ---> 2. Login");
            Console.Write("Enter a name: ");
            name = Console.ReadLine();

            Console.Write("Enter a password: ");
            password = Console.ReadLine();

            token = foob.Login(name, password);
            Console.WriteLine("Token: " + token.ToString() + " (token = -1 is an invalid token)");

            return token;
        }

        public static void Publish(int token)
        {
            Registry.Models.PublishData publishData;
            string name, description, apiEndpoint, operandType;
            int numOfOperands;

            Console.WriteLine("You have selected ---> 3. Publish");
            Console.Write("Enter a name: ");
            name = Console.ReadLine();

            Console.Write("Enter a description: ");
            description = Console.ReadLine();

            Console.Write("Enter an API Endpoint: ");
            apiEndpoint = Console.ReadLine();

            Console.Write("Enter number of operands: ");
            numOfOperands = int.Parse(Console.ReadLine());

            Console.Write("Enter an operand type: ");
            operandType = Console.ReadLine();

            publishData = new Registry.Models.PublishData // Creating the object for the POST request
            {
                Name = name,
                Description = description,
                APIEndpoint = apiEndpoint,
                NumberOfOperands = numOfOperands,
                OperandType = operandType,
                Token = token
            };

            RestRequest request = new RestRequest("api/service/publish/", Method.Post);
            request.AddJsonBody(publishData);
            RestResponse response = client.Post(request);
            Registry.Models.StatusData status = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);
            Console.WriteLine("Status: " + status.Status + "\nReason: " + status.Reason);
        }

        public static void Unpublish(int token)
        {
            Registry.Models.UnpublishData unpublishData;
            string serviceEndpoint;

            Console.WriteLine("You have selected ---> 4. Unpublish");
            Console.Write("Enter a service endpoint: ");
            serviceEndpoint = Console.ReadLine();

            unpublishData = new Registry.Models.UnpublishData
            {
                ServiceEndpoint = serviceEndpoint,
                Token = token
            };

            RestRequest request = new RestRequest("api/service/unpublish/", Method.Post);
            request.AddJsonBody(unpublishData);
            RestResponse response = client.Post(request);
            Registry.Models.StatusData status = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);
            Console.WriteLine("Status: " + status.Status + "\nReason: " + status.Reason);
        }
    }  
}
