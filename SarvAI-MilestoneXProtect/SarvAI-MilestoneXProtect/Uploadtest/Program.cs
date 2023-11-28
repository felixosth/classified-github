using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Uploadtest
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                var file = args[0];


                var result = UploadAndVerify(file);
            }
        }

        static bool UploadAndVerify(string file)
        {
            using (var httpClient = new HttpClient() { BaseAddress = new Uri("http://34.141.188.53:6006")})
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var formData = new MultipartFormDataContent())
            {
                //httpClient.DefaultRequestHeaders.Add("Authorization", "JWT " + token);
                //httpClient.DefaultRequestHeaders.Add("Save-Video", "0");
                //httpClient.DefaultRequestHeaders.Add("Platform", "MILESTONE");

                var videoContent = new StreamContent(fileStream);

                formData.Add(videoContent, "file", "file");

                var result = httpClient.PostAsync("/event", formData).Result;

                var resultStr = result.Content.ReadAsStringAsync().Result;

                return bool.Parse(resultStr);
            }
        }
    }
}
