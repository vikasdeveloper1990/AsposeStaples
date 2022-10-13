
namespace PrintRobot.Services.PrintServices.VendorServices
{
    extern alias Aspose_Drawing;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Aspose.Pdf;
    using Aspose.Pdf.Facades;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Polly;
    using PrintRobot.Services.DataServices;
    using PrintRobot.Services.Extensions;
    using PrintRobot.Services.Models;
    using PrintRobot.Services.Models.Options;
    using PrintRobot.Services.Models.StaplesCpcObjects;
    using PrintRobot.Services.PrintServices.PhotoPrintSupportingClasses;
    using PrintRobot.Services.Shared;
    using StaplesCPC.Objects;
    using StaplesCPC.Objects.PrintProducts.PPDesignJson;
    using StaplesCPC.Objects.VendorIntegration.Enums;

    public class ShopifyService : PrintProcessor
    {

        private readonly AppSettings _appSettings;
        private readonly IDataService _dataService;

        const int _0875Margin = 63;
        const int _025Margin = 25;
        const int _1Inch = 72;
        const int _4x6TopMargin = 27;
        const int _4x6LeftMargin = 81;
        const int _4x6RightMargin = 81;
        const int _4x6BottomMargin = 0;
        const int _5x7TopMargin = 27;
        const int _5x7LeftMargin = 45;
        const int _5x7BottomMargin = 0;
        const int _5x7RightMargin = 45;
        static Aspose.Pdf.Drawing.Graph graph = null;

