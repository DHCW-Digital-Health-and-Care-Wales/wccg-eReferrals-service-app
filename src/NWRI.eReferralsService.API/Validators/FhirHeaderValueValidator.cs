using System.Buffers;
using System.Text.Json;
using Hl7.Fhir.Model;

namespace NWRI.eReferralsService.API.Validators;

public sealed class FhirHeaderValueValidator
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public FhirHeaderValueValidator(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public bool IsValid<T>(string? value) where T : Base
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var span = value.AsSpan();
        var maxSize = System.Buffers.Text.Base64.GetMaxDecodedFromUtf8Length(span.Length);
        var rentedBuffer = ArrayPool<byte>.Shared.Rent(maxSize);

        try
        {
            if (!Convert.TryFromBase64Chars(span, rentedBuffer, out var writtenBytes))
            {
                return false;
            }

            var jsonBytes = new ReadOnlySpan<byte>(rentedBuffer, 0, writtenBytes);
            JsonSerializer.Deserialize<T>(jsonBytes, _jsonSerializerOptions);

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }
}
