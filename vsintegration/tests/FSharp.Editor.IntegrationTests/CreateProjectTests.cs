// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class CreateProjectTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task ClassLibrary()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var expectedCode = """
namespace Library

module Say =
    let hello name =
        printfn "Hello %s" name

""";
        await SolutionExplorer.CreateSingleProjectSolutionAsync(template, TestToken);

        var actualCode = await Editor.GetTextAsync(TestToken);

        Assert.Equal(expectedCode, actualCode);
    }

    [IdeFact]
    public async Task ConsoleApp()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreConsoleApplication;
        var expectedCode = """
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

""";
        await SolutionExplorer.CreateSingleProjectSolutionAsync(template, TestToken);

        var actualCode = await Editor.GetTextAsync(TestToken);

        Assert.Equal(expectedCode, actualCode);
    }

    [IdeFact]
    public async Task XUnitTestProject()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreXUnitTest;
        var expectedCode = """
module Tests

open System
open Xunit

[<Fact>]
let ``My test`` () =
    Assert.True(true)

""";
        await SolutionExplorer.CreateSingleProjectSolutionAsync(template, TestToken);

        var actualCode = await Editor.GetTextAsync(TestToken);

        Assert.Equal(expectedCode, actualCode);
    }
}