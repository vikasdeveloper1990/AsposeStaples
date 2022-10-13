namespace PrintRobot.Services.Shared
{
    extern alias Aspose_Drawing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using Aspose.Pdf;
    using Aspose.Pdf.Facades;
    using Polly;
    using PrintRobot.Services.Models.Options;
    using PrintRobot.Services.Models.StaplesCpcObjects;

    [ExcludeFromCodeCoverage]
    public static class Util
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

        public static void DisposeObjects()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
        }

        public static void CompressSourceFile(Image.Product product, Guid JobID, AppSettings appSettings)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Document document = new Document(System.IO.Path.Combine(appSettings.OutputPath, JobID + ".pdf"));
            Console.WriteLine($"Now entering Compression logic for Source JobID:{JobID}");
            try
            {
                long quality = 0L;
                switch (product)
                {
                    case Image.Product.Calendar:
                    case Image.Product.CalendarProof:
                        quality = long.Parse(appSettings.CalendarCompressionLevel);
                        break;
                    case Image.Product.PhotoPrintStaples:
                    case Image.Product.PhotoPrintVendor:
                    case Image.Product.PhotoProof:
                        quality = long.Parse(appSettings.PhotoPrintCompressionLevel);
                        break;
                }


                float resolutionLimit = Shared.Image.GetLimitResolution(product, appSettings);

                // Open document
                Document pdfDocument = document;
                // Initialize OptimizationOptions
                var optimizeOptions = new Aspose.Pdf.Optimization.OptimizationOptions();
                // Set CompressImages option
                optimizeOptions.ImageCompressionOptions.CompressImages = true;
                // Set ImageQuality option
                optimizeOptions.ImageCompressionOptions.ImageQuality = Convert.ToInt32(quality);
                // Set ResizeImage option
                optimizeOptions.ImageCompressionOptions.ResizeImages = true;
                // Set MaxResolution option
                optimizeOptions.ImageCompressionOptions.MaxResolution = (int)resolutionLimit;
                // Optimize PDF document using OptimizationOptions
                optimizeOptions.ImageCompressionOptions.Version = Aspose.Pdf.Optimization.ImageCompressionVersion.Fast;

                pdfDocument.OptimizeResources(optimizeOptions);
                // Save updated document
                pdfDocument.Save(System.IO.Path.Combine(appSettings.OutputPath, JobID + ".pdf"));
                Console.WriteLine($"Now Exiting Compression logic for Source JobID:{JobID}");
                watch.Stop();

                pdfDocument.Dispose();
                document.Dispose();

                pdfDocument = null;
                document = null;
                Console.WriteLine($"Time taken to compress source mediaclip PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured during Compression logic for Source JobID:{JobID}: exception:{ex}");
                throw ex;

            }
        }

        public static Document CompressOutputFile(Document document, Image.Product product, Guid JobID, AppSettings appSettings)
        {
            Console.WriteLine($"Now entering Compression logic for JobID:{JobID}");
            try
            {
                long quality = 0L;
                switch (product)
                {
                    case Image.Product.Calendar:
                    case Image.Product.CalendarProof:
                        quality = long.Parse(appSettings.CalendarCompressionLevel);
                        break;
                    case Image.Product.PhotoPrintStaples:
                    case Image.Product.PhotoPrintVendor:
                    case Image.Product.PhotoProof:
                        quality = long.Parse(appSettings.PhotoPrintCompressionLevel);
                        break;
                }


                float resolutionLimit = Shared.Image.GetLimitResolution(product, appSettings);

                // Open document
                Document pdfDocument = document;
                // Initialize OptimizationOptions
                var optimizeOptions = new Aspose.Pdf.Optimization.OptimizationOptions();
                // Set CompressImages option
                optimizeOptions.ImageCompressionOptions.CompressImages = true;
                // Set ImageQuality option
                optimizeOptions.ImageCompressionOptions.ImageQuality = Convert.ToInt32(quality);
                // Set ResizeImage option
                optimizeOptions.ImageCompressionOptions.ResizeImages = true;
                // Set MaxResolution option
                optimizeOptions.ImageCompressionOptions.MaxResolution = (int)resolutionLimit;
                // Optimize PDF document using OptimizationOptions
                // Set Imagae Compression Version to fast
                optimizeOptions.ImageCompressionOptions.Version = Aspose.Pdf.Optimization.ImageCompressionVersion.Fast;
                pdfDocument.OptimizeResources(optimizeOptions);
                // Save updated document
                pdfDocument.Save(System.IO.Path.Combine(appSettings.ImpositionFilePath, JobID + ".pdf"));
                Console.WriteLine($"Now Exiting Compression logic for JobID:{JobID}");
                return document;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured during Compression logic for JobID:{JobID}: exception:{ex}");
                throw ex;

            }
        }

        public static List<PhotoPrintImageDetails> ExtractImagesFromPDF(string dataDir, Guid jobId, string jobReference)
        {
            List<PhotoPrintImageDetails> photoPrintDetails = new List<PhotoPrintImageDetails>();
            try
            {
                var docPath = System.IO.Path.Combine(dataDir, jobId + ".pdf");
                // Open input PDF
                PdfExtractor pdfExtractor = new PdfExtractor();
                pdfExtractor.BindPdf(docPath);

                Console.WriteLine($"Shopify vendor file downloaded in path :{System.IO.Path.Combine(dataDir, jobId + ".pdf")}");

                var retryPolicy = Policy.Handle<System.ArgumentException>()
                                       .WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(10));

                var result = Parallel.For(pdfExtractor.StartPage, pdfExtractor.EndPage + 1, (i) => {

                    retryPolicy.Execute(() =>
                    {
                        photoPrintDetails = ExtractImagesFromPagesRange(docPath, jobReference, i, i);
                    });

                });

                Console.WriteLine($"Number of images extracted from PDF for Job ID: {jobId } is : {photoPrintDetails.Count}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured while extracting images out of from mediaclip generated PDF for Job ID: {jobId } and exception message:{ex.Message} and Exception: {ex}");

                throw ex;
            }
            return photoPrintDetails;
        }

        public static List<PhotoPrintImageDetails> ExtractImagesFromPDF1(string dataDir, Guid jobId, string jobReference)
        {
            int imageCount = 0;
            List<PhotoPrintImageDetails> photoPrintDetails = new List<PhotoPrintImageDetails>();
            try
            {
                Document pdfDocument = new Aspose.Pdf.Document(System.IO.Path.Combine(dataDir, jobId + ".pdf"));

                foreach (Page page in pdfDocument.Pages)
                {
                    foreach (XImage image in page.Resources.Images)
                    {
                        Console.WriteLine($"Memmory Stream length for extracted images for JobId for image stream: {jobId} and memory length {image.ToStream().Length}");

                        imageCount++;

                        PhotoPrintImageDetails photoPrintImageDetail = new PhotoPrintImageDetails()
                        {
                            ImageStream = image.ToStream(),
                            Height = image.Height,
                            Width = image.Width,
                            JobReference = jobReference
                        };

                        photoPrintDetails.Add(photoPrintImageDetail);
                    }
                }

                Console.WriteLine($"Number of images extracted from PDF for Job ID: {jobId } is : {imageCount}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured while extracting images out of from mediaclip generated PDF for Job ID: {jobId } is : {imageCount} and exception message:{ex.Message} and Exception: {ex}");
            }
            return photoPrintDetails;
        }

        public static List<PhotoPrintImageDetails> ExtractImagesFromPagesRange(string docPath, string jobReference,
                int startPage, int endPage)
        {
            var photoPrintDetails = new List<PhotoPrintImageDetails>();
            PdfExtractor pdfExtractor = new PdfExtractor();
            pdfExtractor.BindPdf(docPath);

            pdfExtractor.StartPage = startPage;
            pdfExtractor.EndPage = endPage;

            pdfExtractor.ExtractImage();

            // Get extracted images
            while (pdfExtractor.HasNextImage())
            {
                MemoryStream memoryStream = new MemoryStream();
                // Read image into memory stream
                pdfExtractor.GetNextImage(memoryStream);
                pdfExtractor.ExtractImageMode = Aspose.Pdf.ExtractImageMode.DefinedInResources;

                Aspose_Drawing::System.Drawing.Bitmap image = new Aspose_Drawing::System.Drawing.Bitmap(memoryStream);

                Console.WriteLine($"Memory Stream for pageNumber {endPage} is : {memoryStream.Length}");

                PhotoPrintImageDetails photoPrintImageDetail = new PhotoPrintImageDetails()
                {
                    ImageStream = memoryStream,
                    Bitmap = image,
                    Height = image.Height,
                    Width = image.Width,
                    JobReference = jobReference
                };

                photoPrintDetails.Add(photoPrintImageDetail);
            }

            pdfExtractor.Dispose();
            pdfExtractor = null;

            return photoPrintDetails;

        }

        public static void Copy(this Stream source, Stream target)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (target == null)
                throw new ArgumentNullException("target");

            long originalSourcePosition = -1;
            int count = 0;
            byte[] buffer = new byte[0x1000];

            if (source.CanSeek)
            {
                originalSourcePosition = source.Position;
                source.Seek(0, SeekOrigin.Begin);
            }

            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
                target.Write(buffer, 0, count);

            if (originalSourcePosition > -1)
            {
                source.Seek(originalSourcePosition, SeekOrigin.Begin);
            }
        }

        public static Aspose_Drawing::System.Drawing.Bitmap DeepClone(this Aspose_Drawing::System.Drawing.Bitmap source)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (Aspose_Drawing::System.Drawing.Bitmap)formatter.Deserialize(stream);
            }
        }
    }
}
