using System.Diagnostics.CodeAnalysis;

namespace PrintRobot.Services.Models
{
    [ExcludeFromCodeCoverage]
    public class Status
    {
        public bool IsSuccess { get; set; }
        public bool IsImpositionFile { get; set; }
        public string ResponseMessage { get; set; }
    }
}
