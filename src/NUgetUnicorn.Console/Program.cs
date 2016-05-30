﻿using System;
using System.Linq;

using Microsoft.Build.Execution;

using NugetUnicorn.Business.Extensions;
using NugetUnicorn.Business.FuzzyMatcher.Engine;
using NugetUnicorn.Business.FuzzyMatcher.Matchers;
using NugetUnicorn.Business.FuzzyMatcher.Matchers.ReferenceMatcher;
using NugetUnicorn.Business.Microsoft.Build;
using NugetUnicorn.Business.SourcesParser;

namespace NUgetUnicorn.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //[DS] absolute path required
            var projects = SolutionParser.GetProjects(@"D:\dev\Projects\NugetUnicorn\src\NugetUnicorn.sln")
                                         .ToList();
            var referenceMatcher = new ProbabilityMatchEngine<ProjectItemInstance>();
            referenceMatcher.With(new ReferenceMatcher.NugetReference())
                            .With(new ReferenceMatcher.SystemReference())
                            .With(new ReferenceMatcher.ExplicitReference())
                            .With(new ReferenceMatcher.DllReference())
                            .With(new ReferenceMatcher.ProjectReference());

            var wrongReferenceMatcher = new ProbabilityMatchEngine<ReferenceMatcher.DllReference.DllMetadata>();
            wrongReferenceMatcher.With(new WrongReferenceMatcher(projects));

            projects.Select(
                x =>
                    {
                        var name = x.GetProjectName() ?? "NO PROJECT NAME";
                        ConsoleEx.WriteLine(ConsoleColor.Green, $"PROJECT: {name}");
                        return x.Items;
                    })
                    .SelectMany(x => x)
                    .FindBestMatch<ProjectItemInstance, ReferenceMatcher.DllReference.DllMetadata>(referenceMatcher, 0d)
                    .FindBestMatch<ReferenceMatcher.DllReference.DllMetadata, WrongReferenceMatcher.WrongReferencePropabilityMetadata>(wrongReferenceMatcher, 0d)
                    .Do(
                        x => { ConsoleEx.WriteLine(ConsoleColor.Red, $"{x.Probability} -- {x.Project.GetProjectName()} to {x.Reference} ({x.SuspectedProject.GetProjectName()})"); });

            var nugetPackageFileParser = new ProbabilityMatchEngine<ProjectItemInstance>();
            nugetPackageFileParser.With(new NugetPackageFileMatcher());

            projects.SelectMany(x => x.Items)
                    .FindBestMatch<ProjectItemInstance, NugetPackageFileMatcher.NugetPackageFilePropabilityMetadata>(nugetPackageFileParser, 0d)
                    .Do(x => { System.Console.WriteLine($"{x.FullPath}"); });

            System.Console.ReadLine();
        }
    }
}