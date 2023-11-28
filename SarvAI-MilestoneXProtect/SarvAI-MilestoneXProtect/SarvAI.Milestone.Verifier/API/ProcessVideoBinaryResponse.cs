using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier.API
{
    public class ProcessVideoBinaryResponse
    {
        public const string REQ_URL = "/api/processvideobinary/";

        public string timestamp { get; set; }
        public string status { get; set; }
        public Result results { get; set; }
        public float compute_time { get; set; }
        public string detail { get; set; }

        public string RawResponse { get; set; }


        public static async Task<ProcessVideoBinaryResponse> Upload(Uri server, string token, string file)
        {
            using(var httpClient = new HttpClient())
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //httpClient.DefaultRequestHeaders.Add("Authorization", "JWT " + token);
                //httpClient.DefaultRequestHeaders.Add("Save-Video", "0");
                //httpClient.DefaultRequestHeaders.Add("Platform", "MILESTONE");

                var content = new StreamContent(fileStream);

                var result = await httpClient.PostAsync(server, content);

                var resultStr = await result.Content.ReadAsStringAsync();
                ProcessVideoBinaryResponse processResponse = null;

                try
                {
                    processResponse = JsonConvert.DeserializeObject<ProcessVideoBinaryResponse>(resultStr);
                }
                catch
                {
                    processResponse = new ProcessVideoBinaryResponse();
                    processResponse.status = "Error";
                }
                processResponse.RawResponse = resultStr;
                return processResponse;
            }
        }
    }

    public class Result
    {
        public bool should_react { get; set; }
    }

}
