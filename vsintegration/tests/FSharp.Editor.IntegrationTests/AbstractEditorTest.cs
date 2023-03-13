// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Testing;
using System;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests;

public abstract class AbstractEditorTest : AbstractIntegrationTest
{
    private readonly string _solutionName;
    private readonly string _projectName;
    private readonly string _projectTemplate;

    protected AbstractEditorTest(string solutionName, string projectTemplate, string projectName)
    {
        _solutionName = solutionName ?? throw new ArgumentNullException(nameof(solutionName));
        _projectTemplate = projectTemplate ?? throw new ArgumentNullException(nameof(projectTemplate));
        _projectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
    }

    public override async Task InitializeAsync()
    {
        var solutionExplorer = TestServices.SolutionExplorer;
        var token = HangMitigatingCancellationToken;

        await base.InitializeAsync();

        await solutionExplorer.CreateSolutionAsync(_solutionName, token);
        await solutionExplorer.AddProjectAsync(_projectName, _projectTemplate, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
    }
}
