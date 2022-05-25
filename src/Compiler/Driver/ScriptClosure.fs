// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Compute the load closure of a set of script files
module internal FSharp.Compiler.ScriptClosure

open System
open System.Collections.Generic
open System.IO
open System.Text
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range

[<RequireQualifiedAccess>]
type LoadClosureInput =
    {
        FileName: string
        SyntaxTree: ParsedInput option
        ParseDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list
        MetaCommandDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list
    }

[<RequireQualifiedAccess>]
type LoadClosure =
    {
        /// The source files along with the ranges of the #load positions in each file.
        SourceFiles: (string * range list) list

        /// The resolved references along with the ranges of the #r positions in each file.
        References: (string * AssemblyResolution list) list

        /// The resolved pacakge references along with the ranges of the #r positions in each file.
        PackageReferences: (range * string list)[]

        /// Whether we're decided to use .NET Framework analysis for this script
        UseDesktopFramework: bool

        /// Was the SDK directory override given?
        SdkDirOverride: string option

        /// The list of references that were not resolved during load closure. These may still be extension references.
        UnresolvedReferences: UnresolvedAssemblyReference list

        /// The list of all sources in the closure with inputs when available
        Inputs: LoadClosureInput list

        /// The #load, including those that didn't resolve
        OriginalLoadReferences: (range * string * string) list

        /// The #nowarns
        NoWarns: (string * range list) list

        /// Diagnostics seen while processing resolutions
        ResolutionDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list

        /// Diagnostics seen while parsing root of closure
        AllRootFileDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list

        /// Diagnostics seen while processing the compiler options implied root of closure
        LoadClosureRootFileDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list
    }


[<RequireQualifiedAccess>]
type CodeContext =
    | CompilationAndEvaluation // in fsi.exe
    | Compilation  // in fsc.exe
    | Editing // in VS

