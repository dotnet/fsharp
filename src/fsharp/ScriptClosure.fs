// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Compute the load closure of a set of script files
module internal FSharp.Compiler.ScriptClosure

open System
open System.Collections.Generic
open System.IO
open System.Text

open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DotNetFrameworkDependencies
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.Range
open FSharp.Compiler.ReferenceResolver
open FSharp.Compiler.Text

open Microsoft.DotNet.DependencyManager

[<RequireQualifiedAccess>]
type LoadClosureInput = 
    { FileName: string
      SyntaxTree: ParsedInput option
      ParseDiagnostics: (PhasedDiagnostic * bool) list 
      MetaCommandDiagnostics: (PhasedDiagnostic * bool) list }

[<RequireQualifiedAccess>]
type LoadClosure = 
    { /// The source files along with the ranges of the #load positions in each file.
      SourceFiles: (string * range list) list

      /// The resolved references along with the ranges of the #r positions in each file.
      References: (string * AssemblyResolution list) list

      /// The resolved pacakge references along with the ranges of the #r positions in each file.
      PackageReferences: (range * string list)[]

      /// The list of references that were not resolved during load closure. These may still be extension references.
      UnresolvedReferences: UnresolvedAssemblyReference list

      /// The list of all sources in the closure with inputs when available
      Inputs: LoadClosureInput list

      /// The #load, including those that didn't resolve
      OriginalLoadReferences: (range * string * string) list

      /// The #nowarns
      NoWarns: (string * range list) list

      /// Diagnostics seen while processing resolutions
      ResolutionDiagnostics: (PhasedDiagnostic * bool) list

      /// Diagnostics seen while parsing root of closure
      AllRootFileDiagnostics: (PhasedDiagnostic * bool) list

      /// Diagnostics seen while processing the compiler options implied root of closure
      LoadClosureRootFileDiagnostics: (PhasedDiagnostic * bool) list
    }   


[<RequireQualifiedAccess>]
type CodeContext =
    | CompilationAndEvaluation // in fsi.exe
    | Compilation  // in fsc.exe
    | Editing // in VS

