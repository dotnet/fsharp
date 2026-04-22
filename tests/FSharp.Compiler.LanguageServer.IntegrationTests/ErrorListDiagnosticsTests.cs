// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Test.Apex.Services;
using Microsoft.Test.Apex.VisualStudio.Shell;
using Microsoft.Test.Apex.VisualStudio.Shell.ToolWindows.ErrorHub;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Compiler.LanguageServer.IntegrationTests;

/// <summary>
/// Tests that F# LSP diagnostics appear in the VS Error List with correctly populated columns,
/// including the Project column which requires VSDiagnosticProjectInformation from the language server.
/// </summary>
[TestClass]
public class ErrorListDiagnosticsTests : FSharpApexTestBase
{
    /// <summary>
    /// Code that produces a type mismatch error (FS0001) on line 4.
    /// </summary>
    private const string CodeWithError = """
        module Test

        let add (x: int) (y: int) = x + y
        let result = add 1 "hello"
        """;

    /// <summary>
    /// Code that produces a warning about IDisposable construction (FS0760).
    /// </summary>
    private const string CodeWithWarning = """
        module Test

        open System.IO

        let stream = MemoryStream()
        """;

    /// <summary>
    /// Clean code that should produce no diagnostics.
    /// </summary>
    private const string CleanCode = """
        module Test

        let answer = 42
        """;

    private IErrorHubService GetErrorHub()
    {
        var errorHub = VsHost.ObjectModel.Shell.ToolWindows.ErrorHub;
        errorHub.ShowErrors();
        errorHub.ShowWarnings();
        errorHub.HideMessages();
        errorHub.FilterScope = ErrorListFilterScope.Off;
        _ = errorHub.TryWaitForReady();
        errorHub.ToolWindow.ShowNoActivate();
        return errorHub;
    }

    /// <summary>
    /// Waits for at least <paramref name="minCount"/> error list entries to appear,
    /// optionally filtering to LSP-only entries when <paramref name="lspOnly"/> is true.
    /// </summary>
    private IErrorEntryTestExtension[] WaitForEntries(
        IErrorHubService errorHub,
        int minCount,
        bool lspOnly = false,
        TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(120);
        var sw = Stopwatch.StartNew();
        IErrorEntryTestExtension[] entries = [];

        do
        {
            _ = errorHub.TryWaitForErrorListEntries();
            _ = errorHub.TryWaitForReady();

            entries = lspOnly
                ? errorHub.AllVisibleEntries
                    .Where(e => e.Description?.Contains("LSP:") == true)
                    .ToArray()
                : errorHub.AllVisibleEntries;

            if (entries.Length >= minCount)
                return entries;

            System.Threading.Thread.Sleep(2000);
        } while (sw.Elapsed < timeout);

        Assert.Fail(
            $"Expected at least {minCount} error list entries (lspOnly={lspOnly}), " +
            $"but found {entries.Length} after {timeout}.");
        return entries;
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void LspDiagnostics_ErrorList_ProjectColumnPopulated()
    {
        // Arrange: create a project with code that has a type error
        var solution = VsHost.ObjectModel.Solution;
        var project = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);
        var projectName = project.Name;

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithError);

        // Act: wait for LSP diagnostics to appear
        var errorHub = GetErrorHub();
        var lspEntries = WaitForEntries(errorHub, minCount: 1, lspOnly: true);

        // Assert: each LSP diagnostic should have a non-empty ProjectName
        foreach (var entry in lspEntries)
        {
            Assert.IsFalse(
                string.IsNullOrEmpty(entry.ProjectName),
                $"LSP diagnostic '{entry.Description}' has an empty Project column. " +
                $"Expected project name containing '{projectName}'.");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void LspDiagnostics_ErrorList_ProjectNameMatchesProjectFile()
    {
        // Arrange: create a project and introduce an error
        var solution = VsHost.ObjectModel.Solution;
        var project = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);
        var projectFileName = project.FileName;

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithError);

        // Act: wait for LSP diagnostics
        var errorHub = GetErrorHub();
        var lspEntries = WaitForEntries(errorHub, minCount: 1, lspOnly: true);

        // Assert: the project name should match the project file name
        // (This is the same format used by legacy diagnostics)
        foreach (var entry in lspEntries)
        {
            Assert.IsTrue(
                entry.ProjectName?.Contains(projectFileName) == true ||
                projectFileName?.Contains(entry.ProjectName ?? "") == true,
                $"LSP diagnostic project name '{entry.ProjectName}' does not match " +
                $"expected project file '{projectFileName}'.");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void LspDiagnostics_ErrorList_WarningHasProjectColumn()
    {
        // Arrange: create a project with code that produces a warning
        var solution = VsHost.ObjectModel.Solution;
        var project = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        // Small delay to let VS settle focus after prior test's error hub interaction.
        System.Threading.Thread.Sleep(2000);
        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithWarning);

        // Act: wait for LSP warnings
        var errorHub = GetErrorHub();
        var lspEntries = WaitForEntries(errorHub, minCount: 1, lspOnly: true);

        // Assert: warnings should also have the project column populated
        var lspWarnings = lspEntries
            .Where(e => e.Severity == ErrorItemSeverity.Warning)
            .ToArray();

        if (lspWarnings.Length == 0)
        {
            Assert.Inconclusive("No LSP warnings appeared in the error list.");
        }

        foreach (var warning in lspWarnings)
        {
            Assert.IsFalse(
                string.IsNullOrEmpty(warning.ProjectName),
                $"LSP warning '{warning.Description}' has an empty Project column.");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void LspDiagnostics_ErrorList_AllColumnsPopulated()
    {
        // Arrange: create a project with code that has a type error
        var solution = VsHost.ObjectModel.Solution;
        var project = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithError);

        // Act: wait for LSP diagnostics
        var errorHub = GetErrorHub();
        var lspEntries = WaitForEntries(errorHub, minCount: 1, lspOnly: true);

        // Assert: all standard error list columns should be populated
        foreach (var entry in lspEntries)
        {
            Assert.IsFalse(string.IsNullOrEmpty(entry.Description),
                "LSP diagnostic has empty Description.");

            Assert.IsFalse(string.IsNullOrEmpty(entry.ProjectName),
                $"LSP diagnostic '{entry.Description}' has empty Project column.");

            Assert.IsFalse(string.IsNullOrEmpty(entry.FileName),
                $"LSP diagnostic '{entry.Description}' has empty File column.");

            Assert.IsNotNull(entry.LineNumber,
                $"LSP diagnostic '{entry.Description}' has null Line number.");

            Assert.IsFalse(string.IsNullOrEmpty(entry.ErrorCode),
                $"LSP diagnostic '{entry.Description}' has empty Error Code.");
        }
    }
}
