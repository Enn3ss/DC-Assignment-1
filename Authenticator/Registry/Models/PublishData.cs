using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registry.Models
{
    public class PublishData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string APIEndpoint { get; set; }
        public int NumberOfOperands { get; set; }
        public string OperandType { get; set; }
        public int Token { get; set; }
    }
}