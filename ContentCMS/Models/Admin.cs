using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContentCMS.Models
{
    public class Admin
    {
        public SelectList getParentPages(int selectedPageID)
        {
            contentCMSDBDataContext db = new contentCMSDBDataContext();
            var parentPages = from pp in db.pages where pp.parent == 0 && pp.isPublished == true select pp;

            List<object> parentPagesList = new List<object>();
            parentPagesList.Add(new
            {
                Id = 0,
                Name = "None",
                SelectedValue = 0
            });
            foreach (var item in parentPages)
            {
                parentPagesList.Add(new
                {
                    Id = item.pageID,
                    Name = item.pageTitle,
                    SelectedValue = item.pageID
                });
            }

            SelectList newSL = new SelectList(parentPagesList, "Id", "Name", selectedPageID);
            return newSL;
        }
    }

    public class AdminIndexModel
    {
        public string pageTitle { get; set; }
        public DateTime? lastUpdate { get; set; }
        public int pageID { get; set; }
        public bool isPublished { get; set; }
        public string pageURL { get; set; }
        public IEnumerable<AdminIndexModel> subPages { get; set; }
    }

    public class AdminPageModel
    {
        public page page { get; set; }
        public IEnumerable<page> pages { get; set; }
        public int? parentPageID { get; set; }
        public bool? isPublished { get; set; }
    }
}