module ScriptPreprocessClosure = 
    open Internal.Utilities.Text.Lexing
    
    /// Represents an input to the closure finding process
    type ClosureSource = ClosureSource of filename: string * referenceRange: range * sourceText: ISourceText * parseRequired: bool 
        
    /// Represents an output of the closure finding process
    type ClosureFile = ClosureFile of string * range * ParsedInput option * (PhasedDiagnostic * bool) list * (PhasedDiagnostic * bool) list * (string * range) list // filename, range, errors, warnings, nowarns

    type Observed() =
        let seen = System.Collections.Generic.Dictionary<_, bool>()
        member ob.SetSeen check = 
            if not(seen.ContainsKey check) then 
                seen.Add(check, true)
        
        member ob.HaveSeen check =
            seen.ContainsKey check
    
    /// Parse a script from source.
    let ParseScriptText
           (filename: string, sourceText: ISourceText, tcConfig: TcConfig, codeContext,
            lexResourceManager: Lexhelp.LexResourceManager, errorLogger: ErrorLogger) =

        // fsc.exe -- COMPILED\!INTERACTIVE
        // fsi.exe -- !COMPILED\INTERACTIVE
        // Language service
        //     .fs -- EDITING + COMPILED\!INTERACTIVE
        //     .fsx -- EDITING + !COMPILED\INTERACTIVE    
        let defines =
            match codeContext with 
            | CodeContext.CompilationAndEvaluation -> ["INTERACTIVE"]
            | CodeContext.Compilation -> ["COMPILED"]
            | CodeContext.Editing -> "EDITING" :: (if IsScript filename then ["INTERACTIVE"] else ["COMPILED"])

        let isFeatureSupported featureId = tcConfig.langVersion.SupportsFeature featureId
        let lexbuf = UnicodeLexing.SourceTextAsLexbuf(isFeatureSupported, sourceText) 

        let isLastCompiland = (IsScript filename), tcConfig.target.IsExe        // The root compiland is last in the list of compilands.
        ParseOneInputLexbuf (tcConfig, lexResourceManager, defines, lexbuf, filename, isLastCompiland, errorLogger) 

    /// Create a TcConfig for load closure starting from a single .fsx file
    let CreateScriptTextTcConfig 
           (legacyReferenceResolver, defaultFSharpBinariesDir, 
            filename: string, codeContext, 
            useSimpleResolution, useFsiAuxLib, 
            basicReferences, applyCommandLineArgs, 
            assumeDotNetFramework, useSdkRefs,
            tryGetMetadataSnapshot, reduceMemoryUsage) =  

        let projectDir = Path.GetDirectoryName filename
        let isInteractive = (codeContext = CodeContext.CompilationAndEvaluation)
        let isInvalidationSupported = (codeContext = CodeContext.Editing)

        let tcConfigB = 
            TcConfigBuilder.CreateNew
                (legacyReferenceResolver, defaultFSharpBinariesDir, reduceMemoryUsage, projectDir, 
                 isInteractive, isInvalidationSupported, CopyFSharpCoreFlag.No, 
                 tryGetMetadataSnapshot) 

        applyCommandLineArgs tcConfigB

        match basicReferences with 
        | None -> (basicReferencesForScriptLoadClosure useFsiAuxLib useSdkRefs assumeDotNetFramework) |> List.iter(fun f->tcConfigB.AddReferencedAssemblyByPath(range0, f)) // Add script references
        | Some rs -> for m, r in rs do tcConfigB.AddReferencedAssemblyByPath(m, r)

        tcConfigB.resolutionEnvironment <-
            match codeContext with 
            | CodeContext.Editing -> ResolutionEnvironment.EditingOrCompilation true
            | CodeContext.Compilation -> ResolutionEnvironment.EditingOrCompilation false
            | CodeContext.CompilationAndEvaluation -> ResolutionEnvironment.CompilationAndEvaluation
        tcConfigB.framework <- false 
        tcConfigB.useSimpleResolution <- useSimpleResolution
        // Indicates that there are some references not in basicReferencesForScriptLoadClosure which should
        // be added conditionally once the relevant version of mscorlib.dll has been detected.
        tcConfigB.implicitlyResolveAssemblies <- false
        tcConfigB.useSdkRefs <- useSdkRefs

        TcConfig.Create(tcConfigB, validate=true)

    let ClosureSourceOfFilename(filename, m, inputCodePage, parseRequired) = 
        try
            let filename = FileSystem.GetFullPathShim filename
            use stream = FileSystem.FileStreamReadShim filename
            use reader = 
                match inputCodePage with 
                | None -> new StreamReader(stream, true)
                | Some (n: int) -> new StreamReader(stream, Encoding.GetEncoding n) 
            let source = reader.ReadToEnd()
            [ClosureSource(filename, m, SourceText.ofString source, parseRequired)]
        with e -> 
            errorRecovery e m 
            []
            
    let ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn
           (tcConfig: TcConfig, inp: ParsedInput,
            pathOfMetaCommandSource, dependencyProvider) = 

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
        (mainFile, _m, closureSources, origTcConfig:TcConfig, 
         codeContext, lexResourceManager: Lexhelp.LexResourceManager, dependencyProvider: DependencyProvider) =

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
                            | null ->
                                errorR(Error(dependencyProvider.CreatePackageManagerUnknownError(tcConfig.compilerToolPaths, outputDir, packageManagerKey, reportError), m))

                            | dependencyManager ->
                                let directive d =
                                    match d with
                                    | Directive.Resolution -> "r"
                                    | Directive.Include -> "i"

                                let packageManagerTextLines = packageManagerLines |> List.map(fun l -> directive l.Directive, l.Line)
                                let result = dependencyProvider.Resolve(dependencyManager, ".fsx", packageManagerTextLines, reportError, executionTfm, executionRid, tcConfig.implicitIncludeDir, mainFile, scriptName)
                                if result.Success then
                                    // Resolution produced no errors
                                    //Write outputs in F# Interactive and compiler
                                    if codeContext <> CodeContext.Editing then 
                                        for line in result.StdOut do Console.Out.WriteLine(line)
                                        for line in result.StdError do Console.Error.WriteLine(line)

                                    packageReferences.[m] <- [ for script in result.SourceFiles do yield! File.ReadAllLines script ]
                                    if not (Seq.isEmpty result.Roots) then
                                        let tcConfigB = tcConfig.CloneToBuilder()
                                        for folder in result.Roots do 
                                            tcConfigB.AddIncludePath(m, folder, "")
                                        tcConfigB.packageManagerLines <- PackageManagerLine.SetLinesAsProcessed packageManagerKey tcConfigB.packageManagerLines
                                        tcConfig <- TcConfig.Create(tcConfigB, validate=false)
                                    for script in result.SourceFiles do
                                        let scriptText = File.ReadAllText script
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

        and loop (ClosureSource(filename, m, sourceText, parseRequired)) = 
            [   if not (observedSources.HaveSeen(filename)) then
                    observedSources.SetSeen(filename)
                    //printfn "visiting %s" filename
                    if IsScript filename || parseRequired then 
                        let parseResult, parseDiagnostics =
                            let errorLogger = CapturingErrorLogger("FindClosureParse")
                            use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                            let result = ParseScriptText (filename, sourceText, tcConfig, codeContext, lexResourceManager, errorLogger) 
                            result, errorLogger.Diagnostics

                        match parseResult with 
                        | Some parsedScriptAst ->
                            let errorLogger = CapturingErrorLogger("FindClosureMetaCommands")
                            use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                            let pathOfMetaCommandSource = Path.GetDirectoryName filename
                            let preSources = tcConfig.GetAvailableLoadedSources()

                            let tcConfigResult, noWarns = ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn (tcConfig, parsedScriptAst, pathOfMetaCommandSource, dependencyProvider)
                            tcConfig <- tcConfigResult // We accumulate the tcConfig in order to collect assembly references

                            yield! resolveDependencyManagerSources filename

                            let postSources = tcConfig.GetAvailableLoadedSources()
                            let sources = if preSources.Length < postSources.Length then postSources.[preSources.Length..] else []

                            yield! resolveDependencyManagerSources filename
                            for (m, subFile) in sources do
                                if IsScript subFile then 
                                    for subSource in ClosureSourceOfFilename(subFile, m, tcConfigResult.inputCodePage, false) do
                                        yield! loop subSource
                                else
                                    yield ClosureFile(subFile, m, None, [], [], []) 
                            yield ClosureFile(filename, m, Some parsedScriptAst, parseDiagnostics, errorLogger.Diagnostics, noWarns)

                        | None -> 
                            printfn "yielding source %s (failed parse)" filename
                            yield ClosureFile(filename, m, None, parseDiagnostics, [], [])
                    else 
                        // Don't traverse into .fs leafs.
                        printfn "yielding non-script source %s" filename
                        yield ClosureFile(filename, m, None, [], [], []) ]

        let sources = closureSources |> List.collect loop
        let packageReferences = packageReferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Seq.toArray
        sources, tcConfig, packageReferences
        
        
    /// Reduce the full directive closure into LoadClosure
    let GetLoadClosure(ctok, rootFilename, closureFiles, tcConfig: TcConfig, codeContext, packageReferences) = 
    
        // Mark the last file as isLastCompiland. 
        let closureFiles =
            if isNil closureFiles then  
                closureFiles 
            else 
                match List.frontAndBack closureFiles with
                | rest, ClosureFile
                           (filename, m, 
                            Some(ParsedInput.ImplFile (ParsedImplFileInput (name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, _))), 
                            parseDiagnostics, metaDiagnostics, nowarns) -> 

                    let isLastCompiland = (true, tcConfig.target.IsExe)
                    rest @ [ClosureFile
                                (filename, m, 
                                 Some(ParsedInput.ImplFile (ParsedImplFileInput (name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, isLastCompiland))), 
                                 parseDiagnostics, metaDiagnostics, nowarns)]

                | _ -> closureFiles

        // Get all source files.
        let sourceFiles = [ for (ClosureFile(filename, m, _, _, _, _)) in closureFiles -> (filename, m) ]

        let sourceInputs = 
            [  for (ClosureFile(filename, _, input, parseDiagnostics, metaDiagnostics, _nowarns)) in closureFiles ->
                   ({ FileName=filename
                      SyntaxTree=input
                      ParseDiagnostics=parseDiagnostics
                      MetaCommandDiagnostics=metaDiagnostics } : LoadClosureInput) ]

        let globalNoWarns = closureFiles |> List.collect (fun (ClosureFile(_, _, _, _, _, noWarns)) -> noWarns)

        // Resolve all references.
        let references, unresolvedReferences, resolutionDiagnostics = 
            let errorLogger = CapturingErrorLogger("GetLoadClosure") 
        
            use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
            let references, unresolvedReferences = TcAssemblyResolutions.GetAssemblyResolutionInformation(ctok, tcConfig)
            let references = references |> List.map (fun ar -> ar.resolvedPath, ar)
            references, unresolvedReferences, errorLogger.Diagnostics

        // Root errors and warnings - look at the last item in the closureFiles list
        let loadClosureRootDiagnostics, allRootDiagnostics = 
            match List.rev closureFiles with
            | ClosureFile(_, _, _, parseDiagnostics, metaDiagnostics, _) :: _ -> 
                (metaDiagnostics @ resolutionDiagnostics), 
                (parseDiagnostics @ metaDiagnostics @ resolutionDiagnostics)
            | _ -> [], [] // When no file existed.
        
        let isRootRange exn =
            match GetRangeOfDiagnostic exn with
            | Some m -> 
                // Return true if the error was *not* from a #load-ed file.
                let isArgParameterWhileNotEditing = (codeContext <> CodeContext.Editing) && (Range.equals m range0 || Range.equals m rangeStartup || Range.equals m rangeCmdArgs)
                let isThisFileName = (0 = String.Compare(rootFilename, m.FileName, StringComparison.OrdinalIgnoreCase))
                isArgParameterWhileNotEditing || isThisFileName
            | None -> true

        // Filter out non-root errors and warnings
        let allRootDiagnostics = allRootDiagnostics |> List.filter (fst >> isRootRange)
        
        let result: LoadClosure =
            { SourceFiles = List.groupBy fst sourceFiles |> List.map (map2Of2 (List.map snd))
              References = List.groupBy fst references |> List.map (map2Of2 (List.map snd))
              PackageReferences = packageReferences
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
           (ctok, legacyReferenceResolver, defaultFSharpBinariesDir, 
            filename, sourceText, codeContext, 
            useSimpleResolution, useFsiAuxLib, useSdkRefs,
            lexResourceManager: Lexhelp.LexResourceManager, 
            applyCommandLineArgs, assumeDotNetFramework,
            tryGetMetadataSnapshot, reduceMemoryUsage, dependencyProvider) =

        // Resolve the basic references such as FSharp.Core.dll first, before processing any #I directives in the script
        //
        // This is tries to mimic the action of running the script in F# Interactive - the initial context for scripting is created
        // first, then #I and other directives are processed.
        let references0 = 
            let tcConfig = 
                CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir, 
                    filename, codeContext, useSimpleResolution, 
                    useFsiAuxLib, None, applyCommandLineArgs, assumeDotNetFramework, 
                    useSdkRefs, tryGetMetadataSnapshot, reduceMemoryUsage)

            let resolutions0, _unresolvedReferences = TcAssemblyResolutions.GetAssemblyResolutionInformation(ctok, tcConfig)
            let references0 = resolutions0 |> List.map (fun r->r.originalReference.Range, r.resolvedPath) |> Seq.distinct |> List.ofSeq
            references0

        let tcConfig = 
            CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir, filename, 
                 codeContext, useSimpleResolution, useFsiAuxLib, Some references0, 
                 applyCommandLineArgs, assumeDotNetFramework, useSdkRefs,
                 tryGetMetadataSnapshot, reduceMemoryUsage)

        let closureSources = [ClosureSource(filename, range0, sourceText, true)]
        let closureFiles, tcConfig, packageReferences = FindClosureFiles(filename, range0, closureSources, tcConfig, codeContext, lexResourceManager, dependencyProvider)
        GetLoadClosure(ctok, filename, closureFiles, tcConfig, codeContext, packageReferences)

    /// Given source filename, find the full load closure
    /// Used from fsi.fs and fsc.fs, for #load and command line
    let GetFullClosureOfScriptFiles
            (ctok, tcConfig:TcConfig, files:(string*range) list, codeContext, 
             lexResourceManager: Lexhelp.LexResourceManager, dependencyProvider) =

        let mainFile, mainFileRange = List.last files
        let closureSources = files |> List.collect (fun (filename, m) -> ClosureSourceOfFilename(filename, m,tcConfig.inputCodePage,true))
        let closureFiles, tcConfig, packageReferences = FindClosureFiles(mainFile, mainFileRange, closureSources, tcConfig, codeContext, lexResourceManager, dependencyProvider)
        GetLoadClosure(ctok, mainFile, closureFiles, tcConfig, codeContext, packageReferences)        

