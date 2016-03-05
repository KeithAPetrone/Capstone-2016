using DropboxRestAPI.Models.Core;
using Google.Apis.Drive.v2.Data;

namespace OneBox.Models
{
    public class CloudDriveAdapter
    {
        File googleFile;
        MetaData dropboxFile;

        public string Title { get; }
        public string Extension { get; }
        public string ThumbnailLink { get; }
        public string AlternateLink { get; }


        public CloudDriveAdapter(File file)
        {
            this.googleFile = file;
            this.dropboxFile = null;

            this.Title = googleFile.Title;
            this.Extension = googleFile.FileExtension;
            this.ThumbnailLink = googleFile.ThumbnailLink;
            this.AlternateLink = googleFile.AlternateLink;
        }

        public CloudDriveAdapter(MetaData file)
        {
            this.dropboxFile = file;
            this.googleFile = null;

            this.Title = dropboxFile.Name;
            this.Extension = dropboxFile.Extension;
        }
    }
}