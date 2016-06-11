using System.Linq;

using Microsoft.Build.Execution;

namespace NugetUnicorn.Business.Microsoft.Build
{
    public static class ProjectInstanceExtensions
    {
        private const string CONS_TARGET_FILE_NAME = "TargetFileName";

        private const string CONS_MSBUILD_PROJECT_NAME = "MSBuildProjectName";

        public static string GetTargetFileName(this ProjectInstance projectInstance)
        {
            return projectInstance.Properties?
                                  .FirstOrDefault(y => string.Equals(y.Name, CONS_TARGET_FILE_NAME))?
                                  .EvaluatedValue;
        }

        public static string GetProjectName(this ProjectInstance projectInstance)
        {
            return projectInstance?.Properties?
                                   .FirstOrDefault(y => string.Equals(y.Name, CONS_MSBUILD_PROJECT_NAME))?
                                   .EvaluatedValue;
        }
    }
}