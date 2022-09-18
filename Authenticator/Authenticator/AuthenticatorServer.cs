using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthenticatorServer : IAuthenticatorServer
    {
        public string Register(string name, string password)
        {
            string file = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\test.txt";
            string success = "successfully registered";

            try
            {
                File.WriteAllText(file, name + "," + password);
            }
            catch(IOException ex)
            {
                Console.WriteLine("Error info:" + ex.Message);
                success = "unsuccessfully registered";
            }

            return success;
        }

        public int Login(string name, string password)
        {
            return 0;
        }


        public string Validate(int token)
        {
            return "";
        }
    }
}
