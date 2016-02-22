using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneBox.Models
{
    public interface ICloudDrive<T>
    {
        IEnumerable<T> Download();

        Task Upload(string fileName, string path);

        IEnumerable<T> Search(string criteria);
    }
}
