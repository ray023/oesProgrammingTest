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
        public async Task<ActionResult> Index()
        {
            
            List<ClassMate> classMates = new List<ClassMate>();
            string url = "http://media.gettyimages.com/photos/group-of-people-looking-angrily-at-camera-picture-id159626661?s=170667a";
            var emotionClient = new EmotionServiceClient(ConfigurationManager.AppSettings["EmotionApiKey"]);
            /*
            var emotionResults = await emotionClient.RecognizeAsync(url);
            var webClient = new WebClient();
            byte[] imageData = webClient.DownloadData(url); //DownloadData function from here
            MemoryStream stream = new MemoryStream(imageData);
            Image img = Image.FromStream(stream);
            stream.Close();

            ClearUploadFolder();

            foreach (var item in emotionResults)
                classMates.Add(ProcessItem(img, item));

            var p = classMates.OrderByDescending(x => x.EmotionScores.Happiness).ToList();
            */
            return View();
        }

        private void ClearUploadFolder()
        {
            var dir = new DirectoryInfo(GetUploadDirectoryPath());

            foreach (var file in dir.EnumerateFiles("*.jpg"))
                file.Delete();
        }

        private ClassMate ProcessItem(Image img, Emotion emotion)
        {
            var point = new Point(emotion.FaceRectangle.Left, emotion.FaceRectangle.Top);
            var size = new Size(emotion.FaceRectangle.Width, emotion.FaceRectangle.Height);
            var rec = new Rectangle(point, size);
            var croppedFace = cropImage(img, rec);
            var myUniqueFileName = string.Format(@"{0}.jpg", DateTime.Now.Ticks);
            croppedFace.Save(Path.Combine(GetUploadDirectoryPath(), myUniqueFileName),
                System.Drawing.Imaging.ImageFormat.Jpeg);

            return new ClassMate(myUniqueFileName, emotion.Scores);
        }

        private Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private string GetUploadDirectoryPath()
        {
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"),
                                "Uploads");
        }
        
    }
}