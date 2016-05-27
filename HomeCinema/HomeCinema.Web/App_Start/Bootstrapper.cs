using HomeCinema.Web.Infrastructure.Mappings;
using System.Web.Http;

namespace HomeCinema.Web
{
    public class Bootstrapper
    {
        public static void Run()
        {
            // Configure Autofac
            AutofacWebapiConfig.Initialize(GlobalConfiguration.Configuration);

            // Configure AutoMapper
            AutoMapperConfiguration.Configure();
        }
    }
}