using NWRI.eReferralsService.API.Errors;

namespace NWRI.eReferralsService.API.Exceptions;

public sealed class ProxyServerException : BaseFhirException
{
    public ProxyServerException(string message)
    {
        Message = message;
        Errors = [new ProxyServerError(message)];
    }

    public override IEnumerable<BaseFhirHttpError> Errors { get; }

    public override string Message { get; }
}
