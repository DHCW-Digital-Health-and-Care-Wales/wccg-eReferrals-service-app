using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;

namespace NWRI.eReferralsService.API.Models;

public interface IBundleModel<T>
{
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
        Justification = "Static factory method for creating model instances from a FHIR Bundle.")]
    static abstract T FromBundle(Bundle bundle);
}
