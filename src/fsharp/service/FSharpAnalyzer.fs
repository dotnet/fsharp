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
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open FSharp.Compiler.NameResolution
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

type FSharpAnalyzerTextChange = Range * string

[<Sealed>]
type public FSharpAnalyzerCheckFileContext(sourceTexts: (string * ISourceText)[],
        checkResults: FSharpCheckFileResults,
        cancellationToken: CancellationToken,
        tcConfig: TcConfig) = 
    let sourceTextMap = Map.ofArray sourceTexts
    member _.CancellationToken = cancellationToken
    member _.GetFileSource(fileName) = if sourceTextMap.ContainsKey fileName then sourceTextMap.[fileName] else SourceText.readFile fileName tcConfig.inputCodePage
    member _.CheckerModel = checkResults

[<Sealed>]
type public FSharpAnalyzerCheckProjectContext(sourceTexts: (string * ISourceText)[],
        checkResults: FSharpCheckProjectResults,
        cancellationToken: CancellationToken,
        tcConfig: TcConfig) = 
    let sourceTextMap = Map.ofArray sourceTexts
    member _.CancellationToken = cancellationToken
    member _.GetFileSource(fileName) = if sourceTextMap.ContainsKey fileName then sourceTextMap.[fileName] else SourceText.readFile fileName tcConfig.inputCodePage
    member _.CheckerModel = checkResults

[<Sealed>]
type public FSharpAnalysisContext() =
    class end

/// Represents an analyzer
[<AbstractClass>]
type public FSharpAnalyzer(context:FSharpAnalysisContext)  = 
    member _.Context = context
    abstract OnCheckFile: context: FSharpAnalyzerCheckFileContext -> FSharpDiagnostic[]
    abstract OnCheckProject: context: FSharpAnalyzerCheckProjectContext -> FSharpDiagnostic[]
    abstract TryAdditionalToolTip: context: FSharpAnalyzerCheckFileContext * position: Position  -> TaggedText[] option
    abstract TryCodeFix: context: FSharpAnalyzerCheckFileContext * diagnostics: FSharpDiagnostic[]  -> FSharpAnalyzerTextChange[] option
    abstract FixableDiagnosticIds: string[]
    abstract RequiresAssemblyContents: bool

    default _.OnCheckFile(_) = [| |]
    default _.OnCheckProject(_) = [| |]
    default _.TryAdditionalToolTip(_, _)  = None
    default _.TryCodeFix(_, _) = None
    default _.FixableDiagnosticIds = [| |]
    default _.RequiresAssemblyContents = false

module FSharpAnalyzers =
    open FSharp.Compiler.IO.FileSystemAutoOpens
    open FSharp.Compiler.ErrorLogger
    open FSharp.Core.CompilerServices

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
                yield! CreateAnalyzers (analyzerPath, m) 
            // TODO: give a warning here (or in CreateAnalyzers)
        ]

    let RunAnalyzers(analyzers: FSharpAnalyzer list, tcConfig: TcConfig, tcImports: TcImports, tcGlobals: TcGlobals, tcCcu: CcuThunk, sourceFiles: string list, tcFileResults, tcEnvAtEnd: TcEnv) =

        let sourceTexts = [| for sourceFile in sourceFiles -> sourceFile, SourceText.readFile sourceFile tcConfig.inputCodePage |]

        let projectOptions = 
            { 
                ProjectFileName = "compile.fsproj"
                ProjectId = None
                SourceFiles = Array.ofList sourceFiles
                ReferencedProjects = [| |]
                OtherOptions = [| |]
                IsIncompleteTypeCheckEnvironment = true
                UseScriptResolutionRules = false
                LoadTime = DateTime.MaxValue
                OriginalLoadReferences = []
                UnresolvedReferences = None
                Stamp = None
            }

        let tcFileResults, _implFilesRev =
            ([], tcFileResults) ||> List.mapFold (fun implFilesRev (a,b,c) -> 
                let implFilesRev2 = Option.toList b @ implFilesRev
                (a, List.rev implFilesRev2, c), implFilesRev2)

        for (inp, implFileOpt, ccuSig) in tcFileResults do
        
            let parseResults =
                FSharpParseFileResults(diagnostics = [||],
                    input = Some inp,
                    parseHadErrors = false,
                    dependencyFiles = [| |])

            let checkResults = 
                FSharpCheckFileResults.Make
                    (inp.FileName, 
                     "compile.fsproj", 
                     tcConfig, 
                     tcGlobals, 
                     false, 
                     None, 
                     parseResults.ParseTree,
                     projectOptions,
                     [| |], 
                     [| |], 
                     [| |], 
                     [| |],
                     true,
                     ccuSig,
                     tcCcu, 
                     tcImports, 
                     tcEnvAtEnd.AccessRights,
                     TcResolutions.Empty, 
                     TcSymbolUses.Empty,
                     tcEnvAtEnd.NameEnv,
                     None, 
                     implFileOpt,
                     [| |]) 

            let ctxt = FSharpAnalyzerCheckFileContext(sourceTexts, checkResults, CancellationToken.None, tcConfig)

            for analyzer in analyzers do
                 let diagnostics = analyzer.OnCheckFile(ctxt)
                 for diag in diagnostics do
                    let exn = CompilerToolDiagnostic((diag.ErrorNumber, diag.Message), diag.Range)
                    match diag.Severity with 
                    | FSharpDiagnosticSeverity.Error ->  errorR(exn)
                    | FSharpDiagnosticSeverity.Warning ->  warning(exn)
                    | FSharpDiagnosticSeverity.Info -> warning(exn)
                    | FSharpDiagnosticSeverity.Hidden -> ()

#else

    let ImportAnalyzers(tcConfig: TcConfig, g: TcGlobals, m) : FSharpAnalyzer list =

        [  ]

#endif
