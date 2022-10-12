using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintRobot.Services.PrintServices
{
    public interface IPrintProcessorFactory
    {
        PrintProcessor ProcessorFor(string product);
    }
}
