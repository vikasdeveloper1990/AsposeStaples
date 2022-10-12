using Aspose.Pdf;
using PrintRobot.Services.Models.StaplesCpcObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PrintRobot.Services.PrintServices
{
    public class TestClass
    {

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

        public async Task Generate1UpImpositionFiles(List<PhotoPrintImageDetails> photoPrintDetails
                                           , string jobSize, Guid JobID)
        {
            int imagePosition = 1;
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            Document document = new Document();
            Page page = null;
            int _1upPhotNo = 0;
            int totalImages = photoPrintDetails.Count;
            Document document2 = new Document();
            foreach (var imageItem in photoPrintDetails)
            {
                if (imageItem.ImageStream.Length > 0)
                {
                    page = document2.Pages.Add();
                    CreatePage(jobSize, page, imageItem.Width, imageItem.Height);

                    _1upPhotNo++;

                    Shared.Util.DisposeObjects();
                    DrawCuttingEdges(page, jobSize, imageItem, _1upPhotNo, totalImages);

                    imagePosition++;
                }
            }

            watch.Stop();
            Console.WriteLine($"Time taken to generate medicalip value prints 1UP PhotoPrints for JobId:{JobID}  is : {watch.ElapsedMilliseconds} ms");

            document2.Save(System.IO.Path.Combine("C:\\Test", JobID + ".pdf"));
        }

        public static void DrawCuttingEdges(Page page, string size, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages)
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
        }
        public static void DrawCuttingEdges4x6(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages)
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

        public static void DrawCuttingEdges5x7(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages)
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
    }
}
