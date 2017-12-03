using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Emotion;
using oesProgrammingTest.Models;
using System.Configuration;

namespace oesProgrammingTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private string GetUploadDirectoryPath()
        {
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"),
                                "Uploads");
        }
        
    }
}