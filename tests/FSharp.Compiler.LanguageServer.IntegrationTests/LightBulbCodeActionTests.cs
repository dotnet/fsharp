// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Compiler.LanguageServer.IntegrationTests;

[TestClass]
public class LightBulbCodeActionTests : FSharpApexTestBase
{
    private const string CodeWithMissingNewKeyword = """
        module Test

        let x = System.IO.FileStream("dummy.txt", System.IO.FileMode.Create)
        """;

    private const string CodeWithEqualityOperator = """
        module Test

        let x = 1 == 2
        """;

    private const string CodeWithNotMutableAssignment = """
        module Test

        let x = 1
        x <- 2
        """;

    private const string CodeWithMissingFunKeyword = """
        module Test

        let f = x -> x + 1
        """;

    [TestMethod]
    [TestCategory("Integration")]
    public void ConstructorCall_OffersCodeActionToAddNewKeyword()
    {
        // Arrange
        var solution = VsHost.ObjectModel.Solution;
        _ = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithMissingNewKeyword);

        // Position the caret on the constructor call line so the light bulb
        // appears at the location with the diagnostic.
        MoveCaret(editor, line: 3);

        ILightBulbService? lightBulbService = TryGetLightBulbService(editor);
        Assert.IsNotNull(lightBulbService, "Could not access light bulb service from the active text editor.");

        // Act: create a light bulb session and read actions, retrying if
        // the session is dismissed (e.g., when diagnostics refresh).
        // CreateLightBulbSession blocks until VS has computed code actions,
        // so it implicitly waits for the language server to produce diagnostics.
        var actionTitles = WaitForLightBulbActions(lightBulbService, timeout: TimeSpan.FromSeconds(120));

