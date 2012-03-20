using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContentCMS.Models
{
    public class Pages
    {
        private string controllerName;
        contentCMSDBDataContext db = new contentCMSDBDataContext();

        public Pages(string controllerName)
        {
            this.controllerName = controllerName;
        }

        public IQueryable<pageLinks> getPages()
        {
            int parentID;
            var parentPage = (from pp in db.pages
                              where pp.pageURL.Equals(this.controllerName) && pp.isPublished == true
                              select pp).FirstOrDefault();
            if (parentPage == null) { parentID = -1; }
            else
            {
                parentID = parentPage.pageID;
            }
            var pages = from p in db.pages
                        where p.parent.Equals(parentID) && p.isPublished == true
                        orderby p.pageOrder
                        select new pageLinks
                        {
                            pageName = p.pageTitle,
                            pageURL = p.isStatic ? p.pageURL : "/p" + p.pageURL
                        };

            return pages;
        }

        public IQueryable<navigation> getNavigation()
        {
            var pages = from p in db.pages
                        where p.parent.Equals(0) && p.isPublished.Equals(true)
                        orderby p.pageOrder
                        select new navigation
                        {
                            pageName = p.pageTitle,
                            pageURL = p.isContent ? (p.isStatic ? p.pageURL : "/p" + p.pageURL) : null,
                            subPages = (from sp in db.pages
                                        where sp.parent.Equals(p.pageID) && sp.isPublished.Equals(true)
                                        orderby p.pageOrder
                                        select new pageLinks
                                        {
                                            pageName = sp.pageTitle,
                                            pageURL = sp.isStatic ? sp.pageURL : "/p" + sp.pageURL
                                        })
                        };

            return pages;
        }

        public class navigation
        {
            public string pageName { get; set; }
            public IEnumerable<pageLinks> subPages { get; set; }
            public string pageURL { get; set; }
        }

        public class pageLinks
        {
            public string pageName { get; set; }
            public string pageURL { get; set; }

        }
    }
}