using System.ServiceModel;
using System.Web.Http;
using Newtonsoft.Json;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
    [RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
        [Route("addtwonumbers")]
        [HttpPost]
        public string AddTwoNumbers(int[] numbers, int token)
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            Authenticator.IAuthenticatorServer foob = foobFactory.CreateChannel();
            string jsonString;

            if (foob.Validate(token).Equals("validated"))
            {
                NumberData sum = new NumberData
                {
                    Number = numbers[0] + numbers[1]
                };

                jsonString = JsonConvert.SerializeObject(sum, Formatting.Indented);
            }
            else
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonString = JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            return jsonString;
        }

        [Route("addthreenumbers")]
        [HttpPost]
        public string AddThreeNumbers(int[] numbers, int token)
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            Authenticator.IAuthenticatorServer foob = foobFactory.CreateChannel();
            string jsonString;

            if (foob.Validate(token).Equals("validated"))
            {
                NumberData sum = new NumberData
                {
                    Number = numbers[0] + numbers[1] + numbers[2]
                };

                jsonString = JsonConvert.SerializeObject(sum, Formatting.Indented);
            }
            else
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonString = JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            return jsonString;
        }

        [Route("multwonumbers")]
        [HttpPost]
        public string MulTwoNumbers(int[] numbers, int token)
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            Authenticator.IAuthenticatorServer foob = foobFactory.CreateChannel();
            string jsonString;

            if (foob.Validate(token).Equals("validated"))
            {
                NumberData sum = new NumberData
                {
                    Number = numbers[0] * numbers[1]
                };

                jsonString = JsonConvert.SerializeObject(sum, Formatting.Indented);
            }
            else
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonString = JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            return jsonString;
        }

        [Route("multhreenumbers")]
        [HttpPost]
        public string MulThreeNumbers(int[] numbers, int token)
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            Authenticator.IAuthenticatorServer foob = foobFactory.CreateChannel();
            string jsonString;

            if (foob.Validate(token).Equals("validated"))
            {
                NumberData sum = new NumberData
                {
                    Number = numbers[0] * numbers[1] * numbers[2]
                };

                jsonString = JsonConvert.SerializeObject(sum, Formatting.Indented);
            }
            else
            {
                ErrorData error = new ErrorData
                {
                    Status = "Denied",
                    Reason = "Authentication Error"
                };

                jsonString = JsonConvert.SerializeObject(error, Formatting.Indented);
            }

            return jsonString;
        }
    }
}
