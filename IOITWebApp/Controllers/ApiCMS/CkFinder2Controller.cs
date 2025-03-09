using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOITWebApp.Models;

namespace IOITWebApp.Controllers.ApiCMS
{
    [Route("assets/ckfinder2/core/connector/aspx/connector.aspx")]
    [ApiController]
    public class CkFinder2Controller : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public CkFinder2Controller(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpGet]
        [Produces("application/xml")]
        public async Task<IActionResult> Connector([FromQuery] Parameters paging)
        {
            if (paging.command == "Init")
            {
                XElement connector =
                new XElement("Connector",
                    new XElement("Error",
                        new XAttribute("number", 0)),
                    new XElement("ConnectorInfo",
                        new XAttribute("enabled", true),
                        new XAttribute("imgWidth", 980),
                        new XAttribute("imgHeight", 980),
                        new XAttribute("s", ""),
                        new XAttribute("c", ""),
                        new XAttribute("thumbsEnabled", true),
                        new XAttribute("thumbsUrl", "~/Uploads/_thumbs/"),
                        new XAttribute("thumbsDirectAccess", false),
                        new XAttribute("plugins", "imageresize")
                        ),
                    new XElement("ResourceTypes",
                        new XElement("ResourceType",
                            new XAttribute("name", "Images"),
                            new XAttribute("url", "/Uploads/images/"),
                            new XAttribute("allowedExtensions", "bmp,gif,jpeg,jpg,png"),
                            new XAttribute("deniedExtensions", ""),
                            new XAttribute("hash", "cb76eba261536b72"),
                            new XAttribute("hasChildren", true),
                            new XAttribute("acl", "255")
                            )
                        ),
                    new XElement("PluginsInfo",
                        new XElement("imageresize",
                            new XAttribute("smallThumb", "90x90"),
                            new XAttribute("mediumThumb", "120x120"),
                            new XAttribute("largeThumb", "180x180")
                        )
                    )
                );
                return Ok(connector);
            }
            else if (paging.command == "GetFolders")
            {
                XElement connector =
                new XElement("Connector",
                    new XAttribute("resourceType", paging.type),
                    new XElement("Error",
                        new XAttribute("number", 0)),
                    new XElement("CurrentFolder",
                        new XAttribute("path", paging.currentFolder),
                        new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                        new XAttribute("acl", 255)
                        )
                );

                XElement folders = new XElement("Folders");

                //Lấy danh sách folder
                string folderName = "Uploads/" + paging.type + paging.currentFolder;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                var directories = Directory.GetDirectories(path);

                foreach (var item in directories)
                {
                    XElement folder = new XElement("Folder");
                    XAttribute xAttributeName = new XAttribute("name", item.Split('/').LastOrDefault());
                    folder.Add(xAttributeName);
                    var directoriesChild = Directory.GetDirectories(item);
                    XAttribute xAttributeHasChildren = new XAttribute("hasChildren", directoriesChild.Count() > 0 ? true : false);
                    folder.Add(xAttributeHasChildren);
                    XAttribute xAttributeAcl = new XAttribute("acl", "255");
                    folder.Add(xAttributeAcl);
                    folders.Add(folder);
                }
                connector.Add(folders);
                return Ok(connector);
            }
            else if (paging.command == "GetFiles")
            {
                XElement connector =
                new XElement("Connector",
                    new XAttribute("resourceType", paging.type),
                    new XElement("Error",
                        new XAttribute("number", 0)),
                    new XElement("CurrentFolder",
                        new XAttribute("path", paging.currentFolder),
                        new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                        new XAttribute("acl", 255)
                        )
                );

                XElement files = new XElement("Files");

                //Lấy đường dẫn folder
                string folderName = "Uploads/" + paging.type + paging.currentFolder;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                DirectoryInfo d = new DirectoryInfo(path);
                FileInfo[] Files = d.GetFiles(); //Getting Text files

                foreach (FileInfo item in Files)
                {
                    XElement file = new XElement("File");
                    XAttribute xAttributeName = new XAttribute("name", item.Name);
                    file.Add(xAttributeName);
                    string date = "";
                    date += item.CreationTime.Year;
                    date += item.CreationTime.Month < 10 ? ("0" + item.CreationTime.Month) : item.CreationTime.Month + "";
                    date += item.CreationTime.Day < 10 ? ("0" + item.CreationTime.Day) : item.CreationTime.Day + "";
                    date += item.CreationTime.Hour < 10 ? ("0" + item.CreationTime.Hour) : item.CreationTime.Hour + "";
                    date += item.CreationTime.Minute < 10 ? ("0" + item.CreationTime.Minute) : item.CreationTime.Minute + "";
                    date += item.CreationTime.Second < 10 ? ("0" + item.CreationTime.Second) : item.CreationTime.Second + "";
                    XAttribute xAttributeDate = new XAttribute("date", date);
                    file.Add(xAttributeDate);
                    XAttribute xAttributeSize = new XAttribute("size", (int)item.Length / 1024);
                    file.Add(xAttributeSize);
                    files.Add(file);
                }

                connector.Add(files);

                return Ok(connector);
            }
            else if (paging.command == "Thumbnail")
            {
                string folderName = "Uploads/_thumbs/" + paging.type + paging.currentFolder;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                var file = await Thumbnail(paging, path);
                return file;
            }
            else if (paging.command == "DownloadFile")
            {
                string folderName = "Uploads/" + paging.type + paging.currentFolder;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                var file = await Thumbnail(paging, path);
                return file;
            }
            else if (paging.command == "ImageResizeInfo")
            {
                string folderName = "Uploads/" + paging.type + paging.currentFolder;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                string fileName = path + paging.fileName;
                Image img = Image.FromFile(fileName);
                ImageFormat format = img.RawFormat;

                XElement connector =
                  new XElement("Connector",
                      new XAttribute("resourceType", paging.type),
                      new XElement("Error",
                          new XAttribute("number", 0)),
                      new XElement("CurrentFolder",
                          new XAttribute("path", paging.currentFolder),
                          new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                          new XAttribute("acl", 255)
                          ),
                      new XElement("ImageInfo",
                          new XAttribute("width", img.Width),
                          new XAttribute("height", img.Height)
                     )
                );

                return Ok(connector);
            }
            //else if (paging.command == "ImagePreview")
            //{
            //    string folderName = "Uploads/" + paging.type + paging.currentFolder;
            //    string webRootPath = _hostingEnvironment.WebRootPath;
            //    string path = Path.Combine(webRootPath, folderName);
            //    var file = Thumbnail(paging, path);
            //    return file;
            //}
            return null;
        }

