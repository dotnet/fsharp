// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class CodeActionTests : AbstractIntegrationTest
{
    // "Remove unused open declarations" comes from F#'s unused-opens analyzer - a background
    // SemanticDocumentAnalysis whose production is non-deterministic for a freshly-opened single-file project
    // in the headless CI VS, so this test is currently flaky (kept active, not quarantined).
    [IdeFact]
    public async Task UnusedOpenDeclarations()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var code = """
module Library

open System

let x = 42
""";

        Trace.TraceInformation("[UnusedOpenDeclarations] Creating solution...");
        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Restoring NuGet packages...");
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Setting editor text...");
        await Editor.SetTextAsync(code, TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Placing caret at 'open System'...");
        await Editor.PlaceCaretAsync("open System", TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Waiting for project system...");
        await Workspace.WaitForProjectSystemAsync(TestToken);

        // Dump error list before invoking code actions to see if any diagnostics have been produced
        var preEntries = await ErrorList.GetAllEntriesAsync(TestToken);
        Trace.TraceInformation("[UnusedOpenDeclarations] Pre-invoke error list: {0} entries", preEntries.Length);
        foreach (var entry in preEntries)
        {
            Trace.TraceInformation("[UnusedOpenDeclarations]   entry: {0}", entry);
        }

        Trace.TraceInformation("[UnusedOpenDeclarations] Invoking code action list...");
        var codeActions = await Editor.InvokeCodeActionListAsync(TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Waiting for project system (post-invoke)...");
        await Workspace.WaitForProjectSystemAsync(TestToken);

        Trace.TraceInformation("[UnusedOpenDeclarations] Asserting results: codeActions count={0}", codeActions.Count());

        Assert.Single(codeActions);
        var actionSet = codeActions.Single();
        Assert.Equal("CodeFix", actionSet.CategoryName);

        Assert.Single(actionSet.Actions);
        var codeFix = actionSet.Actions.Single();
        Assert.Equal("Remove unused open declarations", codeFix.DisplayText);
        Trace.TraceInformation("[UnusedOpenDeclarations] PASSED");
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
