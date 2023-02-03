// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FSharp.Editor.IntegrationTests
{
    public class CreateProjectTests : AbstractIntegrationTest
    {
        // This is starting up a basic F# lib:
        //
        // namespace Library
        //
        // module Say =
        //    let hello name =
        //        printfn "Hello %s" name
        [IdeFact]
        public async Task BasicFSharpLibraryCompilesAsync()
        {
            var token = HangMitigatingCancellationToken;
            var solutionExplorer = TestServices.SolutionExplorer;
            var errorList = TestServices.ErrorList;

            await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
            await solutionExplorer.AddProjectAsync(
                "Library",
                "Microsoft.FSharp.NETCore.ClassLibrary",
                token);

            await solutionExplorer.RestoreNuGetPackagesAsync(token);

            var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";
            var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(true, token);
            Assert.Contains(expectedBuildSummary, actualBuildSummary);

            await errorList.ShowBuildErrorsAsync(token);
            var errorCount = await errorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_ERROR, token);
            Assert.Equal(0, errorCount);
        }

        // This is starting up a basic F# console app:
        //
        // // For more information see https://aka.ms/fsharp-console-apps
        // printfn "Hello from F#"
        [IdeFact]
        public async Task BasicFSharpConsoleAppCompilesAsync()
        {
            var token = HangMitigatingCancellationToken;
            var solutionExplorer = TestServices.SolutionExplorer;
            var errorList = TestServices.ErrorList;

            await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
            await solutionExplorer.AddProjectAsync(
                "ConsoleApp",
                "Microsoft.FSharp.NETCore.ConsoleApplication",
                token);

            await solutionExplorer.RestoreNuGetPackagesAsync(token);

            var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";
            var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(true, token);
            Assert.Contains(expectedBuildSummary, actualBuildSummary);

            await errorList.ShowBuildErrorsAsync(token);
            var errorCount = await errorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_ERROR, token);
            Assert.Equal(0, errorCount);
        }
    }
}