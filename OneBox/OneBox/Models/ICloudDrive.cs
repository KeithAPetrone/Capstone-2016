using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneBox.Models
{
    public interface ICloudDrive
    {
        IEnumerable<CloudDriveAdapter> Download();

        Task Upload(string fileName, string path);

        IEnumerable<CloudDriveAdapter> Search(string criteria);
    }
}
