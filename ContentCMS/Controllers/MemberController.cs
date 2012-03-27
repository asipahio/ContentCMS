using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ContentCMS.Controllers
{
    public class MemberController : Controller
    {
        //
        // GET: /Member/

        [HttpGet]
        public ActionResult Login(string returnURL)
        {
            ViewData["returnURL"] = returnURL;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnURL)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username");
            }
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password");
            }
            if (!Membership.ValidateUser(username, password))
            {
                ModelState.AddModelError("_Form", "The username or password is wrong");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            FormsAuthentication.SetAuthCookie(username, true);
            if (!string.IsNullOrEmpty(returnURL))
            {
                Response.Redirect(returnURL);
            }
            else
            {
                Response.Redirect(FormsAuthentication.DefaultUrl);
            }

            return View();
        }

    }
}
