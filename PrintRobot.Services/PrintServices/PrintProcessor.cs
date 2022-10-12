using System;
using System.Diagnostics.CodeAnalysis;
using PrintRobot.Services.Models;

namespace PrintRobot.Services.PrintServices
{
    [ExcludeFromCodeCoverage]
    public abstract class PrintProcessor
    {
        public virtual string ProcessorName { get; }

        protected PrintProcessor(string processorName)
        {
            ProcessorName = processorName;
        }

        public abstract Status GeneratePdf(Guid JobID);
    }
}
