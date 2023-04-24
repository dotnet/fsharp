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

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);
        
        await Editor.PlaceCaretAsync("add 1", TestToken);
        await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, TestToken);
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        
        Assert.Contains(expectedText, actualText);
    }

    [IdeFact]
    public async Task FsiAndFsFilesGoToCorrespondentDefinitions()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var fsi = """
module Module

type SomeType =
| Number of int
| Letter of char

val id: t: SomeType -> SomeType
""";
        var fs = """
module Module

type SomeType =
    | Number of int
    | Letter of char

let id (t: SomeType) = t
""";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);

        // hack: when asked to add a file, VS API seems to insert it in the alphabetical order
        // so adding Module.fsi and Module.fs we'll end up having signature file below the code file
        // and this won't work. But it's possible to achieve having the right file order via their renaming
        await SolutionExplorer.AddFileAsync("Library", "AModule.fsi", fsi, TestToken);
        await SolutionExplorer.AddFileAsync("Library", "Module.fs", fs, TestToken);
        await SolutionExplorer.RenameFileAsync("Library", "AModule.fsi", "Module.fsi", TestToken);
        await SolutionExplorer.BuildSolutionAsync(TestToken);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fsi", TestToken);
        await Editor.PlaceCaretAsync("SomeType ->", TestToken);
        await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, TestToken);
        var expectedText = "type SomeType =";
        var expectedWindow = "Module.fsi";
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        var actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fs", TestToken);
        await Editor.PlaceCaretAsync("SomeType)", TestToken);
        await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, TestToken);
        expectedText = "type SomeType =";
        expectedWindow = "Module.fs";
        actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);
    }
}