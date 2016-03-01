using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using OneBox.Models;

namespace OneBox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FileList()
        {
            ViewBag.Message = "Your file list page.";

            List<ICloudDrive> cloudDrives = new List<ICloudDrive>()
            {
                new GoogleDriveDownloader(),
                new DropBoxDownloader()
            };

            TempData["GoogleResult"] = cloudDrives[0].Download();

            new BackgroundSync(new System.TimeSpan(0, 0, 20, 0, 0), new System.TimeSpan(0, 5, 20, 0, 0), cloudDrives[0]);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            List<ICloudDrive> cloudDrives = new List<ICloudDrive>()
            {
                new GoogleDriveDownloader(),
                new DropBoxDownloader()
            };

            var fileName = Path.GetFileName(file.FileName);
            var path = "C:/TempFiles/" + fileName;
            file.SaveAs(Path.Combine(@"C:/TempFiles", fileName));

            foreach (ICloudDrive cd in cloudDrives)
            {
                await cd.Upload(fileName, path);
            }

            return Redirect("FileList");
        }


        [HttpPost]
        public ActionResult FileList(string search)
        {
            List<ICloudDrive> cloudDrives = new List<ICloudDrive>()
            {
                new GoogleDriveDownloader(),
                new DropBoxDownloader()
            };

            TempData["GoogleResult"] = cloudDrives[0].Search(search);

            return View();
        }
    }
}