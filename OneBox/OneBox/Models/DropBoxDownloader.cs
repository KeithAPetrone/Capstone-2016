using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DropboxRestAPI;
using OAuth;

public class DropBoxDownloader
{

    public DropBoxDownloader()
    {
            
    }

    public void DropBoxAuthenticate()
    {
        var consumerKey = "vffs1vook3fyx5x";
        var consumerSecret = "7ecorboxscy56cr";

        var uri = new Uri("https://api.dropbox.com/1/oauth/request_token");

        // Generate a signature
        OAuthBase oAuth = new OAuthBase();
        string nonce = oAuth.GenerateNonce();
        string timeStamp = oAuth.GenerateTimeStamp();
        string parameters;
        string normalizedUrl;
        string signature = oAuth.GenerateSignature(uri, consumerKey, consumerSecret,
            String.Empty, String.Empty, "GET", timeStamp, nonce, OAuthBase.SignatureTypes.HMACSHA1,
            out normalizedUrl, out parameters);

        signature = HttpUtility.UrlEncode(signature);

        StringBuilder requestUri = new StringBuilder(uri.ToString());
        requestUri.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
        requestUri.AppendFormat("oauth_nonce={0}&", nonce);
        requestUri.AppendFormat("oauth_timestamp={0}&", timeStamp);
        requestUri.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
        requestUri.AppendFormat("oauth_version={0}&", "1.0");
        requestUri.AppendFormat("oauth_signature={0}", signature);


        try
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(requestUri.ToString()));
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();

            var queryString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var parts = queryString.Split('&');
            var token = parts[1].Substring(parts[1].IndexOf('=') + 1);
            var tokenSecret = parts[0].Substring(parts[0].IndexOf('=') + 1);
        }
        catch (WebException e)
        { }
    }
}
