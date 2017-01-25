﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Controllers
{
    public class CaptchaController : Controller
    {
        public JsonResult ValidateCaptcha(string CaptchaValue)
        {
            bool b = IsValidCaptchaValue(CaptchaValue.ToUpper());
            if (!b) return Json(string.Empty, JsonRequestBehavior.AllowGet);
            else return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateInvisibleCaptcha(string CaptchaValue)
        {
            bool b = CaptchaValue == "";
            if (!b) return Json(string.Empty, JsonRequestBehavior.AllowGet);
            else return Json(true, JsonRequestBehavior.AllowGet);
        }

        private const int height = 30;
        private const int width = 400;
        private const int length = 4;
        private const string chars = "0123456789";

        public ActionResult Show()
        {
            var randomText = GenerateRandomText(length);
            var hash = ComputeMd5Hash(randomText + GetSalt());
            Session["CaptchaHash"] = hash;

            var rnd = new Random();
            var fonts = new[] { "Verdana", "Times New Roman" };
            float orientationAngle = rnd.Next(0, 359);

            var index0 = rnd.Next(0, fonts.Length);
            var familyName = fonts[index0];

            using (var bmpOut = new Bitmap(width, height))
            {
                var g = Graphics.FromImage(bmpOut);
                var gradientBrush = new LinearGradientBrush(new Rectangle(0, 0, width, height),
                                                            Color.DarkGray, Color.Black,
                                                            orientationAngle);                
                g.FillRectangle(gradientBrush, 0, 0, width, height);
                DrawRandomLines(ref g, width, height);
                g.DrawString(randomText, new Font(familyName, 18), new SolidBrush(Color.White), 150, 2);
                var ms = new MemoryStream();
                bmpOut.Save(ms, ImageFormat.Png);
                var bmpBytes = ms.GetBuffer();
                bmpOut.Dispose();
                ms.Close();

                return new FileContentResult(bmpBytes, "image/png");
            }
        }

        public static bool IsValidCaptchaValue(string captchaValue)
        {
            var expectedHash = System.Web.HttpContext.Current.Session["CaptchaHash"];
            var toCheck = captchaValue + GetSalt();
            var hash = ComputeMd5Hash(toCheck);
            return hash.Equals(expectedHash);
        }

        private static void DrawRandomLines(ref Graphics g, int width, int height)
        {
            var rnd = new Random();
            var pen = new Pen(Color.Gray);
            for (var i = 0; i < 10; i++)
            {
                g.DrawLine(pen, rnd.Next(0, width), rnd.Next(0, height),
                                rnd.Next(0, width), rnd.Next(0, height));
            }
        }

        private static string GetSalt()
        {
            return typeof(CaptchaController).Assembly.FullName;
        }

        private static string ComputeMd5Hash(string input)
        {
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(input);
            HashAlgorithm md5Hasher = MD5.Create();
            return BitConverter.ToString(md5Hasher.ComputeHash(bytes));
        }

        private static string GenerateRandomText(int textLength)
        {
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, textLength)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }
    }
}
