

namespace PrintRobotConsoleApp
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using PrintRobot.Services.PrintServices;

    [ExcludeFromCodeCoverage]
    public class WorkerService : IHostedService
    {
        private readonly IPrintWorkerService _printService;
        private bool _printServerOn { get; set; }
        public WorkerService( IPrintWorkerService printService)
        {
            _printService = printService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int origProcessId = Process.GetCurrentProcess().Id;
            this._printServerOn = true;
             StartPrintRobotProcess();
            Console.WriteLine("Image Print Proof Process started");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._printServerOn = false;

            StopPrintRobotProcess();
            Console.WriteLine("Print Product Printing Process stopped");
            throw new System.NotImplementedException();
        }

        private void StopPrintRobotProcess()
        {
            String msgToLog = String.Format("Print Robot process stopped");
            Console.WriteLine( msgToLog);

        }

        private void StartPrintRobotProcess()
        {
            try
            {
                String msgToLog = String.Format("Print Robot process started");

                Console.WriteLine( msgToLog);

                // The Print File Generation will continue until the server is stopped explicitly
                while (this._printServerOn)
                {

                    _printService.GeneratePrintFiles();  // Print file generation process

                    Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                string msgException = "Exception in StartPrintRobotProcess(): \n" + ex.Message + "\n";
                Console.WriteLine(msgException);
            }
        }

    }
}