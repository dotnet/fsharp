// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Reflection
open System.Threading
open Internal.Utilities.Library  
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

type FSharpAnalyzerTextChange = Range * string

[<Sealed>]
type public FSharpAnalyzerCheckFileContext(sourceTexts: (string * ISourceText)[],
        fileName: string,
        projectOptions: FSharpProjectOptions,
        parseResults: FSharpParseFileResults,
        checkResults: FSharpCheckFileResults) = 
    let sourceTextMap = Map.ofArray sourceTexts
    member _.TryGetFileSource(fileName) = if sourceTextMap.ContainsKey fileName then Some sourceTextMap.[fileName] else None
    member _.FileName = fileName
    member _.ProjectOptions = projectOptions
    member _.ParseFileResults = parseResults
    member _.CheckFileResults = checkResults

[<Sealed>]
type public FSharpAnalyzerCheckProjectContext(sourceTexts: (string * ISourceText)[],
        projectOptions: FSharpProjectOptions,
        checkResults: FSharpCheckProjectResults) = 
    let sourceTextMap = Map.ofArray sourceTexts
    member _.TryGetFileSource(fileName) = if sourceTextMap.ContainsKey fileName then Some sourceTextMap.[fileName] else None
    member _.ProjectOptions = projectOptions
    member _.CheckProjectResults = checkResults

[<Sealed>]
type public FSharpAnalysisContext() =
    class end

/// Represents an analyzer
[<AbstractClass>]
type public FSharpAnalyzer(context:FSharpAnalysisContext)  = 
    member _.Context = context
    abstract OnCheckFile: context: FSharpAnalyzerCheckFileContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]
    abstract OnCheckProject: context: FSharpAnalyzerCheckProjectContext * cancellationToken: CancellationToken -> FSharpDiagnostic[]
    abstract TryAdditionalToolTip: context: FSharpAnalyzerCheckFileContext * position: Position  * cancellationToken: CancellationToken -> TaggedText[] option
    abstract TryCodeFix: context: FSharpAnalyzerCheckFileContext * diagnostics: FSharpDiagnostic[]  * cancellationToken: CancellationToken -> FSharpAnalyzerTextChange[] option
    abstract FixableDiagnosticIds: string[]

    default _.OnCheckFile(_, _) = [| |]
    default _.OnCheckProject(_, _) = [| |]
    default _.TryAdditionalToolTip(_, _, _)  = None
    default _.TryCodeFix(_, _, _) = None
    default _.FixableDiagnosticIds = [| |]

module FSharpAnalyzers =
    open FSharp.Compiler
    open FSharp.Compiler.IO.FileSystemAutoOpens
    open FSharp.Compiler.AbstractIL.IL
    open FSharp.Compiler.ErrorLogger
    open FSharp.Compiler.TcGlobals
    open FSharp.Core.CompilerServices
    open System.IO

#if !NO_EXTENSIONTYPING
    let CreateAnalyzer (analyzerType: System.Type, m) =

        if analyzerType.GetConstructor([| typeof<FSharpAnalysisContext> |]) <> null then

            let ctxt = FSharpAnalysisContext()
            try 
                Activator.CreateInstance(analyzerType, [| box ctxt|]) :?> FSharpAnalyzer |> Some
            with 
            | :? System.Reflection.TargetInvocationException as exn ->
                let exn = exn.InnerException
                errorR (Error(FSComp.SR.etAnalyzerConstructionException(analyzerType.FullName, exn.ToString()), m))
                None
            | :? TypeInitializationException as exn -> 
                let exn = exn.InnerException
                errorR (Error(FSComp.SR.etAnalyzerTypeInitializationException(analyzerType.FullName, exn.ToString()), m))
                None
            |   exn -> 
                errorR (Error(FSComp.SR.etAnalyzerConstructionException(analyzerType.FullName, exn.ToString()), m))
                None
        else
            errorR (Error(FSComp.SR.etAnalyzerDoesNotHaveValidConstructor(analyzerType.FullName), m))
            None

    let CreateAnalyzers (analyzerPath, m:range) =

        let analyzerAssembly = 
            try
                Assembly.UnsafeLoadFrom analyzerPath
            with exn ->
                error (Error(FSComp.SR.etAnalyzerLoadFailure(analyzerPath, exn.ToString()), m))

        [ if analyzerAssembly.GetCustomAttribute(typeof<AnalyzerAssemblyAttribute>) <> null then
            let exportedTypes = analyzerAssembly.GetExportedTypes() 
            for t in exportedTypes do 
                let ca = t.GetCustomAttributes(typeof<AnalyzerAttribute>, true)
                if ca <> null && ca.Length > 0 then 
                    match CreateAnalyzer(t, m) with 
                    | None -> ()
                    | Some a -> a ]

    let ImportAnalyzers(tcConfig: TcConfig, m) =

        [ for analyzerPath in tcConfig.compilerToolPaths do
            if FileSystem.SafeExists(analyzerPath) then
                yield! CreateAnalyzers (analyzerPath, m) ]


#else

    let ImportAnalyzers(tcConfig: TcConfig, g: TcGlobals, m) : FSharpAnalyzer list =

        [  ]

#endif
