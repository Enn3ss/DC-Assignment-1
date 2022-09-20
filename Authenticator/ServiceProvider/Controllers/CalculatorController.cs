using System.ServiceModel;
using System.Web.Http;
using Newtonsoft.Json;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
    [RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
        private readonly Authenticator.IAuthenticatorServer foob;

        public CalculatorController()
        {
            ChannelFactory<Authenticator.IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<Authenticator.IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
        }

        [Route("addtwonumbers")]
        [HttpPost]
        public string AddTwoNumbers(int[] numbers, int token)
        {
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
                StatusData error = new StatusData
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
                StatusData error = new StatusData
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
                StatusData error = new StatusData
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
                StatusData error = new StatusData
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
