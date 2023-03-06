// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.VisualStudio.VSConstants;

namespace FSharp.Editor.IntegrationTests;

public class GoToDefinitionTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task GoesToDefinition_SameFile_Async()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var shell = TestServices.Shell;
        var workspace = TestServices.Workspace;

        var code = """
module Test

let add x y = x + y

let increment = add 1
""";
        var expectedText = "let add x y = x + y";

        await solutionExplorer.CreateSolutionAsync(nameof(GoToDefinitionTests), token);
        await solutionExplorer.AddProjectAsync("Library", template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await editor.SetTextAsync(code, token);
        await solutionExplorer.BuildSolutionAsync(token);
        await editor.PlaceCaretAsync("add 1", token);

        await shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, token);
        await workspace.WaitForAsyncOperationsAsync(token);
        
        var actualText = await editor.GetTextAsync(token);
        Assert.Contains(expectedText, actualText);
    }

    [IdeFact]
    public async Task GoesToDefinition_OtherFile_Async()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var projectName = "Test";

        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var shell = TestServices.Shell;
        var workspace = TestServices.Workspace;

        var code1 = """
module Math1

let add x y = x + y
""";
        var code2 = """
module Math2

open Math1

let increment = add 1
""";
        var expectedText = "let add x y = x + y";

        await solutionExplorer.CreateSolutionAsync(nameof(GoToDefinitionTests), token);
        await solutionExplorer.AddProjectAsync(projectName, template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await solutionExplorer.AddFileAsync(projectName, "Math1.fs", code1, token);
        await solutionExplorer.AddFileAsync(projectName, "Math2.fs", code2, token);
        await solutionExplorer.OpenFileAsync(projectName, "Math2.fs", token);

        await solutionExplorer.BuildSolutionAsync(token);
        await editor.PlaceCaretAsync("add", token);

        await shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, token);
        await workspace.WaitForAsyncOperationsAsync(token);

        var actualText = await editor.GetTextAsync(token);
        Assert.Contains(expectedText, actualText);
    }
}