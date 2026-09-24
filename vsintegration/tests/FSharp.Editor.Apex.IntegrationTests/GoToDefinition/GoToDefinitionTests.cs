// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Apex counterparts of the xUnit/IdeFact tests in
// vsintegration/tests/FSharp.Editor.IntegrationTests/GoToDefinitionTests.cs. Go To Definition is
// invoked idiomatically via the editor caret (IVisualStudioCaretTestExtension.GoToDefinition), and the
// result is read from the active document (current line + window caption) rather than the async
// SolutionExplorer/Editor/Shell helpers used by the xUnit harness.

using FSharp.Editor.Apex.IntegrationTests.TestFramework;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests
{
    [TestClass]
    public class GoToDefinitionTests : FSharpLanguageServiceApexTest
    {
        protected override bool AutomaticallyDismissMessageBoxes => false;

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify Go To Definition navigates from a use of a binding to its definition.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void GoesToDefinition()
        {
            var code = @"module Test

let add x y = x + y

let increment = add 1";

            var document = this.OpenFSharpDocument(code, "add 1");

            // Resolve references so the file type-checks and Go To Definition can bind the symbol.
            this.BuildSolution();

            this.GoToDefinitionAndWait(document, "add 1", line => line.Contains("let add x y = x + y"));
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify Go To Definition stays within the signature file and the implementation file respectively.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void FsiAndFsFilesGoToCorrespondentDefinitions()
        {
            var fsi = @"module Module

type SomeType =
| Number of int
| Letter of char

val id: t: SomeType -> SomeType";

            var fs = @"module Module

type SomeType =
    | Number of int
    | Letter of char

let id (t: SomeType) = t";

            var project = this.Library.ProjectCreation.CreateFSharpProject(ProjectTemplate.ClassLibrary, "Library");

            // The signature file must precede the implementation file in F# compile order. When a file is
            // added, VS inserts it alphabetically, so adding "Module.fsi" then "Module.fs" would place the
            // implementation first ("Module.fs" sorts before "Module.fsi") and the signature would not
            // apply. Add the signature under a name that sorts first, then rename it — the same approach as
            // the source IdeFact test.
            var fsiItem = this.Library.ProjectCreation.AddProjectItemFromContent(project, "AModule.fsi", fsi);
            this.Library.ProjectCreation.AddProjectItemFromContent(project, "Module.fs", fs);
            fsiItem.Rename("Module.fsi");
            this.Library.Synchronization.WaitForSolutionCrawler();
            this.BuildSolution();

            // From the signature file, Go To Definition on the type usage stays in Module.fsi.
            var fsiDocument = this.Library.OpenProjectFile(project, "Module.fsi");
            this.GoToDefinitionAndWait(fsiDocument, "SomeType ->", line => line.Trim() == "type SomeType =", "Module.fsi");

            // From the implementation file, Go To Definition on the type usage stays in Module.fs.
            var fsDocument = this.Library.OpenProjectFile(project, "Module.fs");
            this.GoToDefinitionAndWait(fsDocument, "SomeType)", line => line.Trim() == "type SomeType =", "Module.fs");
        }
    }
}
