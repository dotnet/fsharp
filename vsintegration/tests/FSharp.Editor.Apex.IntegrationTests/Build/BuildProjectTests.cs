// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Apex counterparts of the xUnit/IdeFact tests in
// vsintegration/tests/FSharp.Editor.IntegrationTests/BuildProjectTests.cs. The project is created from
// the .NET SDK class-library template (via `dotnet new`, matching the source tests; Apex's
// ProjectTemplate.ClassLibrary is the legacy .NET Framework template). The template auto-opens the
// default source file, so the source is set through the open document and saved to disk (an
// out-of-band disk write would be superseded by the still-open template buffer, and the compiler would
// build the valid template instead of the intended code). Rather than parsing the "Build: 1 succeeded,
// 0 failed ..." output-pane summary string, these assert on the idiomatic Apex build result:
// BuildManager.Succeeded and BuildManager.Verify.HasFailedWithErrors for the error case.

using System.IO;
using FSharp.Editor.Apex.IntegrationTests.TestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests
{
    [TestClass]
    public class BuildProjectTests : FSharpLanguageServiceApexTest
    {
        protected override bool AutomaticallyDismissMessageBoxes => false;

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify a well-formed F# project builds successfully.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void SuccessfulBuild()
        {
            var code = @"module Test

let answer = 42";

            this.SetLibraryContent(code);

            Assert.IsTrue(this.BuildSolutionSucceeded(), "Build was expected to succeed.");
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify an F# project with an incomplete binding fails to build with FS0010.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void FailedBuild()
        {
            var code = @"module Test

let answer =";

            this.SetLibraryContent(code);

            this.BuildSolution();

            var buildManager = this.Library.VisualStudio.ObjectModel.Solution.BuildManager;
            Assert.IsFalse(buildManager.Succeeded, "Build was expected to fail.");

            // The Apex error-list verifier matches the error DESCRIPTION (the message text); the code
            // 'FS0010' lives in a separate Code column, so match on the FS0010 message instead.
            Assert.IsTrue(
                buildManager.Verify.HasFailedWithErrors(
                    new[] { "Incomplete structured construct" }, alsoVerifyInOutputWindow: false),
                "Build was expected to fail with the FS0010 'Incomplete structured construct' error.");
        }

        /// <summary>
        /// Creates an F# class library and replaces its default Library.fs with <paramref name="code"/>
        /// through the open document (saved to disk), verifying the file on disk matches before building.
        /// </summary>
        private void SetLibraryContent(string code)
        {
            var project = this.Library.ProjectCreation.CreateFSharpProjectFromSdkTemplate("classlib", "Library");
            this.Library.Synchronization.WaitForSolutionCrawler();

            var document = this.Library.OpenProjectFile(project, "Library.fs");
            document.ReplaceAllAndSave(code);

            AssertSourceEquals(code, File.ReadAllText(document.FilePath));
        }
    }
}
