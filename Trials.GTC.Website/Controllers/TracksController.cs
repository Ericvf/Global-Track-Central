using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trials.GTC.Website.Controllers
{
    public class TracksController : Controller
    {
        public ActionResult Index()
        {
            var client = new TrackCentral.TrackCentralClient();


            ViewBag.Title = "Tracks";

            int page = 1;
            int total = 10;

            int? trialsversion = null;
            int? trialstype = null;
            int? difficulty = null;

            string creator = null;
            string keyword = null;

            string sortName = null;
            bool sortDir = false;
            Guid[] tags = default(Guid[]);

            if (this.Request.QueryString.AllKeys.Contains("page"))
                page = Int32.Parse(this.Request.QueryString["page"]);

            if (this.Request.QueryString.AllKeys.Contains("total"))
                total = Int32.Parse(this.Request.QueryString["total"]);

            if (this.Request.QueryString.AllKeys.Contains("v"))
                trialsversion = Int32.Parse(this.Request.QueryString["v"]);

            if (this.Request.QueryString.AllKeys.Contains("t"))
                trialstype = Int32.Parse(this.Request.QueryString["t"]);

            if (this.Request.QueryString.AllKeys.Contains("d"))
                difficulty = Int32.Parse(this.Request.QueryString["d"]);

            if (this.Request.QueryString.AllKeys.Contains("creator"))
                creator = this.Request.QueryString["creator"];

            if (this.Request.QueryString.AllKeys.Contains("k"))
            {
                keyword = this.Request.QueryString["k"];
                ViewBag.Title = "Search results for: " + keyword;
            }

            if (this.Request.QueryString.AllKeys.Contains("dr"))
                sortDir = bool.Parse(this.Request.QueryString["dr"]);

            if (this.Request.QueryString.AllKeys.Contains("s"))
                sortName = this.Request.QueryString["s"];

            if (string.IsNullOrEmpty(sortName))
                sortName = "Submitted";

            var tracks = client.GetTracks(page, total, trialsversion, trialstype, difficulty, creator, keyword, sortName, sortDir, tags);
            return View(tracks);
        }
    }
}
