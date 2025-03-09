using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.EF;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace CNTTVNWebAPIO.ApiCMS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        //private CNTTVNData db = new CNTTVNData();
        private static readonly ILog log = LogMaster.GetLogger("upload", "upload");

        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public UploadController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadFile()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                List<String> files = new List<string>();
                var file = Request.Form.Files[0];
                string folderName = "Uploads";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    def.meta = new Meta(200, "Success");
                    def.data = fullPath;
                    return Ok(def);
                }
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [HttpPost("uploadImage")]
        public IActionResult Upload()
        {
            //string domain = ConfigurationManager.AppSettings["domain"].ToString();
            //User user = db.Users.Find(id);
            //int ShopId = (int)product.ShopId;
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<String> files = new List<string>();
                foreach (var file in httpRequest)
                {
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 100; //Size = 100 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có định dạng: .jpg, .jpeg, .gif, .png, .bmp.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng nhỏ hơn 100MB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length < 0)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng lớn hơn 1KB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            string folderName = "Uploads\\Images";
                            string webRootPath = _hostingEnvironment.ContentRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }

                            DateTime now = DateTime.Now;
                            //fileData = binaryReader.ReadBytes(postedFile.Length);

                            //var filePath = HttpContext..Current.Server.MapPath("~/Images/u/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension);

                            string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                            string rel = "thumbs\\thumb500\\" + name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;


                            //string fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                            string fullPath = Path.Combine(newPath, img);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                //byte[] fileData = null;
                                postedFile.CopyTo(stream);
                                var image = Bitmap.FromStream(stream);
                                //using (var binaryReader = new BinaryReader(stream))
                                //{
                                //fileData = binaryReader.ReadBytes((int)stream.Length);
                                string thumbPath = Path.Combine(newPath, rel);
                                createThumb(500, image, thumbPath, extension);
                                //}
                            }


                            files.Add(img);

                            //def.meta = new Meta(200, "Success");
                            //def.data = fullPath;
                            //return Ok(def);

                            //Stream stream = postedFile.OpenReadStream();
                            //byte[] fileData = null;
                            //using (var binaryReader = new BinaryReader(stream))
                            //{
                            //    DateTime now = DateTime.Now;
                            //    fileData = binaryReader.ReadBytes(postedFile.Length);

                            //    var filePath = HttpContext.Current.Server.MapPath("~/Images/u/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension);
                            //    Utils.createThumb(500, filePath, fileData, extension);
                            //    string img = "/images/u/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                            //    string rel = "/images/u/" + name + "_thumb500_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                            //    files.Add(img);


                            //add avata user
                            //user.Avata = rel;
                            //db.Entry(user).State = EntityState.Modified;
                            //db.SaveChanges();
                        }

                    }
                }
                def.meta = new Meta(200, "Success");
                def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        //[HttpPost("uploadImage/{id}")]
        //public IActionResult Upload(int id)
        //{
        //    //string domain = ConfigurationManager.AppSettings["domain"].ToString();
        //    //User user = db.Users.Find(id);
        //    //int ShopId = (int)product.ShopId;
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        string url = _configuration["AppSettings:baseApi"];
        //        int i = 0;
        //        var httpRequest = Request.Form.Files;

        //        List<String> files = new List<string>();

        //        foreach (var file in httpRequest)
        //        {

        //            var postedFile = httpRequest[i];

        //            byte[] fileData = null;
        //            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
        //            {
        //                fileData = binaryReader.ReadBytes((int)file.Length);
        //            }

        //            //HttpContent fileStreamContent = new StreamContent(file.OpenReadStream());

        //            //fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "haha" };

        //            //fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //            //using (var client = new HttpClient())
        //            //{

        //            //    using (var formData = new MultipartFormDataContent())

        //            //    {

        //            //        formData.Add(fileStreamContent);

        //            //        var response = client.PostAsync(url, formData);

        //            //        return response.Result;

        //            //    }
        //            //}


        //            //using (var formData = new MultipartFormDataContent())

        //            //{

        //            //    formData.Add(fileStreamContent);

        //            //    //RestClient client = new RestClient(_configuration["AppSettings:baseApi"]);
        //            //    //var request = new RestRequest("api/upload/uploadImage", Method.POST);
        //            //    //request.AddFileBytes(formData);
        //            //    //request.AddHeader("Content-Type", "application/octet-stream");
        //            //    //IRestResponse response = client.Execute(request);
        //            //    //var content = response.Content; // raw content as string
        //            //    //log.Info(content);

        //            //    //var response = client.PostAsync(url, formData);

        //            //    //return response.Result;

        //            //}

        //            if (postedFile != null && postedFile.Length > 0)
        //            {
        //                int MaxContentLength = 1024 * 1024 * 10; //Size = 100 MB  

        //                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                if (!AllowedFileExtensions.Contains(extension))
        //                {
        //                    var message = string.Format("Bạn vui lòng upload hình ảnh có định dạng: .jpg, .jpeg, .gif, .png, .bmp.");
        //                    def.meta = new Meta(600, message);
        //                    return Ok(def);
        //                }
        //                else if (postedFile.Length > MaxContentLength)
        //                {
        //                    var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng nhỏ hơn 10MB.");
        //                    def.meta = new Meta(600, message);
        //                    return Ok(def);
        //                }
        //                else if (postedFile.Length < 0)
        //                {
        //                    var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng lớn hơn 1KB.");
        //                    def.meta = new Meta(600, message);
        //                    return Ok(def);
        //                }
        //                else
        //                {
        //                    RestClient client = new RestClient(_configuration["AppSettings:baseApi"]);
        //                    var request = new RestRequest("web/upload/uploadImage/"+id, Method.POST);
        //                    request.AddFileBytes(postedFile.Name, fileData, postedFile.Name, "application/octet-stream");
        //                    IRestResponse response = client.Execute(request);
        //                    var content = response.Content; // raw content as string
        //                    JObject json = JObject.Parse(content);

        //                    if (json["meta"]["error_code"].ToString() == "200")
        //                    {
        //                        def.meta = new Meta(200, "Success");
        //                        files.Add(json["data"][0].ToString());
        //                    }
        //                    else
        //                    {
        //                        def.meta = new Meta(400, "Bad request");
        //                    }
        //                }

        //            }
        //        }
                
        //        def.data = files;
        //        return Ok(def);
        //    }
        //    catch (Exception ex)
        //    {
        //        def.meta = new Meta(400, "Bad request");
        //        return Ok(def);
        //    }
        //}

        //public static void createThumb(int size, int quality, string inputPath, string fullPath)
        //{
        //    try {
        //        using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
        //        {
        //            int width, height;
        //            if (image.Width > image.Height)
        //            {
        //                width = size;
        //                height = Convert.ToInt32(image.Height * size / (double)image.Width);
        //            }
        //            else
        //            {
        //                width = Convert.ToInt32(image.Width * size / (double)image.Height);
        //                height = size;
        //            }
        //            var resized = new Bitmap(width, height);
        //            using (var graphics = Graphics.FromImage(resized))
        //            {
        //                graphics.CompositingQuality = CompositingQuality.HighSpeed;
        //                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                graphics.CompositingMode = CompositingMode.SourceCopy;
        //                graphics.DrawImage(image, 0, 0, width, height);
        //                using (var output = File.Open(
        //                    OutputPath(path, outputDirectory, SystemDrawing), FileMode.Create))
        //                {
        //                    var qualityParamId = Encoder.Quality;
        //                    var encoderParameters = new EncoderParameters(1);
        //                    encoderParameters.Param[0] = new EncoderParameter(qualityParamId, quality);
        //                    var codec = ImageCodecInfo.GetImageDecoders()
        //                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
        //                    resized.Save(output, codec, encoderParameters);
        //                }
        //            }
        //        }

        //    }
        //    catch(Exception e) { }
        //}

        public static void createThumb(int thumbWidth, Image image, string thumbPath, string file_type)
        {
            //extract path
            int lastIndex = thumbPath.LastIndexOf("\\");
            string path = thumbPath.Substring(0, lastIndex);
            Directory.CreateDirectory(path);
            //using (var image = new Bitmap(Image.FromFile(filePath)))
            //{
            //MemoryStream ms = new MemoryStream(input);
            //Image returnImage = Image.FromStream(ms);
            //Image image = Image.FromStream(ms);
            double srcWidth = image.Width;
            double srcHeight = image.Height;
            double thumbHeight = (srcHeight / srcWidth) * thumbWidth;
            Bitmap bmp = new Bitmap(thumbWidth, (int)thumbHeight);

            Graphics gr = Graphics.FromImage(bmp);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.InterpolationMode = InterpolationMode.High;

            Rectangle rectDestination = new Rectangle(0, 0, thumbWidth, (int)thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, (int)srcWidth, (int)srcHeight, GraphicsUnit.Pixel);
            //rotate
            //foreach (var prop in image.PropertyItems)
            //{
            //    if (prop.Id == 0x0112) //value of EXIF
            //    {
            //        int orientationValue = image.GetPropertyItem(prop.Id).Value[0];
            //        RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
            //        bmp.RotateFlip(rotateFlipType);
            //        break;
            //    }
            //}
            if (file_type.ToLower() == ".jpg" || file_type.ToLower() == ".jpeg")
                bmp.Save(thumbPath, ImageFormat.Jpeg);
            else if (file_type.ToLower() == ".png")
                bmp.Save(thumbPath, ImageFormat.Png);
            else if (file_type.ToLower() == ".gif")
                bmp.Save(thumbPath, ImageFormat.Gif);
            else
                bmp.Save(thumbPath, ImageFormat.Jpeg);

            bmp.Dispose();
            image.Dispose();
            //}
        }

        private static string RenderTag(object tagValue)
        {
            // Arrays don't render well without assistance.
            var array = tagValue as Array;
            if (array != null)
            {
                // Hex rendering for really big byte arrays (ugly otherwise)
                if (array.Length > 20 && array.GetType().GetElementType() == typeof(byte))
                    return "0x" + string.Join("", array.Cast<byte>().Select(x => x.ToString("X2")).ToArray());

                return string.Join(", ", array.Cast<object>().Select(x => x.ToString()).ToArray());
            }

            return tagValue.ToString();
        }

        [HttpPost("uploadImage/{id}")]
        public IActionResult UploadImage(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<String> files = new List<string>();
                foreach (var file in httpRequest)
                {
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 100; //Size = 100 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có định dạng: .jpg, .jpeg, .gif, .png, .bmp.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng nhỏ hơn 100MB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length < 0)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng lớn hơn 1KB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            string folderName = _configuration["AppSettings:rootUploadsThumbs"];
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }

                            DateTime now = DateTime.Now;
                            string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; ;
                            //Lấy danh sách tạo ảnh thumb
                            using (var db = new CNTTVNWebContext())
                            {
                                string fullPath = Path.Combine(newPath, img);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    postedFile.CopyTo(stream);

                                    var Thumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == id).FirstOrDefault();
                                    int width = Thumb != null ? Thumb.Width : 300;

                                    var image = Bitmap.FromStream(stream);
                                    string rel = "_thumb\\" + img;
                                    string thumbPath = Path.Combine(newPath, rel);
                                    createThumb(width, image, thumbPath, extension);
                                }
                            }
                            files.Add(img);
                        }


                        //files.Add(img);
                    }
                    i++;

                }
                def.meta = new Meta(200, "Success");
                def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [HttpPost("autoGenThumbs/{type}/{width}")]
        public IActionResult AutoGenThumbs(int type, int width)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                string folderName = _configuration["AppSettings:rootUploadsThumbs"];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //DateTime now = DateTime.Now;
                //string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; 
                //Lấy danh sách tạo ảnh thumb
                using (var db = new CNTTVNWebContext())
                {
                    //Lấy danh sách data
                    if (type == (int)Const.TypeThumb.NEWS)
                    {
                        var listData = db.News.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in listData)
                        {
                            try
                            {

                                string img = item.Image;
                                var ext = img.Substring(img.LastIndexOf('.'));
                                var name = img.Substring(0, img.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                string fullPath = Path.Combine(newPath, img);

                                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(width, image, thumbPath, extension);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    else if (type == (int)Const.TypeThumb.PRODUCT)
                    {
                        var listData = db.Product.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in listData)
                        {
                            try
                            {

                                string img = item.Image;
                                var ext = img.Substring(img.LastIndexOf('.'));
                                var name = img.Substring(0, img.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                string fullPath = Path.Combine(newPath, img);

                                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(width, image, thumbPath, extension);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    else if (type == (int)Const.TypeThumb.SLIDE)
                    {
                        var listData = db.Slide.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in listData)
                        {
                            try
                            {

                                string img = item.Image;
                                var ext = img.Substring(img.LastIndexOf('.'));
                                var name = img.Substring(0, img.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                string fullPath = Path.Combine(newPath, img);

                                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(width, image, thumbPath, extension);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    else if (type == (int)Const.TypeThumb.ICON)
                    {
                        var listData = db.Category.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in listData)
                        {
                            try
                            {

                                string img = item.Icon;
                                var ext = img.Substring(img.LastIndexOf('.'));
                                var name = img.Substring(0, img.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                string fullPath = Path.Combine(newPath, img);

                                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(width, image, thumbPath, extension);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    else if (type == (int)Const.TypeThumb.CATEDOGY)
                    {
                        var listData = db.Category.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in listData)
                        {
                            try
                            {

                                string img = item.Image;
                                var ext = img.Substring(img.LastIndexOf('.'));
                                var name = img.Substring(0, img.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                string fullPath = Path.Combine(newPath, img);

                                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(width, image, thumbPath, extension);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    
                }

                def.meta = new Meta(200, "Success");
                //def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [AllowAnonymous]
        [HttpPost("genThumbImageNews")]
        public IActionResult GenThumbImageNews(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;

                string folderName = _configuration["AppSettings:rootUploadsThumbs"];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //DateTime now = DateTime.Now;
                //string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; 
                //Lấy danh sách tạo ảnh thumb
                using (var db = new CNTTVNWebContext())
                {
                    //Lấy danh sách bài viết
                    var listNews = db.News.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                    var listThumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == (int)Const.TypeThumb.NEWS).ToList();
                    foreach (var item in listNews)
                    {
                        try
                        {

                            string img = item.Image;
                            var ext = img.Substring(img.LastIndexOf('.'));
                            var name = img.Substring(0, img.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            string fullPath = Path.Combine(newPath, img);

                            
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\1_20190116143335095_20190214094643949.jpg";
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\qua_trinh_thi_cong-ngoc-thuy4_20190801082956023.jpg";
                            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                //postedFile.CopyTo(stream);
                                foreach (var itemT in listThumb)
                                {
                                    try
                                    {
                                        //Image image = Image.FromFile(fullPath);
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + itemT.Width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(itemT.Width, image, thumbPath, extension);
                                    }
                                    catch(Exception e) {
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }

                def.meta = new Meta(200, "Success");
                //def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [AllowAnonymous]
        [HttpPost("genThumbImageProduct")]
        public IActionResult GenThumbImageProduct(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;

                string folderName = _configuration["AppSettings:rootUploadsThumbs"];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //DateTime now = DateTime.Now;
                //string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; 
                //Lấy danh sách tạo ảnh thumb
                using (var db = new CNTTVNWebContext())
                {
                    //Lấy danh sách bài viết
                    var listData = db.Product.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                    var listThumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == (int)Const.TypeThumb.PRODUCT).ToList();
                    foreach (var item in listData)
                    {
                        try
                        {

                            string img = item.Image;
                            var ext = img.Substring(img.LastIndexOf('.'));
                            var name = img.Substring(0, img.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            string fullPath = Path.Combine(newPath, img);

                            //Image image = Image.FromFile(fullPath);
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\1_20190116143335095_20190214094643949.jpg";
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\qua_trinh_thi_cong-ngoc-thuy4_20190801082956023.jpg";
                            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                //postedFile.CopyTo(stream);
                                foreach (var itemT in listThumb)
                                {
                                    try
                                    {
                                        var image = Bitmap.FromStream(stream);
                                        string rel = newPath + "\\_thumb" + itemT.Width;
                                        if (!Directory.Exists(rel))
                                        {
                                            Directory.CreateDirectory(rel);
                                        }
                                        rel += "\\" + img;
                                        string thumbPath = Path.Combine(webRootPath, rel);

                                        createThumb(itemT.Width, image, thumbPath, extension);
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }

                def.meta = new Meta(200, "Success");
                //def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        [AllowAnonymous]
        [HttpPost("genThumbImageTest")]
        public IActionResult GenThumbImageTest(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;

                //string folderName = _configuration["AppSettings:rootUploadsThumbs"];
                string folderName = "uploads\\an tuong";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //DateTime now = DateTime.Now;
                //string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; 
                //Lấy danh sách tạo ảnh thumb
                using (var db = new CNTTVNWebContext())
                {
                    //Lấy danh sách bài viết
                    //var listNews = db.News.Where(e=>e.Status != (int)Const.Status.DELETED).ToList();
                    //var listThumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == 1).ToList();
                    //foreach (var item in listNews)
                    //{
                    DirectoryInfo drInfo = new DirectoryInfo(newPath);
                    FileInfo[] files = drInfo.GetFiles();
                    //doc ten cac file
                    foreach (FileInfo f in files)
                    {
                        //ReadFile(f.FullName, idfolder, k);
                        //}
                        try
                        {

                            string img = f.Name;
                            var ext = img.Substring(img.LastIndexOf('.'));
                            var name = img.Substring(0, img.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            string fullPath = Path.Combine(newPath, img);



                            //Image image = Image.FromFile(fullPath);
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\1_20190116143335095_20190214094643949.jpg";
                            //string url = @"D:\VCNTT\Project2019\AutionKoi\WebApp\IOITWebApp\wwwroot\uploads\thumbs\qua_trinh_thi_cong-ngoc-thuy4_20190801082956023.jpg";
                            using (var stream = new FileStream(fullPath, FileMode.Open))
                            {
                                //postedFile.CopyTo(stream);
                                //foreach (var itemT in listThumb)
                                //{
                                //try
                                //{
                                var image = Bitmap.FromStream(stream);

                                string rel = newPath + "\\_thumb" + 350;
                                if (!Directory.Exists(rel))
                                {
                                    Directory.CreateDirectory(rel);
                                }
                                rel += "\\" + img;
                                string thumbPath = Path.Combine(webRootPath, rel);

                                createThumb(350, image, thumbPath, extension);
                                //}
                                //catch { }
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }

                def.meta = new Meta(200, "Success");
                //def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

    }
}
