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
        public StatusData AddTwoNumbers(CalculatorData calcData)
        {
            StatusData status = new StatusData();

            if (foob.Validate(calcData.Token).Equals("validated"))
            {
                int result = calcData.Operands[0] + calcData.Operands[1];
                status.Status = "Successful";
                status.Reason = "PUT SOMETHING HERE";
                status.Data = result.ToString();
            }
            else
            {
                status.Status = "Denied";
                status.Reason = "Authentication Error";
            }

            return status;
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

                //
                int hello = numbers[0] + numbers[1] + numbers[2];
                StatusData test = new StatusData
                {
                    Status = "Successful",
                    Reason = "xxxxxxxxxxx",
                    Data = hello.ToString()
                };

                // comment below out
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

            // return test (StatusData obj(
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
