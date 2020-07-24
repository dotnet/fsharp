// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.IO
open System.Threading

//open FSharp.Compiler.Service.Tests.Common

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range

//[<TestFixture>][<Category "Roslyn Services">]
//type ProjectDiagnosticAnalyzerTests()  =

//    let CreateProjectAndGetOptions(fileContents: string) =
//        let tempName = Path.GetTempFileName()
//        let fileName = Path.ChangeExtension(tempName, ".fs")
//        let projectName = Path.ChangeExtension(tempName, ".fsproj")
//        let dllName = Path.ChangeExtension(tempName, ".dll")
//        File.WriteAllText(fileName, fileContents)

//        let args = mkProjectCommandLineArgs (dllName, [fileName])
//        checker.GetProjectOptionsFromCommandLineArgs (projectName, args)
