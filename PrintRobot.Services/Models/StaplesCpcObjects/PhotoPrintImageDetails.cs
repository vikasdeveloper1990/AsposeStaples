extern alias Aspose_Drawing;

using System;
using System.Drawing;
using System.IO;

namespace PrintRobot.Services.Models.StaplesCpcObjects
{
    [Serializable]
    public class PhotoPrintImageDetails
    {
        public string JobReference { get; set; }
        public Stream ImageStream { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Aspose_Drawing::System.Drawing.Bitmap Bitmap { get; set; }
    }

    public class PhotoPrintImageDetailsDto
    {
        public string JobReference { get; set; }
        public Stream ImageStream { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Aspose_Drawing::System.Drawing.Bitmap Bitmap { get; set; }
    }
}
