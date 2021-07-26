using AutoMapper;
using Pexeso.MappingProfile;
using Xunit;

namespace Pexeso.Web.UnitTests
{
    public class DomainToResponseProfileTests
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}