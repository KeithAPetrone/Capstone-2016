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
    UserCredential credential;
    DriveService service;

    public GoogleDriveDownloader()
    {
        this.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "320329124466-6krqbu5gkdr0d8tfv91plfn31no65l00.apps.googleusercontent.com",
                ClientSecret = "qZ2oSBLR-NS3OK529S4UrTq4",
            },
            new[] { DriveService.Scope.Drive },
            "user",
            CancellationToken.None).Result;

        // Create the service.
        this.service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "OneBox",
        });
    }

    public IEnumerable<File> Download()
    {
        // Create the service.
        this.service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "OneBox",
        });

        // Request the files
        FilesResource.ListRequest listRequest = service.Files.List();
        FileList files = listRequest.Execute();
        IEnumerable<File> daFiles = files.Items;
        daFiles = daFiles.Where(x => x.Shared == false && x.Shared != null).ToList();

        return daFiles;
    }

    public void Upload(File fileUpload, System.IO.MemoryStream stream, string mimeType)
    {
        FilesResource.InsertMediaUpload request = this.service.Files.Insert(fileUpload, stream, mimeType);
        request.Upload();
        stream.Close();
    }
}
