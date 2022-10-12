using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintRobot.Services.PrintServices
{
    [ExcludeFromCodeCoverage]
    public class PrintProcessorFactory : IPrintProcessorFactory
    {
        private readonly IEnumerable<PrintProcessor> _printProcessors;
        public PrintProcessorFactory (IEnumerable<PrintProcessor> printProcessors)
        {
            _printProcessors = printProcessors;
        }

        public PrintProcessor ProcessorFor(string product)
        {
            if (product == null) return null;

            foreach(var printProcessor in _printProcessors)
            {
                if(printProcessor.ProcessorName.Contains(product))
                {
                    return printProcessor;
                }
            }

            return null;

        }
    }
}
