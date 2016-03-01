using DropboxRestAPI.Models.Core;
using Google.Apis.Drive.v2.Data;

namespace OneBox.Models
{
    public class CloudDriveAdapter
    {
        File googleFile = null;
        MetaData dropboxFile = null;


        public CloudDriveAdapter(File file)
        {
            this.googleFile = file;
        }

        public CloudDriveAdapter(MetaData file)
        {
            this.dropboxFile = file;
        }

        public string Title()
        {
            if (googleFile != null)
            {
                return googleFile.Title;
            }
            else if (dropboxFile != null)
            {
                return dropboxFile.Name;
            }
            else
            {
                return "No File Here";
            }
        }
    }
}