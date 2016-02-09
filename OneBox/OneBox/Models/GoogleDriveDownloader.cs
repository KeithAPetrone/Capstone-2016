using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class GoogleDriveDownloader
{
    UserCredential credential;
    DriveService service;

    public GoogleDriveDownloader()
    {
        IEnumerable<string> lines = System.IO.File.ReadLines("C:\\Users\\OneBox\\Documents\\Google.txt");
        this.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = lines.ElementAt(0),
                ClientSecret = lines.ElementAt(1),
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
        // Request the files
        FilesResource.ListRequest listRequest = service.Files.List();
        FileList files = listRequest.Execute();
        IEnumerable<File> daFiles = files.Items;
        //daFiles = daFiles.Where(x => x.Shared == false && x.Shared != null).ToList();
        return daFiles;
    }

    public IEnumerable<File> Search(string criteria)
    {
        FilesResource.ListRequest listRequest = service.Files.List();
        FileList files = listRequest.Execute();
        IEnumerable<File> daFiles = files.Items;
        daFiles = daFiles.Where(x => x.Title.Contains(criteria) || x.FileExtension.Contains(criteria)).ToList();
        return daFiles;
    }

    public void Upload(File fileUpload, System.IO.MemoryStream stream, string mimeType)
    {
        FilesResource.InsertMediaUpload request = this.service.Files.Insert(fileUpload, stream, mimeType);
        request.Upload();
        stream.Close();
    }

    public String Delete(String id)
    {
        service.Files.Delete(id).Execute();

        return id;
    }
}