        public ShopifyService(IOptions<AppSettings> appSettings, IDataService dataService) : base("shopify")
        {
            _appSettings = appSettings.Value;
            _dataService = dataService;
        }
        public override Status GeneratePdf(Guid JobID)
        {
            var job = _dataService.GetVendorJobByJobId(JobID);

            if (job == null || job.JobVendorReference == null || job.JobVendorReference.VendorFiles == null || !job.JobVendorReference.VendorFiles.ContainsKey(VIFileType.Print)
                 || job.JobVendorReference.VendorFiles[VIFileType.Print].Count == 0 || string.IsNullOrEmpty(job.JobVendorReference.VendorFiles[VIFileType.Print][0]))
            {
                if (job == null)
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue: job == null");
                if (job.JobVendorReference == null)
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue: job.JobVendorReference for JobId: {JobID}");
                if (job.JobVendorReference.VendorFiles == null)
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue : job.JobVendorReference.VendorFiles == null for JobId: {JobID}");
                if (job.JobVendorReference.VendorFiles == null)
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue: !job.JobVendorReference.VendorFiles.ContainsKey(VIFileType.Print for JobId: {JobID}");
                if (job.JobVendorReference.VendorFiles[VIFileType.Print].Count == 0)
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue: job.JobVendorReference.VendorFiles[VIFileType.Print].Count == 0 for JobId: {JobID}");
                if (string.IsNullOrEmpty(job.JobVendorReference.VendorFiles[VIFileType.Print][0]))
                    Console.WriteLine($"Shopify GeneratePDF JobVendorReference Issue: Shopify GeneratePDF condition: string.IsNullOrEmpty(job.JobVendorReference.VendorFiles[VIFileType.Print][0]) for JobId: {JobID}");
                Console.WriteLine($"Error: Invalid print file info for Shopify orders for JobId: {JobID}");
                throw new InvalidDataException("Invalid print file info");
            }

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    //job.Type = Job.JobType.MC_PhotoValuePrinting;
                    Console.WriteLine($"Shopify JobType:{job.Type} for JobId: {JobID}");

                    if (Job.isShopifyMultiplePdfType(job.Type))
                    {
                        PhototPrintRoot printRoot = JsonConvert.DeserializeObject<PhototPrintRoot>(job.JobVendorReference.VendorFiles[VIFileType.Print][0]);
                        foreach (PhototPrintPhotoItem item in printRoot.Jobs[0].PhotoItems)
                        {
                            //PhotoItems.Paper is where the URL for the pdf for each photoitem is stored
                            client.DownloadFile(item.Paper, System.IO.Path.Combine(_appSettings.OutputPath, item.PrintFilename));
                        }
                    }
                    else if (IsValueOrSameDayPrintsheet(job.Type))
                    {
                        try
                        {
                            Console.WriteLine($"Http Path for mediaclip PDF in Shopify Service from JobID  : {job.JobVendorReference.VendorFiles[VIFileType.Print][0]}");
                            client.DownloadFile(job.JobVendorReference.VendorFiles[VIFileType.Print][0], System.IO.Path.Combine(_appSettings.OutputPath, JobID + ".pdf"));

                            Console.WriteLine($"Applicaion Path where mediaclip PDF is tsored in  : {System.IO.Path.Combine(_appSettings.OutputPath, JobID + ".pdf")}");

                            Util.CompressSourceFile(Shared.Image.Product.PhotoPrintStaples, JobID, _appSettings);
                            var watch = new System.Diagnostics.Stopwatch();

                            watch.Start();

                            var docPath = System.IO.Path.Combine(System.IO.Path.Combine(_appSettings.OutputPath, JobID + ".pdf"));
                            // Open input PDF
                            PdfExtractor pdfExtractor = new PdfExtractor();
                            pdfExtractor.BindPdf(docPath);

                            int startPage = 1;
                            int endPage = 20;
                            var _1UpDocument = new Document();
                            var _2UpDocument = new Document();
                            while (pdfExtractor.EndPage > endPage)
                            {
                                var photoPrintDetails = new List<PhotoPrintImageDetails>();
                                var documents = ExtractandGenerateValueAndSameDayPrints(docPath, startPage, endPage, pdfExtractor.EndPage,
                                           job.JobReference, JobID, job.Type);

                                startPage = endPage + 1;
                                endPage = endPage + 20;
                                foreach (var document in documents)
                                {
                                    if (_1UpDocument.Pages.Count == 0 || _2UpDocument.Pages.Count == 0)
                                    {
                                        if (document.DocumentTypeName == "1UP")
                                        {
                                            _1UpDocument = document.Document;
                                        }
                                        if (document.DocumentTypeName == "2UP")
                                        {
                                            _2UpDocument = document.Document;
                                        }
                                    }
                                    else
                                    {
                                        if (document.DocumentTypeName == "1UP")
                                        {
                                            _1UpDocument.Pages.Add(document.Document.Pages);
                                        }

                                        if (document.DocumentTypeName == "2UP")
                                        {
                                            _2UpDocument.Pages.Add(document.Document.Pages);
                                        }
                                    }

                                    document.Document.Dispose();
                                    document.Document = null;
                                }
                            }

                            if (pdfExtractor.EndPage < endPage)
                            {
                                var documents = ExtractandGenerateValueAndSameDayPrints(docPath, startPage, pdfExtractor.EndPage, pdfExtractor.EndPage,
                                           job.JobReference, JobID, job.Type);


                                foreach (var document in documents)
                                {
                                    if (_1UpDocument.Pages.Count == 0 || _2UpDocument.Pages.Count == 0)
                                    {
                                        if (document.DocumentTypeName == "1UP")
                                        {
                                            _1UpDocument = document.Document;
                                        }
                                        if (document.DocumentTypeName == "2UP")
                                        {
                                            _2UpDocument = document.Document;
                                        }
                                    }
                                    else
                                    {
                                        if (document.DocumentTypeName == "1UP")
                                        {
                                            _1UpDocument.Pages.Add(document.Document.Pages);
                                        }

                                        if (document.DocumentTypeName == "2UP")
                                        {
                                            _2UpDocument.Pages.Add(document.Document.Pages);
                                        }
                                    }

                                    document.Document.Dispose();
                                    document.Document = null;
                                }
                            }

                            if (_2UpDocument.Pages.Count > 0)
                            {
                                _2UpDocument.Save(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + "_2.pdf"));
                            }

                            if (_1UpDocument.Pages.Count > 0)
                            {
                                _1UpDocument.Save(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + ".pdf"));
                            }

                            _1UpDocument.Dispose();
                            _2UpDocument.Dispose();
                            pdfExtractor.Dispose();

                            _1UpDocument = null;
                            _2UpDocument = null;
                            pdfExtractor = null;

                            GC.Collect();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception generated during generating photo print for mediaclip for JobId: {JobID}: Exception: {ex.Message} and exception:{ex}");
                            GC.Collect();
                            throw ex;
                        }
                    }
                    else
                    {
                        client.DownloadFile(job.JobVendorReference.VendorFiles[VIFileType.Print][0], System.IO.Path.Combine(_appSettings.OutputPath, JobID + ".pdf"));
                    }
                    return new Status()
                    {
                        ResponseMessage = "Successfully Generated",
                        IsSuccess = true
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in downloading shopify file: for Job Id: {JobID}: Exception: {ex.Message}  and exception:{ex}");

                    throw ex;
                }
            }

        }

        internal static bool IsValueOrSameDayPrintsheet(Job.JobType jobType)
        {
            return (jobType == Job.JobType.MC_PhotoValuePrintingSameDay || jobType == Job.JobType.MC_PhotoValuePrinting);
        }

        private List<DocumentType> GetDocumentForValueAndSameDayPrints(List<PhotoPrintImageDetails> photoPrintDetails, Guid JobID, Job.JobType jobType, int startPage, int maxPage)
        {
            try
            {

                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                int photoNo = 0;

                int imagePosition = 1;
                int totalImages = photoPrintDetails.Count;

                Document document = new Document();
                Page page = null;

                var documents = new List<DocumentType>();
                string jobSize = _dataService.GetJobSize(JobID);

                Console.WriteLine($"Shopify job size for JobType:{jobType} is :{jobSize} for JobId: {JobID}");
                switch (jobSize.Trim())
                {
                    case "5x7":
                    case "4x6":

                        var photoPrintDetailsDto = GetPhotoPrintImageDetailsDtos(photoPrintDetails);
                        if (jobType == Job.JobType.MC_PhotoValuePrintingSameDay)
                        {
                            var task = Task.Run(() => documents.Add(Generate4x6and5x7ImpositionFiles(photoPrintDetails, jobSize, JobID, startPage, maxPage)));
                            task.Wait();

                        }

                        if (jobType == Job.JobType.MC_PhotoValuePrinting)
                        {
                            var loadDataTasks = new List<Task>
                           {
                               new Task(() => documents.Add(Generate1UpImpositionFiles(photoPrintDetailsDto, jobSize, JobID, startPage,  maxPage))),
                               new Task(() => documents.Add(Generate4x6and5x7ImpositionFiles(photoPrintDetails, jobSize, JobID, startPage, maxPage)))
                           };

                            Parallel.ForEach(loadDataTasks, task =>
                            {
                                task.Start();
                            });

                            var t = Task.WhenAll(loadDataTasks);

                            t.Wait();
                        }

                        MemoryCleanup(photoPrintDetails, photoPrintDetailsDto);

                        return documents;
                    case "8x10":
                    case "11x14":
                    case "12x18":
                    case "16x20":
                    case "20x24":
                    case "24x36":

                        foreach (var imageItem in photoPrintDetails)
                        {
                            if (imageItem.Bitmap.Width > imageItem.Bitmap.Height)
                            {
                                imageItem.Bitmap.RotateFlip(Aspose_Drawing::System.Drawing.RotateFlipType.Rotate90FlipNone);
                                MemoryStream memoryStream = new MemoryStream();

                                imageItem.Bitmap.Save(memoryStream, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);
                                imageItem.ImageStream = memoryStream;

                            }
                            page = document.Pages.Add();
                            Common.CreatePage(jobSize, page, imageItem.Width, imageItem.Height);

                            photoNo++;

                            Shared.Util.DisposeObjects();
                            Common.DrawCuttingEdges(page, jobSize, imageItem, photoNo, totalImages, imagePosition);

                            imagePosition++;
                        }


                        //document = Shared.Util.CompressOutputFile(document, Shared.Image.Product.PhotoPrintStaples, JobID, _appSettings);

                        Console.WriteLine($"Expected path where file to be downloaded onto {System.IO.Path.Combine(_appSettings.OutputPath, JobID + ".pdf")}");

                        watch.Stop();

                        Console.WriteLine($"Time taken to generate medicalip PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");

                        documents.Add(new DocumentType() { Document = document, DocumentTypeName = "1UP" });

                        MemoryCleanup(photoPrintDetails, null);
                        return documents;
                    //document.Save(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + ".pdf"));

                    default:
                        page = document.Pages.Add();
                        Common.CreatePage(jobSize, page);
                        foreach (var imageItem in photoPrintDetails)
                        {
                            page.Resources.Images.Add(imageItem.ImageStream);
                            switch (imagePosition)
                            {
                                case 1:
                                    photoNo++;

                                    Common.DrawCuttingEdges(page, jobSize, imageItem, photoNo, totalImages, imagePosition);

                                    imagePosition++;
                                    break;
                                case 2:
                                    photoNo++;

                                    Common.DrawCuttingEdges(page, jobSize, imageItem, photoNo, totalImages, imagePosition);
                                    document.Pages.Add(page);
                                    imagePosition = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        watch.Stop();

                        Console.WriteLine($"Time taken to generate medicalip PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");


                        documents.Add(new DocumentType() { Document = document, DocumentTypeName = "1UP" });
                        MemoryCleanup(photoPrintDetails, null);
                        return documents;
                        //document = Shared.Util.CompressOutputFile(document, Shared.Image.Product.PhotoPrintStaples, JobID, _appSettings);
                        //document.Save(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + "_2.pdf"));

                        //break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured at method GetDocumentForValueAndSameDayPrints for Job id {JobID}");
                throw ex;
            }

        }

        public static void DrawCuttingEdges(Page page, string size, PhotoPrintImageDetailsDto imageDetails, int photoNo, int totalImages)
        {
            switch (size)
            {
                case "4x6":
                    DrawCuttingEdges4x6(page, imageDetails, photoNo, totalImages);
                    break;
                case "5x7":
                    DrawCuttingEdges5x7(page, imageDetails, photoNo, totalImages);
                    break;
            }

            imageDetails.ImageStream = null;
            imageDetails.Bitmap = null;
        }
        public static void DrawCuttingEdges4x6(Page page, PhotoPrintImageDetailsDto imageDetails, int photoNo, int totalImages)
        {
            var lineWidth = 10;
            var lineMargin = 10;

            //// Create Graph instance
            var graph = new Aspose.Pdf.Drawing.Graph(0, (float)page.PageInfo.Height);

            // Add graph object to paragraphs collection of page instance
            page.Paragraphs.Add(graph);

            //Add the image to new page
            page.Resources.Images.Add(imageDetails.ImageStream);
            page.Contents.Add(new Aspose.Pdf.Operators.GSave());
            Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(27, 27, page.PageInfo.Width - 27, page.PageInfo.Height - 27);

            Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
            page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
            XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
            page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
            page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


            Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.LLY + 90, (float)rectangle1.LLX - 25 - _1Inch, (float)rectangle1.LLY + 90 });
            Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.URY + 72, (float)rectangle1.URX + lineMargin, (float)rectangle1.URY + 72 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.URY + 72, (float)rectangle1.LLX - 25 - _1Inch, (float)rectangle1.URY + 72 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.LLY + 90, (float)rectangle1.URX + lineMargin, (float)rectangle1.LLY + 90 }); //Add the line to bottom left corner of the page


            Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.LLY, (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.LLY + 75 });
            Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.URY + 85, (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.URY + 110 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.URY + 85, (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.URY + 110 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.LLY, (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.LLY + 75 }); //Add the line to bottom left corner of the page

            // Add rectangle object to shapes collection of Graph object
            graph.Shapes.Add(line1);
            graph.Shapes.Add(line2);
            graph.Shapes.Add(line3);
            graph.Shapes.Add(line4);

            graph.Shapes.Add(line5);
            graph.Shapes.Add(line6);
            graph.Shapes.Add(line7);
            graph.Shapes.Add(line8);

            TextStamp stamp = new TextStamp(imageDetails.JobReference);
            stamp.Height = 15;
            stamp.Width = 35;
            stamp.RotateAngle = 0;
            stamp.XIndent = rectangle1.URX - lineMargin - lineWidth - 40;
            stamp.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp);

            TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
            stamp1.Height = 15;
            stamp1.Width = 15;
            stamp1.RotateAngle = 0;
            stamp1.XIndent = rectangle1.LLX + lineMargin + lineWidth;
            stamp1.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp1);


        }

        public static void DrawCuttingEdges5x7(Page page, PhotoPrintImageDetailsDto imageDetails, int photoNo, int totalImages)
        {
            var lineWidth = 10;
            var lineMargin = 10;

            //// Create Graph instance
            var graph = new Aspose.Pdf.Drawing.Graph(0, (float)page.PageInfo.Height);

            // Add graph object to paragraphs collection of page instance
            page.Paragraphs.Add(graph);

            page.Resources.Images.Add(imageDetails.ImageStream);

            page.Contents.Add(new Aspose.Pdf.Operators.GSave());
            Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(27, 27, page.PageInfo.Width - 27, page.PageInfo.Height - 27);

            Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
            page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
            XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
            page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
            page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


            Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.LLY + 55, (float)rectangle1.LLX - 25 - _1Inch, (float)rectangle1.LLY + 55 });
            Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.URY + 36, (float)rectangle1.URX + lineMargin, (float)rectangle1.URY + 36 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.URY + 36, (float)rectangle1.LLX - 25 - _1Inch, (float)rectangle1.URY + 36 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.LLY + 55, (float)rectangle1.URX + lineMargin, (float)rectangle1.LLY + 55 }); //Add the line to bottom left corner of the page


            Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.LLY - 50, (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.LLY + 40 });
            Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.URY + 50, (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.URY + 75 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.URY + 50, (float)rectangle1.LLX - _0875Margin - 18, (float)rectangle1.URY + 75 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.LLY - 50, (float)rectangle1.URX - _0875Margin - 35, (float)rectangle1.LLY + 40 }); //Add the line to bottom left corner of the page

            // Add rectangle object to shapes collection of Graph object
            graph.Shapes.Add(line1);
            graph.Shapes.Add(line2);
            graph.Shapes.Add(line3);
            graph.Shapes.Add(line4);

            graph.Shapes.Add(line5);
            graph.Shapes.Add(line6);
            graph.Shapes.Add(line7);
            graph.Shapes.Add(line8);

            TextStamp stamp = new TextStamp(imageDetails.JobReference);
            stamp.Height = 15;
            stamp.Width = 35;
            stamp.RotateAngle = 0;
            stamp.XIndent = rectangle1.URX - lineMargin - lineWidth - 40;
            stamp.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp);

            TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
            stamp1.Height = 15;
            stamp1.Width = 15;
            stamp1.RotateAngle = 0;
            stamp1.XIndent = rectangle1.LLX + lineMargin + lineWidth;
            stamp1.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp1);


        }

        public static void CreatePage(string photoSize, Page page, double SetPageWidth = 0, double SetPageHeight = 0)
        {

            switch (photoSize)
            {
                case "4x6":
                    page.SetPageSize(504, 360);
                    page.PageInfo.Margin.Top = _4x6TopMargin;
                    page.PageInfo.Margin.Top = _4x6LeftMargin;
                    page.PageInfo.Margin.Bottom = _4x6BottomMargin;
                    page.PageInfo.Margin.Right = _4x6RightMargin;
                    break;
                case "5x7":
                    page.SetPageSize(576, 432);
                    page.PageInfo.Margin.Top = _5x7TopMargin;
                    page.PageInfo.Margin.Top = _5x7LeftMargin;
                    page.PageInfo.Margin.Bottom = _5x7BottomMargin;
                    page.PageInfo.Margin.Right = _5x7RightMargin;
                    break;
            }
        }

        private DocumentType Generate4x6and5x7ImpositionFiles(List<PhotoPrintImageDetails> photoPrintDetails
                                            , string jobSize, Guid JobID, int startPage, int endPage)
        {
            try
            {
                int imagePosition = 1;
                var watch = new System.Diagnostics.Stopwatch();
                int totalImages = photoPrintDetails.Count;

                int photoNo = startPage;

                watch.Start();
                Document document = new Document();
                Page page = null;

                foreach (var imageItem in photoPrintDetails)
                {
                    if (imageItem.ImageStream.Length > 0)
                    {
                        if (imageItem.Bitmap.Height > imageItem.Bitmap.Width)
                        {
                            imageItem.Bitmap.RotateFlip(Aspose_Drawing::System.Drawing.RotateFlipType.Rotate90FlipNone);
                            MemoryStream memoryStream = new MemoryStream();

                            imageItem.Bitmap.Save(memoryStream, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);
                            imageItem.ImageStream = memoryStream;

                        }
                        if (imagePosition % 2 != 0)
                        {
                            page = document.Pages.Add();
                            Common.CreatePage(jobSize, page, imageItem.Width, imageItem.Height);
                        }

                        Shared.Util.DisposeObjects();
                        Console.WriteLine($"Now processing Photo No: {photoNo} for 4x6and5x7ImpositionFiles for Job Id : {JobID}");
                        Common.DrawCuttingEdges(page, jobSize, imageItem, photoNo, endPage, imagePosition);
                        photoNo++;
                        imagePosition++;
                    }

                }

                watch.Stop();
                Console.WriteLine($"Time taken to generate medicalip value prints 4x6 or 5x7  PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");

                //document = Shared.Util.CompressOutputFile(document, Shared.Image.Product.PhotoPrintStaples, JobID, _appSettings);

                Console.WriteLine($"Expected path where file to be downloaded onto {System.IO.Path.Combine(_appSettings.OutputPath, JobID + $"{startPage}-{endPage}.pdf")}");
                document.Save(System.IO.Path.Combine(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + $"{ startPage}-{ endPage}_2.pdf")));
                return new DocumentType()
                {
                    Document = document,
                    DocumentTypeName = "2UP"
                };

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured at Generate4x6and5x7ImpositionFiles for job Id: {JobID}");
                throw ex;
            }
        }

        private void MemoryCleanup(List<PhotoPrintImageDetails> photoPrintDetails, List<PhotoPrintImageDetailsDto> photoPrintDetailsDto)
        {
            if (photoPrintDetails != null)
            {
                foreach (var photoPrint in photoPrintDetails)
                {
                    //photoPrint.ImageStream.Dispose();
                    //photoPrint.Bitmap.Dispose();

                    photoPrint.Bitmap =null;
                    photoPrint.ImageStream = null;

                }

                photoPrintDetails = null;
            }
            if (photoPrintDetailsDto != null)
            {
                foreach (var photoPrint in photoPrintDetailsDto)
                {
                    //photoPrint.ImageStream.Dispose();
                    //photoPrint.Bitmap.Dispose();

                    photoPrint.Bitmap = null;
                    photoPrint.ImageStream = null;
                }

                photoPrintDetailsDto = null;
            }

            GC.Collect();
        }
        private DocumentType Generate1UpImpositionFiles(List<PhotoPrintImageDetailsDto> photoPrintDetails
                                            , string jobSize, Guid JobID, int startPage, int endPage)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            int _1upPhotNo = startPage;
            int totalImages = photoPrintDetails.Count;
            Document document2 = new Document();
            Page page2 = null;
            foreach (var imageItem in photoPrintDetails)
            {
                if (imageItem.ImageStream.Length > 0)
                {
                    if (imageItem.Bitmap.Height > imageItem.Bitmap.Width)
                    {
                        imageItem.Bitmap.RotateFlip(Aspose_Drawing::System.Drawing.RotateFlipType.Rotate90FlipNone);
                        MemoryStream memoryStream = new MemoryStream();

                        imageItem.Bitmap.Save(memoryStream, Aspose_Drawing::System.Drawing.Imaging.ImageFormat.Png);
                        imageItem.ImageStream = memoryStream;

                    }
                    page2 = document2.Pages.Add();
                    CreatePage(jobSize, page2, imageItem.Width, imageItem.Height);

                    Shared.Util.DisposeObjects();

                    Console.WriteLine($"Now processing Photo No: {_1upPhotNo} for 1 UP for Job Id : {JobID}");
                    DrawCuttingEdges(page2, jobSize, imageItem, _1upPhotNo, endPage);
                    _1upPhotNo++;
                }
            }

            watch.Stop();
            Console.WriteLine($"Time taken to generate medicalip value prints 1UP PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");
            document2.Save(System.IO.Path.Combine(_appSettings.ImpositionFilePath, JobID + $"{startPage}-{endPage}.pdf"));
            return new DocumentType()
            {
                Document = document2,
                DocumentTypeName = "1UP"
            };

        }

        private List<PhotoPrintImageDetailsDto> GetPhotoPrintImageDetailsDtos(List<PhotoPrintImageDetails> photoPrintDetails)
        {
            var photoPrintDetailsDto = new List<PhotoPrintImageDetailsDto>();

            int count = 1;

            try
            {
                foreach (var photoPrint in photoPrintDetails)
                {
                    Console.WriteLine($"Now processing PhotPrint number {count} at GetPhotoPrintImageDetailsDtos");
                    var photoPrintDto = new PhotoPrintImageDetailsDto();

                    var _ms = new MemoryStream();

                    Console.WriteLine($"Memmory length for PhotPrint number {photoPrint.ImageStream.Length}");
                    photoPrint.ImageStream.Copy(_ms);
                    photoPrintDto.ImageStream = _ms;
                    photoPrintDto.Bitmap = new Aspose_Drawing::System.Drawing.Bitmap(_ms);
                    photoPrintDto.Height = photoPrintDto.Bitmap.Height;
                    photoPrintDto.Width = photoPrintDto.Bitmap.Width;
                    photoPrintDto.JobReference = photoPrint.JobReference;

                    photoPrintDetailsDto.Add(photoPrintDto);
                    count++;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured at GetPhotoPrintImageDetailsDtos");
                throw ex;
            }

            return photoPrintDetailsDto;
        }

        private List<DocumentType> ExtractandGenerateValueAndSameDayPrints(string docPath, int startPage, int endPage, int maxPage
            , string JobReference
            , Guid jobId, Job.JobType jobType)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            var photoPrintDetails = new List<PhotoPrintImageDetails>();
            var retryPolicy = Policy.Handle<FileNotFoundException>()
                              .WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(10));

            retryPolicy.Execute(() =>
            {
                photoPrintDetails = Util.ExtractImagesFromPagesRange(docPath, JobReference, startPage, endPage);
            });
            watch.Stop();
            Console.WriteLine($"Time taken to Extract mediaclip PhotoPrints for JobId:{jobId}  is : {watch.ElapsedMilliseconds} ms");

            return GetDocumentForValueAndSameDayPrints(photoPrintDetails, jobId, jobType, startPage, maxPage);
        }
    }
}
