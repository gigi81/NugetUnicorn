﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

using NugetUnicorn.Business.Extensions;
using NugetUnicorn.Business.SourcesParser.ProjectParser.Sax.Parser;
using NugetUnicorn.Business.SourcesParser.ProjectParser.Structure;

namespace NugetUnicorn.Business.SourcesParser.ProjectParser
{
    public class ProjectPoco
    {
        public IReadOnlyCollection<ReferenceBase> References { get; private set; }

        public ProjectPoco(IEnumerable<ProjectStructureItem> projectStructure)
        {
            var references = new List<ReferenceBase>();

            string projectOutputName = null; // here goes project assembly name + output type

            projectStructure.Switch()
                .Case(x => x is ReferenceBase, x => references.Add(x as ReferenceBase))
                .Default(x => { })
                .Do(x => { });

            References = references.AsReadOnly();
        }
    }

    public class ProjectFileParser
    {
        public ProjectPoco Parse(string fullPath)
        {
            var projectStructure = Observable.Create<ProjectStructureItem>(
                x =>
                    {
                        var saxParser = new SaxParser();
                        var d1 = saxParser.Parse(fullPath, CurrentThreadScheduler.Instance)
                                          .Subscribe(new SaxParserObserver(x));
                        var d2 = CurrentThreadScheduler.Instance
                                                       .Schedule(x.OnCompleted);
                        return new CompositeDisposable(d1, d2);
                    }).ToEnumerable();

            return new ProjectPoco(projectStructure);
        }
    }
}