using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AppLaba3.Utils;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace AppLaba3
{
    [Activity(Label = "@string/app_name")]
    public class ResultActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.result);

            TextView keyTextView = FindViewById<TextView>(Resource.Id.key);
            TextView enterDataTextView = FindViewById<TextView>(Resource.Id.enterData);
            TextView fileDataTextView = FindViewById<TextView>(Resource.Id.fileData);

            TextView enterDataUpdateTextView = FindViewById<TextView>(Resource.Id.enterDataUpdate);
            TextView fileDataUpdateTextView = FindViewById<TextView>(Resource.Id.fileDataUpdate);

            HandleData data = JsonConvert.DeserializeObject<HandleData>(Intent.Extras.Get("data").ToString());
            if (String.IsNullOrEmpty(data.Key) && !Regex.IsMatch(data.Key, "^[а-яА-ЯёЁ]+"))
            {
                data.Key = "Скорпион";
            }

            keyTextView.Text = data.Key;
            enterDataTextView.Text = data.DataText;
            fileDataTextView.Text = data.DataFile;

            enterDataUpdateTextView.Text = VigenerСipher.GetDataCipher(data.FlagAction, data.DataText, data.Key);
            fileDataUpdateTextView.Text = VigenerСipher.GetDataCipher(data.FlagAction, data.DataFile, data.Key);

            Button btnBack = FindViewById<Button>(Resource.Id.backOnMainPage);
            Button btnSave = FindViewById<Button>(Resource.Id.btnSaveData);
            btnBack.Click += delegate
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            btnSave.Click += delegate
            {
                string result = "Введенные данные:\n" + 
                    enterDataUpdateTextView.Text + 
                    "\nДанные Файла:\n" + 
                    fileDataUpdateTextView.Text;
                string path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath,
                                            "data.docx");
                Context context = Application.Context;
                string text = "Сохранено!";
                ToastLength duration = ToastLength.Short;

                var toast = Toast.MakeText(context, text, duration);
                try
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Create))
                    {
                        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(fileStream, WordprocessingDocumentType.Document, true))
                        {
                            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                            Body body = mainPart.Document.AppendChild(new Body());
                            Paragraph paragraph = body.AppendChild(new Paragraph());
                            Run run = paragraph.AppendChild(new Run());
                            run.AppendChild(new Text(result.ToString()));
                        }
                    }
                }
                catch (Exception)
                {
                    text = "Ошибка сохранения. Проверьте доступ для сохранения";
                }
                toast.Show();
            };


        }
    }
}