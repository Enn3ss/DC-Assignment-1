﻿using System;
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
            Console.ReadLine();
            host.Close(); // Closing the host once we are done
        }
    }
}
