using System;
using System.Collections.Generic;
using System.Linq;
using DropboxRestAPI;

public class DropBoxDownloader
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

    public IEnumerable<DropboxRestAPI.Models.Core.MetaData> Download()
    {
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

        IEnumerable<DropboxRestAPI.Models.Core.MetaData> done = results;

        done = done.Where(x => x.is_dir == false);

        return done;
    }

    private List<DropboxRestAPI.Models.Core.MetaData> FileGrabber(List<DropboxRestAPI.Models.Core.MetaData> results, List<DropboxRestAPI.Models.Core.MetaData> check)
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

    internal void Upload(string root, string fileName, string path, byte[] contents)
    {
        string putUrl = "https://api-content.dropbox.com/1/files_put/sandbox/some-image.png";
        
        IConsumerRequest putRequest = session.Request().Put().ForUrl(putUrl).WithRawContent(contents);

        string putInfo = putRequest.ReadBody();
    }
    
    public IEnumerable<DropboxRestAPI.Models.Core.MetaData> Search(string criteria)
    {
        IEnumerable<DropboxRestAPI.Models.Core.MetaData> downloaded = Download();

        downloaded = downloaded.Where(x => x.Name.Contains(criteria) || x.Extension.Contains(criteria));

        return downloaded;
    }
}
