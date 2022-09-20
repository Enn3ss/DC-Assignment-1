using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Authenticator;

namespace ServicePublishingConsoleApp
{
    internal class UserConsole
    {
        static void Main(string[] args)
        {
            Authenticator.IAuthenticator;

            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            Authenticator.IAuthenticatorServer foob = foobFactory.CreateChannel();
        }
    }
}
