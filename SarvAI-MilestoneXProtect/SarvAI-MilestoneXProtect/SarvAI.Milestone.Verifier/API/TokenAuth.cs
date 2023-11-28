using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier.API
{
    public class TokenAuth
    {
        public const string REQ_URL = "/api/token-auth/";

        public string token { get; set; }

        public string[] non_field_errors { get; set; }

        public static async Task<TokenAuth> GetToken(Uri server, string username, string password)
        {
            using (var httpClient = new HttpClient())
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                var result = await httpClient.PostAsync(new Uri(server, REQ_URL), formContent);

                var resultStr = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TokenAuth>(resultStr);
            }
        }
    }
}
