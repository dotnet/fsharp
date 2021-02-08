// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text

type FSharpAnalyzerTextChange = Range * string

/// The context for an analyzer when a file is checked
[<Sealed>]
type public FSharpAnalyzerCheckFileContext = 

    internal new: sourceTexts: (string * ISourceText)[] * fileName: string * projectOptions: FSharpProjectOptions * parseResults: FSharpParseFileResults * checkResults: FSharpCheckFileResults -> FSharpAnalyzerCheckFileContext

    member TryGetFileSource: fileName: string -> ISourceText option

    member ProjectOptions: FSharpProjectOptions

    member ParseFileResults: FSharpParseFileResults

    member CheckFileResults: FSharpCheckFileResults

/// The context for an analyzer when a project is checked
[<Sealed>]
type public FSharpAnalyzerCheckProjectContext = 

    internal new: sourceTexts: (string * ISourceText)[] * projectOptions: FSharpProjectOptions * checkResults: FSharpCheckProjectResults -> FSharpAnalyzerCheckProjectContext

    member TryGetFileSource: fileName: string -> ISourceText option

    member ProjectOptions: FSharpProjectOptions

    member CheckProjectResults: FSharpCheckProjectResults

/// The context for an analyzer. Currently empty.
[<Sealed>]
type public FSharpAnalysisContext =
    class end

/// Represents an analyzer. Inherit from this class to create an analyzer.
[<AbstractClass>]
type public FSharpAnalyzer = 

    new: context: FSharpAnalysisContext -> FSharpAnalyzer
    
    member Context: FSharpAnalysisContext
    
    abstract OnCheckFile: FSharpAnalyzerCheckFileContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]

    abstract OnCheckProject: FSharpAnalyzerCheckProjectContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]

    abstract TryAdditionalToolTip: FSharpAnalyzerCheckFileContext * Position * cancellationToken: CancellationToken  -> TaggedText[] option

    abstract TryCodeFix: FSharpAnalyzerCheckFileContext * FSharpDiagnostic[] * cancellationToken: CancellationToken  -> FSharpAnalyzerTextChange[] option

    abstract FixableDiagnosticIds: string[]

    default OnCheckFile: FSharpAnalyzerCheckFileContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]
    default OnCheckProject: FSharpAnalyzerCheckProjectContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]
    default TryAdditionalToolTip: FSharpAnalyzerCheckFileContext * Position * cancellationToken: CancellationToken -> TaggedText[] option
    default TryCodeFix: FSharpAnalyzerCheckFileContext * FSharpDiagnostic[] * cancellationToken: CancellationToken -> FSharpAnalyzerTextChange[] option
    default FixableDiagnosticIds: string[]

module internal FSharpAnalyzers =
    val ImportAnalyzers: tcConfig: TcConfig * m: range -> FSharpAnalyzer list
