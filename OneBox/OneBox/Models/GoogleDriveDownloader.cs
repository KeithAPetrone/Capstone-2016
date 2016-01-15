using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

    public class GoogleDriveDownloader
    {
        public GoogleDriveDownloader()
        {
            
        }

        public List<File> Download()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "320329124466-6krqbu5gkdr0d8tfv91plfn31no65l00.apps.googleusercontent.com",
                    ClientSecret = "qZ2oSBLR-NS3OK529S4UrTq4",
                },
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None).Result;

            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "OneBox",
            });

            File body = new File();
            body.Title = "My document";
            body.Description = "A test document";
            body.MimeType = "text/plain";

            //byte[] byteArray = System.IO.File.ReadAllBytes("document.txt");
            //System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            FilesResource.ListRequest listRequest = service.Files.List();
            FileList files = listRequest.Execute();
            IEnumerable<File> daFiles = files.Items;
            daFiles = daFiles.Where(x => x.Shared == false && x.Shared != null).ToList();
            Console.WriteLine("Percentage Complete:\n0%");
            long? totalDownloadSize = 0;
            //double currentPercent = 0;
            List<File> returnFiles = new List<File>();
            for (int i = 0; i < 10; i++)
            {
                totalDownloadSize += daFiles.ElementAt(i).FileSize;
                returnFiles.Add(daFiles.ElementAt(i));
            }
            //foreach (var item in daFiles)
            //{
                //currentPercent += Convert.ToDouble((double)item.FileSize / (double)totalDownloadSize);
                //var v = service.HttpClient.GetStreamAsync(item.DownloadUrl);
                //var result = v.Result;
                //string path = "..//..//files//" + item.Title;
                //using (var fileStream = System.IO.File.Create(path))
                //{
                //    result.CopyTo(fileStream);
                //}
                //Console.WriteLine((int)(currentPercent * 100) + "%");
            //}
            Console.WriteLine("Done!!!");
        return returnFiles;
        }
    }
