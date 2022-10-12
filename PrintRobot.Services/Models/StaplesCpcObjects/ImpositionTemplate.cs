using System.Diagnostics.CodeAnalysis;
using PrintRobot.Services.Models.Enums;

namespace PrintRobot.Services.Models.StaplesCpcObjects
{
    [ExcludeFromCodeCoverage]
    public class ImpositionTemplate
    {
        public string ImpositionTemplateID { get; set; }
        public string SubProductName { get; set; }
        public string Size { get; set; }
        public string MultiUpQuantity { get; set; }
        public bool Enabled { get; set; }
    }
}
