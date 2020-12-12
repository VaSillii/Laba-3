using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Laba_3;
using System.Net.Http;
using Laba_3.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Net;
using DocumentFormat.OpenXml.Packaging;
using Laba_3.Utils;
using System.Text;
using System.Text.Json;

namespace UnitTestLaba3
{

    [TestClass]
    public class HomeControllerTest
    {
        public static HomeController controller;

        public HomeControllerTest()
        {
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Custom-Header"] = "88-test-tcb";
            httpContext.Request.Method = "GET";
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";

            var mock = new Mock<ILogger<HomeController>>();
            ILogger<HomeController> logger = mock.Object;

            logger = Mock.Of<ILogger<HomeController>>();

            controller = new HomeController(logger)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,

                }
            };
        }

        [TestMethod]
        public void IndexGet()
        {
            controller.ControllerContext.HttpContext.Request.Method = "GET";
            ViewResult result = controller.Index(null) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexPOST()
        {
            controller.ControllerContext.HttpContext.Request.Method = "POST";
            ViewResult result = controller.Index(null) as ViewResult;
            Assert.AreNotEqual(((Dictionary<string, string>)result.ViewData["Errors"]).Count, 0);
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void IndexPOSTDataText()
        {
            controller.ControllerContext.HttpContext.Request.Method = "POST";
            var formCol = new FormCollection(new Dictionary<string,
                Microsoft.Extensions.Primitives.StringValues>
                {
                    { "flag-action", "true" },
                    { "key", "скорпион" },
                    { "data-text", "бщцфаирщри" }
                }
            );

            controller.ControllerContext.HttpContext.Request.Form = formCol;
            ViewResult result = controller.Index(null) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewData["DataText"], "поздравляю");
        }


        [TestMethod]
        public void GenerateFilePostTest()
        {
            controller.ControllerContext.HttpContext.Request.Method = "POST";
            var formCol = new FormCollection(new Dictionary<string,
                Microsoft.Extensions.Primitives.StringValues>
                {
                    { "data", "Тестовая строка в файле!" },
                }
            );
            controller.ControllerContext.HttpContext.Request.Form = formCol;

            FileContentResult result = controller.GenerateFile() as FileContentResult;

            string dataResult = "";
            using (MemoryStream memoryStream = new MemoryStream(result.FileContents)) 
            { 
                using (WordprocessingDocument filestream = WordprocessingDocument.Open(memoryStream, false))
                {
                    dataResult =  filestream.MainDocumentPart.Document.Body.InnerText;
                }
            }

            Assert.AreNotEqual(result.FileContents.Length, 0);
            Assert.AreEqual(result.FileDownloadName, "data.docx");
            Assert.AreEqual(dataResult, "Тестовая строка в файле!");
        }

        [TestMethod]
        public void GenerateFileEmptyPostTest()
        {
            controller.ControllerContext.HttpContext.Request.Method = "POST";
            var formCol = new FormCollection(new Dictionary<string,
                Microsoft.Extensions.Primitives.StringValues>
                {
                    { "data", "" },
                }
            );
            controller.ControllerContext.HttpContext.Request.Form = formCol;

            FileContentResult result = controller.GenerateFile() as FileContentResult;
            Assert.AreEqual(result.FileContents.Length, 0);
            Assert.AreEqual(result.FileDownloadName, "data.docx");
        }

        [TestMethod]
        public void DecryptTest()
        {
            Assert.AreEqual(VigenerСipher.GetDataCipher(true, "дпаг", "скорпион"), "тест");
        }
        [TestMethod]
        public void EncryptTest()
        {
            Assert.AreEqual(VigenerСipher.GetDataCipher(false, "тест", "скорпион"), "дпаг");
        }

        [TestMethod]
        public void APiTest()
        {
            DataJson dataJson = new DataJson();
            dataJson.Key = "скорпион";
            dataJson.FlagAction = false;
            dataJson.DataText = "тест";
            dataJson.DataFile = "";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Custom-Header"] = "88-test-tcb";
            httpContext.Request.Method = "POST";
            httpContext.Request.ContentType = "application/json";
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataJson))))
            {
                controller.ControllerContext.HttpContext.Request.Body = memoryStream;
                controller.ControllerContext.HttpContext.Request.ContentLength = memoryStream.Length;
            }

            var mock = new Mock<ILogger<HomeController>>();
            ILogger<HomeController> logger = mock.Object;

            logger = Mock.Of<ILogger<HomeController>>();

            controller = new HomeController(logger)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            JsonResult result = controller.HandlerData() as JsonResult;
            Assert.IsNotNull(result);
        }
    }
}
