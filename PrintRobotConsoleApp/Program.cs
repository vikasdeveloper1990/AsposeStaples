namespace PrintRobotConsoleApp
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using PrintRobot.Services.DataServices;
    using PrintRobot.Services.Models.Options;
    using PrintRobot.Services.PrintServices;
    using PrintRobot.Services.PrintServices.VendorServices;

    [ExcludeFromCodeCoverage]
    class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        static async Task Main(string[] args)
        {
            // This will load the content of .env file and create related environment variables
            DotNetEnv.Env.TraversePath().Load();
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }


        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();
                configuration
                        .AddEnvironmentVariables()
                        .AddJsonFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + $"/appsettings-local.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configurationRoot = configuration.Build();
                Configuration = configurationRoot;
            }).ConfigureServices((services) =>
            {
                ConfigureOptions<RoboOptions>(nameof(RoboOptions), services);
                ConfigureOptions<AppSettings>(nameof(AppSettings), services);
                services.AddTransient<PrintProcessor, ShopifyService>();
                services.AddSingleton<IPrintProcessorFactory, PrintProcessorFactory>();
                
                services.AddTransient<IDataService, DataService>();
                IServiceCollection serviceCollection = services.AddTransient<IPrintWorkerService, PrintWorkerService>();
                services.AddHostedService<WorkerService>();

                SetAsposLicesnse();

            });


        private static void SetAsposLicesnse()
        {
            Aspose.Pdf.License pdflicense = new Aspose.Pdf.License();

            Aspose.Drawing.License drawinglicense = new Aspose.Drawing.License();

            Console.WriteLine($"Aspose License File : {Configuration.GetValue<string>("Appsettings:AposePDFLicense")}");
            pdflicense.SetLicense(Configuration.GetValue<string>("Appsettings:AposePDFLicense"));
            drawinglicense.SetLicense(Configuration.GetValue<string>("Appsettings:AposePDFLicense"));
            pdflicense.Embedded = true;
        }


        private static void ConfigureOptions<TOptions>(string optionsName, IServiceCollection services) where TOptions : class, new()
        {
            var optionsSection = Configuration.GetSection(optionsName);
            services.Configure<TOptions>(optionsSection);
            var options = new TOptions();
            optionsSection.Bind(options);
            var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(options, null, null);
            Validator.ValidateObject(options, ctx, true);
        }
    }
}
