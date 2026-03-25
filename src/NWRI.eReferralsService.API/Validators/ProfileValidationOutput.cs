namespace NWRI.eReferralsService.API.Validators;

public class ProfileValidationOutput
{
    public bool IsSuccessful { get; init; }
    public List<string>? Errors { get; init; }
}
