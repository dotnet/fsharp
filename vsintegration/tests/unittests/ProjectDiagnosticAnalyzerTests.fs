// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.IO
open System.Threading

open FSharp.Compiler.Service.Tests.Common

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<TestFixture>]
type ProjectDiagnosticAnalyzerTests()  =

    let CreateProjectAndGetOptions(fileContents: string) =
        let tempName = Path.GetTempFileName()
        let fileName = Path.ChangeExtension(tempName, ".fs")
        let projectName = Path.ChangeExtension(tempName, ".fsproj")
        let dllName = Path.ChangeExtension(tempName, ".dll")
        File.WriteAllText(fileName, fileContents)

        let args = mkProjectCommandLineArgs (dllName, [fileName])
        checker.GetProjectOptionsFromCommandLineArgs (projectName, args)

    [<Test>]
    member public this.ProjectDiagnosticsDontReportJustProjectErrors_Bug1596() =
        // https://github.com/Microsoft/visualfsharp/issues/1596
        let fileContents = """
let x = 3
printf "%d" x
"""
        let options = CreateProjectAndGetOptions(fileContents)
        let additionalOptions = {options with OtherOptions = Array.append options.OtherOptions [| "--times" |]}

        let errors = FSharpProjectDiagnosticAnalyzer.GetDiagnostics(additionalOptions)
        Assert.AreEqual(1, errors.Length, "Exactly one warning should have been reported")
        
        let warning = errors.[0]
        Assert.AreEqual(DiagnosticSeverity.Warning, warning.Severity, "Diagnostic severity should be a warning")
        Assert.AreEqual("The command-line option 'times' is for test purposes only", warning.GetMessage())

    [<Test>]
    member public this.ProjectDiagnosticsShouldNotReportDocumentErrors_Bug1596() =
        // https://github.com/Microsoft/visualfsharp/issues/1596
        let fileContents = """
let x = "string value that cannot be printed with %d"
printf "%d" x
"""
        let options = CreateProjectAndGetOptions(fileContents)

        let errors = FSharpProjectDiagnosticAnalyzer.GetDiagnostics(options)
        Assert.AreEqual(0, errors.Length, "No semantic errors should have been reported")
