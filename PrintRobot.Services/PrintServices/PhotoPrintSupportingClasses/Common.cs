extern alias Aspose_Drawing;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aspose.Pdf;
using PrintRobot.Services.Models.StaplesCpcObjects;

namespace PrintRobot.Services.PrintServices.PhotoPrintSupportingClasses
{
    [ExcludeFromCodeCoverage]
    public static class Common
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
        static Aspose.Pdf.Drawing.Graph graph = null;
        public static void DrawCuttingEdges8x10(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages)
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

            //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_5x7LeftMargin, (double)(420), width - 35, _5x7TopMargin);

            //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, (double)(height), width, (double)(height - 25));
            // Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, height / 2, width - 35, height - 20);
            Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
            page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
            XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
            page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
            page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


            Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.LLY + 28, (float)rectangle1.LLX - 25 - _1Inch , (float)rectangle1.LLY + 28 });
            Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.URY + 10, (float)rectangle1.URX + lineMargin, (float)rectangle1.URY + 10 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.URY + 10, (float)rectangle1.LLX - 25 - _1Inch, (float)rectangle1.URY + 10 }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.LLY  + 35, (float)rectangle1.URX + lineMargin, (float)rectangle1.LLY + 35 }); //Add the line to bottom left corner of the page


            Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - 80, (float)rectangle1.LLY - lineMargin, (float)rectangle1.LLX - 80, (float)rectangle1.LLY + _025Margin - lineMargin });
            Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - 100, (float)rectangle1.URY + _025Margin, (float)rectangle1.URX - 100, (float)rectangle1.URY + lineMargin + _0875Margin }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - 80, (float)rectangle1.URY + _025Margin, (float)rectangle1.LLX - 80, (float)rectangle1.URY + lineMargin + _0875Margin }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - 100, (float)rectangle1.LLY - lineMargin, (float)rectangle1.URX - 100, (float)rectangle1.LLY + _025Margin - lineMargin }); //Add the line to bottom left corner of the page

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
            stamp1.Width = 20;
            stamp1.RotateAngle = 0;
            stamp1.XIndent = rectangle1.LLX + lineMargin + lineWidth;
            stamp1.YIndent = rectangle1.LLY - 17;
            page.AddStamp(stamp1);

            line1.GraphInfo.LineWidth = 1;


        }

        public static void DrawCuttingEdgesForRemaining(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages, int imagePosition = 1)
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
            Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_0875Margin - 30, _0875Margin - 30, page.PageInfo.Width - _0875Margin + 30, page.PageInfo.Height - _0875Margin + 30);

            //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_5x7LeftMargin, (double)(420), width - 35, _5x7TopMargin);

            //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, (double)(height), width, (double)(height - 25));
            // Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, height / 2, width - 35, height - 20);
            Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
            page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
            XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
            page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
            page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


            Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.LLY + 20 + _0875Margin, (float)rectangle1.LLX - lineMargin - _1Inch - lineWidth, (float)rectangle1.LLY + 20 + _0875Margin });
            Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.URY + _1Inch - _025Margin, (float)rectangle1.URX + lineMargin, (float)rectangle1.URY + _1Inch - _025Margin }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _0875Margin - _1Inch, (float)rectangle1.URY + _1Inch - _025Margin, (float)rectangle1.LLX - lineMargin - _1Inch - lineWidth, (float)rectangle1.URY + _1Inch - _025Margin }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineWidth - lineMargin - _0875Margin, (float)rectangle1.LLY + 20 + _0875Margin, (float)rectangle1.URX + lineMargin, (float)rectangle1.LLY + 20 + _0875Margin }); //Add the line to bottom left corner of the page


            Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _1Inch, (float)rectangle1.LLY - lineMargin, (float)rectangle1.LLX - _1Inch, (float)rectangle1.LLY + _0875Margin });
            Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineMargin - _1Inch - _025Margin, (float)rectangle1.URY + lineMargin + _0875Margin, (float)rectangle1.URX - lineMargin - _1Inch - _025Margin, (float)rectangle1.URY + lineMargin + _0875Margin + _1Inch }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _1Inch, (float)rectangle1.URY + _1Inch, (float)rectangle1.LLX - _1Inch, (float)rectangle1.URY + lineMargin + _0875Margin + _1Inch }); //Add the line to bottom left corner of the page
            Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineMargin - _1Inch - _025Margin, (float)rectangle1.LLY - lineMargin, (float)rectangle1.URX - lineMargin - _1Inch - _025Margin, (float)rectangle1.LLY + _0875Margin }); //Add the line to bottom left corner of the page

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
            stamp.Height = 25;
            stamp.Width = 25;
            stamp.RotateAngle = 0;
            stamp.XIndent = rectangle1.URX - lineMargin - lineWidth - 40;
            stamp.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp);

            TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
            stamp1.Height = 25;
            stamp1.Width = 25;
            stamp1.RotateAngle = 0;
            stamp1.XIndent = rectangle1.LLX + lineMargin + lineWidth;
            stamp1.YIndent = rectangle1.LLY - lineMargin - lineWidth;
            page.AddStamp(stamp1);

            line1.GraphInfo.LineWidth = 1;

        }

        public static void DrawCuttingEdges(Page page, string size, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages, int imagePosition = 1)
        {
            try
            {
                if (imagePosition % 2 != 0 && (size == "4x6" || size == "5x7"))
                {
                    graph = GetGraph(page, size);
                }
                switch (size)
                {
                    case "4x6":
                        DrawCuttingEdges4x6(page, imageDetails, photoNo, totalImages, graph, imagePosition);
                        break;
                    case "5x7":
                        DrawCuttingEdges5x7(page, imageDetails, photoNo, totalImages, graph, imagePosition);
                        break;
                    case "8x10":
                        DrawCuttingEdges8x10(page, imageDetails, photoNo, totalImages);
                        break;
                    case "11x14":
                    case "12x18":
                    case "16x20":
                    case "20x24":
                    case "24x36":
                        DrawCuttingEdgesForRemaining(page, imageDetails, photoNo, totalImages);
                        break;
                }

                imageDetails.ImageStream.Dispose();
                imageDetails.Bitmap.Dispose();

                //imageDetails.ImageStream = null;
                //imageDetails.Bitmap = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured at drawing cutting edges: Exception Message :{ex.Message} and Exception:{ex}");
                throw ex;
            }
        }

        public static void DrawCuttingEdges4x6(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages, Aspose.Pdf.Drawing.Graph graph, int imagePosition = 1)
        {
            var lineWidth = 10;
            var lineMargin = 10;

            if (imagePosition % 2 != 0)
            {

                //Add the image to new page
                //page.Resources.Images.Add(imageDetails.ImageStream);
                page.Resources.Images.Add(imageDetails.ImageStream);

                // Dummy piece of code or temp code
                //Image img = System.Drawing.Image.FromStream(imageDetails.ImageStream);

                //img.Save(System.IO.Path.GetTempPath() + "\\myImage.Jpeg", ImageFormat.Jpeg);

                page.Contents.Add(new Aspose.Pdf.Operators.GSave());
                Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_4x6LeftMargin, 460, page.PageInfo.Width - _4x6LeftMargin, page.PageInfo.Height - _025Margin);

                Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
                page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
                XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
                page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
                page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


                Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin + lineMargin, (float)rectangle1.LLX - lineMargin - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin + lineMargin });
                Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX + lineWidth * 2 - _025Margin - _4x6LeftMargin, (float)rectangle1.URY - lineMargin + _4x6LeftMargin, (float)rectangle1.URX - _4x6LeftMargin, (float)rectangle1.URY - lineMargin + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle1.URY - lineMargin + _4x6LeftMargin, (float)rectangle1.LLX - lineMargin - _4x6LeftMargin, (float)rectangle1.URY - lineMargin + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX + lineWidth * 2 - _025Margin - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin + lineMargin, (float)rectangle1.URX - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin + lineMargin }); //Add the line to bottom left corner of the page


                Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _4x6LeftMargin, (float)rectangle1.LLY + lineMargin + _025Margin * 2 + lineWidth, (float)rectangle1.LLX - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX - lineMargin - lineWidth - _4x6LeftMargin, (float)rectangle1.LLY + lineMargin + _025Margin * 2 + lineWidth, (float)rectangle1.URX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle1.LLY + _4x6LeftMargin }); //Add the line to bottom left corner of the page


                // Add rectangle object to shapes collection of Graph object
                graph.Shapes.Add(line1);
                graph.Shapes.Add(line2);
                graph.Shapes.Add(line3);
                graph.Shapes.Add(line4);
                graph.Shapes.Add(line5);
                graph.Shapes.Add(line6);


                TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
                stamp1.RotateAngle = 0;
                stamp1.XIndent = rectangle1.LLX + lineMargin + lineWidth;
                stamp1.YIndent = rectangle1.LLY - lineWidth - lineMargin;
                page.AddStamp(stamp1);

                TextStamp stamp = new TextStamp(imageDetails.JobReference);
                stamp.RotateAngle = 0;
                stamp.XIndent = rectangle1.URX - lineMargin - lineWidth - 45;
                stamp.YIndent = (float)rectangle1.LLY - lineWidth - lineMargin;
                page.AddStamp(stamp);



                line1.GraphInfo.LineWidth = 1;
            }
            else
            {
                //Add the image to new page
                page.Resources.Images.Add(imageDetails.ImageStream);
                page.Contents.Add(new Aspose.Pdf.Operators.GSave());
                Aspose.Pdf.Rectangle rectangle = new Aspose.Pdf.Rectangle(_4x6LeftMargin, 8, page.PageInfo.Width - _4x6LeftMargin, 315);

                //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, (double)(height), width, (double)(height - 25));
                Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY });
                page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
                XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
                page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
                page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


                Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.LLX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin + lineMargin, (float)rectangle.LLX - lineMargin - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin + lineMargin });
                Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.URX + lineWidth * 2 - _025Margin - _4x6LeftMargin, (float)rectangle.URY - lineMargin + _4x6LeftMargin, (float)rectangle.URX - _4x6LeftMargin, (float)rectangle.URY - lineMargin + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.LLX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle.URY - lineMargin + _4x6LeftMargin, (float)rectangle.LLX - lineMargin - _4x6LeftMargin, (float)rectangle.URY - lineMargin + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.URX + lineWidth * 2 - _025Margin - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin + lineMargin, (float)rectangle.URX - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin + lineMargin }); //Add the line to bottom left corner of the page


                Aspose.Pdf.Drawing.Line line9 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.LLX - _4x6LeftMargin, (float)rectangle.LLY + lineMargin + _025Margin * 2 + lineWidth, (float)rectangle.LLX - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line10 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.URX - lineMargin - lineWidth - _4x6LeftMargin, (float)rectangle.LLY + lineMargin + _025Margin * 2 + lineWidth, (float)rectangle.URX - lineWidth - lineMargin - _4x6LeftMargin, (float)rectangle.LLY + _4x6LeftMargin }); //Add the line to bottom left corner of the page

                // Add rectangle object to shapes collection of Graph object
                graph.Shapes.Add(line5);
                graph.Shapes.Add(line6);
                graph.Shapes.Add(line7);
                graph.Shapes.Add(line8);

                graph.Shapes.Add(line9);
                graph.Shapes.Add(line10);

                TextStamp stamp = new TextStamp(imageDetails.JobReference);
                stamp.RotateAngle = 90;
                stamp.XIndent = rectangle.URX + lineMargin;
                stamp.YIndent = rectangle.URY - 50 - lineMargin;
                page.AddStamp(stamp);

                TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
                stamp1.RotateAngle = 90;
                stamp1.XIndent = rectangle.URX + lineMargin;
                stamp1.YIndent = (float)rectangle.LLY + lineMargin + 20;
                page.AddStamp(stamp1);

            }

        }

        public static void DrawCuttingEdges5x7(Page page, PhotoPrintImageDetails imageDetails, int photoNo, int totalImages, Aspose.Pdf.Drawing.Graph graph, int imagePosition = 1)
        {

            var lineWidth = 10;
            var lineMargin = 10;

            if (imagePosition % 2 != 0)
            {

                //Add the image to new page
                page.Resources.Images.Add(imageDetails.ImageStream);
                page.Contents.Add(new Aspose.Pdf.Operators.GSave());
                //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_5x7LeftMargin -10, page.PageInfo.Height/2 , (float)page.PageInfo.Width - _5x7LeftMargin, page.PageInfo.Height - _5x7TopMargin);

                Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_5x7LeftMargin, 387.36, (float)page.PageInfo.Width - _5x7LeftMargin, page.PageInfo.Height - _5x7TopMargin);


                //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(_5x7LeftMargin, (double)(420), width - 35, _5x7TopMargin);

                //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, (double)(height), width, (double)(height - 25));
                // Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, height / 2, width - 35, height - 20);
                Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle1.URX - rectangle1.LLX, 0, 0, rectangle1.URY - rectangle1.LLY, rectangle1.LLX, rectangle1.LLY });
                page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
                XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
                page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
                page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


                Aspose.Pdf.Drawing.Line line1 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _5x7LeftMargin - _025Margin * 2 - lineMargin, (float)rectangle1.LLY + lineMargin + _5x7LeftMargin, (float)rectangle1.LLX - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.LLY + lineMargin + _5x7LeftMargin });
                Aspose.Pdf.Drawing.Line line2 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX + lineMargin - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.URY - lineMargin + _5x7LeftMargin, (float)rectangle1.URX + lineMargin + lineWidth - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.URY - lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line3 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.LLX - _5x7LeftMargin - _025Margin * 2 - lineMargin, (float)rectangle1.URY - lineMargin + _5x7LeftMargin, (float)rectangle1.LLX - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.URY - lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line4 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle1.URX + lineMargin - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.LLY + lineMargin + _5x7LeftMargin, (float)rectangle1.URX + lineMargin + lineWidth - _5x7LeftMargin - _025Margin * 2, (float)rectangle1.LLY + lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page

                // Add rectangle object to shapes collection of Graph object
                graph.Shapes.Add(line1);
                graph.Shapes.Add(line2);
                graph.Shapes.Add(line3);
                graph.Shapes.Add(line4);

                TextStamp stamp = new TextStamp(imageDetails.JobReference);
                stamp.RotateAngle = 90;
                stamp.XIndent = rectangle1.URX + lineMargin;
                stamp.YIndent = rectangle1.URY - 50 - lineMargin;
                page.AddStamp(stamp);

                TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
                stamp1.RotateAngle = 90;
                stamp1.XIndent = rectangle1.URX + lineMargin;
                stamp1.YIndent = (float)rectangle1.LLY + lineMargin + 20;
                page.AddStamp(stamp1);

                // line1.GraphInfo.LineWidth = 1;
            }
            else
            {
                //Add the image to new page
                page.Resources.Images.Add(imageDetails.ImageStream);
                page.Contents.Add(new Aspose.Pdf.Operators.GSave());
                Aspose.Pdf.Rectangle rectangle = new Aspose.Pdf.Rectangle(_5x7LeftMargin, 8, (float)page.PageInfo.Width - _5x7LeftMargin, 387.36);

                //Aspose.Pdf.Rectangle rectangle1 = new Aspose.Pdf.Rectangle(35, (double)(height), width, (double)(height - 25));
                Aspose.Pdf.Matrix matrix1 = new Aspose.Pdf.Matrix(new double[] { rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY });
                page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix1));
                XImage ximage1 = page.Resources.Images[page.Resources.Images.Count];
                page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage1.Name));
                page.Contents.Add(new Aspose.Pdf.Operators.GRestore());


                //Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { 0, 0, 100, 300 });

                Aspose.Pdf.Drawing.Line line5 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.LLX - _5x7LeftMargin - _025Margin * 2 - lineMargin, (float)rectangle.LLY + lineMargin + _5x7LeftMargin, (float)rectangle.LLX - _5x7LeftMargin - _025Margin * 2, (float)rectangle.LLY + lineMargin + _5x7LeftMargin });
                Aspose.Pdf.Drawing.Line line6 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.URX + lineMargin - _5x7LeftMargin - _025Margin * 2, (float)rectangle.URY - lineMargin + _5x7LeftMargin, (float)rectangle.URX + lineMargin + lineWidth - _5x7LeftMargin - _025Margin * 2, (float)rectangle.URY - lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line7 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.LLX - _5x7LeftMargin - _025Margin * 2 - lineMargin, (float)rectangle.URY - lineMargin + _5x7LeftMargin, (float)rectangle.LLX - _5x7LeftMargin - _025Margin * 2, (float)rectangle.URY - lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page
                Aspose.Pdf.Drawing.Line line8 = new Aspose.Pdf.Drawing.Line(new float[] { (float)rectangle.URX + lineMargin - _5x7LeftMargin - _025Margin * 2, (float)rectangle.LLY + lineMargin + _5x7LeftMargin, (float)rectangle.URX + lineMargin + lineWidth - _5x7LeftMargin - _025Margin * 2, (float)rectangle.LLY + lineMargin + _5x7LeftMargin }); //Add the line to bottom left corner of the page


                // Add rectangle object to shapes collection of Graph object
                graph.Shapes.Add(line5);
                graph.Shapes.Add(line6);
                graph.Shapes.Add(line7);
                graph.Shapes.Add(line8);

                TextStamp stamp = new TextStamp(imageDetails.JobReference);
                stamp.RotateAngle = 90;
                stamp.XIndent = rectangle.URX + lineMargin;
                stamp.YIndent = rectangle.URY - 50 - lineMargin;
                page.AddStamp(stamp);

                TextStamp stamp1 = new TextStamp(string.Format("{0}/{1}", photoNo, totalImages));
                stamp1.RotateAngle = 90;
                stamp1.XIndent = rectangle.URX + lineMargin;
                stamp1.YIndent = (float)rectangle.LLY + lineMargin + 20;
                page.AddStamp(stamp1);



            }
        }

        public static void CreatePage(string photoSize, Page page, double SetPageWidth = 0, double SetPageHeight = 0)
        {
            page.SetPageSize(612, 792);
            switch (photoSize)
            {
                case "4x6":
                    page.PageInfo.Margin.Top = _4x6TopMargin;
                    page.PageInfo.Margin.Top = _4x6LeftMargin;
                    page.PageInfo.Margin.Bottom = _4x6BottomMargin;
                    page.PageInfo.Margin.Right = _4x6RightMargin;
                    break;
                case "5x7":
                    page.PageInfo.Margin.Top = _5x7TopMargin;
                    page.PageInfo.Margin.Top = _5x7LeftMargin;
                    page.PageInfo.Margin.Bottom = _5x7BottomMargin;
                    page.PageInfo.Margin.Right = _5x7RightMargin;
                    break;
                case "8x10":
                    page.SetPageSize(648, 792);
                    page.PageInfo.Margin.Top = 20;
                    page.PageInfo.Margin.Top = 20;
                    page.PageInfo.Margin.Bottom = 20;
                    page.PageInfo.Margin.Right = 20;
                    break;
                case "11x14":
                case "12x18":
                case "16x20":
                case "20x24":
                case "24x36":
                    page.SetPageSize(SetPageWidth + (_0875Margin * 2), SetPageHeight + (_0875Margin * 2));
                    page.PageInfo.Margin.Top = _0875Margin;
                    page.PageInfo.Margin.Top = _0875Margin;
                    page.PageInfo.Margin.Bottom = _0875Margin;
                    page.PageInfo.Margin.Right = _0875Margin;
                    break;
            }
        }

        private static Aspose.Pdf.Drawing.Graph GetGraph(Page page, string size)
        {
            var graph1 = new Aspose.Pdf.Drawing.Graph(0, (float)page.PageInfo.Height);
            Aspose.Pdf.Drawing.Line line1 = null;
            if (size == "5x7")
            {
                line1 = new Aspose.Pdf.Drawing.Line(new float[] { -_1Inch + 7, (float)page.PageInfo.Height + 30, _1Inch - 30, (float)page.PageInfo.Height + 30 });
            }

            if (size == "4x6")
            {

                line1 = new Aspose.Pdf.Drawing.Line(new float[] { -_1Inch + 7, (float)page.PageInfo.Height + _1Inch - 7, _1Inch - 30, (float)page.PageInfo.Height + _1Inch - 7 });
            }

            graph1.Shapes.Add(line1);
            line1.GraphInfo.LineWidth = 4.32f;
            page.Paragraphs.Add(graph1);

            return graph1;

        }

    }
}
