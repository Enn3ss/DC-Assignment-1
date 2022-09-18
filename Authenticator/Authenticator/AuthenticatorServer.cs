using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthenticatorServer : IAuthenticatorServer
    {
        public string Register(string name, string password)
        {
            string loginFile = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\login.txt";
            // string loginFile = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string success = "successfully registered";

            try
            {
                File.AppendAllText(loginFile, name + "," + password + Environment.NewLine); // Every login is added on a new line
            }
            catch(IOException ex) // Error in opening file
            {
                Console.WriteLine("Error: " + ex.Message);
                success = "unsuccessfully registered";
            }

            return success;
        }

        public int Login(string name, string password)
        {
            string loginFile = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\login.txt";
            // string loginFile = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string tokenFile = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFile = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string[] lines = File.ReadLines(loginFile).ToArray();

            foreach (string line in lines)
            {
                if(line.Equals(name + "," + password))
                {
                    Random rnd = new Random();
                    int num = rnd.Next();
                    File.AppendAllText(tokenFile, num.ToString() + Environment.NewLine);
                    return num; // Return token if name and password match is found in login.txt
                }
            }
            return -1; // Return -1 if name and password cannot be found
        }


        public string Validate(int token)
        {
            string tokenFile = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFile = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string[] lines = File.ReadLines(tokenFile).ToArray();

            foreach (string line in lines)
            {
                if(line.Equals(token.ToString()))
                {
                    return "validated";
                }
            }

            return "not validated";
        }

        public void ClearSavedTokens() // This function needs to clear saved tokens every 'x' minutes
        {
            string tokenFile = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFile = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            File.WriteAllText(tokenFile, String.Empty);
        }
    }
}
