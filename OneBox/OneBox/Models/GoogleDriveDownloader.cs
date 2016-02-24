using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using OneBox.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GoogleDriveDownloader : ICloudDrive<Google.Apis.Drive.v2.Data.File>
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
        daFiles = daFiles.Where(x => x.Title.Contains(criteria) || (x.FileExtension != null && x.FileExtension.Contains(criteria))).ToList();
        return daFiles;
    }

    public async Task Upload(string fileName, string path)
    {
        Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
        body.Title = System.IO.Path.GetFileName(path);
        body.Description = "File uploaded OneBox";
        body.MimeType = GetMimeType(fileName);

        byte[] byteArray = System.IO.File.ReadAllBytes(path);
        MemoryStream stream = new MemoryStream(byteArray);

        FilesResource.InsertMediaUpload request = this.service.Files.Insert(body, stream, body.MimeType);
        request.Upload();
        stream.Close();
    }

    public string Delete(string id)
    {
        service.Files.Delete(id).Execute();

        return id;
    }

    public async Task SyncWithDropBox()
    {
        DropBoxDownloader d = new DropBoxDownloader();
        GoogleDriveDownloader g = new GoogleDriveDownloader();
        IEnumerable<DropboxRestAPI.Models.Core.MetaData> dropbox = (IEnumerable<DropboxRestAPI.Models.Core.MetaData>)d.Download();
        IEnumerable<Google.Apis.Drive.v2.Data.File> googledrive = (IEnumerable<Google.Apis.Drive.v2.Data.File>)g.Download();

        foreach (var dfile in dropbox)
        {
            bool found = false;
            string name = "";
            foreach (var gfile in googledrive)
            {
                name = gfile.Title;
                if (gfile.Title.Equals(dfile.Name))
                {
                    found = true;
                }
            }
            if (!found)
            {
                List<DropboxRestAPI.Models.Core.MetaData> list = new List<DropboxRestAPI.Models.Core.MetaData>();

                var rootFolder = await d.client.Core.Metadata.MetadataAsync("/", list: true);

                var file = rootFolder.contents.FirstOrDefault(x => x.Name.Contains(name));

                var tempFile = Path.GetTempFileName();
                using (var fileStream = System.IO.File.OpenWrite(tempFile))
                {
                    await d.client.Core.Metadata.FilesAsync(file.path, fileStream);
                }

                MemoryStream mem = new MemoryStream();
                using (var fileStream = System.IO.File.OpenRead(tempFile))
                {
                    fileStream.CopyTo(mem);
                    await g.Upload(file.Name, file.path);
                }          
            }
        }
    }

    private bool checkForDuplicates(Google.Apis.Drive.v2.Data.File file)
    {
        IEnumerable<Google.Apis.Drive.v2.Data.File> fileList = new List<Google.Apis.Drive.v2.Data.File>();

        foreach (var f in fileList)
        {
            if (file.Title.Equals(f.Title) && file.FullFileExtension.Equals(f.FullFileExtension))
            {
                if (file.FileSize == f.FileSize)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    private static string GetMimeType(string fileName)
    {
        string mimeType = "application/unknown";
        string ext = System.IO.Path.GetExtension(fileName).ToLower();
        Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        if (regKey != null && regKey.GetValue("Content Type") != null)
            mimeType = regKey.GetValue("Content Type").ToString();
        return mimeType;
    }
}
