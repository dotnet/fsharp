// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Apex counterparts of the xUnit/IdeFact tests in
// vsintegration/tests/FSharp.Editor.IntegrationTests/CreateProjectTests.cs. Each case creates an F#
// project from a .NET SDK template (via `dotnet new`, matching the SDK templates the source tests
// target — Apex's ProjectTemplate enum maps to the legacy .NET Framework F# templates with different
// filenames and content) and asserts the auto-generated default source.

using FSharp.Editor.Apex.IntegrationTests.TestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests
{
    [TestClass]
    public class CreateProjectTests : FSharpLanguageServiceApexTest
    {
        protected override bool AutomaticallyDismissMessageBoxes => false;

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the default source of a new F# class library project.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void ClassLibrary()
        {
            var expectedCode = @"namespace Library

module Say =
    let hello name =
        printfn ""Hello %s"" name";

            var project = this.Library.ProjectCreation.CreateFSharpProjectFromSdkTemplate("classlib", "Library");
            this.Library.Synchronization.WaitForSolutionCrawler();

            var document = this.Library.OpenProjectFile(project, "Library.fs");

            AssertSourceEquals(expectedCode, document.Contents);
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the default source of a new F# console application project.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void ConsoleApp()
        {
            var expectedCode = @"// For more information see https://aka.ms/fsharp-console-apps
printfn ""Hello from F#""";

            var project = this.Library.ProjectCreation.CreateFSharpProjectFromSdkTemplate("console", "ConsoleApp");
            this.Library.Synchronization.WaitForSolutionCrawler();

            var document = this.Library.OpenProjectFile(project, "Program.fs");

            AssertSourceEquals(expectedCode, document.Contents);
        }

        [RetryTestMethod]
        [Timeout(ShortTimeoutMS)]
        [Description("Verify the default source of a new F# xUnit test project.")]
        [Owner("fsharptools")]
        [TestCategory("nightly"), TestCategory("Walkthrough")]
        public void XUnitTestProject()
        {
            var expectedCode = @"module Tests

open System
open Xunit

[<Fact>]
let ``My test`` () =
    Assert.True(true)";

            var project = this.Library.ProjectCreation.CreateFSharpProjectFromSdkTemplate("xunit", "Tests");
            this.Library.Synchronization.WaitForSolutionCrawler();

            var document = this.Library.OpenProjectFile(project, "Tests.fs");

            AssertSourceEquals(expectedCode, document.Contents);
        }
    }
}
