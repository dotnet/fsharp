// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Apex counterparts of the xUnit/IdeFact tests in
// vsintegration/tests/FSharp.Editor.IntegrationTests/CodeActionTests.cs. Each case sets up the same
// triggering F# source, places the caret where that test does, and verifies the same code/error fix
// is offered on the light bulb (asserting on the action's display text). The Apex light-bulb model
// exposes a flat action list, so the CodeFix/ErrorFix category asserted by the source tests is not
// mirrored here.

using FSharp.Editor.Apex.IntegrationTests.TestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests
{
    [TestClass]
    public class CodeActionTests : FSharpLanguageServiceApexTest
    {
        protected override bool AutomaticallyDismissMessageBoxes => false;

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the 'Remove unused open declarations' code fix is offered on an unused open.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void UnusedOpenDeclarations()
        {
            var code = @"module Library

open System

let x = 42";

            this.AssertCodeActionOffered(code, "open System", "Remove unused open declarations");
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the 'Add missing 'fun' keyword' error fix is offered on a lambda missing 'fun'.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void AddMissingFunKeyword()
        {
            var code = @"module Library

let original = []
let transformed = original |> List.map (x -> x)";

            this.AssertCodeActionOffered(code, "->", "Add missing 'fun' keyword");
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the 'Add 'new' keyword' code fix is offered when constructing a disposable.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void AddNewKeywordToDisposables()
        {
            var code = @"module Library

let sr = System.IO.StreamReader("""")";

            this.AssertCodeActionOffered(code, "let sr", "Add 'new' keyword");
        }
    }
}