        // Assert
        bool hasAddNewKeywordAction = actionTitles.Any(title =>
            title.Contains("Add 'new' keyword", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            hasAddNewKeywordAction,
            $"Expected a light bulb action containing \"Add 'new' keyword\" but found: {string.Join(", ", actionTitles)}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void EqualityOperator_OffersConvertToEquals()
    {
        // Arrange
        var solution = VsHost.ObjectModel.Solution;
        _ = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithEqualityOperator);

        MoveCaret(editor, line: 3);

        ILightBulbService? lightBulbService = TryGetLightBulbService(editor);
        Assert.IsNotNull(lightBulbService, "Could not access light bulb service from the active text editor.");

        // Act
        var actionTitles = WaitForLightBulbActions(lightBulbService, timeout: TimeSpan.FromSeconds(120));

        // Assert
        bool hasEqualsAction = actionTitles.Any(title =>
            title.Contains("Use '=' for equality", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            hasEqualsAction,
            $"Expected a light bulb action containing \"Use '=' for equality\" but found: {string.Join(", ", actionTitles)}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void NotMutableAssignment_OffersMakeDeclarationMutable()
    {
        // Arrange
        var solution = VsHost.ObjectModel.Solution;
        _ = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithNotMutableAssignment);

        MoveCaret(editor, line: 4);

        ILightBulbService? lightBulbService = TryGetLightBulbService(editor);
        Assert.IsNotNull(lightBulbService, "Could not access light bulb service from the active text editor.");

        // Act
        var actionTitles = WaitForLightBulbActions(lightBulbService, timeout: TimeSpan.FromSeconds(120));

        // Assert
        bool hasMutableAction = actionTitles.Any(title =>
            title.Contains("mutable", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            hasMutableAction,
            $"Expected a light bulb action containing \"mutable\" but found: {string.Join(", ", actionTitles)}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void MissingFunKeyword_OffersAddFunKeyword()
    {
        // Arrange
        var solution = VsHost.ObjectModel.Solution;
        _ = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(CodeWithMissingFunKeyword);

        MoveCaret(editor, line: 3);

        ILightBulbService? lightBulbService = TryGetLightBulbService(editor);
        Assert.IsNotNull(lightBulbService, "Could not access light bulb service from the active text editor.");

        // Act
        var actionTitles = WaitForLightBulbActions(lightBulbService, timeout: TimeSpan.FromSeconds(120));

        // Assert
        bool hasFunAction = actionTitles.Any(title =>
            title.Contains("Add 'fun' keyword", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            hasFunAction,
            $"Expected a light bulb action containing \"Add 'fun' keyword\" but found: {string.Join(", ", actionTitles)}");
    }

    private static ILightBulbService? TryGetLightBulbService(object editor)
    {
        var lightBulbProperty = editor.GetType().GetProperty("LightBulb", BindingFlags.Public | BindingFlags.Instance);
        return lightBulbProperty?.GetValue(editor) as ILightBulbService;
    }

    private static void MoveCaret(object editor, int line)
    {
        var caretProperty = editor.GetType().GetProperty("Caret", BindingFlags.Public | BindingFlags.Instance);
        var caret = caretProperty?.GetValue(editor);
        var moveMethod = caret?.GetType().GetMethod("MoveToLine", [typeof(int)]);
        moveMethod?.Invoke(caret, [line]);
    }

    /// <summary>
    /// Creates a light bulb session, expands it, and reads action titles.
    /// <see cref="ILightBulbService.CreateLightBulbSession"/> is a blocking RPC
    /// call that can hang indefinitely when no lightbulb is available. Each
    /// attempt is run on a thread-pool thread with a per-call timeout so the
    /// test can fail cleanly instead of hanging.
    /// </summary>
    private static IReadOnlyList<string> WaitForLightBulbActions(
        ILightBulbService lightBulbService,
        TimeSpan timeout)
    {
        var sw = Stopwatch.StartNew();
        var perCallTimeout = TimeSpan.FromSeconds(30);

        while (sw.Elapsed < timeout)
        {
            try
            {
                // Run the blocking RPC call on a background thread so we can
                // enforce a per-call timeout and avoid hanging the test.
                using var cts = new CancellationTokenSource();
                var sessionTask = Task.Run(() =>
                    lightBulbService.CreateLightBulbSession(LightBulbPriority.Medium), cts.Token);

                var remaining = timeout - sw.Elapsed;
                var waitTime = remaining < perCallTimeout ? remaining : perCallTimeout;

                if (!sessionTask.Wait(waitTime))
                {
                    // CreateLightBulbSession is still blocking — cancel and suppress
                    // any unobserved exception before retrying.
                    cts.Cancel();
                    sessionTask.ContinueWith(t => _ = t.Exception, TaskContinuationOptions.OnlyOnFaulted);
                    Thread.Sleep(2000);
                    continue;
                }

                var lightBulb = sessionTask.Result;

                if (lightBulb == null)
                {
                    Thread.Sleep(2000);
                    continue;
                }

                if (!lightBulb.IsExpanded)
                {
                    lightBulb.Expand();
                }

                List<string> titles = [];
                foreach (LightBulbSuggestedAction action in lightBulb.Actions)
                {
                    CollectActionTitles(action, titles);
                }

                if (titles.Count > 0)
                {
                    return titles;
                }

                // Dismiss the empty session before retrying.
                lightBulb.Dismiss();
                Thread.Sleep(2000);
            }
            catch (EditorException)
            {
                // The light bulb session was dismissed (e.g., diagnostics refreshed).
                // Wait for the language server to stabilize and retry.
                Thread.Sleep(5000);
            }
            catch (RemotingException)
            {
                // VS process crashed or IPC pipe broke.
                VsHost = null!;
                Assert.Fail("VS host process crashed (IPC pipe ended) while waiting for light bulb actions.");
                return [];
            }
            catch (AggregateException ex) when (ex.InnerException is RemotingException)
            {
                VsHost = null!;
                Assert.Fail("VS host process crashed (IPC pipe ended) while waiting for light bulb actions.");
                return [];
            }
            catch (AggregateException ex) when (ex.InnerException is EditorException)
            {
                Thread.Sleep(5000);
            }
        }

        return [];
    }

    private static void CollectActionTitles(LightBulbSuggestedAction action, ICollection<string> titles)
    {
        titles.Add(action.Text ?? string.Empty);

        foreach (LightBulbSuggestedAction nestedAction in action.NestedActions)
        {
            CollectActionTitles(nestedAction, titles);
        }
    }
}