type LoadClosure with
    /// Analyze a script text and find the closure of its references. 
    /// Used from FCS, when editing a script file.  
    //
    /// A temporary TcConfig is created along the way, is why this routine takes so many arguments. We want to be sure to use exactly the
    /// same arguments as the rest of the application.
    static member ComputeClosureOfScriptText
                     (ctok, legacyReferenceResolver, defaultFSharpBinariesDir, 
                      filename: string, sourceText: ISourceText, implicitDefines, useSimpleResolution: bool, 
                      useFsiAuxLib, useSdkRefs, lexResourceManager: Lexhelp.LexResourceManager, 
                      applyCompilerOptions, assumeDotNetFramework, tryGetMetadataSnapshot,
                      reduceMemoryUsage, dependencyProvider) = 

        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptText
            (ctok, legacyReferenceResolver, defaultFSharpBinariesDir, filename, sourceText, 
             implicitDefines, useSimpleResolution, useFsiAuxLib, useSdkRefs, lexResourceManager, 
             applyCompilerOptions, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage, dependencyProvider)

    /// Analyze a set of script files and find the closure of their references.
    static member ComputeClosureOfScriptFiles
                     (ctok, tcConfig: TcConfig, files:(string*range) list, implicitDefines,
                      lexResourceManager: Lexhelp.LexResourceManager, dependencyProvider) =

        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptFiles (ctok, tcConfig, files, implicitDefines, lexResourceManager, dependencyProvider)
