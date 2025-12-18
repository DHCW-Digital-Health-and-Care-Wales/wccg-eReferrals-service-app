using Firely.Fhir.Packages;
using Firely.Fhir.Validation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Helpers;

namespace WCCG.eReferralsService.API.Validators
{
    public partial class FhirBundleProfileValidator : IFhirBundleProfileValidator
    {
        private static partial class Log
        {
            [LoggerMessage(Level = LogLevel.Debug, Message = "FHIR profile validation disabled (FhirValidation:Enabled=false)")]
            public static partial void FhirProfileValidationDisabled(ILogger logger);

            [LoggerMessage(Level = LogLevel.Debug, Message = "Starting FHIR profile validation (bundleType={BundleType}, entryCount={EntryCount})")]
            public static partial void StartingFhirProfileValidation(ILogger logger, string bundleType, int entryCount);

            [LoggerMessage(Level = LogLevel.Debug, Message = "Completed FHIR profile validation (issues={IssueCount})")]
            public static partial void CompletedFhirProfileValidation(ILogger logger, int issueCount);

            [LoggerMessage(Level = LogLevel.Debug, Message = "Building FHIR validator (configuredPackagePaths={ConfiguredCount}, foundPackageFiles={FoundCount})")]
            public static partial void BuildingFhirValidator(ILogger logger, int configuredCount, int foundCount);

            [LoggerMessage(Level = LogLevel.Warning, Message = "Some configured FHIR package files were not found and will be ignored (missingCount={MissingCount})")]
            public static partial void SomeConfiguredPackageFilesMissing(ILogger logger, int missingCount);

            [LoggerMessage(Level = LogLevel.Debug, Message = "Missing FHIR package paths: {MissingPaths}")]
            public static partial void MissingFhirPackagePaths(ILogger logger, string missingPaths);

            [LoggerMessage(Level = LogLevel.Debug, Message = "Using FHIR package files: {PackagePaths}")]
            public static partial void UsingFhirPackageFiles(ILogger logger, string packagePaths);
        }

        private readonly FhirValidationConfig _config;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<FhirBundleProfileValidator> _logger;

        private readonly Lazy<Validator> _validator;

        public FhirBundleProfileValidator(
            IOptions<FhirValidationConfig> config,
            IHostEnvironment hostEnvironment,
            ILogger<FhirBundleProfileValidator> logger)
        {
            _config = config.Value;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _validator = new Lazy<Validator>(BuildValidator, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public OperationOutcome Validate(Bundle bundle)
        {
            if (!_config.Enabled)
            {
                Log.FhirProfileValidationDisabled(_logger);
                return OperationOutcomeCreator.CreateEmptyOperationOutcome();
            }

            Log.StartingFhirProfileValidation(
                _logger,
                bundle.Type?.ToString() ?? "(null)",
                bundle.Entry?.Count ?? 0);

            var outcome = _validator.Value.Validate(bundle);

            outcome.Id ??= Guid.NewGuid().ToString();
            outcome.Meta ??= new Meta();
            outcome.Meta.Profile = [FhirConstants.OperationOutcomeProfile];

            Log.CompletedFhirProfileValidation(_logger, outcome.Issue?.Count ?? 0);

            return outcome;
        }

        private Validator BuildValidator()
        {
            var coreSource = ZipSource.CreateValidationSource();

            var packagePaths = _config.PackagePaths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .ToArray();

            var resolvedPackagePaths = packagePaths
                .Select(ResolvePath)
                .ToArray();

            var existingPackagePaths = resolvedPackagePaths
                .Where(File.Exists)
                .ToArray();

            var missingPackagePaths = resolvedPackagePaths
                .Where(p => !File.Exists(p))
                .ToArray();

            Log.BuildingFhirValidator(_logger, packagePaths.Length, existingPackagePaths.Length);

            if (missingPackagePaths.Length > 0)
            {
                Log.SomeConfiguredPackageFilesMissing(_logger, missingPackagePaths.Length);
                Log.MissingFhirPackagePaths(_logger, string.Join("; ", missingPackagePaths));
            }

            if (packagePaths.Length == 0)
            {
                throw new InvalidOperationException(
                    "FHIR profile validation is enabled, but no package paths are configured (FhirValidation:PackagePaths is empty).");
            }

            if (missingPackagePaths.Length > 0)
            {
                throw new InvalidOperationException(
                    "FHIR profile validation is enabled, but one or more configured package files do not exist. See debug logs for missing paths.");
            }

            Log.UsingFhirPackageFiles(_logger, string.Join("; ", existingPackagePaths));
            var packageSource = new FhirPackageSource(
                ModelInfo.ModelInspector,
                existingPackagePaths
            );

            var multiResolver = new MultiResolver(coreSource, packageSource);
            var cachedMultiResolver = new CachedResolver(multiResolver);

            var snapshotSource = new SnapshotSource(cachedMultiResolver);

            IAsyncResourceResolver resolver = new CachedResolver(snapshotSource);

            var terminologyService = new LocalTerminologyService(resolver);
            return new Validator(resolver, terminologyService);
        }

        private string ResolvePath(string packagePath)
        {
            return Path.IsPathRooted(packagePath)
                ? packagePath
                : Path.Combine(_hostEnvironment.ContentRootPath, packagePath);
        }
    }
}
