namespace PrintRobot.Services.PrintServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using PrintRobot.Services.DataServices;
    using PrintRobot.Services.Models;
    using PrintRobot.Services.Models.Options;
    using StaplesCPC.Objects;

    [ExcludeFromCodeCoverage]
    public class PrintWorkerService : IPrintWorkerService
    {
        private readonly IDataService _dataService;
        private readonly AppSettings _appSettings;
        private readonly RoboOptions _roboOptions;
        private readonly IPrintProcessorFactory _printProcessorFactory;
        static readonly object _programLock = new object();

        private static int _currentThreadCount = 0;


        public PrintWorkerService(IDataService dataService, IOptions<AppSettings> appSettings, IOptions<RoboOptions> roboOptions, IPrintProcessorFactory printProcessorFactory)
        {
            _dataService = dataService;
            _appSettings = appSettings.Value;
            _roboOptions = roboOptions.Value;
            _printProcessorFactory = printProcessorFactory;
        }

        public void GeneratePrintFiles()
        {
            try
            {
                lock (_programLock)
                {
                    // Get next Print Job whenever a thread is available
                    if (_currentThreadCount < _roboOptions.ThreadNumber)
                    {
                        // Get Next Print JobID
                        List<BatchPrintableProductType> internalProductTypes = GetRobotProductTypes();
                        string sJobID = _dataService.GetNextPrintJobIdForProduct(internalProductTypes);

                        Guid jobID;
                        if (!string.IsNullOrEmpty(sJobID) && Guid.TryParse(sJobID, out jobID))
                        {
                            // Increment the thread count
                            _currentThreadCount += 1;
                            // Start a new thread for the Job
                            var mainTask = Task.Factory.StartNew((() => GeneratePDFPrintFile(jobID)), TaskCreationOptions.LongRunning);
                            //var mainTask = Task.Factory.StartNew(() => { throw new Exception("Throw an Exception"); });  // FOR TESTING EXCEPTION THROWN
                            Task CompletionTask = mainTask.ContinueWith(task => OnComplete(sJobID), TaskContinuationOptions.NotOnFaulted);
                            Task FailureTask = mainTask.ContinueWith(task => OnFailure(sJobID), TaskContinuationOptions.OnlyOnFaulted);

                            if (_appSettings.EnableDebug)
                            {
                                String msgToLog = String.Format("Started a New thread for JobID = " + sJobID + "; Current thread count = " + _currentThreadCount);
                                Console.WriteLine( msgToLog);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msgException = "Exception in GeneratePrintFiles(): \n" + ex.Message + "\n";
                Console.WriteLine( msgException);
            }
        }

        private void GeneratePDFPrintFile(Guid jobID)
        {
            try
            {
                var status = new Status();
                Job theJob = _dataService.GetJob(jobID);

                // Update JobPrintFileStatus record
                _dataService.UpdateJobPrintFileStatus(jobID, JobPrintFileStatus.JobPrintingStatus.InProgress.ToString(), 0, "Print Robot V2 - " + Environment.MachineName, "Pdf file generation started");

                var processor = _printProcessorFactory.ProcessorFor(GetJobType(theJob));
                if (_appSettings.EnableDebug)
                {
                    Console.WriteLine( $"Before calling {GetJobType(theJob)} GeneratePDF(jobID) for Job Id: {jobID}");
                }

                // Generate Calendar PDF
                status = processor.GeneratePdf(jobID);

                if (_appSettings.EnableDebug)
                {
                    Console.WriteLine( $"After calling {GetJobType(theJob)} GeneratePDF(jobID) for Job Id: {jobID}");
                }

                if (!status.IsSuccess)
                {
                    _dataService.UpdateJobPrintFileStatus(jobID, JobPrintFileStatus.JobPrintingStatus.Failed.ToString(), 0, "Print Robot V2 - " + Environment.MachineName, "Failed to generate Imposition file: " + status.ResponseMessage);
                }
                else
                {
                    _dataService.UpdateJobPrintFileStatus(jobID, JobPrintFileStatus.JobPrintingStatus.PrintFileGenerated.ToString(), 0, "Print Robot V2 - " + Environment.MachineName, "Pdf file generation completed");
                }


            }
            catch (Exception ex)
            {
                // Update JobPrintFileStatus record
                _dataService.UpdateJobPrintFileStatus(jobID, JobPrintFileStatus.JobPrintingStatus.Failed.ToString(), 0, "Print Robot V2 - " + Environment.MachineName, "Failed to generate Pdf file: " + ex.Message);
                string msgException = "Exception in GeneratePDFPrintFile(): " + ex.Message;
                Console.WriteLine( $"FOr Job iD : {jobID} with excepion", msgException);
            }

        }

        private List<BatchPrintableProductType> GetRobotProductTypes()
        {
            //string printProductList = Config.GetValue(Config.ManagedKey.PrintProducts);
            List<BatchPrintableProductType> internalProductTypes = new List<BatchPrintableProductType>();
            List<string> lowerPrintProductList = _roboOptions.PrintProducts;
            lowerPrintProductList.ForEach(x => x.ToLower());

            if (lowerPrintProductList.Contains("shopify"))
            {
                internalProductTypes.Add(BatchPrintableProductType.Shopify);
            }
            else if (lowerPrintProductList.Contains("calendar"))
            {
                internalProductTypes.Add(BatchPrintableProductType.Calendar);
            }
            else if (lowerPrintProductList.Contains("photoprinting"))
            {
                internalProductTypes.Add(BatchPrintableProductType.PhotoPrinting);
            }
            else if (lowerPrintProductList.Contains("canvapp"))
            {
                internalProductTypes.Add(BatchPrintableProductType.CanvaPP);
            }
            else if (lowerPrintProductList.Contains("promo products"))
            {
                internalProductTypes.Add(BatchPrintableProductType.PromoProduct);
            }
            return internalProductTypes;
        }

        private string GetJobType(Job theJob)
        {
            if (theJob.IsShopify)
            {
                return "shopify";
            }

            return string.Empty;
        }

        public void OnComplete(string sJobID)
        {
            lock (_programLock)
            {
                // Decrement the thread count
                _currentThreadCount -= 1;

                if (_appSettings.EnableDebug)
                {
                    String msgToLog = String.Format("OnComplete: Ended the thread for Job = " + sJobID + "; Current thread count = " + _currentThreadCount);
                    Console.WriteLine( msgToLog);
                }
            }
        }

        public void OnFailure(string sJobID)
        {
            lock (_programLock)
            {
                // Decrement the thread count
                _currentThreadCount -= 1;

                //Console.WriteLine("OnFailure: Ended the thread for Job = " + sJobID + "; Current thread count = " + _currentThreadCount[PrintProducts.Calendar]);
                if (_appSettings.EnableDebug)
                {
                    String msgToLog = String.Format("OnFailure: Ended the thread for Job = " + sJobID + "; Current thread count = " + _currentThreadCount);
                    Console.WriteLine(msgToLog);
                }
            }
        }
    }
}

