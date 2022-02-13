using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AdaTheDev.TableStorageBulkInsert.Startup))]

namespace AdaTheDev.TableStorageBulkInsert
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            var settings = new Settings { AzureStorageConnectionString = configuration["AzureWebJobsStorage"]};
     
            builder.Services.AddSingleton(settings);
        }
    }
}