using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WinServLite2.Graph
{
    public static class GraphAPI
    {
        // secret: wt5U1W~~_4Y_bjoCDKXbV9090s9-eN69.s
        private static string ClientId = "f9bdc862-39e7-4239-a515-6e968b3e2a2a";
        private static string Tenant = "fd593077-012a-4009-ba7a-467c4c5d0e59";
        private static string Instance = "https://login.microsoftonline.com/";

        static string[] scopes = new string[] { "user.read", "Group.ReadWrite.All" };
        //static string[] scopes = new string[] { "user.read", "user.read.all", "Group.ReadWrite.All", "Directory.ReadWrite.All" };
        static string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";
        static string plannerTaskAPIEndpoint = "https://graph.microsoft.com/v1.0/planner/tasks";

        static string winservPlanId = "PhX-qs5wiU2VuO1qL5h03pcAA2rh";
        //static string winservPlanId = "-Pcqd8hwMUWuZcRATsO5EJcAA-4i";
        static string todoBucketId = "2ET-AnwBNUuOo5fnO5Zml5cANLba";
        //static string todoBucketId = "sUDwMkMHXkCF-rfXorKCG5cAERXB";

        private static IPublicClientApplication _clientApp;

        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }

        public static void Init()
        {
            _clientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority($"{Instance}{Tenant}")
                .WithDefaultRedirectUri()
                .Build();
            TokenCacheHelper.EnableSerialization(_clientApp.UserTokenCache);
        }

        public static async Task<AuthenticationResult> CheckLogin(Window window)
        {
            AuthenticationResult authResult = null;
            var app = PublicClientApp;

            var accounts = await app.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        .WithParentActivityOrWindow(new WindowInteropHelper(window).Handle) // optional, used to center the browser on the window
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    MessageBox.Show($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }

            return authResult;
        }


        public static async Task<string> UserInfo(AuthenticationResult authResult)
        {
            var resultText = string.Empty;

            if (authResult != null)
            {
                resultText = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
            }
            return resultText;
        }

        public static async Task<PlanTask> CreatePlan(AuthenticationResult authResult, string title, string description)
        {
            var checklist = new Dictionary<string, PlannerChecklistItem>()
            {
                { Guid.NewGuid().ToString(), new PlannerChecklistItem{ title = "Utför installation", orderHint = " ! ! !", isChecked = false  } },
                { Guid.NewGuid().ToString(), new PlannerChecklistItem{ title = "Dokumentera", orderHint = " ! !", isChecked = false } },
                { Guid.NewGuid().ToString(), new PlannerChecklistItem{ title = "För över bilder till NAS", orderHint = " !", isChecked = false } },
            };

            var resultString = await PostHttpContentWithToken(
                new
                {
                    planId = winservPlanId,
                    title,
                    assignments = new { },
                    bucketId = todoBucketId,
                    startDateTime = DateTime.UtcNow.ToString("o"),
                    details = new
                    {
                        previewType = "description",
                        description,
                        checklist
                    }
                }, plannerTaskAPIEndpoint, authResult.AccessToken);

            var task = JsonConvert.DeserializeObject<PlanTask>(resultString);

            if(task.id == null)
            {
                var exception = JsonConvert.DeserializeObject<GraphException>(resultString);
                if (exception.Message != null)
                    throw exception;
                else
                    throw new Exception("An unknown error occured");
            }

            return task;
        }

        public static async Task<PlanTask> GetTask(AuthenticationResult authResult, string taskId)
        {
            return JsonConvert.DeserializeObject<PlanTask>(await GetHttpContentWithToken($"{plannerTaskAPIEndpoint}/{taskId}", authResult.AccessToken));
        }

        public static async Task<PlanTaskDetails> GetTaskDetails(AuthenticationResult authResult, string taskId)
        {
            return JsonConvert.DeserializeObject<PlanTaskDetails>(await GetHttpContentWithToken($"{plannerTaskAPIEndpoint}/{taskId}/details", authResult.AccessToken));
        }

        public static async Task<string> PatchAsync(object postData, string reqUrl, AuthenticationResult authenticationResult, string etag = null)
        {
            return await PatchHttpContentWithToken(postData, $"{plannerTaskAPIEndpoint}/{reqUrl}", authenticationResult.AccessToken, etag);
        }

        public static async Task<string> PatchHttpContentWithToken(object postData, string url, string token, string etag)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(new HttpMethod("PATCH"), url);
                //Add the token in Authorization header
                request.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                if(etag != null)
                    request.Headers.Add("If-Match", etag);

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static async Task<string> PostHttpContentWithToken(object postData, string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
                //Add the token in Authorization header
                request.Content =  new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static async void Logout()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    //this.ResultText.Text = "User has signed-out";
                    //this.CallGraphButton.Visibility = Visibility.Visible;
                    //this.SignOutButton.Visibility = Visibility.Collapsed;
                }
                catch (MsalException ex)
                {
                    MessageBox.Show($"Error signing-out user: {ex.Message}");
                    //ResultText.Text = ;
                }
            }
        }

    }
}
