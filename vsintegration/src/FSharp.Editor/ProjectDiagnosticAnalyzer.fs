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

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpProjectDiagnosticAnalyzer() =
    inherit ProjectDiagnosticAnalyzer()
    
    // We are constructing our own descriptors at run-time. Compiler service is already doing error formatting and localization.
    override this.SupportedDiagnostics with get() = ImmutableArray<DiagnosticDescriptor>.Empty

    override this.AnalyzeProjectAsync(_, _): Task<ImmutableArray<Diagnostic>> =
        Task.FromResult(ImmutableArray<Diagnostic>.Empty)