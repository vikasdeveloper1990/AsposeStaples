
namespace PrintRobot.Services.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PrintRobot.Services.Models.Options;
    using PrintRobot.Services.Models.StaplesCpcObjects;
    using PrintRobot.Services.PrintServices;
    using StaplesCPC.Objects;
    using StaplesCPC.Objects.Catalogue;

    [ExcludeFromCodeCoverage]
    public class DataService : IDataService
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly object _locker = new object();

        public DataService(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _appSettings = appSettings.Value;
        }

        public string GetNextPrintJobIdForProduct(List<BatchPrintableProductType> productTypes)
        {
            String msg = string.Empty;
            string strProductTypes = string.Empty;
            foreach (BatchPrintableProductType type in productTypes)
            {
                strProductTypes += type.ToString() + ",";
            }
            strProductTypes = strProductTypes.Trim(new char[] { ' ', ',' });

            lock (_locker)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
                {
                    connection.Open();

                    using (var cmd = new SqlCommand("usp_GetNextPrintJobId_V2", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("productTypes", SqlDbType.VarChar).Value = strProductTypes;
                        var job = cmd.ExecuteScalar();
                        msg = job.ToString();
                    }

                };

                if (!string.IsNullOrEmpty(msg))
                {
                    if (_appSettings.EnableDebug)
                    {
                        JobPrintFileStatus jps = GetJobPrintFileStatus(msg);
                        string tempmsg = string.Empty;
                        if (jps != null)
                            tempmsg = msg + " " + jps.FailingReason;
                        Console.WriteLine("GetNextPrintJobId.log", tempmsg);
                    }
                }
            }

            return msg;
        }

        public JobPrintFileStatus GetJobPrintFileStatus(string strJobId, bool needDesignInfo = true, Job job = null)
        {
            Job _job = new Job();
            Guid gJobId = new Guid(strJobId);
            JobPrintFileStatus jobPrintFileStatus = new JobPrintFileStatus();
            if (job == null)
                _job = GetJob(gJobId, needDesignInfo);

            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                jobPrintFileStatus = connection.Query<JobPrintFileStatus>("usp_GetJobPrintFileStatus", new { JobID = strJobId }, commandType: CommandType.StoredProcedure).ToList().FirstOrDefault();

            }

            if (jobPrintFileStatus != null && _job != null)
            {
                jobPrintFileStatus.JobReference = _job.JobReference;
                jobPrintFileStatus.HasCalendarXml = _job.HasCalendarXml;
                jobPrintFileStatus.JobType = _job.Type;
            }

            return jobPrintFileStatus;
        }

        public Job GetJob(Guid gJobId, bool needDesignInfo = true)
        {
            Job oJob = GetVendorJobByJobId(gJobId);
            if (oJob == null)
                return null;

            if (needDesignInfo)
                FillJobWithOrderItemInfo(oJob);

            return oJob;
        }

        public Job GetVendorJobByJobId(object guidJobId)
        {
            JobVendorReferenceResult jobVendorReferenceResult = new JobVendorReferenceResult();
            List<JobVendorReferenceResult> jobVendorReferenceResultList = new List<JobVendorReferenceResult>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                jobVendorReferenceResultList = connection.Query<JobVendorReferenceResult>("usp_GetVendorJobById", new { JobID = guidJobId }, commandType: CommandType.StoredProcedure).ToList();
            };

            return MapperService.ToJob(jobVendorReferenceResultList.FirstOrDefault());
        }

        public string UpdateJobPrintFileStatus(Guid gJobId, string strStatus, int intLastFaceNumGenerated, string strMachineName, string strFailingReason)
        {
            string result = string.Empty;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                 result = connection.Query<int>("usp_UpdateJobPrintFileStatus", new { JobID = gJobId, Status= strStatus, LastFaceNumGenerated = intLastFaceNumGenerated 
                ,MachineName = strMachineName, FailingReason = strFailingReason }, commandType: CommandType.StoredProcedure).ToString();
            }

            return result;
        }

        public ImpositionTemplate GetImpositionTemplateId(Job job)
        {
            var size = string.Empty;
            List<ImpositionTemplate> impositionTemplates = new List<ImpositionTemplate>();
            var match = Regex.Match(job.Details.ToLower(), "\"size\":\"\\d+(\\.\\d+)?x\\d+(\\.?\\d+)?\"").Value;
            if (!string.IsNullOrEmpty(match))
            {
                var numbers = match.Split(':')[1].Trim('"').Split('x');
                size = $"{numbers[0]} x {numbers[1]}";
            }

            if (string.IsNullOrEmpty(size)) return null;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                impositionTemplates = connection.Query<ImpositionTemplate>("SELECT [ImpositionTemplateID],[SubProductName],[Size],[MultiUpQuantity],[Enabled]  FROM [dbo].[ImpositionTemplate] WHERE [Enabled] = 1").ToList();
            }

            return impositionTemplates.Where(x =>x.SubProductName == job.Type.ToString() && x.Size == size)
                .FirstOrDefault();
            
        }

        public PrintSetup GetPrintSetupByName(string strName)
        {
            PrintSetup printSetup = new PrintSetup();
            List<JobVendorReferenceResult> jobVendorReferenceResultList = new List<JobVendorReferenceResult>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                printSetup = connection.Query<PrintSetup>("select * from PrintSetup where Name = @Name", new { Name = strName }).ToList().FirstOrDefault();
            };

            return printSetup;
        }

        private void FillJobWithOrderItemInfo(Job oJob)
        {
            if (oJob != null)
            {
                OrderItem oOrderItem = GetOrderItemForJob(oJob.Id);
                if (oOrderItem != null)
                {
                    oJob.Size = oOrderItem.OptionSize;
                }
                else if (oJob.Type.Equals(StaplesCPC.Objects.Job.JobType.CanvaPP_Photobook))
                {
                    string sizeValue = "";
                    string upcValue = "";
                    string infoValue = "";
                    try
                    {
                        bool hasValue = oJob.JobVendorReference.VendorAttributes.TryGetValue("VendorAttributesInfo", out infoValue);
                        if (hasValue)
                        {
                            JObject obj = JsonConvert.DeserializeObject<JObject>(infoValue);
                            sizeValue = (string)obj["size"];
                            upcValue = (string)obj["upc"];
                            oJob.Size = sizeValue;
                            oJob.JobVendorReference.VendorSKUs = upcValue;
                        }
                        else
                        {
                            oJob.Size = String.Empty;
                        }
                    }
                    catch
                    {
                        oJob.Size = String.Empty;
                    }
                }
                else
                {
                    oJob.Size = String.Empty;
                }

                oJob.Faces = GetFaces(oJob.Id);

                SortedDictionary<int, Face> dicFace = oJob.Faces;
                SortedDictionary<int, Face>.Enumerator oEnumerator = dicFace.GetEnumerator();

                while (oEnumerator.MoveNext())
                {
                    Face oFace = oEnumerator.Current.Value;
                    Int32 intFaceId = oFace.Id;
                    oFace.ImageFields = GetImageFields(oFace.Id);
                    oFace.TextFields = GetTextFields(oFace.Id);
                    //oFace.CalendarFields = GetCalendarFields(oFace.Id);
                }

                //if (oJob.IsCalendar || oJob.IsYearInView)
                //{
                //    oJob.Calendar_xml = CalendarController.GetCalendarXml(oJob.Id);
                //}
                //else if (oJob.IsBusinessCardInMinutes)
                //{
                //    oJob.Bcim_xml = BcimController.GetBcimXml(oJob.Id);
                //}

                //if (oJob.IsPrintProduct || oJob.IsPhotoPrinting)
                //{
                //    JobDataAccessor da = new JobDataAccessor();
                //    oJob.Json = da.GetDesignData(oJob.Id);
                //}
            }
        }

        private Dictionary<string, TextField> GetTextFields(int id)
        {
            TextField image = new TextField();
            List<TextField> images = new List<TextField>();
            Dictionary<string, TextField> dicImage = new Dictionary<string, TextField>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                images = connection.Query<TextField>("select * from ImageField where FaceId = @FaceId", new { FaceId = id }).ToList();
            }

            foreach (TextField imageFieldRecord in images)
            {
                if (!dicImage.ContainsKey(imageFieldRecord.Name))
                {
                    dicImage.Add(imageFieldRecord.Name, imageFieldRecord);
                }
                else
                {
                    dicImage[imageFieldRecord.Name] = imageFieldRecord;
                }
            }

            return dicImage;
        }

        private Dictionary<string, ImageField> GetImageFields(int id)
        {
            ImageField image = new ImageField();
            List<ImageField> images = new List<ImageField>();
            Dictionary<string, ImageField> dicImage = new Dictionary<string, ImageField>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                images = connection.Query<ImageField>("select * from ImageField where FaceId = @FaceId", new { FaceId = id }).ToList();
            }

            foreach (ImageField imageFieldRecord in images)
            {
                if (!dicImage.ContainsKey(imageFieldRecord.Name))
                {
                    dicImage.Add(imageFieldRecord.Name, imageFieldRecord);
                }
                else
                {
                    dicImage[imageFieldRecord.Name] = imageFieldRecord;
                }
            }

            return dicImage;
        }

        private SortedDictionary<int, Face> GetFaces(Guid id)
        {
            Face face = new Face();
            List<Face> faces = new List<Face>();
            SortedDictionary<int, Face> dicFace = new SortedDictionary<int, Face>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                faces = connection.Query<Face>("select * from Face where JobId = @JobId", new { JobId = id }).ToList();
            }

            foreach (Face facerecord in faces)
            {
                if (!dicFace.ContainsKey(facerecord.FaceName))
                {
                    dicFace.Add(facerecord.FaceName, facerecord);
                }
                else
                {
                    dicFace[facerecord.FaceName] = facerecord;
                }
            }

            return dicFace;
        }

        private OrderItem GetOrderItemForJob(Guid id)
        {
            OrderItem orderItem = new OrderItem();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                orderItem = connection.Query<OrderItem>("select * from OrderItems where JobId = @JobId", new { JobId = id }).ToList().FirstOrDefault();
            }

            return orderItem;
        }

        public string GetJobSize(Guid jobId)
        {
            string jobSize = string.Empty;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MySqlConnectionString")))
            {
                connection.Open();

                jobSize = connection.Query<string>("select OptionSize from OrderItems where JobId = @JobId", new { JobId = jobId }).ToList().FirstOrDefault();
            }

            return jobSize;
        }
    }
}
