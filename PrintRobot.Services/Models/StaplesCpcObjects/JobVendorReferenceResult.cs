namespace PrintRobot.Services.Models.StaplesCpcObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class JobVendorReferenceResult
    {
        public System.Guid Id { get; set; }

        public System.Nullable<System.DateTime> CalendarStartDate { get; set; }

        public string CartId { get; set; }

        public string Details { get; set; }

        public string JobReference { get; set; }

        public string LanguageCode { get; set; }

        public bool LowDPI { get; set; }

        public int ModifiedBy { get; set; }

        public System.DateTime ModifiedDate { get; set; }

        public string NameReference { get; set; }

        public bool ShowInHistory { get; set; }

        public string Status { get; set; }

        public string StoreId { get; set; }

        public string Type { get; set; }

        public int UserId { get; set; }

        public System.Nullable<int> VendorReferenceId1 { get; set; }

        public System.Nullable<int> VendorReferenceId2 { get; set; }

        public System.Nullable<int> VendorID { get; set; }

        public string VendorItemID { get; set; }

        public string VendorPackageID { get; set; }

        public string VendorCartID { get; set; }

        public string VendorSKUs { get; set; }

        public string StaplesSKUs { get; set; }

        public string VendorProofImages { get; set; }

        public string VendorAttributes { get; set; }

        public string PrintProductDesigner { get; set; }

        public Nullable<System.DateTime> PickUpDateTime { get; set; }

        public string JobSpecialNotes { get; set; }
        public string OptionSize { get; set; }


    }
}