module ScriptPreprocessClosure =

    /// Represents an input to the closure finding process
    type ClosureSource =
        ClosureSource of
            fileName: string *
            referenceRange: range *
            sourceText: ISourceText *
            parseRequired: bool

    /// Represents an output of the closure finding process
    type ClosureFile =
        ClosureFile of
            fileName: string *
            range: range *
            parsedInput: ParsedInput option *
            parseDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list *
            metaDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list *
            nowarns: (string * range) list

    type Observed() =
        let seen = Dictionary<_, bool>()
        member _.SetSeen check =
            if not(seen.ContainsKey check) then
                seen.Add(check, true)

        member _.HaveSeen check =
            seen.ContainsKey check

    /// Parse a script file (or any input file referenced by '#load')
    let ParseScriptClosureInput
        (
            fileName: string,
            sourceText: ISourceText,
            tcConfig: TcConfig,
            codeContext,
            lexResourceManager: Lexhelp.LexResourceManager,
            diagnosticsLogger: DiagnosticsLogger
        ) =

        // fsc.exe -- COMPILED\!INTERACTIVE
        // fsi.exe -- !COMPILED\INTERACTIVE
        // Language service
        //     .fs -- EDITING + COMPILED\!INTERACTIVE
        //     .fsx -- EDITING + !COMPILED\INTERACTIVE
        let defines =
            match codeContext with
            | CodeContext.CompilationAndEvaluation -> ["INTERACTIVE"]
            | CodeContext.Compilation -> ["COMPILED"]
            | CodeContext.Editing -> "EDITING" :: (if IsScript fileName then ["INTERACTIVE"] else ["COMPILED"])

        let tcConfigB = tcConfig.CloneToBuilder() 
        tcConfigB.conditionalDefines <- defines @ tcConfig.conditionalDefines
        let tcConfig = TcConfig.Create(tcConfigB, false)
        
        let lexbuf = UnicodeLexing.SourceTextAsLexbuf(true, tcConfig.langVersion, sourceText)

        // The root compiland is last in the list of compilands.
        let isLastCompiland = (IsScript fileName, tcConfig.target.IsExe)
        ParseOneInputLexbuf (tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

    /// Create a TcConfig for load closure starting from a single .fsx file
    let CreateScriptTextTcConfig
        (
            legacyReferenceResolver,
            defaultFSharpBinariesDir,
            fileName: string,
            codeContext,
            useSimpleResolution,
            useFsiAuxLib,
            basicReferences,
            applyCommandLineArgs,
            assumeDotNetFramework,
            useSdkRefs,
            sdkDirOverride,
            tryGetMetadataSnapshot,
            reduceMemoryUsage
        ) =

        let projectDir = Path.GetDirectoryName fileName
        let isInteractive = (codeContext = CodeContext.CompilationAndEvaluation)
        let isInvalidationSupported = (codeContext = CodeContext.Editing)

        let rangeForErrors = mkFirstLineOfFile fileName
        let tcConfigB =
            TcConfigBuilder.CreateNew(legacyReferenceResolver,
                defaultFSharpBinariesDir,
                reduceMemoryUsage,
                projectDir,
                isInteractive,
                isInvalidationSupported,
                CopyFSharpCoreFlag.No,
                tryGetMetadataSnapshot,
                sdkDirOverride,
                rangeForErrors)
        tcConfigB.SetPrimaryAssembly (if assumeDotNetFramework then PrimaryAssembly.Mscorlib else PrimaryAssembly.System_Runtime)
        tcConfigB.SetUseSdkRefs useSdkRefs

        applyCommandLineArgs tcConfigB

        // Work out the references for the script in its location. This may produce diagnostics.
        let scriptDefaultReferencesDiagnostics =

            match basicReferences with
            | None ->
                let diagnosticsLogger = CapturingDiagnosticsLogger("ScriptDefaultReferences")
                use unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind (fun _ -> diagnosticsLogger)
                let references, useDotNetFramework = tcConfigB.FxResolver.GetDefaultReferences useFsiAuxLib

                // If the user requested .NET Core scripting but something went wrong and we reverted to
                // .NET Framework scripting then we must adjust both the primaryAssembly and fxResolver
                if useDotNetFramework <> assumeDotNetFramework then
                    tcConfigB.SetPrimaryAssembly (if useDotNetFramework then PrimaryAssembly.Mscorlib else PrimaryAssembly.System_Runtime)

                // Add script references
                for reference in references do
                    tcConfigB.AddReferencedAssemblyByPath(range0, reference)

                diagnosticsLogger.Diagnostics

            | Some (rs, diagnostics) ->
                for m, reference in rs do
                    tcConfigB.AddReferencedAssemblyByPath(m, reference)
                diagnostics

        tcConfigB.resolutionEnvironment <-
            match codeContext with
            | CodeContext.Editing -> LegacyResolutionEnvironment.EditingOrCompilation true
            | CodeContext.Compilation -> LegacyResolutionEnvironment.EditingOrCompilation false
            | CodeContext.CompilationAndEvaluation -> LegacyResolutionEnvironment.CompilationAndEvaluation

        tcConfigB.implicitlyReferenceDotNetAssemblies <- false

        tcConfigB.useSimpleResolution <- useSimpleResolution

        // Indicates that there are some references not in basicReferencesForScriptLoadClosure which should
        // be added conditionally once the relevant version of mscorlib.dll has been detected.
        tcConfigB.implicitlyResolveAssemblies <- false

        tcConfigB.SetUseSdkRefs useSdkRefs

        TcConfig.Create(tcConfigB, validate=true), scriptDefaultReferencesDiagnostics

    let ClosureSourceOfFilename(fileName, m, inputCodePage, parseRequired) =
        try
            let fileName = FileSystem.GetFullPathShim fileName
            use stream = FileSystem.OpenFileForReadShim(fileName)
            use reader =
                match inputCodePage with
                | None -> new StreamReader(stream, true)
                | Some (n: int) -> new StreamReader(stream, Encoding.GetEncoding n)
            let source = reader.ReadToEnd()
            [ClosureSource(fileName, m, SourceText.ofString source, parseRequired)]
        with exn ->
            errorRecovery exn m
            []

    let ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn
        (
            tcConfig: TcConfig,
            inp: ParsedInput,
            pathOfMetaCommandSource,
            dependencyProvider
        ) =

        let tcConfigB = tcConfig.CloneToBuilder()
        let mutable nowarns = []
        let getWarningNumber = fun () (m, s) -> nowarns <- (s, m) :: nowarns
        let addReferenceDirective = fun () (m, s, directive) -> tcConfigB.AddReferenceDirective(dependencyProvider, m, s, directive)
        let addLoadedSource = fun () (m, s) -> tcConfigB.AddLoadedSource(m, s, pathOfMetaCommandSource)
        try
            ProcessMetaCommandsFromInput (getWarningNumber, addReferenceDirective, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
        with ReportedError _ ->
            // Recover by using whatever did end up in the tcConfig
            ()

        try
            TcConfig.Create(tcConfigB, validate=false), nowarns
        with ReportedError _ ->
            // Recover by using a default TcConfig.
            let tcConfigB = tcConfig.CloneToBuilder()
            TcConfig.Create(tcConfigB, validate=false), nowarns

    let FindClosureFiles
        (
            mainFile,
            closureSources,
            origTcConfig:TcConfig,
            codeContext,
            lexResourceManager: Lexhelp.LexResourceManager,
            dependencyProvider: DependencyProvider
        ) =

        let mutable tcConfig = origTcConfig

        let observedSources = Observed()
        let loadScripts = HashSet<_>()
        let packageReferences = Dictionary<range, string list>(HashIdentity.Structural)

        // Resolve the packages
        let rec resolveDependencyManagerSources scriptName =
            if not (loadScripts.Contains scriptName) then
                [ for kv in tcConfig.packageManagerLines do
                    let packageManagerKey, packageManagerLines = kv.Key, kv.Value
                    match packageManagerLines with
                    | [] -> ()
                    | { Directive=_; LineStatus=_; Line=_; Range=m } :: _ ->
                        let reportError =
                            ResolvingErrorReport (fun errorType err msg ->
                                let error = err, msg
                                match errorType with
                                | ErrorReportType.Warning -> warning(Error(error, m))
                                | ErrorReportType.Error -> errorR(Error(error, m)))

                        match origTcConfig.packageManagerLines |> Map.tryFind packageManagerKey with
                        | Some oldDependencyManagerLines when oldDependencyManagerLines = packageManagerLines -> ()
                        | _ ->
                            let outputDir =  tcConfig.outputDir |> Option.defaultValue ""
                            match dependencyProvider.TryFindDependencyManagerByKey(tcConfig.compilerToolPaths, outputDir, reportError, packageManagerKey) with
                            | Null ->
                                errorR(Error(dependencyProvider.CreatePackageManagerUnknownError(tcConfig.compilerToolPaths, outputDir, packageManagerKey, reportError), m))

                            | NonNull dependencyManager ->
                                let directive d =
                                    match d with
                                    | Directive.Resolution -> "r"
                                    | Directive.Include -> "i"

                                let packageManagerTextLines = packageManagerLines |> List.map(fun l -> directive l.Directive, l.Line)
                                let tfm, rid = tcConfig.FxResolver.GetTfmAndRid()
                                let result = dependencyProvider.Resolve(dependencyManager, ".fsx", packageManagerTextLines, reportError, tfm, rid, tcConfig.implicitIncludeDir, mainFile, scriptName)
                                if result.Success then
                                    // Resolution produced no errors
                                    //Write outputs in F# Interactive and compiler
                                    if codeContext <> CodeContext.Editing then
                                        for line in result.StdOut do Console.Out.WriteLine(line)
                                        for line in result.StdError do Console.Error.WriteLine(line)

                                    packageReferences[m] <- [ for script in result.SourceFiles do yield! FileSystem.OpenFileForReadShim(script).ReadLines() ]
                                    if not (Seq.isEmpty result.Roots) then
                                        let tcConfigB = tcConfig.CloneToBuilder()
                                        for folder in result.Roots do
                                            tcConfigB.AddIncludePath(m, folder, "")
                                        tcConfigB.packageManagerLines <- PackageManagerLine.SetLinesAsProcessed packageManagerKey tcConfigB.packageManagerLines
                                        tcConfig <- TcConfig.Create(tcConfigB, validate=false)

                                    if not (Seq.isEmpty result.Resolutions) then
                                        let tcConfigB = tcConfig.CloneToBuilder()
                                        for resolution in result.Resolutions do
                                            tcConfigB.AddReferencedAssemblyByPath(m, resolution)
                                        tcConfig <- TcConfig.Create(tcConfigB, validate = false)

                                    for script in result.SourceFiles do
                                        use stream = FileSystem.OpenFileForReadShim(script)
                                        let scriptText = stream.ReadAllText()
                                        loadScripts.Add script |> ignore
                                        let iSourceText = SourceText.ofString scriptText
                                        yield! loop (ClosureSource(script, m, iSourceText, true))

                                else
                                    // Send outputs via diagnostics
                                    if (result.StdOut.Length > 0 || result.StdError.Length > 0) then
                                        for line in Array.append result.StdOut result.StdError do
                                            errorR(Error(FSComp.SR.packageManagerError(line), m))
                                    // Resolution produced errors update packagerManagerLines entries to note these failure
                                    // failed resolutions will no longer be considered
                                    let tcConfigB = tcConfig.CloneToBuilder()
                                    tcConfigB.packageManagerLines <- PackageManagerLine.RemoveUnprocessedLines packageManagerKey tcConfigB.packageManagerLines
                                    tcConfig <- TcConfig.Create(tcConfigB, validate=false)]
            else []

        and loop (ClosureSource(fileName, m, sourceText, parseRequired)) =
            [   if not (observedSources.HaveSeen(fileName)) then
                    observedSources.SetSeen(fileName)
                    //printfn "visiting %s" fileName
                    if IsScript fileName || parseRequired then
                        let parseResult, parseDiagnostics =
                            let diagnosticsLogger = CapturingDiagnosticsLogger("FindClosureParse")
                            use _unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind (fun _ -> diagnosticsLogger)
                            let result = ParseScriptClosureInput (fileName, sourceText, tcConfig, codeContext, lexResourceManager, diagnosticsLogger)
                            result, diagnosticsLogger.Diagnostics

                        let diagnosticsLogger = CapturingDiagnosticsLogger("FindClosureMetaCommands")
                        use _unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind (fun _ -> diagnosticsLogger)
                        let pathOfMetaCommandSource = Path.GetDirectoryName fileName
                        let preSources = tcConfig.GetAvailableLoadedSources()

                        let tcConfigResult, noWarns = ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn (tcConfig, parseResult, pathOfMetaCommandSource, dependencyProvider)
                        tcConfig <- tcConfigResult // We accumulate the tcConfig in order to collect assembly references

                        yield! resolveDependencyManagerSources fileName

                        let postSources = tcConfig.GetAvailableLoadedSources()
                        let sources = if preSources.Length < postSources.Length then postSources[preSources.Length..] else []

                        yield! resolveDependencyManagerSources fileName
                        for m, subFile in sources do
                            if IsScript subFile then
                                for subSource in ClosureSourceOfFilename(subFile, m, tcConfigResult.inputCodePage, false) do
                                    yield! loop subSource
                            else
                                ClosureFile(subFile, m, None, [], [], [])
                        ClosureFile(fileName, m, Some parseResult, parseDiagnostics, diagnosticsLogger.Diagnostics, noWarns)

                    else
                        // Don't traverse into .fs leafs.
                        printfn "yielding non-script source %s" fileName
                        ClosureFile(fileName, m, None, [], [], []) ]

        let sources = closureSources |> List.collect loop
        let packageReferences = packageReferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Seq.toArray
        sources, tcConfig, packageReferences

    /// Reduce the full directive closure into LoadClosure
    let GetLoadClosure(rootFilename, closureFiles, tcConfig: TcConfig, codeContext, packageReferences, earlierDiagnostics) =

        // Mark the last file as isLastCompiland.
        let closureFiles =
            if isNil closureFiles then
                closureFiles
            else
                match List.frontAndBack closureFiles with
                | rest, ClosureFile
                           (fileName, m,
                            Some(ParsedInput.ImplFile (ParsedImplFileInput (name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, _, trivia))),
                            parseDiagnostics, metaDiagnostics, nowarns) ->

                    let isLastCompiland = (true, tcConfig.target.IsExe)
                    rest @ [ClosureFile
                                (fileName, m,
                                 Some(ParsedInput.ImplFile (ParsedImplFileInput (name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, isLastCompiland, trivia))),
                                 parseDiagnostics, metaDiagnostics, nowarns)]

                | _ -> closureFiles

        // Get all source files.
        let sourceFiles = [ for ClosureFile(fileName, m, _, _, _, _) in closureFiles -> (fileName, m) ]

        let sourceInputs =
            [  for ClosureFile(fileName, _, input, parseDiagnostics, metaDiagnostics, _nowarns) in closureFiles ->
                   ({ FileName=fileName
                      SyntaxTree=input
                      ParseDiagnostics=parseDiagnostics
                      MetaCommandDiagnostics=metaDiagnostics } : LoadClosureInput) ]

        let globalNoWarns = closureFiles |> List.collect (fun (ClosureFile(_, _, _, _, _, noWarns)) -> noWarns)

        // Resolve all references.
        let references, unresolvedReferences, resolutionDiagnostics =
            let diagnosticsLogger = CapturingDiagnosticsLogger("GetLoadClosure")

            use unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind (fun _ -> diagnosticsLogger)
            let references, unresolvedReferences = TcAssemblyResolutions.GetAssemblyResolutionInformation(tcConfig)
            let references = references |> List.map (fun ar -> ar.resolvedPath, ar)
            references, unresolvedReferences, diagnosticsLogger.Diagnostics

        // Root errors and warnings - look at the last item in the closureFiles list
        let loadClosureRootDiagnostics, allRootDiagnostics =
            match List.rev closureFiles with
            | ClosureFile(_, _, _, parseDiagnostics, metaDiagnostics, _) :: _ ->
                (earlierDiagnostics @ metaDiagnostics @ resolutionDiagnostics),
                (parseDiagnostics @ earlierDiagnostics @ metaDiagnostics @ resolutionDiagnostics)
            | _ -> [], [] // When no file existed.

        let isRootRange exn =
            match GetRangeOfDiagnostic exn with
            | Some m ->
                // Return true if the error was *not* from a #load-ed file.
                let isArgParameterWhileNotEditing = (codeContext <> CodeContext.Editing) && (equals m range0 || equals m rangeStartup || equals m rangeCmdArgs)
                let isThisFileName = (0 = String.Compare(rootFilename, m.FileName, StringComparison.OrdinalIgnoreCase))
                isArgParameterWhileNotEditing || isThisFileName
            | None -> true

        // Filter out non-root errors and warnings
        let allRootDiagnostics = allRootDiagnostics |> List.filter (fst >> isRootRange)

        let result: LoadClosure =
            { SourceFiles = List.groupBy fst sourceFiles |> List.map (map2Of2 (List.map snd))
              References = List.groupBy fst references |> List.map (map2Of2 (List.map snd))
              PackageReferences = packageReferences
              UseDesktopFramework = (tcConfig.primaryAssembly = PrimaryAssembly.Mscorlib)
              SdkDirOverride = tcConfig.sdkDirOverride
              UnresolvedReferences = unresolvedReferences
              Inputs = sourceInputs
              NoWarns = List.groupBy fst globalNoWarns |> List.map (map2Of2 (List.map snd))
              OriginalLoadReferences = tcConfig.loadedSources
              ResolutionDiagnostics = resolutionDiagnostics
              AllRootFileDiagnostics = allRootDiagnostics
              LoadClosureRootFileDiagnostics = loadClosureRootDiagnostics }

        result

    /// Given source text, find the full load closure. Used from service.fs, when editing a script file
    let GetFullClosureOfScriptText
        (
            legacyReferenceResolver,
            defaultFSharpBinariesDir,
            fileName,
            sourceText,
            codeContext,
            useSimpleResolution,
            useFsiAuxLib,
            useSdkRefs,
            sdkDirOverride,
            lexResourceManager: Lexhelp.LexResourceManager,
            applyCommandLineArgs,
            assumeDotNetFramework,
            tryGetMetadataSnapshot,
            reduceMemoryUsage,
            dependencyProvider
        ) =

        // Resolve the basic references such as FSharp.Core.dll first, before processing any #I directives in the script
        //
        // This is tries to mimic the action of running the script in F# Interactive - the initial context for scripting is created
        // first, then #I and other directives are processed.
        let references0, assumeDotNetFramework, scriptDefaultReferencesDiagnostics =
            let tcConfig, scriptDefaultReferencesDiagnostics =
                CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir,
                    fileName, codeContext, useSimpleResolution,
                    useFsiAuxLib, None, applyCommandLineArgs, assumeDotNetFramework,
                    useSdkRefs, sdkDirOverride, tryGetMetadataSnapshot, reduceMemoryUsage)

            let resolutions0, _unresolvedReferences = TcAssemblyResolutions.GetAssemblyResolutionInformation(tcConfig)
            let references0 = resolutions0 |> List.map (fun r->r.originalReference.Range, r.resolvedPath) |> Seq.distinct |> List.ofSeq
            references0, tcConfig.assumeDotNetFramework, scriptDefaultReferencesDiagnostics

        let tcConfig, scriptDefaultReferencesDiagnostics =
            CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir, fileName,
                 codeContext, useSimpleResolution, useFsiAuxLib, Some (references0, scriptDefaultReferencesDiagnostics),
                 applyCommandLineArgs, assumeDotNetFramework, useSdkRefs, sdkDirOverride,
                 tryGetMetadataSnapshot, reduceMemoryUsage)

        let closureSources = [ClosureSource(fileName, range0, sourceText, true)]
        let closureFiles, tcConfig, packageReferences = FindClosureFiles(fileName, closureSources, tcConfig, codeContext, lexResourceManager, dependencyProvider)
        GetLoadClosure(fileName, closureFiles, tcConfig, codeContext, packageReferences, scriptDefaultReferencesDiagnostics)

    /// Given source file fileName, find the full load closure
    /// Used from fsi.fs and fsc.fs, for #load and command line
    let GetFullClosureOfScriptFiles
        (
            tcConfig:TcConfig,
            files:(string*range) list,
            codeContext,
            lexResourceManager: Lexhelp.LexResourceManager,
            dependencyProvider
        ) =

        let mainFile, _mainFileRange = List.last files
        let closureSources = files |> List.collect (fun (fileName, m) -> ClosureSourceOfFilename(fileName, m,tcConfig.inputCodePage,true))
        let closureFiles, tcConfig, packageReferences = FindClosureFiles(mainFile, closureSources, tcConfig, codeContext, lexResourceManager, dependencyProvider)
        GetLoadClosure(mainFile, closureFiles, tcConfig, codeContext, packageReferences, [])

