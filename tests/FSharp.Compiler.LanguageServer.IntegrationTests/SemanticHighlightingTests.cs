// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Shell;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Compiler.LanguageServer.IntegrationTests;

[TestClass]
public class SemanticHighlightingTests : FSharpApexTestBase
{
    private const string Code = """
        namespace Sample

        module Test =
            let hello name =
                printfn "Hello %s" name
        """;

    [TestMethod]
    [TestCategory("Integration")]
    public void LspSemanticHighlighting_ClassifiesFunctionsLocalsAndPrintfSpecifier()
    {
        SetSemanticClassificationProvider("lsp");
        RestartVisualStudioHost();

        var solution = VsHost.ObjectModel.Solution;
        _ = solution.CreateProject(ProjectLanguage.FSharp, ProjectTemplate.Default);

        var document = VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        var editor = document.Editor;

        editor.Selection.SelectAll();
        editor.KeyboardCommands.Delete();
        editor.Edit.InsertTextInBuffer(Code);

        AssertClassification(editor, "Sample", ["namespace name"]);
        AssertClassification(editor, "Test", ["class name"]);
        AssertClassification(editor, "hello", ["method name"]);
        AssertClassification(editor, "name", ["local name"], occurrence: 2);
        AssertClassification(editor, "printfn", ["method name"]);
        AssertClassification(editor, "name", ["local name"], occurrence: 3);
        AssertClassification(editor, "%s", ["string - escape character"]);
    }

    private void SetSemanticClassificationProvider(string provider)
    {
        var settings = VsHost.Get<UnifiedSettingsService>();
        settings.SetValue("fsharp.semanticClassificationProvider", provider);
    }

    private static void AssertClassification(
        ITextEditorTestExtension editor,
        string text,
        IReadOnlyCollection<string> expectedClassifications,
        int occurrence = 1)
    {
        var classification = WaitForClassification(editor, text, expectedClassifications, occurrence);

        Assert.IsTrue(
            IsExpectedClassification(classification.Name, expectedClassifications),
            $"Expected '{text}' occurrence {occurrence} to have one of classifications " +
            $"[{string.Join(", ", expectedClassifications)}], but got '{classification.Name}'. " +
            $"All classifications: {string.Join(", ", editor.Classification.GetAllClassifications().Select(c => $"{c.Contents}:{c.Name}"))}");
    }

    private static Classification WaitForClassification(
        ITextEditorTestExtension editor,
        string text,
        IReadOnlyCollection<string> expectedClassifications,
        int occurrence)
    {
        var sw = Stopwatch.StartNew();
        Classification? lastClassification = null;

        while (sw.Elapsed < TimeSpan.FromSeconds(120))
        {
            var position = PositionOf(editor.Contents, text, occurrence);
            lastClassification = editor.Classification.GetClassification(editor.GetPositionFromBufferLocation(position));

            if (lastClassification is not null && IsExpectedClassification(lastClassification.Name, expectedClassifications))
            {
                return lastClassification;
            }

            System.Threading.Thread.Sleep(500);
        }

        Assert.Fail(
            $"Timed out waiting for '{text}' occurrence {occurrence} to have one of classifications " +
            $"[{string.Join(", ", expectedClassifications)}]. Last classification: '{lastClassification?.Name ?? "<none>"}'.");

        throw new InvalidOperationException("Unreachable");
    }

    private static bool IsExpectedClassification(string actualClassification, IReadOnlyCollection<string> expectedClassifications)
    {
        var actualClassificationParts = actualClassification.Split(';');

        return expectedClassifications.Any(expectedClassification =>
            string.Equals(actualClassification, expectedClassification, StringComparison.Ordinal) ||
            actualClassificationParts.Any(part => string.Equals(part, expectedClassification, StringComparison.Ordinal)) ||
            actualClassificationParts.Any(part => string.Equals(part, $"Semantic - {expectedClassification}", StringComparison.Ordinal)) ||
            actualClassification.Contains($"Semantic - {expectedClassification}", StringComparison.Ordinal));
    }

    private static int PositionOf(string contents, string text, int occurrence)
    {
        var start = 0;

        for (var i = 0; i < occurrence; i++)
        {
            var index = contents.IndexOf(text, start, StringComparison.Ordinal);
            Assert.AreNotEqual(-1, index, $"Could not find occurrence {occurrence} of '{text}'.");
            start = index + text.Length;

            if (i == occurrence - 1)
            {
                return index;
            }
        }

        throw new InvalidOperationException("Unreachable");
    }
}
