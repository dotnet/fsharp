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
    public async Task GoesToDefinition()
    {
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

        await solutionExplorer.CreateSolutionAsync(nameof(GoToDefinitionTests), TestToken);
        await solutionExplorer.AddProjectAsync("Library", template, TestToken);
        await solutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await editor.SetTextAsync(code, TestToken);
        
        await editor.PlaceCaretAsync("add 1", TestToken);
        await shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, TestToken);
        var actualText = await editor.GetCurrentLineTextAsync(TestToken);
        
        Assert.Contains(expectedText, actualText);
    }
}