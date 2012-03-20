using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContentCMS.Models;

namespace ContentCMS.Controllers
{
    public class PagesController : Controller
    {
        contentCMSDBDataContext db = new contentCMSDBDataContext();

        public ActionResult Index(string pageNames, string section)
        {
            string pagePath = "/" + section;

            if (!string.IsNullOrEmpty(pageNames))
            {
                foreach (string pageName in pageNames.Split('/'))
                {
                    if (pageName.Length > 0)
                    {
                        pagePath += "/" + pageName;
                    }
                }
            }

            pagePath += "/";

            var page = (from p in db.pages where p.pageURL.Equals(pagePath) select p).FirstOrDefault();

            if (page != null)
            {
                ViewData["page_title"] = page.pageTitle;
                ViewData["page_text"] = page.pageContent;
            }

            ViewData["pageURL"] = pagePath;

            Pages pages = new Pages("/" + section + "/");
            var navItems = pages.getPages();

            return View(navItems);
        }

        public PartialViewResult Navigation()
        {
            var rd = ControllerContext.ParentActionViewContext.RouteData;
            string currentController = rd.GetRequiredString("controller");

            Pages pages = new Pages(currentController);

            var navItems = pages.getNavigation();

            return PartialView(navItems);
        }

        public PartialViewResult Sidebar(string id)
        {
            string currentController = id;
            if (string.IsNullOrEmpty(id))
            {
                var rd = ControllerContext.ParentActionViewContext.RouteData;
                currentController = rd.GetRequiredString("controller");
            }
            Pages pages = new Pages("/" + currentController + "/");

            var navItems = pages.getPages();

            ViewData["pageURL"] = currentController;

            return PartialView(navItems);
        }

    }
}
