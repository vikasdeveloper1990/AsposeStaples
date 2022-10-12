namespace PrintRobotServices.UnitTest
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using PrintRobot.Services.DataServices;
    using PrintRobot.Services.Models.Options;
    using PrintRobot.Services.PrintServices.VendorServices;
    using StaplesCPC.Objects;
    using Xunit;
    public class ShopifyServiceUnitTest
    {

        private readonly Mock<IDataService> _dataService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ShopifyService _shopifyService;
        private IConfiguration _configuration;

        public ShopifyServiceUnitTest()
        {
            _dataService = new Mock<IDataService>();


            // Configuration Section
            _configuration = InitConfiguration();


            var Appsettings = new AppSettings();
            _configuration.GetSection("AppSettings").Bind(Appsettings);
            _appSettings = Options.Create(Appsettings);


            _shopifyService = new ShopifyService(_appSettings, _dataService.Object);

        }

        [Fact]
        public void Shopify_ConstructorTest()
        {
            var shopifyConstructor = new ShopifyService(_appSettings, _dataService.Object);

            Assert.NotNull(shopifyConstructor);
        }

        [Fact]
        public void Shopify_Generate_Pdf_File_Successful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObject());

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_Pdf_File_UnSuccessful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObjectWithWrongUrl());

            //Act ,// Assert
            Assert.Throws<System.Net.WebException>(() => _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405")));

        }

        [Fact]
        public void Shopify_Generate_Pdf_File_NullJob_UnSuccessful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObject());

            //Assert
            Assert.Throws<System.NullReferenceException>(() => _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C406")));

        }

        [Fact]
        public void Shopify_Generate_Pdf_File_NullVendorRefernce_UnSuccessful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObjectWithoutVendorReference());

            //Assert
            Assert.Throws<System.NullReferenceException>(() => _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405")));

        }

        [Fact]
        public void Shopify_Generate_Pdf_File_NullVendorFiles_UnSuccessful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObjectWithoutVendorFiles());

            //Assert
            Assert.Throws<System.NullReferenceException>(() => _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405")));

        }

        [Fact]
        public void Shopify_Generate_Pdf_File_NullVendorPrintFiles_UnSuccessful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetShopifyJobObjectWithoutVendorPrintFiles());

            //Assert
            Assert.Throws<System.ArgumentOutOfRangeException>(() => _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405")));

        }

        [Fact]
        public void Shopify_Generate_MultiPdf_File_Successful_Test()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObject());

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_5x7()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("5x7");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_4x6()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("4x6");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_24x36()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("24x36");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }
        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_8x10()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("8x10");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_12x18()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("12x18");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_20x24()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("20x24");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        [Fact]
        public void Shopify_Generate_SameDay_File_Successful_Test_11x14()
        {
            //Arrange
            _dataService.Setup(x => x.GetVendorJobByJobId(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"))).Returns(() => GetMultiPdfShopifyJobObjectMediaClip());
            _dataService.Setup(x => x.GetJobSize(It.IsAny<Guid>())).Returns("11x14");

            //Act
            var status = _shopifyService.GeneratePdf(new Guid("C584BFF7-4F34-4474-A1E9-0989D337C405"));

            //Assert
            Assert.Equal("Successfully Generated", status.ResponseMessage);


        }

        private Job GetShopifyJobObject()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"VendorID\":25,\"VendorItemID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorPackageID\":\"40935297122492\",\"VendorCartID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorSKUs\":\"52d614-6\",\"StaplesSKUs\":null,\"VendorFiles\":{\"Preview\":[\"/Dynamic/WebProofs/Preview/c584bff7-4f34-4474-a1e9-0989d337c405_t3070.png\"],\"PreviewThumbnail\":[\"https://render.mediacliphub.com/projects/60f2796f-61b8-49ea-9aea-c1e4f7033532/versions/70669336/thumb?_alg=HS512&_exp=1650326400&_iat=1650312000&_iss=hub&_nbf=1650312000&_sub=hubUser.a437461c-fee4-48ff-a2c6-3305f46000af&_sig=eb14a1542a18ce52eca084dad459117ac01b923075eb25e3c907c3a38ce5d5c74b88d4804a5c10c96b8bdd14f94d858c58220a016c9dfbd65a7e29b01c8cb2e2\"],\"Print\":[\"https://renderstouse.blob.core.windows.net/40209155-f31c-4ffb-9fd2-13b2e2b3886e/cfecf857-3d08-4d5c-9f23-ed50542fca10_40209155-f31c-4ffb-9fd2-13b2e2b3886e.pdf?sv=2020-08-04&se=2028-03-19T17%3A23%3A11Z&sr=b&sp=r&sig=%2B0s3iZcQnrRx0r8OTTGkmP%2BRkUdaKJLCYmW7dyYtnyQ%3D\"]},\"VendorAttributes\":{\"VendorCustomerId\":\"6411920769212\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetShopifyJobObjectWithWrongUrl()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"VendorID\":25,\"VendorItemID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorPackageID\":\"40935297122492\",\"VendorCartID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorSKUs\":\"52d614-6\",\"StaplesSKUs\":null,\"VendorFiles\":{\"Preview\":[\"/Dynamic/WebProofs/Preview/c584bff7-4f34-4474-a1e9-0989d337c405_t3070.png\"],\"PreviewThumbnail\":[\"https://render.mediacliphub.com/projects/60f2796f-61b8-49ea-9aea-c1e4f7033532/versions/70669336/thumb?_alg=HS512&_exp=1650326400&_iat=1650312000&_iss=hub&_nbf=1650312000&_sub=hubUser.a437461c-fee4-48ff-a2c6-3305f46000af&_sig=eb14a1542a18ce52eca084dad459117ac01b923075eb25e3c907c3a38ce5d5c74b88d4804a5c10c96b8bdd14f94d858c58220a016c9dfbd65a7e29b01c8cb2e\"],\"Print\":[\"https://renderstouse.blob.core.windows.net/40209155-f31c-4ffb-9fd2-13b2e2b3886e/cfecf857-3d08-4d5c-9f23-ed50542fca10_40209155-f31c-4ffb-9fd2-13b2e2b3886e.pdf?sv=2020-08-04&se=2028-03-19T17%3A23%3A11Z&sr=b&sp=r&sig=%2B0s3iZcQnrRx0r8OTTGkmP%2BRkUdaKJLCYmW7dyYtnyQ%3\"]},\"VendorAttributes\":{\"VendorCustomerId\":\"6411920769212\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetShopifyJobObjectWithoutVendorReference()
        {
            return JsonConvert.DeserializeObject<Job>("{\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetShopifyJobObjectWithoutVendorFiles()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"VendorID\":25,\"VendorItemID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorPackageID\":\"40935297122492\",\"VendorCartID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorSKUs\":\"52d614-6\",\"StaplesSKUs\":null,\"VendorAttributes\":{\"VendorCustomerId\":\"6411920769212\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetShopifyJobObjectWithoutVendorPrintFiles()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"VendorID\":25,\"VendorItemID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorPackageID\":\"40935297122492\",\"VendorCartID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorSKUs\":\"52d614-6\",\"StaplesSKUs\":null,\"VendorFiles\":{\"Preview\":[\"/Dynamic/WebProofs/Preview/c584bff7-4f34-4474-a1e9-0989d337c405_t3070.png\"],\"PreviewThumbnail\":[\"https://render.mediacliphub.com/projects/60f2796f-61b8-49ea-9aea-c1e4f7033532/versions/70669336/thumb?_alg=HS512&_exp=1650326400&_iat=1650312000&_iss=hub&_nbf=1650312000&_sub=hubUser.a437461c-fee4-48ff-a2c6-3305f46000af&_sig=eb14a1542a18ce52eca084dad459117ac01b923075eb25e3c907c3a38ce5d5c74b88d4804a5c10c96b8bdd14f94d858c58220a016c9dfbd65a7e29b01c8cb2e2\"], \"Print\": []},\"VendorAttributes\":{\"VendorCustomerId\":\"6411920769212\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetMultiPdfShopifyJobObject()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"VendorID\":25,\"VendorItemID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorPackageID\":\"40935297122492\",\"VendorCartID\":\"60f2796f-61b8-49ea-9aea-c1e4f7033532\",\"VendorSKUs\":\"52d614-6\",\"StaplesSKUs\":null,\"VendorFiles\":{\"Preview\":[\"/Dynamic/WebProofs/Preview/c584bff7-4f34-4474-a1e9-0989d337c405_t3070.png\"],\"PreviewThumbnail\":[\"https://render.mediacliphub.com/projects/60f2796f-61b8-49ea-9aea-c1e4f7033532/versions/70669336/thumb?_alg=HS512&_exp=1650326400&_iat=1650312000&_iss=hub&_nbf=1650312000&_sub=hubUser.a437461c-fee4-48ff-a2c6-3305f46000af&_sig=eb14a1542a18ce52eca084dad459117ac01b923075eb25e3c907c3a38ce5d5c74b88d4804a5c10c96b8bdd14f94d858c58220a016c9dfbd65a7e29b01c8cb2e2\"],\"Print\":[\"https://renderstouse.blob.core.windows.net/40209155-f31c-4ffb-9fd2-13b2e2b3886e/cfecf857-3d08-4d5c-9f23-ed50542fca10_40209155-f31c-4ffb-9fd2-13b2e2b3886e.pdf?sv=2020-08-04&se=2028-03-19T17%3A23%3A11Z&sr=b&sp=r&sig=%2B0s3iZcQnrRx0r8OTTGkmP%2BRkUdaKJLCYmW7dyYtnyQ%3D\"]},\"VendorAttributes\":{\"VendorCustomerId\":\"6411920769212\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"c584bff7-4f34-4474-a1e9-0989d337c405\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5806512,\"StoreId\":\"S236\",\"Status\":1,\"Type\":25000,\"JobReference\":\"e7115e-1\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":false,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5806512,\"ModifiedDate\":\"2022-04-20T13:23:13.12\"}");
        }

        private Job GetMultiPdfShopifyJobObjectMediaClip()
        {
            return JsonConvert.DeserializeObject<Job>("{\"JobVendorReference\":{\"JobId\":\"caeb75b9-df18-4a0a-a31f-5387cede86aa\",\"VendorID\":25,\"VendorItemID\":\"f89b2f52-3fe0-49ca-8fc1-a6cba1fd910b\",\"VendorPackageID\":\"41429986345148\",\"VendorCartID\":\"f89b2f52-3fe0-49ca-8fc1-a6cba1fd910b\",\"VendorSKUs\":\"b50e1d-1-sd\",\"StaplesSKUs\":null,\"VendorFiles\":{\"Preview\":[\"/Dynamic/WebProofs/Preview/caeb75b9-df18-4a0a-a31f-5387cede86aa_t790.png\"],\"PreviewThumbnail\":[\"https://render.mediacliphub.com/projects/f89b2f52-3fe0-49ca-8fc1-a6cba1fd910b/versions/79735102/thumb?_alg=HS512&_exp=1658880000&_iat=1658865600&_iss=hub&_nbf=1658865600&_sub=hubUser.bc86befa-57b0-4586-809c-af8d62ecad33&_sig=7e7f0a6f483311955f6bf807a1b6973688182f99102fca2c8678bd718b9c2235ea439abbef24ccc5bce21fb7618b17cbae2683287811a9faa0cf098053404e73\"],\"Print\":[\"https://renderstouse.blob.core.windows.net/dd942a3d-bc6b-417a-8562-ceea2e4af49e/prints-set-01.pdf?sv=2021-06-08&se=2028-06-24T21%3A11%3A32Z&sr=b&sp=r&sig=1r3YgIybXok9NROafKXpzS%2F994McYgj7lyAy4GjlHz8%3D\"]},\"VendorAttributes\":{\"VendorCustomerId\":\"5805283672252\",\"Released\":\"true\"}},\"NameReference\":null,\"IsCalendarFlash\":false,\"Calendar_xml\":null,\"Json\":null,\"Bcim_xml\":null,\"HasCalendarXml\":false,\"HasBcimXml\":false,\"Ref1\":0,\"Ref2\":0,\"PrintProdDesigner\":null,\"CartId\":null,\"PickUpDateTime\":null,\"JobSpecialNotes\":null,\"IsYearInView\":false,\"IsNewEngine\":false,\"IsLowDPI\":false,\"IsSaved\":false,\"IsReOrder\":false,\"IsGoodImagePath\":false,\"Id\":\"caeb75b9-df18-4a0a-a31f-5387cede86aa\",\"TempJobId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":5813412,\"StoreId\":\"S005\",\"Status\":5,\"Type\":25007,\"JobReference\":\"c6321e-2\",\"Details\":\"\",\"Faces\":{\"0\":{\"Id\":0,\"ParentID\":0,\"JobId\":\"00000000-0000-0000-0000-000000000000\",\"FaceName\":0,\"TemplateName\":null,\"TextFields\":{},\"ImageFields\":{},\"ProduitAssocie\":null,\"SubProductType\":0,\"IsImageLowRes\":false,\"IsLoaded\":false,\"CurrentImageName\":null,\"CurrentTextFieldName\":null,\"CalendarFields\":{},\"Shapes\":{},\"ModifiedBy\":0,\"ModifiedDate\":\"0001-01-01T00:00:00\"}},\"IsColor\":false,\"CurrentOptionSet\":null,\"MinimalQuantity\":250,\"Size\":null,\"CalendarStartDate\":null,\"IsFlashShowGallery\":false,\"JobLanguage\":\"en-CA\",\"JobUploadPath\":null,\"IsUploadYourOwn\":false,\"IsCalendar\":false,\"IsCanvaPSP\":false,\"IsCanvaPP\":false,\"IsCanvaEducation\":false,\"IsShredding\":false,\"IsCanvaSterling\":false,\"IsCanvaPPSterling\":false,\"IsShopifySpecialOrder\":true,\"IsCanvaEnvelope\":false,\"IsShopify\":true,\"IsSpartan\":false,\"IsPromoProduct\":false,\"IsBusinessCardInMinutes\":false,\"IsLabel\":false,\"IsFlashLabel\":false,\"IsLabelSameDay\":false,\"IsFlashLabelSameDay\":false,\"IsFlashCalendar\":false,\"isShippingExpresspost\":false,\"IsShipping\":false,\"IsOliverLabel\":false,\"isOliverCustomPackage\":false,\"IsOliverRegularPackage\":false,\"IsOliverRegularProduct\":false,\"PrintColumnsCount\":0,\"IsAgendaJournal\":false,\"IsDoubleSided\":false,\"IsGreetingCard\":false,\"IsPanelCard\":false,\"IsApolloGreetingCard\":false,\"IsCanvaGreetingCard\":false,\"IsApolloGreetingCardSameDay\":false,\"IsNPPProducts\":false,\"IsPhoenixGreetingCard\":false,\"IsPNI\":false,\"IsStoragePipe\":false,\"IsHostopia\":false,\"IsFujifilm\":false,\"IsTaylor\":false,\"IsPGW\":false,\"IsFuji\":false,\"IsApollo\":false,\"IsApolloSterling\":false,\"IsApolloFuji\":false,\"IsNebs\":false,\"IsBusinessCard\":false,\"IsSameDay\":true,\"IsGiftOptionEligible\":false,\"IsGiftOptionEligibleStaples\":false,\"IsGiftOptionEligibleFuji\":false,\"IsEnvelopeOrFreeItems\":false,\"IncludesEnvelope\":false,\"IsBusinessCardSameDay\":false,\"IsFileSubmission\":false,\"IsPartnerIntegration\":false,\"IsPersonalCard\":false,\"UsesNewPDFGeneration\":false,\"IsPoster\":false,\"isPosterSameDay\":false,\"IsLimitedOptionsPoster\":false,\"IsBanner\":false,\"IsBannerSameDay\":false,\"IsLimitedOptionsBanner\":false,\"IsFlyer\":false,\"IsBookmark\":false,\"IsPostCard\":false,\"IsPostCardSameDay\":false,\"IsPostCard6x4UploadYourOwn\":false,\"IsPostCard7x5UploadYourOwn\":false,\"IsPostCardFullColour\":false,\"IsHtml\":false,\"IsFlash\":false,\"IsPhotoBook\":false,\"IsPhotoPrinting\":false,\"IsWedding\":false,\"IsVertical\":false,\"IsStaples\":false,\"IsSterling\":false,\"IsRicoh\":false,\"IsSolutionBuilder\":false,\"IsRicohDocumentSameDay\":false,\"Vendor\":25,\"IsKioskModeOnly\":false,\"IsNonIntegratedPOSTProduct\":false,\"BJobFail\":false,\"IsExternalVendor\":false,\"IsSpecialOrder\":false,\"IsPrintProduct\":false,\"IsShopify_Staples\":true,\"IsShopify_Vendor\":false,\"ModifiedBy\":5813412,\"ModifiedDate\":\"2022-07-26T17:11:32.99\"}");
        }

        public IConfigurationRoot InitConfiguration()
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json").Build();
            return config;
        }
    }
}
