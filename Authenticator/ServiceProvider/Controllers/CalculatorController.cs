using System.Web.Http;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
    [RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
        [Route("addtwonumbers")]
        [HttpGet]
        public NumberData AddTwoNumbers(int[] numbers)
        {
            NumberData sum = new NumberData
            {
                number = numbers[0] + numbers[1]
            };

            return sum;
        }

        [Route("addthreenumbers")]
        [HttpGet]
        public NumberData AddThreeNumbers(int[] numbers)
        {
            NumberData sum = new NumberData
            {
                number = numbers[0] + numbers[1] + numbers[2]
            };

            return sum;
        }

        [Route("multwonumbers")]
        [HttpGet]
        public NumberData MulTwoNumbers(int[] numbers)
        {
            NumberData product = new NumberData
            {
                number = numbers[0] * numbers[1]
            };

            return product;
        }

        [Route("multhreenumbers")]
        [HttpGet]
        public NumberData MulThreeNumbers(int[] numbers)
        {
            NumberData product = new NumberData
            {
                number = numbers[0] * numbers[1] * numbers[2]
            };

            return product;
        }
    }
}
