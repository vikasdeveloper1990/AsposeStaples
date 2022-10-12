using System;

namespace PrintRobot.Services.PrintServices
{
    public interface IPrintService
    {
        string GeneratePdf(Guid JobID);
    }
}
