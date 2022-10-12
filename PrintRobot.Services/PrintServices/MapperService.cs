
using StaplesCPC.Objects;
using StaplesCPC.Objects.Extensions;
using StaplesCPC.Objects.VendorIntegration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PrintRobot.Services.PrintServices
{
    [ExcludeFromCodeCoverage]
    public static class MapperService
    {
        public static Job ToJob(this Models.StaplesCpcObjects.JobVendorReferenceResult obj)
        {
            Job returnObj = new Job()
            {
                Id = obj.Id,
                UserId = obj.UserId,
                StoreId = obj.StoreId,
                Status = (Job.JobStatus)Enum.Parse(typeof(Job.JobStatus), obj.Status),
                Type = (Job.JobType)Enum.Parse(typeof(Job.JobType), obj.Type),
                ModifiedBy = obj.ModifiedBy,
                ModifiedDate = obj.ModifiedDate,
                NameReference = obj.NameReference,
                IsLowDPI = obj.LowDPI,
                CalendarStartDate = obj.CalendarStartDate,
                JobLanguage = obj.LanguageCode,
                Details = obj.Details,
                JobReference = obj.JobReference,
                CartId = obj.CartId,
                Ref1 = obj.VendorReferenceId1.HasValue ? obj.VendorReferenceId1.Value : 0,
                Ref2 = obj.VendorReferenceId2.HasValue ? obj.VendorReferenceId2.Value : 0,
                JobVendorReference = obj.ToDTO(),
                PrintProdDesigner = obj.PrintProductDesigner,
                PickUpDateTime = obj.PickUpDateTime,
                JobSpecialNotes = obj.JobSpecialNotes
            };
            return returnObj;
        }

        public static JobVendorReference ToDTO(this Models.StaplesCpcObjects.JobVendorReferenceResult obj)
        {
            if (!obj.VendorID.HasValue)
                return null;
            JobVendorReference returnObj = new JobVendorReference
            {
                JobId = obj.Id,
                VendorID = (VendorName)obj.VendorID,
                VendorItemID = obj.VendorItemID,
                VendorPackageID = obj.VendorPackageID,
                VendorCartID = obj.VendorCartID,
                //ShippingOptionID = obj.ShippingOptionID,  //RH 2013-10-24: ShippingOptionID dropped from JobVendorReference
                VendorSKUs = obj.VendorSKUs,
                StaplesSKUs = obj.StaplesSKUs
            };
            returnObj.VendorFiles = obj.VendorProofImages.ToVendorFiles();
            returnObj.VendorAttributes = obj.VendorAttributes.ToVendorAttributes();

            return returnObj;
        }
    }  
}
