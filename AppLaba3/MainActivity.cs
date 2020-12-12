using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;


using AppLaba3.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace AppLaba3
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            FindViewById<Button>(Resource.Id.btnDecrypt).Click += delegate {
                DecryptData(true);
            };

            FindViewById<Button>(Resource.Id.btnEncrypt).Click += delegate {
                DecryptData(false);
            };

            FindViewById<Button>(Resource.Id.btnSelectFile).Click += delegate { BtnSelectFileOnClick(); };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private static string ReadFileDocx(string path)
        {
            string data = "";
            if (File.Exists(path))
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(path)))
                    {
                        using (WordprocessingDocument filestream = WordprocessingDocument.Open(memoryStream, false))
                        {
                            data = filestream.MainDocumentPart.Document.Body.InnerText;
                        }
                    }
                }
                catch (Exception)
                {
                    data = "";
                }
            }
            return data;
        }

        private void DecryptData(bool flagAction) 
        {
            EditText keyEditText = FindViewById<EditText>(Resource.Id.edittextKey);
            EditText dataEditText = FindViewById<EditText>(Resource.Id.editTextData);
            TextView fileTextView = FindViewById<TextView>(Resource.Id.selectFile);

            string dataFile = "";
            if (!String.IsNullOrEmpty(fileTextView.Hint))
            {
                dataFile = fileTextView.Hint;//ReadFileDocx(fileTextView.Hint);
            }
            var intent = new Intent(this, typeof(ResultActivity));

            string data = JsonConvert.SerializeObject(new HandleData(keyEditText.Text, flagAction, dataEditText.Text, dataFile));

            intent.PutExtra("data", data);
            StartActivity(intent);
        }

        private async void BtnSelectFileOnClick()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } }
                });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Выбранный файл"
            });

            if (pickResult != null)
            {
                TextView fileTextView = FindViewById<TextView>(Resource.Id.selectFile);
                fileTextView.Text = pickResult.FileName;
                try
                {
                    using (WordprocessingDocument filestream = WordprocessingDocument.Open(await pickResult.OpenReadAsync(), false))
                    {
                        fileTextView.Hint = filestream.MainDocumentPart.Document.Body.InnerText;
                    }
                }
                catch (Exception)
                {
                    fileTextView.Hint = "Ошибка";
                }
            }
        }
    }
}