using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PrintRobot.Services.Models.Options
{
    [ExcludeFromCodeCoverage]
    public class RoboOptions
    {
        public int SleepTimePerCycle { get; set; }
        public int ThreadNumber { get; set; }
        public List<string> PrintProducts { get; set; }
        public int MaxCPUPercent { get; set; }
        public int MaxRAMPercent { get; set; }
    }
}
