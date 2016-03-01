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

            ICloudDrive[] cloudDrives = new ICloudDrive[2];

            ICloudDrive<Google.Apis.Drive.v2.Data.File> g = new ICloudDrive();
            //ICloudDrive<DropboxRestAPI.Models.Core.MetaData> d = new DropBoxDownloader();

            IEnumerable<Google.Apis.Drive.v2.Data.File> googleresults = g.Download();
            TempData["GoogleResult"] = googleresults;

            //IEnumerable<DropboxRestAPI.Models.Core.MetaData> dropboxresults = d.Download();
            //TempData["DropBoxResult"] = dropboxresults;

            //TempData["DropBoxDownloader"] = d;

            new BackgroundSync(new System.TimeSpan(0, 0, 20, 0, 0), new System.TimeSpan(0, 5, 20, 0, 0), g);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            ICloudDrive<Google.Apis.Drive.v2.Data.File> g = new GoogleDriveDownloader();
            ICloudDrive<DropboxRestAPI.Models.Core.MetaData> d = new DropBoxDownloader();

            var fileName = Path.GetFileName(file.FileName);
            var path = "C:/TempFiles/" + fileName;
            file.SaveAs(Path.Combine(@"C:/TempFiles", fileName));

            await g.Upload(fileName, path);

            await d.Upload(fileName, path);

            return Redirect("FileList");
        }


        [HttpPost]
        public ActionResult FileList(string search)
        {
            ICloudDrive<Google.Apis.Drive.v2.Data.File> g = new GoogleDriveDownloader();
            //ICloudDrive<DropboxRestAPI.Models.Core.MetaData> d = new DropBoxDownloader();

            IEnumerable<Google.Apis.Drive.v2.Data.File> googleresults = g.Search(search);
            TempData["GoogleResult"] = googleresults;

            //IEnumerable<DropboxRestAPI.Models.Core.MetaData> dropboxresults = d.Search(search);
            //TempData["DropBoxResult"] = googleresults;

            //TempData["DropBoxDownloader"] = d;

            return View();
        }
    }
}