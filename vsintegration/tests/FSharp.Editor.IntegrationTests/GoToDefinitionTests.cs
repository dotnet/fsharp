// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
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

        var code = """
module Test

let add x y = x + y

let increment = add 1
""";
        var expectedText = "let add x y = x + y";

        await SolutionExplorer.CreateSolutionAsync(nameof(GoToDefinitionTests), TestToken);
        await SolutionExplorer.AddProjectAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);
        
        await Editor.PlaceCaretAsync("add 1", TestToken);
        await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, TestToken);
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        
        Assert.Contains(expectedText, actualText);
    }
}