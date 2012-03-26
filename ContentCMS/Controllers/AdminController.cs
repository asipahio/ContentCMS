using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContentCMS.Models;
using System.Text.RegularExpressions;

namespace ContentCMS.Controllers
{
    public class AdminController : Controller
    {
        contentCMSDBDataContext db = new contentCMSDBDataContext();
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var content = from pages in db.pages
                              where pages.parent.Equals(0)
                              orderby pages.parent ascending, pages.pageURL ascending
                              select new AdminIndexModel
                              {
                                  pageTitle = pages.pageTitle,
                                  lastUpdate = pages.lastUpdate,
                                  pageID = pages.pageID,
                                  isPublished = pages.isPublished,
                                  pageURL = pages.isContent ? (pages.isStatic ? pages.pageURL : "/p" + pages.pageURL) : null,
                                  subPages = (from sp in db.pages
                                              where sp.parent.Equals(pages.pageID)
                                              select new AdminIndexModel
                                              {
                                                  pageTitle = sp.pageTitle,
                                                  lastUpdate = sp.lastUpdate,
                                                  pageID = sp.pageID,
                                                  pageURL = sp.isStatic ? sp.pageURL : "/p" + sp.pageURL,
                                                  isPublished = sp.isPublished
                                              })
                              };



            return View(content);
        }

        [HttpGet]
        public ActionResult addtemplate()
        {
            Admin admin = new Admin();
            ViewData["parentPages"] = admin.getParentPages(0);
            ViewData["isPublished"] = false;

            return View();
        }