type LoadClosure with
    /// Analyze a script text and find the closure of its references.
    /// Used from FCS, when editing a script file.
    ///
    /// A temporary TcConfig is created along the way, is why this routine takes so many arguments. We want to be sure to use exactly the
    /// same arguments as the rest of the application.
    static member ComputeClosureOfScriptText
        (
            legacyReferenceResolver,
            defaultFSharpBinariesDir,
            fileName: string,
            sourceText: ISourceText,
            implicitDefines,
            useSimpleResolution: bool,
            useFsiAuxLib,
            useSdkRefs,
            sdkDir,
            lexResourceManager: Lexhelp.LexResourceManager,
            applyCompilerOptions,
            assumeDotNetFramework,
            tryGetMetadataSnapshot,
            reduceMemoryUsage,
            dependencyProvider
        ) =

        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptText
            (legacyReferenceResolver, defaultFSharpBinariesDir, fileName, sourceText,
             implicitDefines, useSimpleResolution, useFsiAuxLib, useSdkRefs, sdkDir, lexResourceManager,
             applyCompilerOptions, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage, dependencyProvider)

    /// Analyze a set of script files and find the closure of their references.
    static member ComputeClosureOfScriptFiles
        (
            tcConfig: TcConfig,
            files:(string*range) list,
            implicitDefines,
            lexResourceManager: Lexhelp.LexResourceManager,
            dependencyProvider
        ) =

        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptFiles (tcConfig, files, implicitDefines, lexResourceManager, dependencyProvider)
