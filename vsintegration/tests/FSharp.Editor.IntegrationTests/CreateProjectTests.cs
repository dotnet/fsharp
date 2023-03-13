// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class CreateProjectTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task ClassLibrary()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;

        var expectedCode = """
namespace Library

module Say =
    let hello name =
        printfn "Hello %s" name

""";

        await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
        await solutionExplorer.AddProjectAsync("Library", template, token);

        var actualCode = await editor.GetTextAsync(token);

        Assert.Equal(expectedCode, actualCode);
    }

    [IdeFact]
    public async Task ConsoleApp()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreConsoleApplication;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;

        var expectedCode = """
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

""";

        await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
        await solutionExplorer.AddProjectAsync("ConsoleApp", template, token);

        var actualCode = await editor.GetTextAsync(token);

        Assert.Equal(expectedCode, actualCode);
    }

    [IdeFact]
    public async Task XUnitTestProject()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreXUnitTest;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;

        var expectedCode = """
module Tests

open System
open Xunit

[<Fact>]
let ``My test`` () =
    Assert.True(true)

""";

        await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
        await solutionExplorer.AddProjectAsync("ConsoleApp", template, token);

        var actualCode = await editor.GetTextAsync(token);

        Assert.Equal(expectedCode, actualCode);
    }
}