using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using File = System.IO.File;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthenticatorServer : IAuthenticatorServer
    {
        public string Register(string name, string password) // Saves name + password in login.txt
        {
            string loginFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\login.txt";
            // string loginFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*

            try // Try open login.txt to check for duplicate logins
            {
                string[] lines = File.ReadLines(loginFilePath).ToArray();

                foreach (string line in lines)
                {
                    if (line.Equals(name + "," + password)) // If login.txt contains name + password already
                    {
                        Console.WriteLine("Name and password already exists");
                        return "unsuccessfully registered"; // Return if login exists
                    }
                }
            }
            catch (FileNotFoundException ex) // Must catch if login.txt has yet to be created
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            File.AppendAllText(loginFilePath, name + "," + password + Environment.NewLine); // Add login if not duplicate

            return "successfully registered";
        }

        public int Login(string name, string password) // Checks if name + password exist in login.txt
        {
            string loginFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\login.txt";
            // string loginFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string tokenFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string[] lines = File.ReadLines(loginFilePath).ToArray();

            foreach (string line in lines)
            {
                if(line.Equals(name + "," + password))
                {
                    Random rnd = new Random();
                    int num = rnd.Next();
                    File.AppendAllText(tokenFilePath, num.ToString() + Environment.NewLine);
                    return num; // Return token if name and password match is found in login.txt
                }
            }
            return -1; // Return -1 if name and password cannot be found
        }


        public string Validate(int token) // Checks if token is valid if it's in token.txt
        {
            string tokenFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            string[] lines = File.ReadLines(tokenFilePath).ToArray();

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
            string tokenFilePath = @"C:\Users\Nathan Sutandi\Documents\GitHub\DC-Assignment-1\token.txt";
            // string tokenFilePath = *PUT YOUR FILE PATH HERE AND COMMENT MINE OUT*
            File.WriteAllText(tokenFilePath, String.Empty);
        }
    }
}
