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
                status.Reason = "Authentication Validated";
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
        public StatusData AddThreeNumbers(CalculatorData calcData)
        {
            StatusData status = new StatusData();

            if (foob.Validate(calcData.Token).Equals("validated")) {
                int result = calcData.Operands[0] + calcData.Operands[1] + calcData.Operands[2];
                status.Status = "Successful";
                status.Reason = "Authentication Validated";
                status.Data = result.ToString();
            }
            else {
                status.Status = "Denied";
                status.Reason = "Authentication Error";
            }

            return status;
        }

        [Route("multwonumbers")]
        [HttpPost]
        public StatusData MulTwoNumbers(CalculatorData calcData)
        {
            StatusData status = new StatusData();

            if (foob.Validate(calcData.Token).Equals("validated")) {
                int result = calcData.Operands[0] * calcData.Operands[1];
                status.Status = "Successful";
                status.Reason = "Authentication Validated";
                status.Data = result.ToString();
            }
            else {
                status.Status = "Denied";
                status.Reason = "Authentication Error";
            }

            return status;
        }

        [Route("multhreenumbers")]
        [HttpPost]
        public StatusData MulThreeNumbers(CalculatorData calcData)
        {
            StatusData status = new StatusData();

            if (foob.Validate(calcData.Token).Equals("validated")) {
                int result = calcData.Operands[0] * calcData.Operands[1] * calcData.Operands[2];
                status.Status = "Successful";
                status.Reason = "Authentication Validated";
                status.Data = result.ToString();
            }
            else {
                status.Status = "Denied";
                status.Reason = "Authentication Error";
            }

            return status;
        }
    }
}
