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
    public class EmotionAnalyzerController : Controller
    {
        DirectoryInfo _dirInfo;
        public EmotionAnalyzerController()
        {
            _dirInfo = new DirectoryInfo(GetUploadDirectory());
        }
        public ActionResult Index()
        {
            @ViewBag.UploadFiles = GetUploadedFiles();
            return View();
        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    SaveUploadedFile(file);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"ERROR Uploading File: {ex.Message.ToString()}";
                }
            }
            else
                ViewBag.Message = "No file specified.";

            return View();
        }
        private List<string> GetUploadedFiles()
        {
            return _dirInfo
                        .GetFiles()
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => x.Name)
                        .ToList() ?? new List<string>();
        }
        public async Task<ActionResult> Analyze()
        {
            var currentFile = Request.QueryString["fileName"];
            
            if (string.IsNullOrEmpty(currentFile))
                RedirectToAction("Index");

            string fileAndLocation = Path.Combine(GetUploadDirectory(), currentFile);
            var emotionClient = new EmotionServiceClient(ConfigurationManager.AppSettings["EmotionApiKey"]);

            var emotionResults = await emotionClient.RecognizeAsync(System.IO.File.Open(fileAndLocation,FileMode.Open));

            Image img = Image.FromStream(System.IO.File.Open(fileAndLocation, FileMode.Open));

            List<ClassMate> classMates = new List<ClassMate>();
            foreach (var item in emotionResults)
                classMates.Add(ProcessItem(img, item));

            @ViewBag.CurrentFile = currentFile;
            @ViewBag.ClassMates = classMates;
            return View();
        }
        private void SaveUploadedFile(HttpPostedFileBase file)
        {
            string path = Path.Combine(GetUploadDirectory(),
                                       Path.GetFileName(file.FileName));
            file.SaveAs(path);
        }
        private string GetUploadDirectory()
        {
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"),
                                "Uploads");
        }
        private void ClearAnalyzeFolder()
        {
            var dir = new DirectoryInfo(GetAnalyzeDirectory());

            foreach (var file in dir.EnumerateFiles("*.jpg"))
                file.Delete();
        }
        private string GetAnalyzeDirectory()
        {
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"),
                                "AnalyzeDirectory");
        }
        private ClassMate ProcessItem(Image img, Emotion emotion)
        {
            var point = new Point(emotion.FaceRectangle.Left, emotion.FaceRectangle.Top);
            var size = new Size(emotion.FaceRectangle.Width, emotion.FaceRectangle.Height);
            var rec = new Rectangle(point, size);
            var croppedFace = cropImage(img, rec);
            var myUniqueFileName = string.Format(@"{0}.jpg", DateTime.Now.Ticks);
            croppedFace.Save(Path.Combine(GetAnalyzeDirectory(), myUniqueFileName),
                System.Drawing.Imaging.ImageFormat.Jpeg);

            return new ClassMate(myUniqueFileName, emotion.Scores);
        }
        private Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
    }
}
