using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPWrapper;

namespace WacVisitor
{
    public class clsImage
    {
        public static Image Base64ToImage(string base64String)
        {
            if (base64String == "")
                return null;

            byte[] byteArrayIn = new byte[0];
            try
            {
                byteArrayIn = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(byteArrayIn))
                {
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }

        }
        public static string ImageToBase64(string path)
        {
            try
            {
                using (Image image = Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        image.Dispose();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        public static string CONVERT_WEBP_TO_PNG(string webp, string png)
        {
            string base64png = "";
            byte[] rawWebP = File.ReadAllBytes(webp);
            WebPDecoderOptions decoderOptions = new WebPDecoderOptions();
            decoderOptions.use_threads = 1;     //Use multhreading
            decoderOptions.flip = 0;            //Flip the image
            using (WebP webpp = new WebP())
            {
                Bitmap bmp = webpp.Decode(rawWebP, decoderOptions);
                System.IO.MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                base64png = Convert.ToBase64String(byteImage); // Get Base64
            }

            if (System.IO.File.Exists(webp))
                System.IO.File.Delete(webp);

            return base64png;

            #region backup
            //var dwebp = @"dwebp.exe";
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = dwebp;
            //startInfo.Arguments = webp + " -o " + png;

            //startInfo.CreateNoWindow = true; // I've read some articles this is required for launching exe in Web environment
            //startInfo.UseShellExecute = false;
            //try
            //{
            //    using (Process exeProcess = Process.Start(startInfo))
            //    {
            //        exeProcess.WaitForExit();
            //    }
            //}
            //catch (Exception ex)
            //{                
            //    Console.WriteLine(ex.Message);
            //    Console.ReadLine();
            //}

            //string base64png = ImageToBase64(png);

            //if (System.IO.File.Exists(webp))
            //    System.IO.File.Delete(webp);
            //if (System.IO.File.Exists(png))
            //    System.IO.File.Delete(png);

            //return base64png;
            #endregion
        }
    }
}
