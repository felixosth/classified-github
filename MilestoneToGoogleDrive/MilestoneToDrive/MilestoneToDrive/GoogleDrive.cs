using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace MilestoneToDrive
{
    internal class GoogleDrive
    {
        // https://developers.google.com/drive/api/v3/quickstart/dotnet
        // https://developers.google.com/drive/api/v3/manage-uploads
        // https://developers.google.com/workspace/guides/create-credentials

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveFile };
        static string ApplicationName = "MilestoneToDrive";

        private readonly DriveService service;

        public GoogleDrive(string fileName = "credentials.json")
        {
            UserCredential credential;

            using (var stream =
                new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public string CreateMyDirectory()
        {
            var folderId = Config.Instance.DriveFolderId;

            if (!string.IsNullOrWhiteSpace(folderId))
            {
                var getReq = service.Files.Get(folderId);
                getReq.Fields = "id";
                try
                {
                    var existingFolder = getReq.Execute();

                    if (existingFolder.Id != null)
                        return existingFolder.Id;
                }
                catch
                {
                }
            }


            var fileMetadata = new File()
            {
                Name = ApplicationName,
                MimeType = Helper.MimeType.Folder
            };

            var createReq = service.Files.Create(fileMetadata);
            createReq.Fields = "id";
            var newFolder = createReq.Execute();

            Config.Instance.DriveFolderId = newFolder.Id;
            Config.Instance.Save();

            return newFolder.Id;
        }

        public void UploadFile(string name, string filePath, Action<Google.Apis.Upload.IUploadProgress> progressChangedAction = null)
        {
            string folderId = CreateMyDirectory();

            var fileMetadata = new File()
            {
                Name = name,
                MimeType = Helper.MimeType.KnownTypes[System.IO.Path.GetExtension(filePath)]
            };
            fileMetadata.Parents = new List<string>() { folderId };


            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, fileMetadata.MimeType);
                request.Fields = "id";

                if(progressChangedAction != null)
                    request.ProgressChanged += progressChangedAction;
                request.Upload();
            }
            var file = request.ResponseBody;
            //Console.WriteLine("File ID: " + file.Id);
        }

        //public void ListFiles()
        //{

        //    // Define parameters of request.
        //    FilesResource.ListRequest listRequest = service.Files.List();
        //    listRequest.PageSize = 10;
        //    listRequest.Fields = "nextPageToken, files(id, name)";

        //    // List files.
        //    IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
        //        .Files;
        //    Console.WriteLine("Files:");
        //    if (files != null && files.Count > 0)
        //    {
        //        foreach (var file in files)
        //        {
        //            Console.WriteLine("{0} ({1})", file.Name, file.Id);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No files found.");
        //    }
        //    Console.Read();
        //}
    }
}
