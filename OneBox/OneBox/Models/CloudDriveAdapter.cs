using DropboxRestAPI.Models.Core;
using Google.Apis.Drive.v2.Data;
using System;

namespace OneBox.Models
{
    public class CloudDriveAdapter
    {
        File googleFile;
        MetaData dropboxFile;


        public CloudDriveAdapter(File file)
        {
            this.googleFile = file;
            this.dropboxFile = null;
        }

        public CloudDriveAdapter(MetaData file)
        {
            this.dropboxFile = file;
            this.googleFile = null;
        }

        public Tuple<File, MetaData> GetFile()
        {
            return new Tuple<File, MetaData>(this.googleFile, this.dropboxFile);
        }
    }
}