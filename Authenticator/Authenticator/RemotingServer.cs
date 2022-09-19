using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    internal class RemotingServer
    {
        static void Main(string[] args)
        {
            AuthenticatorServer server = new AuthenticatorServer();
            
            String success = server.Register("test33", "pass44");
            String success2 = server.Register("steber", "ringo");

            Console.WriteLine(success);
            Console.WriteLine(success2);
            

            //int token = server.Login("steber1", "ringo");
            //Console.WriteLine(token);

            //string line = server.Validate(90179164);
            //Console.WriteLine(line);
        }
    }
}
