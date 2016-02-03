using System.Collections.Generic;
using System.Linq;
using DropboxRestAPI;

public class DropBoxDownloader
{
    string consumerKey = "";
    string consumerSecret = "";
    string accessToken = "";
    Client client;

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

    public List<DropboxRestAPI.Models.Core.MetaData> Download()
    {
        List<DropboxRestAPI.Models.Core.MetaData> list = new List<DropboxRestAPI.Models.Core.MetaData>();

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

        }
    }
}
