using FluentAssertions;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.Unit.Tests.Constants;

public class ValidationMessagesTests
{
    [Fact]
    public void ShouldCamelCasePropertyNameForMissingEntityField()
    {
        var message = ValidationMessages.MissingEntityField<ServiceRequest>(nameof(ServiceRequest.BasedOn));

        message.Should().Be("ServiceRequest.basedOn is required");
    }

    [Fact]
    public void ShouldUseProvidedLabelForMissingEntityField()
    {
        var message = ValidationMessages.MissingEntityField<ServiceRequest>("occurrencePeriod");

        message.Should().Be("ServiceRequest.occurrencePeriod is required");
    }
}
