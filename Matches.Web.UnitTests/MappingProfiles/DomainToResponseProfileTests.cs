using AutoMapper;
using Matches.MappingProfile;
using Xunit;

namespace Matches.Web.UnitTests.MappingProfiles
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