using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static DigitalRevision.Global.Helper;

namespace DigitalRevision.DataSource
{
    class TryggDRIFTAlarmDataSource : DataSourceBase
    {
        public override string Name => "TryggDrift larm";
        public override double Version => 1.0;

        private readonly HttpClient httpClient = new HttpClient();

        private const string _apiKeyFile = ".config";
        private string apiKey;

        public TryggDRIFTAlarmDataSource()
        {
            IsEnabled = false;
            AllowUserToEnable = false;

            if (File.Exists(_apiKeyFile))
            {
                apiKey = File.ReadAllLines(_apiKeyFile).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ShowError("TryggDRIFT API-key is empty");
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);

                    IsEnabled = true;
                    AllowUserToEnable = true;
                }
            }
            else
                ShowError("No TryggDRIFT API-key found (.config not found)");
        }

        public HttpRequestMessage CreateRequest()
        {
#if DEBUG
            var url = "http://tryggdrift.local:8011/backend/api.php";
#else
            var url = "https://drift.tryggconnect.se/backend/api.php";
#endif

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                string.Format(url + $"?action=getAlarmHistoryWithKey&mguid={GetMGUID()}&months=12&sortdate=desc")
                );
            return request;
        }

        public override async Task CollectData(string folderDestination)
        {
            ProgressPercentage = 0;

            var request = CreateRequest();
            HttpResponseMessage response;
            try
            {
                ProgressIsIndeterminate = true;
                response = await httpClient.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                ShowError("Could not connect to TryggDrift");
                return;
            }

            byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
            if (responseBytes.Length == 0)
            {
                ShowError("Server returned empty string");
                return;
            }

            string responseString = Encoding.UTF8.GetString(responseBytes);
            ProgressIsIndeterminate = false;
            if (response.IsSuccessStatusCode)
            {
                var path = Path.Combine(folderDestination, "alarms.csv");
                try
                {
                    WriteCsvFileFromJson(responseString, path, (progress) => ProgressPercentage = progress);
                }
                catch (JsonException)
                {
                    ShowError("Server returned unexpected message:\n" + responseString);
                }
            }
            else
                ShowError($"Server returned incorrect status code: {response.StatusCode}\nMessage: {responseString}");
        }
    }
}
