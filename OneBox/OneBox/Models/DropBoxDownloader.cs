using System.Collections.Generic;
using System.Linq;
using DropboxRestAPI;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using OneBox.Models;
using DropboxRestAPI.Models.Core;
using System;

public class DropBoxDownloader : ICloudDrive
{
    string consumerKey = "";
    string consumerSecret = "";
    string accessToken = "";
    public Client client;

    public DropBoxDownloader()
    {
        IEnumerable<string> lines = System.IO.File.ReadLines("C:\\Users\\OneBox\\Documents\\DropBox.txt");
        this.consumerKey = lines.ElementAt(0);
        this.consumerSecret = lines.ElementAt(1);
        this.accessToken = lines.ElementAt(2);

        var options = new Options
        {
            ClientId = consumerKey, //App key
            ClientSecret = consumerSecret, //App secret
            AccessToken = accessToken,
            RedirectUri = "https://www.dropbox.com/1/oauth2/authorize"
        };
        
        // Initialize a new Client (without an AccessToken)
        client = new Client(options);
    }

    public IEnumerable<CloudDriveAdapter> Download()
    {
        //List<DropboxRestAPI.Models.Core.MetaData> list = new List<DropboxRestAPI.Models.Core.MetaData>();

        //var rootFolder = await client.Core.Metadata.MetadataAsync("/", list: true);

        //foreach (var f in rootFolder.contents)
        //{
        //    list.Add(f);
        //}

        //foreach (var l in list)
        //{
        //    var tempFile = Path.GetTempFileName();
        //    using (var fileStream = System.IO.File.OpenWrite(tempFile))
        //    {
        //        client.Core.Metadata.FilesAsync(l.path, fileStream);
        //    }
        //}

        List<DropboxRestAPI.Models.Core.MetaData> results = new List<DropboxRestAPI.Models.Core.MetaData>();

        // Get the OAuth Request Url
        var authRequestUrl = client.Core.OAuth2.Authorize("code");

        // TODO: Navigate to authRequestUrl using the browser, and retrieve the Authorization Code from the response
        var authCode = "...";

        // Exchange the Authorization Code with Access/Refresh tokens
        var token = client.Core.OAuth2.TokenAsync(authCode);

        // Get root folder with content
        var rootFolder = client.Core.Metadata.MetadataAsync("/", list: true);

        // Find a file in the root folder
        var file = rootFolder.Result.contents.FirstOrDefault(x => x.is_dir == false);

        for (int i = 0; i < rootFolder.Result.contents.Count(); i++)
        {
            results.Add(rootFolder.Result.contents.ElementAt(i));
        }

        results = FileGrabber(results, results);

        IEnumerable<MetaData> done = results;

        done = done.Where(x => x.is_dir == false);

        List<CloudDriveAdapter> finalresults = new List<CloudDriveAdapter>();
        foreach (var f in done)
        {
            finalresults.Add(new CloudDriveAdapter(f));
        }
        return finalresults;
    }

    private List<MetaData> FileGrabber(List<MetaData> results, List<MetaData> check)
    {
        for (int i = 0; i < check.Count; i++)
        {
            if (check.ElementAt(i).is_dir)
            {
                var folder = client.Core.Metadata.MetadataAsync(check.ElementAt(i).path, list: true);
                results = FileGrabber(results, folder.Result.contents);
            }
            else if (!results.Contains(check.ElementAt(i)))
            {
                results.Add(check.ElementAt(i));
            }
        }
        return results;
    }

    public async Task Upload(string fileName, string path)
    {
        var rootFolder = await client.Core.Metadata.MetadataAsync("/", list: true);
        var file = rootFolder.contents.FirstOrDefault(x => x.is_dir == false);
        //var newFolder = await client.Core.FileOperations.CreateFolderAsync("/New Folder");

        using (var fileStream = System.IO.File.OpenRead(path))
        {
            var uploadedFile = await client.Core.Metadata.FilesPutAsync(fileStream, "/" + fileName);
        }
    }

    public IEnumerable<CloudDriveAdapter> Search(string criteria)
    {
        IEnumerable<CloudDriveAdapter> downloaded = Download();

        downloaded = downloaded.Where(x => x.Title.Contains(criteria) || x.Extension.Contains(criteria));

        List<CloudDriveAdapter> finalresults = new List<CloudDriveAdapter>();
        return finalresults;
    }

    private static string UpperCaseUrlEncode(string s)
    {
        char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
        for (int i = 0; i < temp.Length - 2; i++)
        {
            if (temp[i] == '%')
            {
                temp[i + 1] = char.ToUpper(temp[i + 1]);
                temp[i + 2] = char.ToUpper(temp[i + 2]);
            }
        }

        var values = new Dictionary<string, string>()
    {
        { "+", "%20" },
        { "(", "%28" },
        { ")", "%29" }
    };

        var data = new StringBuilder(new string(temp));
        foreach (string character in values.Keys)
        {
            data.Replace(character, values[character]);
        }
        return data.ToString();
    }

    IEnumerable<CloudDriveAdapter> ICloudDrive.Download()
    {
        throw new NotImplementedException();
    }

    IEnumerable<CloudDriveAdapter> ICloudDrive.Search(string criteria)
    {
        throw new NotImplementedException();
    }
}
