// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text

type FSharpAnalyzerTextChange = Range * string

/// The context for an analyzer when a file is checked
[<Sealed>]
type public FSharpAnalyzerCheckFileContext = 

    internal new: sourceTexts: (string * ISourceText)[] * checkResults: FSharpCheckFileResults * cancellationToken: CancellationToken * tcConfig: TcConfig -> FSharpAnalyzerCheckFileContext

    member CancellationToken: CancellationToken 

    member GetFileSource: fileName: string -> ISourceText

    member CheckerModel: FSharpCheckFileResults

/// The context for an analyzer when a project is checked
[<Sealed>]
type public FSharpAnalyzerCheckProjectContext = 

    internal new: sourceTexts: (string * ISourceText)[] * checkResults: FSharpCheckProjectResults * cancellationToken: CancellationToken * tcConfig: TcConfig -> FSharpAnalyzerCheckProjectContext

    member CancellationToken: CancellationToken 

    member GetFileSource: fileName: string -> ISourceText

    member CheckerModel: FSharpCheckProjectResults

/// The context for an analyzer. Currently empty.
[<Sealed>]
type public FSharpAnalysisContext =
    class end

/// Represents an analyzer. Inherit from this class to create an analyzer.
[<AbstractClass>]
type public FSharpAnalyzer = 

    new: context: FSharpAnalysisContext -> FSharpAnalyzer
    
    member Context: FSharpAnalysisContext
    
    abstract RequiresAssemblyContents: bool

    abstract OnCheckFile: FSharpAnalyzerCheckFileContext -> FSharpDiagnostic[]

    abstract OnCheckProject: FSharpAnalyzerCheckProjectContext -> FSharpDiagnostic[]

    abstract TryAdditionalToolTip: FSharpAnalyzerCheckFileContext * position: Position -> TaggedText[] option

    abstract TryCodeFix: FSharpAnalyzerCheckFileContext * diagnostics: FSharpDiagnostic[] -> FSharpAnalyzerTextChange[] option

    abstract FixableDiagnosticIds: string[]

    default RequiresAssemblyContents: bool
    default OnCheckFile: FSharpAnalyzerCheckFileContext -> FSharpDiagnostic[]
    default OnCheckProject: FSharpAnalyzerCheckProjectContext -> FSharpDiagnostic[]
    default TryAdditionalToolTip: FSharpAnalyzerCheckFileContext * position: Position -> TaggedText[] option
    default TryCodeFix: FSharpAnalyzerCheckFileContext * diagnostics: FSharpDiagnostic[] -> FSharpAnalyzerTextChange[] option
    default FixableDiagnosticIds: string[]

module internal FSharpAnalyzers =
    val ImportAnalyzers: tcConfig: TcConfig * m: range -> FSharpAnalyzer list
