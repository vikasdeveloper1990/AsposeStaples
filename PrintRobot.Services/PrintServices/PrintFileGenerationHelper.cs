using System.Diagnostics.CodeAnalysis;
using StaplesCPC.Objects;

namespace PrintRobot.Services.PrintServices
{
    [ExcludeFromCodeCoverage]
    public static class PrintFileGenerationHelper
    {
        internal static bool IsMultiUpPrintsheet(Job.JobType jobType)
        {
            return (jobType == Job.JobType.CanvaPP_Label || jobType == Job.JobType.CanvaPP_LabelSameDay);
        }


    }


}

