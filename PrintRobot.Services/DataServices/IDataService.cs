
namespace PrintRobot.Services.DataServices
{
    using System;
    using System.Collections.Generic;
    using PrintRobot.Services.Models.StaplesCpcObjects;
    using StaplesCPC.Objects;
    using StaplesCPC.Objects.Catalogue;

    public interface IDataService
    {
        string GetNextPrintJobIdForProduct(List<BatchPrintableProductType> internalProductTypes);
        Job GetJob(Guid gJobId, bool needDesignInfo = true);
        Job GetVendorJobByJobId(object guidJobId);
        JobPrintFileStatus GetJobPrintFileStatus(string strJobId, bool needDesignInfo = true, Job job = null);
        string UpdateJobPrintFileStatus(Guid gJobId, string strStatus, int intLastFaceNumGenerated, string strMachineName, string strFailingReason);
        PrintSetup GetPrintSetupByName(string strName);
        ImpositionTemplate GetImpositionTemplateId(Job job);
        string GetJobSize(Guid jobId);
    }
}
