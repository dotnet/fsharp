// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Threading.Tasks;
using WindowsInput.Native;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class NavigationTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task Navigate_Async()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var shell = TestServices.Shell;
        var editor = TestServices.Editor;
        var input = TestServices.Input;
        var workarounds = TestServices.Workarounds;
        var projectName = "Library";
        var code1 = """
namespace Library

module Math1 = 
    let add x y = x + y

""";
        var code2 = """
namespace Library

module Math2 = 
    let subtract x y = x - y

""";

        await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
        await solutionExplorer.AddProjectAsync(projectName, template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await solutionExplorer.AddFileAsync(projectName, "Math1.fs", code1, token);
        await solutionExplorer.AddFileAsync(projectName, "Math2.fs", code2, token);
        await solutionExplorer.OpenFileAsync(projectName, "Math1.fs", token);

        await shell.ShowNavigateToDialogAsync(token);
        await input.SendToNavigateToAsync(new InputKey[] { "subtract", VirtualKeyCode.RETURN }, token);

        await workarounds.WaitForNavigationAsync(token);
        Assert.Equal("Math2.fs", await shell.GetActiveWindowCaptionAsync(token));
        Assert.Equal("subtract", await editor.GetSelectedTextAsync(token));
    }
}
