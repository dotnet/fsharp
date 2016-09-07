// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.SolutionCrawler

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

open Microsoft.VisualStudio.FSharp.LanguageService

[<ExportLanguageService(typeof<IDocumentDifferenceService>, FSharpCommonConstants.FSharpLanguageName)>]
type FSharpDocumentDifferenceService() =
    interface IDocumentDifferenceService with
        member this.GetDifferenceAsync(_,_,_) =
            // No incremental anaylsis for now.
            Task.FromResult(new DocumentDifferenceResult(InvocationReasons.DocumentChanged))
