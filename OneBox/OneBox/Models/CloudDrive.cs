using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneBox.Models
{
    interface CloudDrive
    {
        IEnumerable<object> Download();

        Task Upload(string fileName, string path);

        IEnumerable<object> Search(string criteria);
    }
}
