using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Laba_3.Models;
using Laba_3.Utils;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Text.Json;
using System.Text;

namespace Laba_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        [Route("api/handler")]
        [Consumes("application/json")]
        public JsonResult HandlerData()
        {
            ResponseClient responseClient = new ResponseClient();
            try
            {
                byte[] buffer = new byte[2000];

                StringBuilder builder = new StringBuilder();
                while (true)
                {
                    var bytesRemaining = Request.Body.Read(buffer, offset: 0, buffer.Length);
                    if (bytesRemaining == 0)
                    {
                        break;
                    }

                    var encodedString = Encoding.UTF8.GetString(buffer, 0, bytesRemaining);
                    builder.Append(encodedString);
                }

                DataJson dataJson = JsonSerializer.Deserialize<DataJson>(builder.ToString());

                if (String.IsNullOrEmpty(dataJson.DataText) && String.IsNullOrEmpty(dataJson.DataText))
                {
                    responseClient.Errors.Add("Input data is empty");
                    responseClient.Errors.Add("File data is empty");
                    return Json(JsonSerializer.Serialize(responseClient));
                }

                dataJson.Key = String.IsNullOrEmpty(dataJson.Key) ? "Скорпион" : dataJson.Key;
                responseClient.Key = dataJson.Key;

                if (!String.IsNullOrEmpty(dataJson.DataText))
                {
                    responseClient.DataTextOld = dataJson.DataText;
                    responseClient.DataTextNew = VigenerСipher.GetDataCipher(dataJson.FlagAction, dataJson.DataText, dataJson.Key);
                }
                if (!String.IsNullOrEmpty(dataJson.DataFile))
                {
                    responseClient.DataTextOld = dataJson.DataText;
                    dataJson.DataText = VigenerСipher.GetDataCipher(dataJson.FlagAction, dataJson.DataFile, dataJson.Key);
                }

                return Json(JsonSerializer.Serialize(responseClient));
            }
            catch (Exception)
            {
                responseClient.Errors.Add("Data is not valid");
                return Json(JsonSerializer.Serialize(responseClient));
            }

        }


        [HttpPost]
        [Route("download-file")]
        public FileContentResult GenerateFile()
        {
            var mimeType = System.Net.Mime.MediaTypeNames.Application.Octet;
            string file_name = "data.docx";

            if (!String.IsNullOrEmpty(Request.Form["data"]))
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {

                        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                        {
                            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                            Body body = mainPart.Document.AppendChild(new Body());
                            Paragraph paragraph = body.AppendChild(new Paragraph());
                            Run run = paragraph.AppendChild(new Run());
                            run.AppendChild(new Text(Request.Form["data"].ToString()));
                        }
                        return File(memoryStream.ToArray(), mimeType, file_name);
                    }
                }
                catch (Exception)
                {
                    return File(new byte[0], mimeType, file_name);
                }
            } else
            {
                return File(new byte[0], mimeType, file_name);
            }
        }
        
        public IActionResult Index(IFormFile uploadedFile)
        {
            if (Request.Method.Equals("POST"))
            {
                Dictionary<string, string> errors = new Dictionary<string, string>();
                bool flagKey = !String.IsNullOrEmpty(Request.Form["key"]) && Regex.IsMatch(Request.Form["key"], "^[а-яА-ЯёЁ]+");
                bool flagActionWithData = false;
                bool flagDataText = !String.IsNullOrEmpty(Request.Form["data-text"]);
                bool flagDataFile = uploadedFile != null && uploadedFile.Length > 0;

                ViewData["InitialDataText"] = Request.Form["data-text"];
                ViewData["InitialKey"] = Request.Form["data -text"];

                try
                {
                    flagActionWithData = bool.Parse(Request.Form["flag-action"]);
                }
                catch (Exception)
                {
                    errors.Add("flagAction", "Не понятна операция с данынми");
                }

                if (!flagDataText && !flagDataFile)
                {
                    errors.Add("flagDataText", "Текст не задан");
                    errors.Add("flagDataFile", "Файл не задан ");
                }

                if (errors.Count == 0)
                {
                    ViewData["Status"] = "Успешно";
                    string key = flagKey ? Request.Form["key"].ToString() : "Скорпион";
                    ViewData["Key"] = key;

                    if (flagDataText)
                    {
                        ViewData["DataText"] = VigenerСipher.GetDataCipher(flagActionWithData, Request.Form["data-text"], key);
                    }
                    if (flagDataFile)
                    {
                        string textFile = ReadFileDocx(uploadedFile.OpenReadStream());
                        ViewData["InitialDataFile"] = textFile;
                        ViewData["DataFile"] = VigenerСipher.GetDataCipher(flagActionWithData, textFile, key);
                    }
                } 
                else
                {
                    ViewData["Status"] = "Ошибка";
                    ViewData["Errors"] = errors;
                }
                return View("Index", ViewBag);
            }
            else
            {
                return View();
            }
        }

        private static string  ReadFileDocx(Stream stream)
        {
            try
            {
                using (WordprocessingDocument filestream = WordprocessingDocument.Open(stream, false))
                {
                    return filestream.MainDocumentPart.Document.Body.InnerText;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
