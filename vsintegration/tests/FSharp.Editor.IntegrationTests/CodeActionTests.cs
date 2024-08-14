// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class CodeActionTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task UnusedOpenDeclarations()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var code = """
module Library

open System

let x = 42
""";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);
        await Editor.PlaceCaretAsync("open System", TestToken);

        await Workspace.WaitForProjectSystemAsync(TestToken);
        var codeActions = await Editor.InvokeCodeActionListAsync(TestToken);
        await Workspace.WaitForProjectSystemAsync(TestToken);

        Assert.Single(codeActions);
        var actionSet = codeActions.Single();
        Assert.Equal("CodeFix", actionSet.CategoryName);

        Assert.Single(actionSet.Actions);
        var codeFix = actionSet.Actions.Single();
        Assert.Equal("Remove unused open declarations", codeFix.DisplayText);
    }

    [IdeFact]
    public async Task AddMissingFunKeyword()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var code = """
module Library

let original = []
let transformed = original |> List.map (x -> x)
""";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);
        await Editor.PlaceCaretAsync("->", TestToken);

        await Workspace.WaitForProjectSystemAsync(TestToken);
        var codeActions = await Editor.InvokeCodeActionListAsync(TestToken);
        await Workspace.WaitForProjectSystemAsync(TestToken);

        Assert.Single(codeActions);
        var actionSet = codeActions.Single();
        Assert.Equal("ErrorFix", actionSet.CategoryName);

        Assert.Single(actionSet.Actions);
        var errorFix = actionSet.Actions.Single();
        Assert.Equal("Add missing 'fun' keyword", errorFix.DisplayText);
    }

    [IdeFact]
    public async Task AddNewKeywordToDisposables()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var code = """
module Library

let sr = System.IO.StreamReader("")
""";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);
        await Editor.PlaceCaretAsync("let sr", TestToken);

        await Workspace.WaitForProjectSystemAsync(TestToken);
        var codeActions = await Editor.InvokeCodeActionListAsync(TestToken);
        await Workspace.WaitForProjectSystemAsync(TestToken);

        Assert.Single(codeActions);
        var actionSet = codeActions.Single();
        Assert.Equal("CodeFix", actionSet.CategoryName);

        Assert.Single(actionSet.Actions);
        var codeFix = actionSet.Actions.Single();
        Assert.Equal("Add 'new' keyword", codeFix.DisplayText);
    }
}
