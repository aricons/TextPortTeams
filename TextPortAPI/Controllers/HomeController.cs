﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TextPortAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("http://textport.com/api");
        }
    }
}
