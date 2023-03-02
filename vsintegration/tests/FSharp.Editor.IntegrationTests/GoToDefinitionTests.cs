// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class GoToDefinitionTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task GoToDefinition_MethodInSameFile_Async()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var code = """
module Test

let add x y = x + y

let increment = add 1
""";

        await solutionExplorer.CreateSolutionAsync(nameof(BuildProjectTests), token);
        await solutionExplorer.AddProjectAsync("Library", template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await editor.SetTextAsync(code, token);

        await solutionExplorer.BuildSolutionAsync(token);

        await TestServices.Editor.PlaceCaretAsync("add", token);

        await TestServices.Editor.InvokeGoToDefinitionAsync(token);

        //await TestServices.Editor.WaitForCurrentLineTextAsync("private void IncrementCount()", ControlledHangMitigatingCancellationToken);
    }
}