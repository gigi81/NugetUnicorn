using System.Linq;

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

using NugetUnicorn.Business.FuzzyMatcher.Engine;

namespace NugetUnicorn.Business.FuzzyMatcher.Matchers.SolutionFileParsers
{
    public class NugetPackageFileMatcher : ProbabilityMatch<ProjectItem>
    {
        private const string CONST_ITEM_TYPE_NONE = "None";

        private const string CONST_METADATA_NAME_FILENAME = "Filename";

        private const string CONST_METADATA_NAME_IDENTITY = "Identity";

        private const string CONST_FILENAME_PACKAGES_CONFIG = "packages.config";

        private const string CONST_METADATA_NAME_FULLPATH = "FullPath";

        public override ProbabilityMatchMetadata<ProjectItem> CalculateProbability(ProjectItem dataSample)
        {
            if (!string.Equals(dataSample.ItemType, CONST_ITEM_TYPE_NONE))
            {
                return base.CalculateProbability(dataSample);
            }

            var hasFilename = dataSample.Metadata.Any(x => string.Equals(x.Name, CONST_METADATA_NAME_FILENAME));
            if (!hasFilename)
            {
                return base.CalculateProbability(dataSample);
            }

            var filename = dataSample.GetMetadataValue(CONST_METADATA_NAME_IDENTITY);
            if (string.IsNullOrEmpty(filename) || !string.Equals(filename, CONST_FILENAME_PACKAGES_CONFIG))
            {
                return base.CalculateProbability(dataSample);
            }

            var hasFullPath = dataSample.Metadata.Any(x => string.Equals(x.Name, CONST_METADATA_NAME_FULLPATH));
            if (!hasFullPath)
            {
                return base.CalculateProbability(dataSample);
            }

            var fullPath = dataSample.GetMetadataValue(CONST_METADATA_NAME_FULLPATH);
            if (string.IsNullOrEmpty(fullPath))
            {
                return base.CalculateProbability(dataSample);
            }

            return new NugetPackageFilePropabilityMetadata(dataSample, this, 1d, fullPath);
        }

        public class NugetPackageFilePropabilityMetadata : SomeProbabilityMatchMetadata<ProjectItem>
        {
            public string FullPath { get; }

            public NugetPackageFilePropabilityMetadata(ProjectItem sample, ProbabilityMatch<ProjectItem> match, double probability, string fullPath)
                : base(sample, match, probability)
            {
                FullPath = fullPath;
            }
        }
    }
}