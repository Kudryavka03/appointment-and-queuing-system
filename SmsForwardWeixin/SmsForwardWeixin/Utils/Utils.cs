using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace SmsForwardWeixin.Utils
{
    public class Utils
    {
        public static void GenOtp()
        {
            while (true)
            {
                // if (Program.expiredToken1 == "") Program.expiredToken1 = System.Guid.NewGuid().ToString(); 应该不需要
                Program.expiredToken2 = Program.expiredToken1;
                Program.expiredToken1 = System.Guid.NewGuid().ToString();
                Console.WriteLine($"Gen New Otp Code: {Program.expiredToken1}");
                Thread.Sleep(60000);    // 60秒自动生成新Token
            }
        }
        public static string GenQrCode(string data)   // 确保data是只有数字的
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = 200,
                    Height = 200,
                }
            };
            var path = System.Guid.NewGuid().ToString("N");
            var img = writer.Write(data);
            using (MemoryStream ms = new MemoryStream())
            {
                img.Encode(ms, SkiaSharp.SKEncodedImageFormat.Jpeg, 100);
                File.WriteAllBytes($"ImageTest/{path}.jpg", ms.ToArray());
            }
            return path+".jpg";    // 返回生成路径
        }
    }
}

