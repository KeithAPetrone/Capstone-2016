using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
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

    public IEnumerable<Google.Apis.Drive.v2.Data.File> Download()
    {
        // Request the files
        FilesResource.ListRequest listRequest = service.Files.List();
        FileList files = listRequest.Execute();
        IEnumerable<Google.Apis.Drive.v2.Data.File> daFiles = files.Items;
        //daFiles = daFiles.Where(x => x.Shared == false && x.Shared != null).ToList();
        return daFiles;
    }

    public IEnumerable<Google.Apis.Drive.v2.Data.File> Search(string criteria)
    {
        FilesResource.ListRequest listRequest = service.Files.List();
        FileList files = listRequest.Execute();
        IEnumerable<Google.Apis.Drive.v2.Data.File> daFiles = files.Items;
        daFiles = daFiles.Where(x => x.Title.Contains(criteria) || x.FileExtension.Contains(criteria)).ToList();
        return daFiles;
    }

    public void Upload(Google.Apis.Drive.v2.Data.File fileUpload, System.IO.MemoryStream stream, string mimeType)
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

    public void SyncWithDropBox()
    {
        DropBoxDownloader d = new DropBoxDownloader();
        GoogleDriveDownloader g = new GoogleDriveDownloader();
        var dropbox = d.Download();
        var googledrive = g.Download();

        foreach (var dfile in dropbox)
        {
            bool found = false;
            foreach (var gfile in googledrive)
            {
                if (gfile.Title.Equals(dfile.Name))
                {
                    found = true;
                }
            }
            if (!found)
            {
                var path = "C:/TempFiles/" + dfile.Name;
                file.SaveAs(Path.Combine(@"C:/TempFiles", dfile.Name));


                g.Upload(dfile);
            }
        }
    }
}
