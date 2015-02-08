using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace _2_1_galleriet
{
    public class Gallery
    {
        // Fields

        const int MAX_IMAGE_WIDTH = 600;
        const int MAX_IMAGE_HEIGHT = 500;
        public const string IMAGE_PATH = "Content/upload/img";
        const int THUMBNAIL_WIDTH = 300;
        const int THUMBNAIL_HEIGHT = 150;
        const string THUMBNAIL_PATH = "Content/upload/img/tn";
        const string THUMBNAIL_PRE_EXTENSION = "tn_";
        static private IReadOnlyCollection<string> ApprovedExtensionsCollection;
        static readonly Regex ApprovedExtensionsRegex;
        static readonly string UploadImagePath;
        static readonly string UploadThumbnailPath;
        static readonly Regex SanitizePathRegex;

        // Constructors
        static Gallery()
        {
            /* Konstruktorn är statisk och dess uppgift är att initiera de statiska ”readonly” fälten */

            // Set up approved extensions Collection (readonly)
            ApprovedExtensionsCollection = Array.AsReadOnly(new string[] { "gif", "jpg", "png" });

            // Create regular extpression for sanitizing path strings.
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            SanitizePathRegex = new Regex(string.Format("[{0}]", Regex.Escape(invalidChars)));

            // Set image and thumbnail path
            UploadImagePath = HttpContext.Current.Server.MapPath(String.Format(@"~/{0}", IMAGE_PATH));
            UploadThumbnailPath = HttpContext.Current.Server.MapPath(String.Format(@"~/{0}", THUMBNAIL_PATH));

            // Set up approved extensions regex
            ApprovedExtensionsRegex = new Regex(@"^.*\.(" + string.Join("|", ApprovedExtensionsCollection) + ")$");
        }

        // Methods
        public IEnumerable<string> GetImageNames(bool getThumbnails = false)
        {
            char[] trimStartDot;
            IEnumerable<string> files;
            FileInfo fileInfo;
            List<string> returnFilesList = new List<string>();

            // Setup the dot char (.) to trim from extension.
            trimStartDot = new char[] { '.' };

            // Get all files from directory
            files = Directory.GetFiles(getThumbnails ? UploadThumbnailPath : UploadImagePath)

                // Select files with right extensions using linq
                .Where(f => ApprovedExtensionsCollection.Contains(Path.GetExtension(f).TrimStart(trimStartDot).ToLower()))
                // Sort ascending using linq
                .OrderBy(f => f);
            
            // Add right path for files
            foreach (var file in files)
            {
                fileInfo = new FileInfo(file);
                returnFilesList.Add(String.Format("{0}/{1}", (getThumbnails ? THUMBNAIL_PATH : IMAGE_PATH), fileInfo.Name));
            }

            // Return  value
            return returnFilesList;
        }

        public bool ImageExists(string name)
        {
            /*
                *  ImageExists är en statisk metod som returnerar true om en bild med angivet namn finns katalogen
                för uppladdade bilder; annars false.
                */

            if(!IsValidFileName(name))
            {
                throw new ArgumentException("The given argument is not a valid image filename");
            }

            return File.Exists(String.Format("{0}/{1}", UploadImagePath, name));
        }

        private bool IsValidImage (Image image)
        {
            bool returnValue = false;

            /*
             * IsValidImage returnerar true om den uppladdade filens innehåll verkligen är av typen gif, jpeg eller
                png. Om image refererar till ett System.Drawing.Image-objekt kan dess MIME-typ undersökas med
                image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid.
             */
            if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid ||
                image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid || 
                image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            {
                returnValue = true;
            }

            return returnValue;
        }

        private bool IsValidFileName(string fileName)
        {
            bool returnValue = false;

            // Check for bad given filename
            if (!SanitizePathRegex.IsMatch(fileName) && // No bad chars are matched
                ApprovedExtensionsRegex.IsMatch(Path.GetExtension(fileName).ToLower())) // File extension is valid
            {
                returnValue = true;
            }

            return returnValue;
        }

        public string SaveImage(Stream imageStream, string fileName)
        {
            System.IO.MemoryStream imageMemoryStream;
            System.Drawing.Image image, thumbnail;
            byte[] imageData;

            /*
             * SaveImage verifierar att filen är av rätt MIME-typ (annars kastas ett undantag), säkerställer att
                filnamnet är unik, sparar bilden samt skapar och sparar en tumnagelbild. Filnamnet bilden sparas
                under returneras. 
             */

            // Sanitize filename
            fileName = SanitizePathRegex.Replace(fileName, "");

            // Check that the filename is allowed
            if (!IsValidFileName(fileName))
            {
                throw new ArgumentException(String.Format("Endast bilder med följande filändelser är tillåtna: {0}", string.Join(", ", ApprovedExtensionsCollection)));
            }

            // Check that the image does not exist (also checks if file name is valid)
            if (ImageExists(fileName))
            {
                throw new ArgumentException("Det finns redan en bild med detta filnamn.");
            }

            // Read stream
            imageData = new byte[imageStream.Length];
            imageStream.Read(imageData, 0, imageData.Length);
            imageMemoryStream = new System.IO.MemoryStream(imageData);
            
            // Create image from stream
            image = Image.FromStream(imageMemoryStream);

            // Check if image is valid
            if (!IsValidImage(image))
            {
                throw new ArgumentException("Filen kunde inte tolkas som en giltig bild.");
            }

            
            // Resize and Save image
            resizeImageTo(image, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT).Save(string.Format("{0}/{1}", UploadImagePath, fileName));

            // Create thumbnail from image and Save it.
            resizeImageTo(image, THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT).Save(string.Format("{0}/{1}{2}", UploadThumbnailPath, THUMBNAIL_PRE_EXTENSION, fileName));

            return fileName;
        }

        private Image resizeImageTo(Image OrginalImage, int maxWidth, int maxHeight)
        {
            int newWidth, newHeight;
            Graphics graphic;

            newWidth = maxHeight * OrginalImage.Width / OrginalImage.Height;
            newHeight = maxHeight;

            // Create image with new dimensions
            graphic = Graphics.FromImage(new Bitmap(newWidth, newHeight));

            // Set options to high quality
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            // Draw orginal image to image with new dimensions
            graphic.DrawImage(OrginalImage, 0, 0, newWidth, newHeight);

            return new System.Drawing.Bitmap(OrginalImage, new Size(newWidth, newHeight));
        }
    }
}