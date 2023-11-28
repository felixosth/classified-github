using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLite2.Graph
{
    public class GraphException : Exception
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        public override string Message => Error.message;
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public Innererror innerError { get; set; }
    }

    public class Innererror
    {
        public DateTime date { get; set; }
        public string requestid { get; set; }
        public string clientrequestid { get; set; }
    }
}
