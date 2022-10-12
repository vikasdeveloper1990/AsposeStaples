using System.Diagnostics.CodeAnalysis;

namespace PrintRobot.Services.Models.Options
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public string AposePDFLicense { get; set; }
        public bool EnableDebug { get; set; }
        public string ImageOutputPath { get; set; }
        public string OutputPath { get; set; }
        public string ImpositionFilePath { get; set; }
        public string HiResBackgroundImages { get; set; }
        public string CalendarCompressionLevel { get; set; }
        public string PhotoPrintCompressionLevel { get; set; }
        public string PromoProductMaxDPI { get; set; }
        public string PromoProductProofMaxDPI { get; set; }
        public string CalendarMaxDPI { get; set; }
        public string CalendarProofMaxDPI { get; set; }
        public string PhotoProofMaxDPI { get; set; }
        public string PhotoPrintStaplesMaxDPI { get; set; }
        public string PhotoPrintVendorMaxDPI { get; set; }
    }
}
