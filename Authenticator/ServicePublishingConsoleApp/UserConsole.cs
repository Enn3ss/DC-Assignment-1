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
                try {
                    Console.WriteLine(menuStr);
                    menuOption = int.Parse(Console.ReadLine()); // Will throw exception if input is not a number

                    switch (menuOption) {
                        case 1:
                            Register();
                            break;
                        case 2:
                            token = Login();
                            break;
                        case 3:
                            Publish(token);
                            break;
                        case 4:
                            Unpublish(token);
                            break;
                        case 5:
                            Console.WriteLine("Exiting the Service Publishing Console Application...");
                            break;
                        default:
                            Console.WriteLine("\nERROR: Please enter a vaild number.\n");
                            break;
                    }
                }
                catch (FormatException ex) {
                    Console.WriteLine("\nERROR: " + ex.Message + "\n");
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

            try {
                status = foob.Register(name, password);
                Console.WriteLine("Register Status: " + status);
            }
            catch (FaultException ex) {
                Console.WriteLine("\nERROR: " + ex.Message + "\n");
            }
        }

        public static int Login()
        {
            string name, password;
            int token = -1;

            Console.WriteLine("You have selected ---> 2. Login");
            Console.Write("Enter a name: ");
            name = Console.ReadLine();

            Console.Write("Enter a password: ");
            password = Console.ReadLine();

            try {
                token = foob.Login(name, password);

                if (token == -1) {
                    Console.WriteLine("Token: " + token.ToString() + " (token = -1 is an invalid token)");
                    Console.WriteLine("\nERROR: There is no account under that name.\n");
                }
                else {
                    Console.WriteLine("Token: " + token.ToString());
                }
            }
            catch (FaultException ex) {
                Console.WriteLine("\nERROR: " + ex.Message + "\n");
            }
            return token;
        }

        public static void Publish(int token)
        {
            Registry.Models.PublishData publishData;
            string name, description, apiEndpoint, operandType;
            int numOfOperands;

            try {
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
            catch (Exception ex) when (ex is FormatException || ex is FaultException) {
                Console.WriteLine("\nERROR: " + ex.Message + "\n");
            }
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

            try {
                RestRequest request = new RestRequest("api/service/unpublish/", Method.Post);
                request.AddJsonBody(unpublishData);
                RestResponse response = client.Post(request);
                Registry.Models.StatusData status = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);
                Console.WriteLine("Status: " + status.Status + "\nReason: " + status.Reason);
            }
            catch (FaultException ex) {
                Console.WriteLine("\nERROR: " + ex.Message + "\n");
            }
        }
    }  
}
