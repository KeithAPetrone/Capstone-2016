using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebBackgrounder;

namespace OneBox.Models
{
    public class BackgroundSync : Job
    {
        GoogleDriveDownloader g;

        public BackgroundSync(TimeSpan interval, TimeSpan timeout, GoogleDriveDownloader g) : base("Background Synchronization", interval, timeout)
        {
            this.g = g;
        }

        public override Task Execute()
        {
            Thread.Sleep(3000);
            return g.SyncWithDropBox();
        }
    }
}