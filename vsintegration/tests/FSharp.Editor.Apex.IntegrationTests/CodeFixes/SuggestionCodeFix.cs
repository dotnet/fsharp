// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Ported verbatim from the TypeScript-VS repository
// (VS/LanguageService/Tests/Integration/CodeFixes/SuggestionCodeFix.cs) and then adjusted so that
// the setup is tailored for the F# extension. A single test case is kept, exercising an F# code fix
// (Wrap expression in parentheses, FS0597). The triggering code is taken from the F# code-fix unit
// tests: vsintegration/tests/FSharp.Editor.Tests/CodeFixes/WrapExpressionInParenthesesTests.fs.

using System;
using System.Linq;
using FSharp.Editor.Apex.IntegrationTests.TestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests
{
    [TestClass]
    public class SuggestionCodeFix : FSharpLanguageServiceApexTest
    {
        #region Private members
        protected override bool AutomaticallyDismissMessageBoxes => false;
        private const string code = @"module Library

let rng = System.Random()

printfn ""Hello %d"" rng.Next(5)";
        private const string expectedCode = @"module Library

let rng = System.Random()

printfn ""Hello %d"" (rng.Next(5))";
        #endregion

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify code-fix offers suggestion to wrap an expression in parentheses in an F# file.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void SuggestionCodeFixInFSharpFile()
        {
            var document = this.OpenFSharpDocument(code, "rng.Next");

            this.InvokeCodeFix(document, "rng.Next");
            this.Library.Synchronization.WaitForSolutionCrawler();

            this.Library.Synchronization.WaitFor(() => expectedCode == document.Contents, TimeSpan.FromSeconds(15));
        }

        #region Helps
        public void InvokeCodeFix(TextDocumentView document, string expression)
        {
            var lightBulb = this.MoveToExpressionAndExpandLightBulb(document, expression);
            var globalScopeAction = lightBulb.Actions.Single(action =>
                action.Text.IndexOf("Wrap", StringComparison.OrdinalIgnoreCase) >= 0
            );

            globalScopeAction.Invoke();
        }
        #endregion
    }
}