        [ValidateInput(false), HttpPost]
        public ActionResult addtemplate(string pageTitle, int? parentPageID, bool? isPublished, string pageContent)
        {
            if (string.IsNullOrEmpty(pageContent))
            {
                ModelState.AddModelError("page_text", "Text cannot be empty");
            }
            if (string.IsNullOrEmpty(pageTitle))
            {
                ModelState.AddModelError("page_title", "Title cannot be empty");
            }
            if (!ModelState.IsValid)
            {
                Admin admin = new Admin();
                ViewData["parentPages"] = admin.getParentPages((int)parentPageID);
                ViewData["isPublished"] = false;
                ViewData["page_title"] = pageTitle;
                ViewData["page_text"] = pageContent;
                ViewData["isPublished"] = isPublished;
                return View();
            }

            var parentPage = db.pages.Single(d => d.pageID == (int)parentPageID);

            string pageURL = parentPage.pageURL + createPageURL(pageTitle) + "/";

            page pageToAdd = new page
            {
                lastUpdate = DateTime.Now,
                parent = (int)parentPageID,
                isPublished = (bool)isPublished,
                isContent = true,
                isStatic = false,
                pageContent = pageContent,
                pageTitle = pageTitle,
                pageURL = pageURL
            };

            db.pages.InsertOnSubmit(pageToAdd);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditTemplate(int id)
        {
            AdminPageModel pagewithcache = new AdminPageModel();
            pagewithcache.page = db.pages.Single(d => d.pageID == id);
            pagewithcache.pages = from p in db.pages
                                  where p.pageURL == pagewithcache.page.pageURL && p.isPublished == false
                                  orderby p.lastUpdate descending
                                  select p;

            int parentPageID = (from ppu in db.pages where ppu.pageID == pagewithcache.page.parent select ppu.pageID).FirstOrDefault();

            Admin admin = new Admin();
            ViewData["parentPages"] = admin.getParentPages(parentPageID);


            return View(pagewithcache);
        }

        [ValidateInput(false), HttpPost]
        public ActionResult EditTemplate(int id, Models.AdminPageModel pagetoedit)
        {
            var pageoriginal = (from p in db.pages where p.pageID == id select p).First();
            Admin admin = new Admin();
            ViewData["parentPages"] = admin.getParentPages((int)pageoriginal.parent);

            if (string.IsNullOrEmpty(pagetoedit.page.pageContent))
            {
                ModelState.AddModelError("page_text", "Text cannot be empty");
            }
            if (string.IsNullOrEmpty(pagetoedit.page.pageTitle))
            {
                ModelState.AddModelError("page_title", "Title cannot be empty");
            }
            if (pageoriginal.pageTitle.Length < pageoriginal.pageTitle.Length)
            {
                ModelState.AddModelError("press_link", "Your link cannot be longer than " + pageoriginal.pageTitle.Length + " characters. Currently it is " + pagetoedit.page.pageTitle.Length);
            }
            if (!ModelState.IsValid)
            {
                return View(pagetoedit);
            }

            var parentPage = db.pages.Single(d => d.pageID == (int)pagetoedit.parentPageID);

            string pageURL = parentPage.pageURL + createPageURL(pagetoedit.page.pageTitle) + "/";

            var cachedPages = from p in db.pages
                              where p.pageURL == pageoriginal.pageURL && p.isPublished == false
                              orderby p.lastUpdate descending
                              select p;
            foreach (var cachedPage in cachedPages)
            {
                cachedPage.pageURL = pageURL;
            }

            var pagetocache = new Models.page();
            pagetocache.lastUpdate = pageoriginal.lastUpdate;
            pagetocache.pageContent = pageoriginal.pageContent;
            pagetocache.pageTitle = pageoriginal.pageTitle;
            pagetocache.pageURL = pageURL;
            pagetocache.isPublished = false;
            pagetocache.pageOrder = pageoriginal.pageOrder;
            pagetocache.parent = (int)pagetoedit.parentPageID;
            
            //Need to implement the the asp.net user guid to the pages table
            //pagetocache.author = userguid; 

            db.pages.InsertOnSubmit(pagetocache);

            pageoriginal.pageTitle = pagetoedit.page.pageTitle;
            pageoriginal.pageContent = pagetoedit.page.pageContent;
            pageoriginal.pageURL = pageURL;
            pageoriginal.isPublished = (bool)pagetoedit.isPublished;
            pageoriginal.parent = (int)pagetoedit.parentPageID;
            pageoriginal.pageOrder = pagetoedit.page.pageOrder;
            pageoriginal.lastUpdate = DateTime.Now;

            //Need to implement the the asp.net user guid to the pages table
            //pageoriginal.author = userguid; 

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult setpageasdefault(int id, int oldid)
        {
            var pageoriginal = (from p in db.pages where p.pageID == oldid select p).First();
            var pagecached = (from p in db.pages where p.pageID == id select p).First();
            var temppage = new Models.page();
            temppage.lastUpdate = pageoriginal.lastUpdate;
            temppage.pageContent = pageoriginal.pageContent;
            temppage.pageTitle = pageoriginal.pageTitle;
            temppage.pageURL = pageoriginal.pageURL;

            pageoriginal.lastUpdate = DateTime.Now;
            pageoriginal.pageContent = pagecached.pageContent;
            pageoriginal.pageTitle = pagecached.pageTitle;
            pageoriginal.pageURL = pagecached.pageURL;

            pagecached.lastUpdate = temppage.lastUpdate;
            pagecached.pageContent = temppage.pageContent;
            pagecached.pageTitle = temppage.pageTitle;
            pagecached.pageURL = temppage.pageURL;
            db.SubmitChanges();


            return RedirectToAction("EditTemplate", new { id = oldid });
        }

        [ HttpGet]
        public ActionResult DeleteTemplate(int id)
        {
            var pagetodel = (from p in db.pages where p.pageID.Equals(id) select p).First();

            //If the page to be deleted is a main (not a cached one) page then, this will delete all the cached pages.
            //This implementation should change. There should be a mainPage flag in the database determining which page is the main one
            if (pagetodel.isPublished)
            {
                var cachedPagesToDel = (from p in db.pages where p.pageURL.Equals(pagetodel.pageURL) select p).First();
                db.pages.DeleteOnSubmit(cachedPagesToDel);
            }

            db.pages.DeleteOnSubmit(pagetodel);

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        private string createPageURL(string pageName)
        {
            pageName = pageName.Replace(' ', '-');
            return Regex.Replace(pageName.ToLower(), "[^a-z0-9_-]", string.Empty);
        }

        public PartialViewResult Sidebar()
        {
            var rd = ControllerContext.ParentActionViewContext.RouteData;
            string currentView = rd.GetRequiredString("action");
            ViewData["currentView"] = currentView;

            return PartialView();
        }

        public ActionResult PagePreview(int id)
        {
            var page = (from p in db.pages where p.pageID.Equals(id) select p).FirstOrDefault();

            var parentPage = (from p in db.pages where p.pageID == page.parent select p).FirstOrDefault();
            ViewData["pageheader"] = parentPage.pageTitle;

            if (page != null)
            {
                ViewData["page_title"] = page.pageTitle;
                ViewData["page_text"] = page.pageContent;
            }

            Pages pages = new Pages(parentPage.pageURL);
            var navItems = pages.getPages();

            return View(navItems);
        }
    }
}
