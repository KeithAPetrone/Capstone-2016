using DropboxRestAPI.Models.Core;
using Google.Apis.Drive.v2.Data;

namespace OneBox.Models
{
    public class CloudDriveAdapter
    {
        public CloudDriveAdapter(File file)
        {

        }

        public CloudDriveAdapter(MetaData file)
        {

        }

        public string Title(File file)
        {
            return file.Title;
        }

        public string Title(MetaData file)
        {
            return file.Name;
        }
    }
}