        [HttpPost]
        [Produces("application/xml")]
        public ActionResult ConnectorPost([FromQuery] Parameters paging)
        {
            if (paging.command == "FileUpload")
            {
                var httpRequest = Request.Form.Files;
                foreach (var file in httpRequest)
                {
                    string folderName = "Uploads/" + paging.type + paging.currentFolder;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string rel = "Uploads/_thumbs/" + paging.type + paging.currentFolder + file.FileName;
                    string fullPath = Path.Combine(webRootPath, folderName);
                    fullPath += file.FileName;
                    if (file.Length > 0)
                    {
                        try
                        {
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                var image = Bitmap.FromStream(stream);
                                //resize image
                                string typeFile = "." + file.FileName.Split('.').LastOrDefault();
                                //int size = int.Parse(paging.size.Split('x').FirstOrDefault());
                                //createThumb(980, image, fullPath, typeFile);

                                string thumbPath = Path.Combine(webRootPath, rel);
                                //string typeFile = "." + file.FileName.Split('.').LastOrDefault();
                                createThumb(100, image, thumbPath, typeFile);

                                string result = "<script type=\"text/javascript\">window.parent.OnUploadCompleted('" + file.FileName + "','') ;</script>";
                                return new ContentResult
                                {
                                    ContentType = "text/html",
                                    StatusCode = (int)HttpStatusCode.OK,
                                    Content = result
                                };

                                //return Content(result);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            else if (paging.command == "CreateFolder")
            {
                //create folder
                string folderName = "Uploads/" + paging.type + paging.currentFolder + paging.newFolderName;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //create folder thumbs
                string folderChildName = "Uploads/_thumbs/" + paging.type + paging.currentFolder + paging.newFolderName;
                string pathThumb = Path.Combine(webRootPath, folderChildName);
                if (!Directory.Exists(pathThumb))
                {
                    Directory.CreateDirectory(pathThumb);
                }

                XElement connector =
                new XElement("Connector",
                    new XAttribute("resourceType", paging.type),
                    new XElement("Error",
                        new XAttribute("number", 0)),
                    new XElement("CurrentFolder",
                        new XAttribute("path", paging.currentFolder),
                        new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                        new XAttribute("acl", 255)
                        ),
                    new XElement("NewFolder",
                        new XAttribute("name", paging.newFolderName)
                    )
                );

                return Ok(connector);
            }
            else if (paging.command == "DeleteFile")
            {
                try
                {
                    //delete file
                    string folderName = "Uploads/" + paging.type + paging.currentFolder + paging.fileName;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = Path.Combine(webRootPath, folderName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception ex) { }
                XElement connector =
                  new XElement("Connector",
                      new XAttribute("resourceType", paging.type),
                      new XElement("Error",
                          new XAttribute("number", 0)),
                      new XElement("CurrentFolder",
                          new XAttribute("path", paging.currentFolder),
                          new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                          new XAttribute("acl", 255)
                          ),
                      new XElement("DeletedFile",
                          new XAttribute("name", paging.fileName)
                          )
                  );

                return Ok(connector);
            }
            else if (paging.command == "DeleteFolder")
            {
                try
                {
                    //delete folder
                    string folderName = "Uploads/" + paging.type + paging.currentFolder;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = Path.Combine(webRootPath, folderName);
                    if (Directory.Exists(path))
                    {
                        //delete file
                        DirectoryInfo d = new DirectoryInfo(path);
                        FileInfo[] Files = d.GetFiles(); //Getting Text files
                        foreach (FileInfo item in Files)
                        {
                            System.IO.File.Delete(item.FullName);
                        }

                        Directory.Delete(path);
                    }

                    //delete folder thumbs
                    string folderChildName = "Uploads/_thumbs/" + paging.type + paging.currentFolder + paging.newFolderName;
                    string pathThumb = Path.Combine(webRootPath, folderChildName);
                    if (Directory.Exists(pathThumb))
                    {
                        //delete file
                        DirectoryInfo d = new DirectoryInfo(pathThumb);
                        FileInfo[] Files = d.GetFiles(); //Getting Text files
                        foreach (FileInfo item in Files)
                        {
                            System.IO.File.Delete(item.FullName);
                        }
                        Directory.Delete(pathThumb);
                    }
                }
                catch (Exception ex) { }
                XElement connector =
                   new XElement("Connector",
                       new XAttribute("resourceType", paging.type),
                       new XElement("Error",
                           new XAttribute("number", 0)),
                       new XElement("CurrentFolder",
                           new XAttribute("path", paging.currentFolder),
                           new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                           new XAttribute("acl", 255)
                           )
                   );

                return Ok(connector);
            }
            else if (paging.command == "ImageResize")
            {
                try
                {
                    string width = Request.Form["width"].ToString();
                    string height = Request.Form["height"].ToString();
                    string fileName = Request.Form["fileName"].ToString();
                    string small = Request.Form["small"].ToString();
                    string medium = Request.Form["medium"].ToString();
                    string large = Request.Form["large"].ToString();
                    //FormData jsonData = JsonConvert.DeserializeObject<FormData>(data);

                    string folderName = "Uploads/" + paging.type + paging.currentFolder + fileName;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string fullPath = Path.Combine(webRootPath, folderName);
                    Image image = Image.FromFile(fullPath);
                    //string typeFile = "." + paging.fileName.Split('.').LastOrDefault();
                    string imageName = fileName.Substring(0, fileName.LastIndexOf('.'));
                    string typeFile = fileName.Substring(fileName.LastIndexOf('.'));
                    int size = 0; //int.Parse(paging.size.Split('x').FirstOrDefault());
                    if (width != "")
                    {
                        size = int.Parse(width);
                        imageName += "_" + width + "x" + height + typeFile;
                    }
                    else if (small == "1")
                    {
                        size = 90;
                        imageName += "_small" + typeFile;
                    }
                    else if (medium == "1")
                    {
                        size = 120;
                        imageName += "_medium" + typeFile;
                    }
                    else if (large == "1")
                    {
                        size = 180;
                        imageName += "_large" + typeFile;
                    }
                    string resizeName = "Uploads/" + paging.type + paging.currentFolder + imageName;
                    string resizePath = Path.Combine(webRootPath, resizeName);
                    createThumb(size, image, resizePath, typeFile);

                    string thumbName = "Uploads/thumbs/" + paging.type + paging.currentFolder + imageName;
                    string thumbPath = Path.Combine(webRootPath, thumbName);
                    createThumb(100, image, thumbPath, typeFile);
                }
                catch (Exception ex) { }

                XElement connector =
                   new XElement("Connector",
                       new XAttribute("resourceType", paging.type),
                       new XElement("Error",
                           new XAttribute("number", 0)),
                       new XElement("CurrentFolder",
                           new XAttribute("path", paging.currentFolder),
                           new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                           new XAttribute("acl", 255)
                           )
                   );
                return Ok(connector);
            }
            else if (paging.command == "RenameFile")
            {
                try
                {
                    //rename file
                    string folderName = "Uploads/" + paging.type + paging.currentFolder;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = Path.Combine(webRootPath, folderName);
                    string oldName = path + paging.fileName;
                    string newName = path + paging.newFileName;
                    System.IO.File.Move(oldName, newName);
                    //rename file thumb
                    string folderNameThumb = "Uploads/_thumbs/" + paging.type + paging.currentFolder;
                    string pathThumb = Path.Combine(webRootPath, folderNameThumb);
                    string oldNameThumb = pathThumb + paging.fileName;
                    string newNameThumb = pathThumb + paging.newFileName;
                    System.IO.File.Move(oldNameThumb, newNameThumb);
                }
                catch (Exception ex) { }
                XElement connector =
                  new XElement("Connector",
                      new XAttribute("resourceType", paging.type),
                      new XElement("Error",
                          new XAttribute("number", 0)),
                      new XElement("CurrentFolder",
                          new XAttribute("path", paging.currentFolder),
                          new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                          new XAttribute("acl", 255)
                          ),
                      new XElement("RenamedFile",
                          new XAttribute("name", paging.fileName),
                          new XAttribute("newName", paging.newFileName)
                      )
                  );


                return Ok(connector);

            }
            else if (paging.command == "RenameFolder")
            {
                try
                {
                    string folderName = "Uploads/" + paging.type;
                    string folderNameThumb = "Uploads/_thumbs/" + paging.type;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = Path.Combine(webRootPath, folderName);
                    string pathThumb = Path.Combine(webRootPath, folderNameThumb);

                    string[] folder = paging.currentFolder.Split('/');
                    string oldName = path + paging.currentFolder;
                    string oldNameThumb = pathThumb + paging.currentFolder;
                    string newName = path;
                    string newNameThumb = pathThumb;
                    if (folder.Length >= 2)
                    {
                        for (int i = 0; i < folder.Length - 2; i++)
                        {
                            newName += folder[i] + "/";
                            newNameThumb += folder[i] + "/";
                        }
                    }
                    newName += paging.newFolderName;
                    newNameThumb += paging.newFolderName;

                    Directory.Move(oldName, newName);
                    //rename thumb
                    Directory.Move(oldNameThumb, newNameThumb);
                }
                catch (Exception ex)
                {

                }
                XElement connector =
                   new XElement("Connector",
                       new XAttribute("resourceType", paging.type),
                       new XElement("Error",
                           new XAttribute("number", 0)),
                       new XElement("CurrentFolder",
                           new XAttribute("path", paging.currentFolder),
                           new XAttribute("url", "/Uploads/" + paging.type + paging.currentFolder),
                           new XAttribute("acl", 255)
                           ),
                       new XElement("RenamedFolder",
                           new XAttribute("newName", paging.newFolderName),
                           new XAttribute("newPath", "/" + paging.newFolderName + "/"),
                           new XAttribute("newUrl", "/Uploads/" + paging.type + paging.newFolderName + "/")
                       )
                   );
                return Ok(connector);
            }

            return Ok();
        }

        [HttpGet("{path}")]
        public async Task<FileResult> Thumbnail([FromQuery] Parameters paging, [FromRoute] string path)
        {
            string filePath = path + "/" + paging.fileName;

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    await file.ReadAsync(bytes, 0, (int)file.Length);
                    await ms.WriteAsync(bytes, 0, (int)file.Length);
                    return File(bytes, System.Net.Mime.MediaTypeNames.Image.Jpeg, paging.fileName);
                }
            }
        }

        public static void createThumb(int thumbWidth, Image image, string thumbPath, string file_type)
        {
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
    }
}