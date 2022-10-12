namespace PrintRobot.Services.Shared
{
    extern alias Aspose_Drawing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Aspose_Drawing::System.Drawing;
    using Aspose_Drawing::System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Extensions.Options;
    using PrintRobot.Services.Models.Options;
    using StaplesCPC.Objects.PrintProducts.PPDesignJson;


    [ExcludeFromCodeCoverage]
    public static class Image
    {

        public enum Product
        {
            Calendar,
            CalendarProof,
            PhotoPrintStaples,
            PhotoPrintVendor,
            PhotoProof,
            PromoProduct,
            PromoProductProof
        }

        static IServiceProvider services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        public static Bitmap GetBitmap(string imagePath, AppSettings appSettings, bool IsBackground = false, int userId = 0)
        {
            bool useOriginalPath = true;


            Bitmap bmpOriginal = null;
            Bitmap bmpConverted = null;
            Bitmap bmpReturn = null;

            try
            {
                if (IsBackground)
                {
                    //imagePath = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HiResBackgroundImages"], imagePath + ".png");
                    imagePath = appSettings.HiResBackgroundImages + imagePath + ".png";
                    useOriginalPath = false;
                }
                //imagePath = "https://cpcdevweb25.staplescan.com" + imagePath;
                if (imagePath.StartsWith("http"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    //Cloudinary image
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(imagePath);
                    MemoryStream ms = new MemoryStream(bytes);
                    return new Bitmap(Aspose_Drawing::System.Drawing.Image.FromStream(ms));
                }

                string convertedPath = GetImagePath(imagePath, useOriginalPath);

                if (convertedPath.Contains("PrintProductApps"))
                {
                    throw new Exception("Invalid image file: " + convertedPath);
                }

                Shared.Util.DisposeObjects();

                try
                {
                    bmpOriginal = (Bitmap)Aspose_Drawing::System.Drawing.Image.FromFile(convertedPath);
                    bmpReturn = bmpOriginal;
                }
                catch
                {
                    //original bitmap not found
                }

                convertedPath = GetImagePath(imagePath, false);
                try
                {
                    bmpConverted = (Bitmap)Aspose_Drawing::System.Drawing.Image.FromFile(convertedPath);
                    bmpReturn = bmpConverted;
                }
                catch
                {
                    //converted bitmap not found                
                }

                if (bmpConverted == null && bmpOriginal == null)
                {
                    throw new Exception("Original or converted image file not found: " + convertedPath);
                }

                if (bmpOriginal != null && bmpConverted != null)
                {
                    bmpReturn = bmpOriginal;

                    if ((bmpOriginal.Width == bmpConverted.Width) &&
                        (bmpOriginal.Height == bmpConverted.Height) &&
                        (bmpOriginal.HorizontalResolution < 72) &&
                        (bmpOriginal.VerticalResolution < 72))
                    {
                        bmpReturn = bmpConverted;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Issue in generating BitMpa image" + ex.Message);
            }

            return bmpReturn;
        }


        public static string GetImagePath(string imagePath, bool useOriginalPath)
        {
            imagePath = GetDynamicAndStaticImagePath(imagePath);

            if (useOriginalPath)
            {
                string originalImageFolder = Path.GetDirectoryName(imagePath);
                //check if last folder is a guid - if it is, then it's a job folder
                List<String> foldersList = originalImageFolder.Split(Path.DirectorySeparatorChar).ToList();
                Guid jobFolder;
                bool isJobFolder = Guid.TryParse(foldersList.ElementAt(originalImageFolder.Split(Path.DirectorySeparatorChar).ToList().Count - 1), out jobFolder);
                if (isJobFolder)
                {
                    originalImageFolder = Directory.GetParent(Directory.GetParent(imagePath).FullName).FullName;
                }
                //originalImageFolder.Split(Path.DirectorySeparatorChar).ToList().ElementAt(originalImageFolder.Split(Path.DirectorySeparatorChar).ToList().Count - 1)
                originalImageFolder = Path.Combine(originalImageFolder, "Add some value latere");
                if (Directory.Exists(originalImageFolder))
                {
                    string[] files = Directory.GetFiles(originalImageFolder, Path.GetFileNameWithoutExtension(imagePath) + "*");
                    if (files.Length > 0)
                    {
                        //use original image
                        return Path.Combine(originalImageFolder, files[0]);
                    }
                }
            }
            //use option image
            return imagePath;
        }

        private static string GetDynamicAndStaticImagePath(string imagePath)
        {
            return imagePath
               .Replace("/static", @"")
               .Replace("/Static", @"")
               .Replace("/dynamic", @"")
               .Replace("/Dynamic", @"")
               .Replace("/", @"\");
        }

        public static MemoryStream GetMemoryStream(Bitmap BitmapImage)
        {
            MemoryStream stream = new MemoryStream();
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Aspose_Drawing::System.Drawing.Imaging.Encoder.Quality, 100L);
            BitmapImage.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
            return stream;
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
        public static Bitmap ResizeBitmap(Bitmap BitmapImage, int Width, int Height)
        {
            if ((BitmapImage.Width == Width) && (BitmapImage.Height == Height))
            {
                return BitmapImage;
            }
            else
            {
                Bitmap resizedBitmap = new Bitmap(Width, Height);
                resizedBitmap.SetResolution(BitmapImage.HorizontalResolution, BitmapImage.VerticalResolution);
                Graphics graphics = Graphics.FromImage(resizedBitmap);

                graphics.CompositingMode = Aspose_Drawing::System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = Aspose_Drawing::System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = Aspose_Drawing::System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = Aspose_Drawing::System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = Aspose_Drawing::System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                Bitmap destinationBitmap = BitmapImage;

                graphics.DrawImage(destinationBitmap, new System.Drawing.Rectangle(0, 0, Width, Height));

                MemoryStream ms = ms = new MemoryStream();
                resizedBitmap.Save(ms, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);
                Shared.Util.DisposeObjects();
                return resizedBitmap;
            }
        }

        public static Bitmap CropBitmap(Bitmap BitmapImage, int StartX, int StartY, int Width, int Height, int InnerX = 0, int InnerY = 0)
        {
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0 + InnerX, 0 + InnerY, Width, Height);
            Aspose_Drawing::System.Drawing.Bitmap destImage = new Bitmap(Width, Height);
            destImage.SetResolution(BitmapImage.HorizontalResolution, BitmapImage.VerticalResolution);
            destImage.MakeTransparent();
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = Aspose_Drawing::System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = Aspose_Drawing::System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = Aspose_Drawing::System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = Aspose_Drawing::System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = Aspose_Drawing::System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                using (var wrapMode = new Aspose_Drawing::System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(Aspose_Drawing::System.Drawing.Drawing2D.WrapMode.Clamp);
                    graphics.DrawImage(BitmapImage, destRect, StartX, StartY, Width, Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            MemoryStream ms = ms = new MemoryStream();
            destImage.Save(ms, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);
            Shared.Util.DisposeObjects();
            return destImage;
        }

        public class ImageResolution
        {
            public float VerticalResolution { get; set; }
            public float HorizontalResolution { get; set; }
            public Bitmap Image { get; set; }
        }

        public static ImageResolution GetImageResolution(string ImagePath, Product product, AppSettings appSettings, double Width = 0, double Height = 0, bool IsBackground = false)
        {
            float limitResolution = 300;//GetLimitResolution(product);

            Aspose_Drawing::System.Drawing.Bitmap bmpImage = Shared.Image.GetBitmap(ImagePath, appSettings, IsBackground);
            ImageResolution resReturn = new ImageResolution();
            resReturn.HorizontalResolution = bmpImage.HorizontalResolution;
            resReturn.VerticalResolution = bmpImage.VerticalResolution;
            if (Width > 0 && Height > 0)
            {
                if (bmpImage.VerticalResolution < 196 || bmpImage.VerticalResolution < 196)
                {
                    float ResX = Convert.ToSingle((bmpImage.Width * bmpImage.HorizontalResolution) / Width);
                    float ResY = Convert.ToSingle((bmpImage.Height * bmpImage.VerticalResolution) / Height);
                    resReturn.HorizontalResolution = Math.Min(ResX, ResY);
                    resReturn.VerticalResolution = Math.Min(ResX, ResY);
                }
            }
            resReturn.HorizontalResolution = Math.Min(resReturn.HorizontalResolution, limitResolution);
            resReturn.VerticalResolution = Math.Min(resReturn.HorizontalResolution, limitResolution);
            bmpImage.SetResolution(resReturn.HorizontalResolution, resReturn.VerticalResolution);
            resReturn.Image = bmpImage;
            return resReturn;
        }


        public static Aspose_Drawing::System.Drawing.Bitmap GetProcessedBitmap(PhototPrintImageBox imageBox, string Size, string Border, int BorderWidth, AppSettings appsettings)
        {
            if (imageBox.ImageZoomValue == 0)
                throw new Exception("Invalid ImageZoomValue: 0");

            Aspose_Drawing::System.Drawing.Bitmap bmpImage = Shared.Image.GetBitmap(imageBox.ImageItem.FullImageURL, appsettings);

            int AdjustPositionXInImageContainer = 0;
            int AdjustPositionYInImageContainer = 0;

            //resize(zoom)
            bmpImage = Shared.Image.ResizeBitmap(bmpImage, (int)imageBox.ImageBoxImage.SizeX, (int)imageBox.ImageBoxImage.SizeY);

            switch (imageBox.ImageBoxImage.Rotation)
            {
                case 0:
                case 360:
                    break;

                case 90:
                case 270:
                    //create a square image based on largest side of bitmap
                    Bitmap largerBitmap = new Bitmap(Math.Max(bmpImage.Width, bmpImage.Height), Math.Max(bmpImage.Width, bmpImage.Height));
                    largerBitmap.SetResolution(bmpImage.HorizontalResolution, bmpImage.VerticalResolution);

                    //initial xy position (0,0) after rotation
                    int newy = 0;
                    int newx = 0;
                    int differencetweensides = 0;
                    if (bmpImage.Height > bmpImage.Width) //portrait
                    {
                        differencetweensides = bmpImage.Height - bmpImage.Width;
                        newy = (differencetweensides / 2);
                        AdjustPositionXInImageContainer = differencetweensides / 2;
                    }
                    else //landscape
                    {
                        differencetweensides = bmpImage.Width - bmpImage.Height;
                        newx = (differencetweensides / 2);
                        AdjustPositionYInImageContainer = differencetweensides / 2;
                    }

                    //rotate image
                    if (imageBox.ImageBoxImage.Rotation == 90)
                        bmpImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    else
                        bmpImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

                    //copy image to larger image
                    Graphics graphics = Graphics.FromImage(largerBitmap);
                    graphics.CompositingMode = Aspose_Drawing::System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.CompositingQuality = Aspose_Drawing::System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = Aspose_Drawing::System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = Aspose_Drawing::System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = Aspose_Drawing::System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphics.DrawImage(bmpImage, new System.Drawing.Rectangle(newx, newy, bmpImage.Width, bmpImage.Height));
                    MemoryStream ms = ms = new MemoryStream();
                    largerBitmap.Save(ms, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);

                    bmpImage = largerBitmap;

                    break;

                case 180:
                    bmpImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

            }

            bmpImage = Shared.Image.CropBitmap(bmpImage,
                   Math.Abs((int)imageBox.ImageBoxImage.PosX) + AdjustPositionXInImageContainer,
                   Math.Abs((int)imageBox.ImageBoxImage.PosY) + AdjustPositionYInImageContainer,
                   Math.Min((int)imageBox.SizeX, bmpImage.Width),
                   Math.Min((int)imageBox.SizeY, bmpImage.Height));

            //Add Borders
            if (Border.ToLower().Trim() == "white")
            {
                using (Graphics graphics = Graphics.FromImage(bmpImage))
                {
                    using (Aspose_Drawing::System.Drawing.SolidBrush rectangle = new Aspose_Drawing::System.Drawing.SolidBrush(System.Drawing.Color.White))
                    {
                        //Top
                        graphics.FillRectangle(rectangle, new System.Drawing.Rectangle(0, 0, bmpImage.Width, BorderWidth));
                        //Bottom
                        graphics.FillRectangle(rectangle, new System.Drawing.Rectangle(0, bmpImage.Height - BorderWidth, bmpImage.Width, BorderWidth));
                        //Left
                        graphics.FillRectangle(rectangle, new System.Drawing.Rectangle(0, 0, BorderWidth, bmpImage.Height));
                        //Right
                        graphics.FillRectangle(rectangle, new System.Drawing.Rectangle(bmpImage.Width - BorderWidth, 0, BorderWidth, bmpImage.Height));
                    }
                }
            }

            //Rotate image, if needed
            switch (Size)
            {
                case "4x6":
                case "5x7":
                    if (bmpImage.Height > bmpImage.Width)
                    {
                        bmpImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    break;
            }

            return bmpImage;
        }

        public static float GetLimitResolution(Product product, AppSettings appSettings)
        {
            float limitResolution;

            if (product == Product.PromoProduct)
                limitResolution = float.Parse(appSettings.PromoProductMaxDPI);
            else if (product == Product.PromoProductProof)
                limitResolution = float.Parse(appSettings.PromoProductProofMaxDPI);
            else if (product == Product.Calendar)
                limitResolution = float.Parse(appSettings.CalendarMaxDPI);
            else if (product == Product.CalendarProof)
                limitResolution = float.Parse(appSettings.CalendarProofMaxDPI);
            else if (product == Product.PhotoProof)
                limitResolution = float.Parse(appSettings.PhotoProofMaxDPI);
            else if (product == Product.PhotoPrintStaples)
                limitResolution = float.Parse(appSettings.PhotoPrintStaplesMaxDPI);
            else
                limitResolution = float.Parse(appSettings.PhotoPrintVendorMaxDPI);

            return limitResolution;
        }

    }

}
