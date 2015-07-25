// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Driver for F# compiler. 
// 
// Roughly divides into:
//    - Parsing
//    - Flags 
//    - Importing IL assemblies
//    - Compiling (including optimizing)
//    - Linking (including ILX-IL transformation)


module internal Microsoft.FSharp.Compiler.Driver 

open System.IO
open System.Reflection
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Text
open Internal.Utilities
open Internal.Utilities.Collections
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.AbstractIL.IL
#if NO_COMPILER_BACKEND
#else
open Microsoft.FSharp.Compiler.Ilxgen
#endif
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
#if SQM_SUPPORT
open Microsoft.FSharp.Compiler.SqmLogger
#endif
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Infos.AccessibilityLogic
open Microsoft.FSharp.Compiler.Infos.AttributeChecking
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Opt
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Fscopts
open Microsoft.FSharp.Compiler.DiagnosticMessage

#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This code has logic used by the project system to do the front-end logic that starts at command-line arguments 
// and gets as far as importing all references (used for deciding the icon, normal or typeProvider, of the assembly).
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

//----------------------------------------------------------------------------
// Reporting - warnings, errors
//----------------------------------------------------------------------------

type ErrorLoggerThatAccumulatesErrors private (implicitIncludeDir, showFullPaths, flatErrors, errorStyle, globalWarnLevel, specificWarnOn, specificWarnOff, specificWarnAsError, specificWarnAsWarn, globalWarnAsError) = 
    inherit ErrorLogger("ErrorLoggerThatAccumulatesErrors")
    let messages = ResizeArray()
    let mutable errorsCount = 0
    new(tcConfigB : TcConfigBuilder) = 
        ErrorLoggerThatAccumulatesErrors(
            tcConfigB.implicitIncludeDir, 
            tcConfigB.showFullPaths, 
            tcConfigB.flatErrors, 
            tcConfigB.errorStyle, 
            tcConfigB.globalWarnLevel, 
            tcConfigB.specificWarnOn, 
            tcConfigB.specificWarnOff,
            tcConfigB.specificWarnAsError,
            tcConfigB.specificWarnAsWarn,
            tcConfigB.globalWarnAsError
            )
    new(tcConfig : TcConfig) = 
        ErrorLoggerThatAccumulatesErrors(
            tcConfig.implicitIncludeDir, 
            tcConfig.showFullPaths, 
            tcConfig.flatErrors, 
            tcConfig.errorStyle, 
            tcConfig.globalWarnLevel, 
            tcConfig.specificWarnOn, 
            tcConfig.specificWarnOff,
            tcConfig.specificWarnAsError,
            tcConfig.specificWarnAsWarn,
            tcConfig.globalWarnAsError
            )
    member this.ProcessMessage(err, isError) = 
        let writer = new System.IO.StringWriter()

        let writeError err = 
            writeViaBufferWithEnvironmentNewLines writer (OutputErrorOrWarning (implicitIncludeDir, showFullPaths, flatErrors, errorStyle, false)) err

        let isError = 
            if isError then
                writeError err
                true
            else
                if (ReportWarningAsError globalWarnLevel specificWarnOff specificWarnOn specificWarnAsError specificWarnAsWarn globalWarnAsError err) then 
                    writeError err
                    true
                elif ReportWarning globalWarnLevel specificWarnOff specificWarnOn err then 
                    writeViaBufferWithEnvironmentNewLines writer (OutputErrorOrWarning (implicitIncludeDir, showFullPaths, flatErrors, errorStyle, true)) err
                    false
                else 
                    false // will not be used        
        let text = writer.ToString()
        if text.Length <> 0 then Some (isError, text) else None
        
    member this.GetMessages() = List.ofSeq messages
    override this.ErrorSinkImpl(err) = 
        errorsCount <- errorsCount + 1
        messages.Add(this.ProcessMessage(err, true).Value)
    override this.WarnSinkImpl(warn) = 
        match this.ProcessMessage (warn, false) with
        | Some ((isError, _) as res) -> 
            if isError then errorsCount <- errorsCount + 1
            messages.Add(res)
        | _ -> ()

    override this.ErrorCount = errorsCount

[<AbstractClass>]
type ErrorLoggerThatQuitsAfterMaxErrors(tcConfigB:TcConfigBuilder, exiter : Exiter, caption) = 
    inherit ErrorLogger(caption)

    let errors = ref 0
    let errorNumbers = ref []
    let warningNumbers = ref []

    abstract HandleIssue : tcConfigB : TcConfigBuilder * error : PhasedError * isWarning : bool -> unit
    abstract HandleTooManyErrors : text : string -> unit

    override x.ErrorCount = !errors
    override x.ErrorSinkImpl(err) = 
                if !errors >= tcConfigB.maxErrors then 
                    x.HandleTooManyErrors(FSComp.SR.fscTooManyErrors())
#if SQM_SUPPORT
                    SqmLoggerWithConfigBuilder tcConfigB !errorNumbers !warningNumbers
#endif
                    exiter.Exit 1

                x.HandleIssue(tcConfigB, err, false)

                incr errors
                errorNumbers := (GetErrorNumber err) :: !errorNumbers

                match err.Exception with 
                | InternalError _ 
                | Failure _ 
                | :? KeyNotFoundException -> 
                    match tcConfigB.simulateException with
                    | Some _ -> () // Don't show an assert for simulateException case so that unittests can run without an assert dialog.                     
                    | None -> System.Diagnostics.Debug.Assert(false,sprintf "Bug seen in compiler: %s" (err.ToString()))
                | _ -> 
                    ()

    override x.WarnSinkImpl(err) =  
        if (ReportWarningAsError tcConfigB.globalWarnLevel tcConfigB.specificWarnOff tcConfigB.specificWarnOn tcConfigB.specificWarnAsError tcConfigB.specificWarnAsWarn tcConfigB.globalWarnAsError err) then
            x.ErrorSink(err)
        elif ReportWarning tcConfigB.globalWarnLevel tcConfigB.specificWarnOff tcConfigB.specificWarnOn err then
            x.HandleIssue(tcConfigB, err, true)
            warningNumbers :=  (GetErrorNumber err) :: !warningNumbers
    
    override x.WarningNumbers = !warningNumbers
    override x.ErrorNumbers = !errorNumbers

/// Create an error logger that counts and prints errors 
let ConsoleErrorLoggerThatQuitsAfterMaxErrors (tcConfigB:TcConfigBuilder, exiter : Exiter) : ErrorLogger = 
    upcast {
        new ErrorLoggerThatQuitsAfterMaxErrors(tcConfigB, exiter, "ConsoleErrorLoggerThatQuitsAfterMaxErrors") with
            
            member this.HandleTooManyErrors(text : string) = 
                DoWithErrorColor true (fun () -> Printf.eprintfn "%s" text)

            member this.HandleIssue(tcConfigB, err, isWarning) =
                DoWithErrorColor isWarning (fun () -> 
                    (writeViaBufferWithEnvironmentNewLines stderr (OutputErrorOrWarning (tcConfigB.implicitIncludeDir,tcConfigB.showFullPaths,tcConfigB.flatErrors,tcConfigB.errorStyle,isWarning)) err;
                    stderr.WriteLine())
                    );
    }

//    val TypeCheck : TcConfig * TcImports * TcGlobals * ErrorLogger * string * NiceNameGenerator * TypeChecker.TcEnv * Input list * Exiter -> 
//              TcState * TypeChecker.TopAttribs * Tast.TypedAssembly * TypeChecker.TcEnv
let TypeCheck (tcConfig,tcImports,tcGlobals,errorLogger:ErrorLogger,assemblyName,niceNameGen,tcEnv0,inputs, exiter : Exiter) =
    try 
        if isNil inputs then error(Error(FSComp.SR.fscNoImplementationFiles(),Range.rangeStartup))
        let ccuName = assemblyName
        let tcInitialState = TypecheckInitialState (rangeStartup,ccuName,tcConfig,tcGlobals,tcImports,niceNameGen,tcEnv0)
        TypecheckClosedInputSet ((fun () -> errorLogger.ErrorCount > 0),tcConfig,tcImports,tcGlobals,None,tcInitialState,inputs)
    with e -> 
        errorRecovery e rangeStartup
#if SQM_SUPPORT
        SqmLoggerWithConfig tcConfig errorLogger.ErrorNumbers errorLogger.WarningNumbers
#endif
        exiter.Exit 1

/// This error logger delays the messages it recieves. At the end, call ForwardDelayedErrorsAndWarnings
/// to send the held messages.     
type DelayAndForwardErrorLogger(exiter : Exiter, errorLoggerProvider : ErrorLoggerProvider) =
    inherit ErrorLogger("DelayAndForwardErrorLogger")
    let mapToErrorNumber items = 
        items |> Seq.map (fun (err,_) -> GetErrorNumber err) |> Seq.toList
    let delayed = new ResizeArray<_>()
    let errors = ref 0
    override x.ErrorSinkImpl(e) = 
        errors := !errors + 1
        delayed.Add (e,true)
    override x.ErrorCount = delayed |> Seq.filter snd |> Seq.length
    override x.WarnSinkImpl(e) = delayed.Add(e,false)
    member x.ForwardDelayedErrorsAndWarnings(errorLogger:ErrorLogger) = 
        // Eagerly grab all the errors and warnings from the mutable collection
        let errors = delayed |> Seq.toList
        // Now report them
        for (e,isError) in errors do
            if isError then errorLogger.ErrorSink(e) else errorLogger.WarnSink(e)
        // Clear errors just reported. Keep errors count.
        delayed.Clear()
    member x.ForwardDelayedErrorsAndWarnings(tcConfigB:TcConfigBuilder) = 
        let errorLogger =  errorLoggerProvider.CreateErrorLoggerThatQuitsAfterMaxErrors(tcConfigB, exiter)
        x.ForwardDelayedErrorsAndWarnings(errorLogger)
    member x.FullErrorCount = !errors
    override x.WarningNumbers = delayed |> Seq.filter(fun (_, flag) -> flag = false) |> mapToErrorNumber
    override x.ErrorNumbers = delayed |> Seq.filter(fun (_, flag) -> flag = true) |> mapToErrorNumber

and [<AbstractClass>]
    ErrorLoggerProvider() =
    member this.CreateDelayAndForwardLogger(exiter) = DelayAndForwardErrorLogger(exiter, this)
    abstract CreateErrorLoggerThatQuitsAfterMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

/// Check for .fsx and, if present, compute the load closure for of #loaded files.
let AdjustForScriptCompile(tcConfigB:TcConfigBuilder,commandLineSourceFiles,lexResourceManager) =

    let combineFilePath file =
        try
            if FileSystem.IsPathRootedShim(file) then file
            else Path.Combine(tcConfigB.implicitIncludeDir, file)
        with _ ->
            error (Error(FSComp.SR.pathIsInvalid(file),rangeStartup)) 
            
    let commandLineSourceFiles = 
        commandLineSourceFiles 
        |> List.map combineFilePath
        
    let allSources = ref []       
    
    let tcConfig = TcConfig.Create(tcConfigB,validate=false) 
    
    let AddIfNotPresent(filename:string) =
        if not(!allSources |> List.mem filename) then
            allSources := filename::!allSources
    
    let AppendClosureInformation(filename) =
        if IsScript filename then 
            let closure = LoadClosure.ComputeClosureOfSourceFiles(tcConfig,[filename,rangeStartup],CodeContext.Compilation,lexResourceManager=lexResourceManager,useDefaultScriptingReferences=false)
            let references = closure.References |> List.map snd |> List.concat |> List.map (fun r->r.originalReference) |> List.filter (fun r->r.Range<>range0)
            references |> List.iter (fun r-> tcConfigB.AddReferencedAssemblyByPath(r.Range,r.Text))
            closure.NoWarns |> List.map(fun (n,ms)->ms|>List.map(fun m->m,n)) |> List.concat |> List.iter tcConfigB.TurnWarningOff
            closure.SourceFiles |> List.map fst |> List.iter AddIfNotPresent
            closure.RootWarnings |> List.iter warnSink
            closure.RootErrors |> List.iter errorSink
            
            else AddIfNotPresent(filename)
         
    // Find closure of .fsx files.
    commandLineSourceFiles |> List.iter AppendClosureInformation

    List.rev !allSources

let abortOnError (errorLogger:ErrorLogger, _tcConfig:TcConfig, exiter : Exiter) = 
    if errorLogger.ErrorCount > 0 then
#if SQM_SUPPORT
        SqmLoggerWithConfig _tcConfig errorLogger.ErrorNumbers errorLogger.WarningNumbers
#endif
        exiter.Exit 1

type DelayedDisposables() = 
    let items = Stack<System.IDisposable>()
    member this.Register(i) = items.Push i
    interface System.IDisposable with
        member this.Dispose() = 
            let l = List.ofSeq items
            items.Clear()
            for i in l do 
                try i.Dispose() with _ -> ()

type DefaultLoggerProvider() = 
    inherit ErrorLoggerProvider()
    override this.CreateErrorLoggerThatQuitsAfterMaxErrors(tcConfigBuilder, exiter) = ConsoleErrorLoggerThatQuitsAfterMaxErrors(tcConfigBuilder, exiter)

// The project system needs to be able to somehow crack open assemblies to look for type providers in order to update the assembly reference icon when a user does 'Build'.
// Rather than have the PS re-code that logic, it re-uses the existing code in the very front end of the compiler that parses the command-line and imports the referenced assemblies.
// This code used to be in fsc.exe.  The PS only references FSharp.LanguageService.Compiler, so this code moved from fsc.exe to FS.C.S.dll so that the PS can re-use it.
// A great deal of the logic of this function is repeated in fsi.fs, so maybe should refactor fsi.fs to call into this as well.
let getTcImportsFromCommandLine(typeProviderAssemblyFound : (string->unit) option,
                                argv : string[], 
                                defaultFSharpBinariesDir : string, 
                                directoryBuildingFrom : string, 
                                lcidFromCodePage : int option, 
                                setProcessThreadLocals : TcConfigBuilder -> unit, 
                                displayBannerIfNeeded : TcConfigBuilder -> unit, 
                                optimizeForMemory : bool,
                                exiter : Exiter,
                                errorLoggerProvider : ErrorLoggerProvider,
                                disposables : DelayedDisposables) 
                                    : TcGlobals * TcImports * TcImports * Tast.CcuThunk * Tast.TypedAssembly * TypeChecker.TopAttribs * TcConfig * string * string option * string * ErrorLogger
                                    =
    

    let tcConfigB = Build.TcConfigBuilder.CreateNew(defaultFSharpBinariesDir, optimizeForMemory, directoryBuildingFrom, isInteractive=false, isInvalidationSupported=false)
    // Preset: --optimize+ -g --tailcalls+ (see 4505)
    SetOptimizeSwitch tcConfigB On
    SetDebugSwitch    tcConfigB None Off
    SetTailcallSwitch tcConfigB On    

    // Now install a delayed logger to hold all errors from flags until after all flags have been parsed (for example, --vserrors)
    let delayForFlagsLogger =  errorLoggerProvider.CreateDelayAndForwardLogger(exiter)// DelayAndForwardErrorLogger(exiter)
    let _unwindEL_1 = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayForFlagsLogger)          
    
    // Share intern'd strings across all lexing/parsing
    let lexResourceManager = new Lexhelp.LexResourceManager()

    // process command line, flags and collect filenames 
    let sourceFiles = 

        // The ParseCompilerOptions function calls imperative function to process "real" args
        // Rather than start processing, just collect names, then process them. 
        try 
            let inputFilesRef   = ref ([] : string list)
            let collect name = 
                let lower = String.lowercase name
                if List.exists (Filename.checkSuffix lower) [".resx"]  then
                    warning(Error(FSComp.SR.fscResxSourceFileDeprecated name,rangeStartup))
                    tcConfigB.AddEmbeddedResource name
                else
                    inputFilesRef := name :: !inputFilesRef
            let abbrevArgs = abbrevFlagSet tcConfigB true
    
            // This is where flags are interpreted by the command line fsc.exe.
            ParseCompilerOptions collect (GetCoreFscCompilerOptions tcConfigB) (List.tail (PostProcessCompilerArgs abbrevArgs argv))
            let inputFiles = List.rev !inputFilesRef

            // Check if we have a codepage from the console
            match tcConfigB.lcid with
            | Some _ -> ()
            | None -> tcConfigB.lcid <- lcidFromCodePage

            setProcessThreadLocals(tcConfigB)

            (* step - get dll references *)
            let dllFiles,sourceFiles = List.partition Filename.isDll inputFiles
            match dllFiles with
            | [] -> ()
            | h::_ -> errorR (Error(FSComp.SR.fscReferenceOnCommandLine(h),rangeStartup))

            dllFiles |> List.iter (fun f->tcConfigB.AddReferencedAssemblyByPath(rangeStartup,f))
          
            let sourceFiles = AdjustForScriptCompile(tcConfigB,sourceFiles,lexResourceManager)                     
            sourceFiles

        with 
            e -> 
            errorRecovery e rangeStartup
#if SQM_SUPPORT
            SqmLoggerWithConfigBuilder tcConfigB delayForFlagsLogger.ErrorNumbers delayForFlagsLogger.WarningNumbers
#endif
            delayForFlagsLogger.ForwardDelayedErrorsAndWarnings(tcConfigB)
            exiter.Exit 1 
    
    tcConfigB.sqmNumOfSourceFiles <- sourceFiles.Length
    tcConfigB.conditionalCompilationDefines <- "COMPILED" :: tcConfigB.conditionalCompilationDefines 
    displayBannerIfNeeded tcConfigB

    // Create tcGlobals and frameworkTcImports
    let outfile,pdbfile,assemblyName = 
        try 
            tcConfigB.DecideNames sourceFiles 
        with e ->
            errorRecovery e rangeStartup
#if SQM_SUPPORT
            SqmLoggerWithConfigBuilder tcConfigB delayForFlagsLogger.ErrorNumbers delayForFlagsLogger.WarningNumbers
#endif
            delayForFlagsLogger.ForwardDelayedErrorsAndWarnings(tcConfigB)
            exiter.Exit 1 
                    
    // DecideNames may give "no inputs" error. Abort on error at this point. bug://3911
    if not tcConfigB.continueAfterParseFailure && delayForFlagsLogger.FullErrorCount > 0 then
#if SQM_SUPPORT
        SqmLoggerWithConfigBuilder tcConfigB delayForFlagsLogger.ErrorNumbers delayForFlagsLogger.WarningNumbers
#endif
        delayForFlagsLogger.ForwardDelayedErrorsAndWarnings(tcConfigB)
        exiter.Exit 1
    
    // If there's a problem building TcConfig, abort    
    let tcConfig = 
        try
            TcConfig.Create(tcConfigB,validate=false)
        with e ->
#if SQM_SUPPORT
            SqmLoggerWithConfigBuilder tcConfigB delayForFlagsLogger.ErrorNumbers delayForFlagsLogger.WarningNumbers
#endif
            delayForFlagsLogger.ForwardDelayedErrorsAndWarnings(tcConfigB)
            exiter.Exit 1
    
    let errorLogger =  errorLoggerProvider.CreateErrorLoggerThatQuitsAfterMaxErrors(tcConfigB, exiter)

    // Install the global error logger and never remove it. This logger does have all command-line flags considered.
    let _unwindEL_2 = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
    
    // Forward all errors from flags
    delayForFlagsLogger.ForwardDelayedErrorsAndWarnings(errorLogger)

    // step - decideNames 
    if not tcConfigB.continueAfterParseFailure then 
        abortOnError(errorLogger, tcConfig, exiter)

    let tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig = 
    
        ReportTime tcConfig "Import mscorlib"

        if tcConfig.useIncrementalBuilder then 
            ReportTime tcConfig "Incremental Parse and Typecheck"
            let builder = 
                new IncrementalFSharpBuild.IncrementalBuilder(tcConfig, directoryBuildingFrom, assemblyName, NiceNameGenerator(), lexResourceManager, sourceFiles,
                                                                ensureReactive=false, 
                                                                errorLogger=errorLogger,
                                                                keepGeneratedTypedAssembly=true)
            let tcState,topAttribs,typedAssembly,_tcEnv,tcImports,tcGlobals,tcConfig = builder.TypeCheck()
            tcGlobals,tcImports,tcImports,tcState.Ccu,typedAssembly,topAttribs,tcConfig
        else
        
            ReportTime tcConfig "Import mscorlib and FSharp.Core.dll"
            ReportTime tcConfig "Import system references"
            let foundationalTcConfigP = TcConfigProvider.Constant(tcConfig)
            let sysRes,otherRes,knownUnresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)
            let tcGlobals,frameworkTcImports = TcImports.BuildFrameworkTcImports (foundationalTcConfigP, sysRes, otherRes)

            // register framework tcImports to be disposed in future
            disposables.Register frameworkTcImports

            // step - parse sourceFiles 
            ReportTime tcConfig "Parse inputs"
            use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)            
            let inputs : (ParsedInput * string) list =
                try  
                    sourceFiles 
                    |> tcConfig.ComputeCanContainEntryPoint 
                    |> List.zip sourceFiles
                    // PERF: consider making this parallel, once uses of global state relevant to parsing are cleaned up 
                    |> List.choose (fun (filename:string,isLastCompiland:bool) -> 
                        let pathOfMetaCommandSource = Path.GetDirectoryName(filename)
                        match ParseOneInputFile(tcConfig,lexResourceManager,["COMPILED"],filename,isLastCompiland,errorLogger,(*retryLocked*)false) with
                        | Some(input)->Some(input,pathOfMetaCommandSource)
                        | None -> None
                        ) 
                with e -> 
                    errorRecoveryNoRange e
#if SQM_SUPPORT
                    SqmLoggerWithConfig tcConfig errorLogger.ErrorNumbers errorLogger.WarningNumbers
#endif
                    exiter.Exit 1

            if tcConfig.parseOnly then exiter.Exit 0 
            if not tcConfig.continueAfterParseFailure then 
                abortOnError(errorLogger, tcConfig, exiter)

            if tcConfig.printAst then                
                inputs |> List.iter (fun (input,_filename) -> printf "AST:\n"; printfn "%+A" input; printf "\n") 

            let tcConfig = (tcConfig,inputs) ||> List.fold ApplyMetaCommandsFromInputToTcConfig 
            let tcConfigP = TcConfigProvider.Constant(tcConfig)

            ReportTime tcConfig "Import non-system references"
            let tcGlobals,tcImports =  
                let tcImports = TcImports.BuildNonFrameworkTcImports(tcConfigP,tcGlobals,frameworkTcImports,otherRes,knownUnresolved)
                tcGlobals,tcImports

            // register tcImports to be disposed in future
            disposables.Register tcImports

            if not tcConfig.continueAfterParseFailure then 
                abortOnError(errorLogger, tcConfig, exiter)

#if EXTENSIONTYPING
            match typeProviderAssemblyFound with
            | None -> ()
            | Some dialog ->
                tcImports.GetImportedAssemblies()
                |> List.filter (fun ia -> not (ia.TypeProviders |> List.isEmpty))
                |> List.iter (fun ia -> ia.FSharpViewOfMetadata.FileName |> Option.iter dialog)
#else
            ignore typeProviderAssemblyFound
#endif

            if tcConfig.importAllReferencesOnly then exiter.Exit 0 

            ReportTime tcConfig "Typecheck"
            use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)            
            let tcEnv0 = GetInitialTypecheckerEnv (Some assemblyName) rangeStartup tcConfig tcImports tcGlobals

            // typecheck 
            let inputs : ParsedInput list = inputs |> List.map fst
            let tcState,topAttrs,typedAssembly,_tcEnvAtEnd = 
                TypeCheck(tcConfig,tcImports,tcGlobals,errorLogger,assemblyName,NiceNameGenerator(),tcEnv0,inputs,exiter)

            let generatedCcu = tcState.Ccu
            abortOnError(errorLogger, tcConfig, exiter)
            ReportTime tcConfig "Typechecked"

            (tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig)
                    
    tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig,outfile,pdbfile,assemblyName,errorLogger

// only called from the project system, as a way to run the front end of the compiler far enough to determine if there are type provider assemblies
let runFromCommandLineToImportingAssemblies(typeProviderAssemblyFound : (string -> unit),
                                            argv : string[], 
                                            defaultFSharpBinariesDir : string, 
                                            directoryBuildingFrom : string, 
                                            exiter : Exiter) =

    use d = new DelayedDisposables() // ensure that any resources that can be allocated in getTcImportsFromCommandLine will be correctly disposed

    let tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig,outfile,pdbfile,assemblyName,errorLogger = 
            getTcImportsFromCommandLine(Some typeProviderAssemblyFound, argv, defaultFSharpBinariesDir, directoryBuildingFrom, None, (fun _ -> ()), 
                    (fun tcConfigB -> 
                        // (kind of abusing this lambda for an unintended purpose, but this is a convenient and correctly-timed place to poke the tcConfigB)
                        tcConfigB.importAllReferencesOnly <- true // stop after importing assemblies (do not typecheck, we don't need typechecking)
                        // for flags below, see IncrementalBuilder.fs:CreateBackgroundBuilderForProjectOptions, as there are many similarities, as these are the two places that we create this from VS code-paths
                        tcConfigB.openBinariesInMemory <- true    // uses more memory but means we don't take read-exclusions on the DLLs we reference (important for VS code path)
                        tcConfigB.openDebugInformationForLaterStaticLinking <- false // Never open PDB files for the PS, even if --standalone is specified
                        if tcConfigB.framework then
                            System.Diagnostics.Debug.Assert(false, "Project system requires --noframework flag")
                            tcConfigB.framework<-false
                        ),   
                    true, // optimizeForMemory - want small memory footprint in VS
                    exiter, 
                    DefaultLoggerProvider(), // this function always use default set of loggers
                    d)

    // we don't care about the result, we just called 'getTcImportsFromCommandLine' to have the effect of invoke typeProviderAssemblyFound 
    ignore(tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig,outfile,pdbfile,assemblyName,errorLogger)


#if NO_COMPILER_BACKEND
#else

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Code from here on down is just used by fsc.exe
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

let BuildInitialDisplayEnvForSigFileGeneration tcGlobals = 
    let denv = DisplayEnv.Empty tcGlobals
    let denv = 
        { denv with 
           showImperativeTyparAnnotations=true;
           showHiddenMembers=true;
           showObsoleteMembers=true;
           showAttributes=true; }
    denv.SetOpenPaths 
        [ FSharpLib.RootPath 
          FSharpLib.CorePath 
          FSharpLib.CollectionsPath 
          FSharpLib.ControlPath 
          (IL.splitNamespace FSharpLib.ExtraTopLevelOperatorsName); ] 


module InterfaceFileWriter =

    let WriteInterfaceFile (tcGlobals, tcConfig:TcConfig, infoReader, typedAssembly) =
        let (TAssembly declaredImpls) = typedAssembly

        /// Use a UTF-8 Encoding with no Byte Order Mark
        let os = 
            if tcConfig.printSignatureFile="" then System.Console.Out
            else (File.CreateText tcConfig.printSignatureFile :> TextWriter)

        if tcConfig.printSignatureFile <> "" && not (List.exists (Filename.checkSuffix tcConfig.printSignatureFile) lightSyntaxDefaultExtensions) then
            fprintfn os "#light" 
            fprintfn os "" 

        for (TImplFile(_,_,mexpr,_,_)) in declaredImpls do
            let denv = BuildInitialDisplayEnvForSigFileGeneration tcGlobals
            writeViaBufferWithEnvironmentNewLines os (fun os s -> Printf.bprintf os "%s\n\n" s)
              (NicePrint.layoutInferredSigOfModuleExpr true denv infoReader AccessibleFromSomewhere range0 mexpr |> Layout.squashTo 80 |> Layout.showL)
       
        if tcConfig.printSignatureFile <> "" then os.Close()


module XmlDocWriter =

    let getDoc xmlDoc = 
        match XmlDoc.Process xmlDoc with
        | XmlDoc [| |] -> ""
        | XmlDoc strs  -> strs |> Array.toList |> String.concat System.Environment.NewLine

    let hasDoc xmlDoc =
        // No need to process the xml doc - just need to know if there's anything there
        match xmlDoc with
        | XmlDoc [| |] -> false
        | _ -> true
        
    let computeXmlDocSigs (tcGlobals,generatedCcu:CcuThunk) =
        (* the xmlDocSigOf* functions encode type into string to be used in "id" *)
        let g = tcGlobals
        let doValSig ptext (v:Val)  = if (hasDoc v.XmlDoc) then v.XmlDocSig <- XmlDocSigOfVal g ptext v
        let doTyconSig ptext (tc:Tycon) = 
            if (hasDoc tc.XmlDoc) then tc.XmlDocSig <- XmlDocSigOfTycon [ptext; tc.CompiledName]
            for vref in tc.MembersOfFSharpTyconSorted do 
                doValSig ptext vref.Deref
            for uc in tc.UnionCasesAsList do
                if (hasDoc uc.XmlDoc) then uc.XmlDocSig <- XmlDocSigOfUnionCase [ptext; tc.CompiledName; uc.Id.idText]
            for rf in tc.AllFieldsAsList do
                if (hasDoc rf.XmlDoc) then
                    rf.XmlDocSig <-
                        if tc.IsRecordTycon && (not rf.IsStatic) then 
                            // represents a record field, which is exposed as a property
                            XmlDocSigOfProperty [ptext; tc.CompiledName; rf.Id.idText]
                        else
                            XmlDocSigOfField [ptext; tc.CompiledName; rf.Id.idText]

        let doModuleMemberSig path (m:ModuleOrNamespace) = m.XmlDocSig <- XmlDocSigOfSubModul [path]
        (* moduleSpec - recurses *)
        let rec doModuleSig path (mspec:ModuleOrNamespace) = 
            let mtype = mspec.ModuleOrNamespaceType
            let path = 
                (* skip the first item in the path which is the assembly name *)
                match path with 
                | None -> Some ""
                | Some "" -> Some mspec.LogicalName
                | Some p -> Some (p+"."+mspec.LogicalName)
            let ptext = match path with None -> "" | Some t -> t
            if mspec.IsModule then doModuleMemberSig ptext mspec;
            let vals = 
                mtype.AllValsAndMembers
                |> Seq.toList
                |> List.filter (fun x  -> not x.IsCompilerGenerated) 
                |> List.filter (fun x -> x.MemberInfo.IsNone || x.IsExtensionMember)
            List.iter (doModuleSig  path)  mtype.ModuleAndNamespaceDefinitions;
            List.iter (doTyconSig  ptext) mtype.ExceptionDefinitions;
            List.iter (doValSig    ptext) vals;
            List.iter (doTyconSig  ptext) mtype.TypeDefinitions
       
        doModuleSig None generatedCcu.Contents;          

    let writeXmlDoc (assemblyName,generatedCcu:CcuThunk,xmlfile) =
        if not (Filename.hasSuffixCaseInsensitive "xml" xmlfile ) then 
            error(Error(FSComp.SR.docfileNoXmlSuffix(), Range.rangeStartup));
        (* the xmlDocSigOf* functions encode type into string to be used in "id" *)
        let members = ref []
        let addMember id xmlDoc = 
            if hasDoc xmlDoc then
                let doc = getDoc xmlDoc
                members := (id,doc) :: !members
        let doVal (v:Val) = addMember v.XmlDocSig v.XmlDoc
        let doUnionCase (uc:UnionCase) = addMember uc.XmlDocSig uc.XmlDoc
        let doField (rf:RecdField) = addMember rf.XmlDocSig rf.XmlDoc
        let doTycon (tc:Tycon) = 
            addMember tc.XmlDocSig tc.XmlDoc;
            for vref in tc.MembersOfFSharpTyconSorted do 
                doVal vref.Deref 
            for uc in tc.UnionCasesAsList do
                doUnionCase uc
            for rf in tc.AllFieldsAsList do
                doField rf

        let modulMember (m:ModuleOrNamespace) = addMember m.XmlDocSig m.XmlDoc
        
        (* moduleSpec - recurses *)
        let rec doModule (mspec:ModuleOrNamespace) = 
            let mtype = mspec.ModuleOrNamespaceType
            if mspec.IsModule then modulMember mspec;
            let vals = 
                mtype.AllValsAndMembers
                |> Seq.toList
                |> List.filter (fun x  -> not x.IsCompilerGenerated) 
                |> List.filter (fun x -> x.MemberInfo.IsNone || x.IsExtensionMember)
            List.iter doModule mtype.ModuleAndNamespaceDefinitions;
            List.iter doTycon mtype.ExceptionDefinitions;
            List.iter doVal vals;
            List.iter doTycon mtype.TypeDefinitions
       
        doModule generatedCcu.Contents;

        use os = File.CreateText(xmlfile)

        fprintfn os ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        fprintfn os ("<doc>");
        fprintfn os ("<assembly><name>%s</name></assembly>") assemblyName;
        fprintfn os ("<members>");
        !members |> List.iter (fun (id,doc) -> 
            fprintfn os  "<member name=\"%s\">" id
            fprintfn os  "%s" doc
            fprintfn os  "</member>");
        fprintfn os "</members>"; 
        fprintfn os "</doc>";   


//----------------------------------------------------------------------------
// cmd line - option state
//----------------------------------------------------------------------------

let getModuleFileName() = 
    Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                           System.AppDomain.CurrentDomain.FriendlyName)  

let defaultFSharpBinariesDir = Filename.directoryName (getModuleFileName())


let outpath outfile extn =
  String.concat "." (["out"; Filename.chopExtension (Filename.fileNameOfPath outfile); extn])
  


let GenerateInterfaceData(tcConfig:TcConfig) = 
    (* (tcConfig.target = Dll || tcConfig.target = Module) && *)
    not tcConfig.standalone && not tcConfig.noSignatureData 

type ILResource with 
    /// Read the bytes from a resource local to an assembly
    member r.Bytes = 
        match r.Location with 
        | ILResourceLocation.Local b -> b()
        | _-> error(InternalError("Bytes",rangeStartup))

let EncodeInterfaceData(tcConfig:TcConfig,tcGlobals,exportRemapping,_errorLogger:ErrorLogger,generatedCcu,outfile,exiter:Exiter) = 
    try 
      if GenerateInterfaceData(tcConfig) then 
        if verbose then dprintfn "Generating interface data attribute...";
        let resource = WriteSignatureData (tcConfig,tcGlobals,exportRemapping,generatedCcu,outfile)
        if verbose then dprintf "Generated interface data attribute!\n";
        // REVIEW: need a better test for this
        let outFileNoExtension = Filename.chopExtension outfile
        let isCompilerServiceDll = outFileNoExtension.Contains("FSharp.LanguageService.Compiler")
        if tcConfig.useOptimizationDataFile || tcGlobals.compilingFslib || isCompilerServiceDll then 
            let sigDataFileName = (Filename.chopExtension outfile)+".sigdata"
            File.WriteAllBytes(sigDataFileName,resource.Bytes);
        let sigAttr = mkSignatureDataVersionAttr tcGlobals (IL.parseILVersion Internal.Utilities.FSharpEnvironment.FSharpBinaryMetadataFormatRevision) 
        // The resource gets written to a file for FSharp.Core
        let resources = 
            [ if not tcGlobals.compilingFslib && not isCompilerServiceDll then 
                 yield  resource ]
        [sigAttr], resources
      else 
        [],[]
    with e -> 
        errorRecoveryNoRange e
#if SQM_SUPPORT
        SqmLoggerWithConfig tcConfig _errorLogger.ErrorNumbers _errorLogger.WarningNumbers
#endif
        exiter.Exit 1


//----------------------------------------------------------------------------
// EncodeOptimizationData
//----------------------------------------------------------------------------

let GenerateOptimizationData(tcConfig) = 
    (* (tcConfig.target =Dll || tcConfig.target = Module) && *)
    GenerateInterfaceData(tcConfig) 

let EncodeOptimizationData(tcGlobals,tcConfig,outfile,exportRemapping,data) = 
    if GenerateOptimizationData tcConfig then 
        let data = map2Of2 (Opt.RemapLazyModulInfo tcGlobals exportRemapping) data
        if verbose then dprintn "Generating optimization data attribute...";
        // REVIEW: need a better test for this
        let outFileNoExtension = Filename.chopExtension outfile
        let isCompilerServiceDll = outFileNoExtension.Contains("FSharp.LanguageService.Compiler")
        if tcConfig.useOptimizationDataFile || tcGlobals.compilingFslib || isCompilerServiceDll then 
            let ccu,modulInfo = data
            let bytes = Pickle.pickleObjWithDanglingCcus outfile tcGlobals ccu Opt.p_LazyModuleInfo modulInfo
            let optDataFileName = (Filename.chopExtension outfile)+".optdata"
            File.WriteAllBytes(optDataFileName,bytes);
        // As with the sigdata file, the optdata gets written to a file for FSharp.Core, FSharp.Compiler.Silverlight and FSharp.LanguageService.Compiler
        if tcGlobals.compilingFslib || isCompilerServiceDll then 
            []
        else
            let (ccu, optData) = 
                if tcConfig.onlyEssentialOptimizationData || tcConfig.useOptimizationDataFile 
                then map2Of2 Opt.AbstractLazyModulInfoToEssentials data 
                else data
            [ WriteOptimizationData (tcGlobals, outfile, ccu, optData) ]
    else
        [ ]

//----------------------------------------------------------------------------
// .res file format, for encoding the assembly version attribute. 
//--------------------------------------------------------------------------

// Helpers for generating binary blobs
module BinaryGenerationUtilities = 
    // Little-endian encoding of int32 
    let b0 n =  byte (n &&& 0xFF)
    let b1 n =  byte ((n >>> 8) &&& 0xFF)
    let b2 n =  byte ((n >>> 16) &&& 0xFF)
    let b3 n =  byte ((n >>> 24) &&& 0xFF)

    let i16 (i:int32) = [| b0 i; b1 i |]
    let i32 (i:int32) = [| b0 i; b1 i; b2 i; b3 i |]

    // Emit the bytes and pad to a 32-bit alignment
    let Padded initialAlignment (v:byte[]) = 
        [| yield! v
           for _ in 1..(4 - (initialAlignment + v.Length) % 4) % 4 do
               yield 0x0uy |]

// Generate nodes in a .res file format. These are then linked by Abstract IL using the 
// linkNativeResources function, which invokes the cvtres.exe utility
module ResFileFormat = 
    open BinaryGenerationUtilities
    
    let ResFileNode(dwTypeID,dwNameID,wMemFlags,wLangID,data:byte[]) =
        [| yield! i32 data.Length  // DWORD ResHdr.dwDataSize
           yield! i32 0x00000020  // dwHeaderSize
           yield! i32 ((dwTypeID <<< 16) ||| 0x0000FFFF)  // dwTypeID,sizeof(DWORD)
           yield! i32 ((dwNameID <<< 16) ||| 0x0000FFFF)   // dwNameID,sizeof(DWORD)
           yield! i32 0x00000000 // DWORD       dwDataVersion
           yield! i16 wMemFlags // WORD        wMemFlags
           yield! i16 wLangID   // WORD        wLangID
           yield! i32 0x00000000 // DWORD       dwVersion
           yield! i32 0x00000000 // DWORD       dwCharacteristics
           yield! Padded 0 data |]

    let ResFileHeader() = ResFileNode(0x0,0x0,0x0,0x0,[| |]) 

// Generate the VS_VERSION_INFO structure held in a Win32 Version Resource in a PE file
//
// Web reference: http://www.piclist.com/tecHREF/os/win/api/win32/struc/src/str24_5.htm
module VersionResourceFormat = 
    open BinaryGenerationUtilities

    let VersionInfoNode(data:byte[]) =
        [| yield! i16 (data.Length + 2) // wLength : int16; // Specifies the length, in bytes, of the VS_VERSION_INFO structure. This length does not include any padding that aligns any subsequent version resource data on a 32-bit boundary. 
           yield! data |]

    let VersionInfoElement(wType, szKey, valueOpt: byte[] option, children:byte[][], isString) =
        // for String structs, wValueLength represents the word count, not the byte count
        let wValueLength = (match valueOpt with None -> 0 | Some value -> (if isString then value.Length / 2 else value.Length))
        VersionInfoNode
            [| yield! i16 wValueLength // wValueLength: int16. Specifies the length, in words, of the Value member. This value is zero if there is no Value member associated with the current version structure. 
               yield! i16 wType        // wType : int16; Specifies the type of data in the version resource. This member is 1 if the version resource contains text data and 0 if the version resource contains binary data. 
               yield! Padded 2 szKey 
               match valueOpt with 
               | None -> yield! []
               | Some value -> yield! Padded 0 value 
               for child in children do 
                   yield! child  |]

    let Version((v1,v2,v3,v4):ILVersionInfo) = 
        [| yield! i32 (int32 v1 <<< 16 ||| int32 v2) // DWORD dwFileVersionMS; // Specifies the most significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons. 
           yield! i32 (int32 v3 <<< 16 ||| int32 v4) // DWORD dwFileVersionLS; // Specifies the least significant 32 bits of the file's binary version number. This member is used with dwFileVersionMS to form a 64-bit value used for numeric comparisons. 
        |]

    let String(string,value) = 
        let wType = 0x1 // Specifies the type of data in the version resource. This member is 1 if the version resource contains text data and 0 if the version resource contains binary data. 
        let szKey = Bytes.stringAsUnicodeNullTerminated string
        VersionInfoElement(wType, szKey, Some(Bytes.stringAsUnicodeNullTerminated value),[| |],true)

    let StringTable(language,strings) = 
        let wType = 0x1 // Specifies the type of data in the version resource. This member is 1 if the version resource contains text data and 0 if the version resource contains binary data. 
        let szKey = Bytes.stringAsUnicodeNullTerminated language
             // Specifies an 8-digit hexadecimal number stored as a Unicode string. The four most significant digits represent the language identifier. The four least significant digits represent the code page for which the data is formatted. 
             // Each Microsoft Standard Language identifier contains two parts: the low-order 10 bits specify the major language, and the high-order 6 bits specify the sublanguage. For a table of valid identifiers see Language Identifiers. 
                       
        let children =  
            [| for string in strings do
                   yield String(string) |] 
        VersionInfoElement(wType, szKey, None,children,false)

    let StringFileInfo(stringTables: #seq<string * #seq<string * string> >) = 
        let wType = 0x1 // Specifies the type of data in the version resource. This member is 1 if the version resource contains text data and 0 if the version resource contains binary data. 
        let szKey = Bytes.stringAsUnicodeNullTerminated "StringFileInfo" // Contains the Unicode string StringFileInfo
        // Contains an array of one or more StringTable structures. Each StringTable structures szKey member indicates the appropriate language and code page for displaying the text in that StringTable structure. 
        let children =  
            [| for stringTable in stringTables do
                   yield StringTable(stringTable) |] 
        VersionInfoElement(wType, szKey, None,children,false)
        
    let VarFileInfo(vars: #seq<int32 * int32>) = 
        let wType = 0x1 // Specifies the type of data in the version resource. This member is 1 if the version resource contains text data and 0 if the version resource contains binary data. 
        let szKey = Bytes.stringAsUnicodeNullTerminated "VarFileInfo" // Contains the Unicode string StringFileInfo
        // Contains an array of one or more StringTable structures. Each StringTable structures szKey member indicates the appropriate language and code page for displaying the text in that StringTable structure. 
        let children =  
            [| for (lang,codePage) in vars do
                   let szKey = Bytes.stringAsUnicodeNullTerminated "Translation"
                   yield VersionInfoElement(0x0,szKey, Some([| yield! i16 lang
                                                               yield! i16 codePage |]), [| |],false) |] 
        VersionInfoElement(wType, szKey, None,children,false)
        
    let VS_FIXEDFILEINFO(fileVersion:ILVersionInfo,
                         productVersion:ILVersionInfo,
                         dwFileFlagsMask,
                         dwFileFlags,dwFileOS,
                         dwFileType,dwFileSubtype,
                         lwFileDate:int64) = 
        let dwStrucVersion = 0x00010000
        [| yield! i32  0xFEEF04BD // DWORD dwSignature; // Contains the value 0xFEEFO4BD. This is used with the szKey member of the VS_VERSION_INFO structure when searching a file for the VS_FIXEDFILEINFO structure. 
           yield! i32 dwStrucVersion // DWORD dwStrucVersion; // Specifies the binary version number of this structure. The high-order word of this member contains the major version number, and the low-order word contains the minor version number. 
           yield! Version fileVersion // DWORD dwFileVersionMS,dwFileVersionLS; // Specifies the most/least significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons. 
           yield! Version productVersion // DWORD dwProductVersionMS,dwProductVersionLS; // Specifies the most/least significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons. 
           yield! i32 dwFileFlagsMask // DWORD dwFileFlagsMask; // Contains a bitmask that specifies the valid bits in dwFileFlags. A bit is valid only if it was defined when the file was created. 
           yield! i32 dwFileFlags // DWORD dwFileFlags; // Contains a bitmask that specifies the Boolean attributes of the file. This member can include one or more of the following values: 
                  //          VS_FF_DEBUG 0x1L             The file contains debugging information or is compiled with debugging features enabled. 
                  //          VS_FF_INFOINFERRED            The file's version structure was created dynamically; therefore, some of the members in this structure may be empty or incorrect. This flag should never be set in a file's VS_VERSION_INFO data. 
                  //          VS_FF_PATCHED            The file has been modified and is not identical to the original shipping file of the same version number. 
                  //          VS_FF_PRERELEASE            The file is a development version, not a commercially released product. 
                  //          VS_FF_PRIVATEBUILD            The file was not built using standard release procedures. If this flag is set, the StringFileInfo structure should contain a PrivateBuild entry. 
                  //          VS_FF_SPECIALBUILD            The file was built by the original company using standard release procedures but is a variation of the normal file of the same version number. If this flag is set, the StringFileInfo structure should contain a SpecialBuild entry. 
           yield! i32 dwFileOS //Specifies the operating system for which this file was designed. This member can be one of the following values: Flag 
                  //VOS_DOS 0x0001L  The file was designed for MS-DOS. 
                  //VOS_NT  0x0004L  The file was designed for Windows NT. 
                  //VOS__WINDOWS16  The file was designed for 16-bit Windows. 
                  //VOS__WINDOWS32  The file was designed for the Win32 API. 
                  //VOS_OS216 0x00020000L  The file was designed for 16-bit OS/2. 
                  //VOS_OS232  0x00030000L  The file was designed for 32-bit OS/2. 
                  //VOS__PM16  The file was designed for 16-bit Presentation Manager. 
                  //VOS__PM32  The file was designed for 32-bit Presentation Manager. 
                  //VOS_UNKNOWN  The operating system for which the file was designed is unknown to Windows. 
           yield! i32 dwFileType // Specifies the general type of file. This member can be one of the following values: 
     
                //VFT_UNKNOWN The file type is unknown to Windows. 
                //VFT_APP  The file contains an application. 
                //VFT_DLL  The file contains a dynamic-link library (DLL). 
                //VFT_DRV  The file contains a device driver. If dwFileType is VFT_DRV, dwFileSubtype contains a more specific description of the driver. 
                //VFT_FONT  The file contains a font. If dwFileType is VFT_FONT, dwFileSubtype contains a more specific description of the font file. 
                //VFT_VXD  The file contains a virtual device. 
                //VFT_STATIC_LIB  The file contains a static-link library. 

           yield! i32 dwFileSubtype //     Specifies the function of the file. The possible values depend on the value of dwFileType. For all values of dwFileType not described in the following list, dwFileSubtype is zero. If dwFileType is VFT_DRV, dwFileSubtype can be one of the following values: 
                      //VFT2_UNKNOWN  The driver type is unknown by Windows. 
                      //VFT2_DRV_COMM  The file contains a communications driver. 
                      //VFT2_DRV_PRINTER  The file contains a printer driver. 
                      //VFT2_DRV_KEYBOARD  The file contains a keyboard driver. 
                      //VFT2_DRV_LANGUAGE  The file contains a language driver. 
                      //VFT2_DRV_DISPLAY  The file contains a display driver. 
                      //VFT2_DRV_MOUSE  The file contains a mouse driver. 
                      //VFT2_DRV_NETWORK  The file contains a network driver. 
                      //VFT2_DRV_SYSTEM  The file contains a system driver. 
                      //VFT2_DRV_INSTALLABLE  The file contains an installable driver. 
                      //VFT2_DRV_SOUND  The file contains a sound driver. 
                      //
                      //If dwFileType is VFT_FONT, dwFileSubtype can be one of the following values: 
                      // 
                      //VFT2_UNKNOWN  The font type is unknown by Windows. 
                      //VFT2_FONT_RASTER  The file contains a raster font. 
                      //VFT2_FONT_VECTOR  The file contains a vector font. 
                      //VFT2_FONT_TRUETYPE  The file contains a TrueType font. 
                      //
                      //If dwFileType is VFT_VXD, dwFileSubtype contains the virtual device identifier included in the virtual device control block. 
           yield! i32 (int32 (lwFileDate >>> 32)) // Specifies the most significant 32 bits of the file's 64-bit binary creation date and time stamp. 
           yield! i32 (int32 lwFileDate) //Specifies the least significant 32 bits of the file's 64-bit binary creation date and time stamp. 
         |] 


    let VS_VERSION_INFO(fixedFileInfo,stringFileInfo,varFileInfo)  =
        let wType = 0x0 
        let szKey = Bytes.stringAsUnicodeNullTerminated "VS_VERSION_INFO" // Contains the Unicode string VS_VERSION_INFO
        let value = VS_FIXEDFILEINFO (fixedFileInfo)
        let children =  
            [| yield StringFileInfo(stringFileInfo) 
               yield VarFileInfo(varFileInfo) 
            |] 
        VersionInfoElement(wType, szKey, Some(value),children,false)
       
    let VS_VERSION_INFO_RESOURCE(data) = 
        let dwTypeID = 0x0010
        let dwNameID = 0x0001
        let wMemFlags = 0x0030 // REVIEW: HARDWIRED TO ENGLISH
        let wLangID = 0x0
        ResFileFormat.ResFileNode(dwTypeID, dwNameID,wMemFlags,wLangID,VS_VERSION_INFO(data))
        
module ManifestResourceFormat =
    
    let VS_MANIFEST_RESOURCE(data, isLibrary) =
        let dwTypeID = 0x0018
        let dwNameID = if isLibrary then 0x2 else 0x1
        let wMemFlags = 0x0
        let wLangID = 0x0
        ResFileFormat.ResFileNode(dwTypeID, dwNameID, wMemFlags, wLangID, data)

/// Helpers for finding attributes
module AttributeHelpers = 

    /// Try to find an attribute that takes a string argument
    let TryFindStringAttribute tcGlobals attrib attribs =
        match TryFindFSharpAttribute tcGlobals (mkMscorlibAttrib tcGlobals attrib) attribs with
        | Some (Attrib(_,_,[ AttribStringArg(s) ],_,_,_,_))  -> Some (s)
        | _ -> None
        
    let TryFindIntAttribute tcGlobals attrib attribs =
        match TryFindFSharpAttribute tcGlobals (mkMscorlibAttrib tcGlobals attrib) attribs with
        | Some (Attrib(_,_,[ AttribInt32Arg(i) ],_,_,_,_)) -> Some (i)
        | _ -> None
        
    let TryFindBoolAttribute tcGlobals attrib attribs =
        match TryFindFSharpAttribute tcGlobals (mkMscorlibAttrib tcGlobals attrib) attribs with
        | Some (Attrib(_,_,[ AttribBoolArg(p) ],_,_,_,_)) -> Some (p)
        | _ -> None

    // Try to find an AssemblyVersion attribute 
    let TryFindVersionAttribute tcGlobals attrib attribName attribs =
        match TryFindStringAttribute tcGlobals attrib attribs with
        | Some versionString ->
             try Some(IL.parseILVersion versionString)
             with e -> 
                 warning(Error(FSComp.SR.fscBadAssemblyVersion(attribName, versionString),Range.rangeStartup));
                 None
        | _ -> None


let injectedCompatTypes = set [ "System.Tuple`1"; 
        "System.Tuple`2" ; 
        "System.Tuple`3" ; 
        "System.Tuple`4"; 
        "System.Tuple`5"; 
        "System.Tuple`6"; 
        "System.Tuple`7"; 
        "System.Tuple`8"; 
        "System.ITuple"; 
        "System.Tuple"; 
        //"System.System_LazyDebugView`1"; 
        //"System.Threading.LazyExecutionMode"; 
        //"System.Threading.LazyInternalExceptionHolder"; 
        //"System.Threading.LazyBlock`1"; 
        "System.Collections.IStructuralComparable"; 
        "System.Collections.IStructuralEquatable"; 
      ]

let typesForwardedToMscorlib = 
    set [  "System.AggregateException";
            "System.Threading.CancellationTokenRegistration";
            "System.Threading.CancellationToken";
            "System.Threading.CancellationTokenSource";
            "System.Lazy`1";
            "System.IObservable`1";
            "System.IObserver`1";
        ]
let typesForwardedToSystemNumerics =
    set [ "System.Numerics.BigInteger"; ]
      
let createMscorlibExportList tcGlobals =
    // We want to write forwarders out for all injected types except for System.ITuple, which is internal
    // Forwarding System.ITuple will cause FxCop failures on 4.0
    Set.union (Set.filter (fun t -> t <> "System.ITuple") injectedCompatTypes) typesForwardedToMscorlib |>
        Seq.map (fun t -> 
                    {   ScopeRef = tcGlobals.sysCcu.ILScopeRef ; 
                        Name = t ; 
                        IsForwarder = true ; 
                        Access = ILTypeDefAccess.Public ; 
                        Nested = mkILNestedExportedTypes List.empty<ILNestedExportedType> ; 
                        CustomAttrs = mkILCustomAttrs List.empty<ILAttribute>  }) |> 
        Seq.toList

let createSystemNumericsExportList tcGlobals =
    let sysAssemblyRef = tcGlobals.sysCcu.ILScopeRef.AssemblyRef
    let systemNumericsAssemblyRef = ILAssemblyRef.Create("System.Numerics", sysAssemblyRef.Hash, sysAssemblyRef.PublicKey, sysAssemblyRef.Retargetable, sysAssemblyRef.Version, sysAssemblyRef.Locale)
    typesForwardedToSystemNumerics |>
        Seq.map (fun t ->
                    {   ScopeRef = ILScopeRef.Assembly(systemNumericsAssemblyRef)
                        Name = t;
                        IsForwarder = true ;
                        Access = ILTypeDefAccess.Public ;
                        Nested = mkILNestedExportedTypes List.empty<ILNestedExportedType> ;
                        CustomAttrs = mkILCustomAttrs List.empty<ILAttribute> }) |>
        Seq.toList
            
module MainModuleBuilder = 
    let CreateMainModule  
            (tcConfig:TcConfig,tcGlobals,
             pdbfile,assemblyName,outfile,topAttrs,
             (iattrs,intfDataResources),optDataResources,
             codegenResults,assemVerFromAttrib,metadataVersion,secDecls) =


        if !progress then dprintf "Creating main module...\n";
        let ilTypeDefs = 
            //let topTypeDef = mkILTypeDefForGlobalFunctions tcGlobals.ilg (mkILMethods [], emptyILFields)
            mkILTypeDefs codegenResults.ilTypeDefs
            


        let mainModule = 
            let hashAlg = AttributeHelpers.TryFindIntAttribute tcGlobals "System.Reflection.AssemblyAlgorithmIdAttribute" topAttrs.assemblyAttrs
            let locale = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyCultureAttribute" topAttrs.assemblyAttrs
            let flags =  match AttributeHelpers.TryFindIntAttribute tcGlobals "System.Reflection.AssemblyFlagsAttribute" topAttrs.assemblyAttrs with | Some(f) -> f | _ -> 0x0
            
            // You're only allowed to set a locale if the assembly is a library
            if (locale <> None && locale.Value <> "") && tcConfig.target <> Dll then
              error(Error(FSComp.SR.fscAssemblyCultureAttributeError(),rangeCmdArgs))
            
            // Add the type forwarders to any .NET DLL post-.NET-2.0, to give binary compatibility
            let exportedTypesList = if (tcConfig.compilingFslib && tcConfig.compilingFslib40) then (List.append (createMscorlibExportList tcGlobals) (createSystemNumericsExportList tcGlobals)) else []
            
            mkILSimpleModule assemblyName (fsharpModuleName tcConfig.target assemblyName) (tcConfig.target = Dll || tcConfig.target = Module) tcConfig.subsystemVersion tcConfig.useHighEntropyVA ilTypeDefs hashAlg locale flags (mkILExportedTypes exportedTypesList) metadataVersion

        let disableJitOptimizations = not (tcConfig.optSettings.jitOpt())
                       
        let tcVersion = tcConfig.version.GetVersionInfo(tcConfig.implicitIncludeDir)
                  
        let reflectedDefinitionAttrs, reflectedDefinitionResources = 
            codegenResults.quotationResourceInfo 
            |> List.map (fun (referencedTypeDefs, reflectedDefinitionBytes) -> 
                let reflectedDefinitionResourceName = QuotationPickler.SerializedReflectedDefinitionsResourceNameBase+"-"+assemblyName+"-"+string(newUnique())+"-"+string(hash reflectedDefinitionBytes)
                let reflectedDefinitionAttr = mkCompilationMappingAttrForQuotationResource tcGlobals (reflectedDefinitionResourceName, referencedTypeDefs)
                let reflectedDefinitionResource = 
                  { Name=reflectedDefinitionResourceName;
                    Location = ILResourceLocation.Local (fun () -> reflectedDefinitionBytes);
                    Access= ILResourceAccess.Public;
                    CustomAttrs = emptyILCustomAttrs }
                reflectedDefinitionAttr, reflectedDefinitionResource) 
            |> List.unzip

        let manifestAttrs = 
            mkILCustomAttrs
                 [ if not tcConfig.internConstantStrings then 
                       yield mkILCustomAttribute tcGlobals.ilg
                                 (mkILTyRef (tcGlobals.ilg.traits.ScopeRef, "System.Runtime.CompilerServices.CompilationRelaxationsAttribute"),
                                  [tcGlobals.ilg.typ_Int32],[ILAttribElem.Int32( 8)], []) 
                   yield! iattrs
                   yield! codegenResults.ilAssemAttrs
                   if Option.isSome pdbfile then 
                       yield (tcGlobals.ilg.mkDebuggableAttributeV2 (tcConfig.jitTracking, tcConfig.ignoreSymbolStoreSequencePoints, disableJitOptimizations, false (* enableEnC *) )) 
                   yield! reflectedDefinitionAttrs ]

        // Make the manifest of the assembly
        let manifest = 
             if tcConfig.target = Module then None else
             let man = mainModule.ManifestOfAssembly
             let ver = 
                 match assemVerFromAttrib with 
                 | None -> tcVersion
                 | Some v -> v
             Some { man with Version= Some ver;
                             CustomAttrs = manifestAttrs;
                             DisableJitOptimizations=disableJitOptimizations;
                             JitTracking= tcConfig.jitTracking;
                             SecurityDecls=secDecls } 

        let resources = 
          mkILResources 
            [ for file in tcConfig.embedResources do
                 let name,bytes,pub = 
                     let lower = String.lowercase file
                     if List.exists (Filename.checkSuffix lower) [".resx"]  then
                         let file = tcConfig.ResolveSourceFile(rangeStartup,file,tcConfig.implicitIncludeDir)
                         let outfile = (file |> Filename.chopExtension) + ".resources"
                         
                         let readResX(f:string) = 
                             use rsxr = new System.Resources.ResXResourceReader(f)
                             rsxr 
                             |> Seq.cast 
                             |> Seq.toList
                             |> List.map (fun (d:System.Collections.DictionaryEntry) -> (d.Key :?> string), d.Value)
                         let writeResources((r:(string * obj) list),(f:string)) = 
                             use writer = new System.Resources.ResourceWriter(f)
                             r |> List.iter (fun (k,v) -> writer.AddResource(k,v))
                         writeResources(readResX(file),outfile);
                         let file,name,pub = TcConfigBuilder.SplitCommandLineResourceInfo outfile
                         let file = tcConfig.ResolveSourceFile(rangeStartup,file,tcConfig.implicitIncludeDir)
                         let bytes = FileSystem.ReadAllBytesShim file
                         FileSystem.FileDelete outfile
                         name,bytes,pub
                     else

                         let file,name,pub = TcConfigBuilder.SplitCommandLineResourceInfo file
                         let file = tcConfig.ResolveSourceFile(rangeStartup,file,tcConfig.implicitIncludeDir)
                         let bytes = FileSystem.ReadAllBytesShim file
                         name,bytes,pub
                 yield { Name=name; 
                         Location=ILResourceLocation.Local (fun () -> bytes); 
                         Access=pub; 
                         CustomAttrs=emptyILCustomAttrs }
               
              yield! reflectedDefinitionResources
              yield! intfDataResources
              yield! optDataResources
              for ri in tcConfig.linkResources do 
                 let file,name,pub = TcConfigBuilder.SplitCommandLineResourceInfo ri
                 yield { Name=name; 
                         Location=ILResourceLocation.File(ILModuleRef.Create(name=file, hasMetadata=false, hash=Some (sha1HashBytes (FileSystem.ReadAllBytesShim file))), 0);
                         Access=pub; 
                         CustomAttrs=emptyILCustomAttrs } ]

        //NOTE: the culture string can be turned into a number using this:
        //    sprintf "%04x" (System.Globalization.CultureInfo.GetCultureInfo("en").KeyboardLayoutId )
        let assemblyVersionResources =
            let assemblyVersion = 
                match tcConfig.version with
                | VersionNone ->assemVerFromAttrib
                | _ -> Some(tcVersion)
            match assemblyVersion with 
            | None -> []
            | Some(assemblyVersion) ->
                let FindAttribute key attrib = 
                    match AttributeHelpers.TryFindStringAttribute tcGlobals attrib topAttrs.assemblyAttrs with
                    | Some text  -> [(key,text)]
                    | _ -> []

                let fileVersion = 
                    match AttributeHelpers.TryFindVersionAttribute tcGlobals "System.Reflection.AssemblyFileVersionAttribute" "AssemblyFileVersionAttribute" topAttrs.assemblyAttrs with
                    | Some v -> v
                    | None -> assemblyVersion

                let productVersion = 
                    match AttributeHelpers.TryFindVersionAttribute tcGlobals "System.Reflection.AssemblyInformationalVersionAttribute" "AssemblyInformationalVersionAttribute" topAttrs.assemblyAttrs with
                    | Some v -> v
                    | None -> assemblyVersion

                let stringFileInfo = 
                     // 000004b0:
                     // Specifies an 8-digit hexadecimal number stored as a Unicode string. The four most significant digits represent the language identifier. The four least significant digits represent the code page for which the data is formatted. 
                     // Each Microsoft Standard Language identifier contains two parts: the low-order 10 bits specify the major language, and the high-order 6 bits specify the sublanguage. For a table of valid identifiers see Language Identifiers.                                           //
                     // see e.g. http://msdn.microsoft.com/en-us/library/aa912040.aspx 0000 is neutral and 04b0(hex)=1252(dec) is the code page.
                      [ ("000004b0", [ yield ("Assembly Version", (let v1,v2,v3,v4 = assemblyVersion in sprintf "%d.%d.%d.%d" v1 v2 v3 v4))
                                       yield ("FileVersion", (let v1,v2,v3,v4 = fileVersion in sprintf "%d.%d.%d.%d" v1 v2 v3 v4))
                                       yield ("ProductVersion", (let v1,v2,v3,v4 = productVersion in sprintf "%d.%d.%d.%d" v1 v2 v3 v4))
                                       yield! FindAttribute "Comments" "System.Reflection.AssemblyDescriptionAttribute" 
                                       yield! FindAttribute "FileDescription" "System.Reflection.AssemblyTitleAttribute" 
                                       yield! FindAttribute "ProductName" "System.Reflection.AssemblyProductAttribute" 
                                       yield! FindAttribute "CompanyName" "System.Reflection.AssemblyCompanyAttribute" 
                                       yield! FindAttribute "LegalCopyright" "System.Reflection.AssemblyCopyrightAttribute" 
                                       yield! FindAttribute "LegalTrademarks" "System.Reflection.AssemblyTrademarkAttribute" ]) ]

            
            // These entries listed in the MSDN documentation as "standard" string entries are not yet settable
            
            // InternalName: The Value member identifies the file's internal name, if one exists. For example, this string could contain the module name for Windows dynamic-link libraries (DLLs), a virtual device name for Windows virtual devices, or a device name for MS-DOS device drivers. 
            // OriginalFilename: The Value member identifies the original name of the file, not including a path. This enables an application to determine whether a file has been renamed by a user. This name may not be MS-DOS 8.3-format if the file is specific to a non-FAT file system. 
            // PrivateBuild: The Value member describes by whom, where, and why this private version of the file was built. This string should only be present if the VS_FF_PRIVATEBUILD flag is set in the dwFileFlags member of the VS_FIXEDFILEINFO structure. For example, Value could be 'Built by OSCAR on \OSCAR2'. 
            // SpecialBuild: The Value member describes how this version of the file differs from the normal version. This entry should only be present if the VS_FF_SPECIALBUILD flag is set in the dwFileFlags member of the VS_FIXEDFILEINFO structure. For example, Value could be 'Private build for Olivetti solving mouse problems on M250 and M250E computers'. 



                // "If you use the Var structure to list the languages your application 
                // or DLL supports instead of using multiple version resources, 
                // use the Value member to contain an array of DWORD values indicating the 
                // language and code page combinations supported by this file. The 
                // low-order word of each DWORD must contain a Microsoft language identifier, 
                // and the high-order word must contain the IBM code page number. 
                // Either high-order or low-order word can be zero, indicating that 
                // the file is language or code page independent. If the Var structure is 
                // omitted, the file will be interpreted as both language and code page independent. "
                let varFileInfo = [ (0x0, 0x04b0)  ]

                let fixedFileInfo = 
                    let dwFileFlagsMask = 0x3f // REVIEW: HARDWIRED
                    let dwFileFlags = 0x00 // REVIEW: HARDWIRED
                    let dwFileOS = 0x04 // REVIEW: HARDWIRED
                    let dwFileType = 0x01 // REVIEW: HARDWIRED
                    let dwFileSubtype = 0x00 // REVIEW: HARDWIRED
                    let lwFileDate = 0x00L // REVIEW: HARDWIRED
                    (fileVersion,productVersion,dwFileFlagsMask,dwFileFlags,dwFileOS,dwFileType,dwFileSubtype,lwFileDate)

                let vsVersionInfoResource = 
                    VersionResourceFormat.VS_VERSION_INFO_RESOURCE(fixedFileInfo,stringFileInfo,varFileInfo)
                
                
                let resource = 
                    [| yield! ResFileFormat.ResFileHeader()
                       yield! vsVersionInfoResource |]
#if DUMP_ASSEMBLY_RESOURCE
                for i in 0..(resource.Length+15)/16 - 1 do
                    for j in 0..15 do
                        if j % 2 = 0 then printf " " 
                        printf "%02x" resource.[min (i*16+j) (resource.Length - 1)]
                    printf " " 
                    for j in 0..15 do
                        printf "%c" (let c = char resource.[min (i*16+j) (resource.Length - 1)] in if c > ' ' && c < '~' then c else '.')
                    printfn "" 
#endif
                [ resource ]
          
        // a user cannot specify both win32res and win32manifest        
        if not(tcConfig.win32manifest = "") && not(tcConfig.win32res = "") then
            error(Error(FSComp.SR.fscTwoResourceManifests(),rangeCmdArgs));
                      
        let win32Manifest =
           // use custom manifest if provided
           if not(tcConfig.win32manifest = "") then
               tcConfig.win32manifest
           // don't embed a manifest if target is not an exe, if manifest is specifically excluded, if another native resource is being included, or if running on mono
           elif not(tcConfig.target.IsExe) || not(tcConfig.includewin32manifest) || not(tcConfig.win32res = "") || runningOnMono then
               ""
           // otherwise, include the default manifest
           else
               System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + @"default.win32manifest"
        
        let nativeResources = 
            [ for av in assemblyVersionResources do
                  yield Lazy.CreateFromValue av
              if not(tcConfig.win32res = "") then
                  yield Lazy.CreateFromValue (FileSystem.ReadAllBytesShim tcConfig.win32res) 
              if tcConfig.includewin32manifest && not(win32Manifest = "") && not(runningOnMono) then
                  yield  Lazy.CreateFromValue [|   yield! ResFileFormat.ResFileHeader() 
                                                   yield! (ManifestResourceFormat.VS_MANIFEST_RESOURCE((FileSystem.ReadAllBytesShim win32Manifest), tcConfig.target = Dll))|]]


        // Add attributes, version number, resources etc. 
        {mainModule with 
              StackReserveSize = tcConfig.stackReserveSize
              Name = (if tcConfig.target = Module then Filename.fileNameOfPath outfile else mainModule.Name);
              SubSystemFlags = (if tcConfig.target = WinExe then 2 else 3) ;
              Resources= resources;
              ImageBase = (match tcConfig.baseAddress with None -> 0x00400000l | Some b -> b);
              IsDLL=(tcConfig.target = Dll || tcConfig.target=Module);
              Platform = tcConfig.platform ;
              Is32Bit=(match tcConfig.platform with Some X86 -> true | _ -> false);
              Is64Bit=(match tcConfig.platform with Some AMD64 | Some IA64 -> true | _ -> false);          
              Is32BitPreferred = if tcConfig.prefer32Bit && not tcConfig.target.IsExe then (error(Error(FSComp.SR.invalidPlatformTarget(),rangeCmdArgs))) else tcConfig.prefer32Bit;
              CustomAttrs= 
                  mkILCustomAttrs 
                      [ if tcConfig.target = Module then 
                           yield! iattrs 
                        yield! codegenResults.ilNetModuleAttrs ];
              NativeResources=nativeResources;
              Manifest = manifest }



/// OPTIONAL STATIC LINKING OF ALL DLLs THAT DEPEND ON THE F# LIBRARY
module StaticLinker = 
    let debugStaticLinking = condition "FSHARP_DEBUG_STATIC_LINKING"

    let StaticLinkILModules (tcConfig, ilGlobals, ilxMainModule, dependentILModules: (CcuThunk option * ILModuleDef) list) = 
        if isNil dependentILModules then 
            ilxMainModule,(fun x -> x) 
        else

            // Check no dependent assemblies use quotations   
            let dependentCcuUsingQuotations = dependentILModules |> List.tryPick (function (Some ccu,_) when ccu.UsesFSharp20PlusQuotations -> Some ccu | _ -> None)   
            match dependentCcuUsingQuotations with   
            | Some ccu -> error(Error(FSComp.SR.fscQuotationLiteralsStaticLinking(ccu.AssemblyName),rangeStartup));   
            | None -> ()  
                
            // Check we're not static linking a .EXE
            if dependentILModules |> List.exists (fun (_,x) -> not x.IsDLL)  then 
                error(Error(FSComp.SR.fscStaticLinkingNoEXE(),rangeStartup))

            // Check we're not static linking something that is not pure IL
            if dependentILModules |> List.exists (fun (_,x) -> not x.IsILOnly)  then 
                error(Error(FSComp.SR.fscStaticLinkingNoMixedDLL(),rangeStartup))

            // The set of short names for the all dependent assemblies
            let assems = 
                set [ for (_,m) in dependentILModules  do
                         match m.Manifest with 
                         | Some m -> yield m.Name 
                         | _ -> () ]
            
            // A rewriter which rewrites scope references to things in dependent assemblies to be local references 
            let rewriteExternalRefsToLocalRefs x = 
                if assems.Contains (getNameOfScopeRef x) then ILScopeRef.Local else x

            let savedManifestAttrs = 
                [ for (_,depILModule) in dependentILModules do 
                    match depILModule.Manifest with 
                    | Some m -> 
                        for ca in m.CustomAttrs.AsList do
                           if ca.Method.MethodRef.EnclosingTypeRef.FullName = typeof<CompilationMappingAttribute>.FullName then 
                               yield ca
                    | _ -> () ]

            let savedResources = 
                let allResources = [ for (ccu,m) in dependentILModules do for r in m.Resources.AsList do yield (ccu, r) ]
                // Don't save interface, optimization or resource definitions for provider-generated assemblies.
                // These are "fake".
                let isProvided (ccu: CcuThunk option) = 
#if EXTENSIONTYPING
                    match ccu with 
                    | Some c -> c.IsProviderGenerated 
                    | None -> false
#else
                    ignore ccu
                    false
#endif

                // Save only the interface/optimization attributes of generated data 
                let intfDataResources,others = allResources |> List.partition (snd >> IsSignatureDataResource)
                let intfDataResources = 
                    [ for (ccu,r) in intfDataResources do 
                          if GenerateInterfaceData tcConfig && not (isProvided ccu) then 
                              yield r ]

                let optDataResources,others = others |> List.partition (snd >> IsOptimizationDataResource)
                let optDataResources = 
                    [ for (ccu,r) in optDataResources do 
                          if GenerateOptimizationData tcConfig && not (isProvided ccu) then 
                              yield r ]

                let otherResources = others |> List.map snd 

                let result = intfDataResources@optDataResources@otherResources
                result

            let moduls = ilxMainModule :: (List.map snd dependentILModules)

            // NOTE: version resources from statically linked DLLs are dropped in the binary reader/writer
            let savedNativeResources = 
                [ //yield! ilxMainModule.NativeResources 
                  for m in moduls do 
                      yield! m.NativeResources ]

            let topTypeDefs,normalTypeDefs = 
                moduls 
                |> List.map (fun m -> m.TypeDefs.AsList |> List.partition (fun td -> isTypeNameForGlobalFunctions td.Name)) 
                |> List.unzip

            let topTypeDef = 
                let topTypeDefs = List.concat topTypeDefs
                mkILTypeDefForGlobalFunctions ilGlobals
                   (mkILMethods (topTypeDefs |> List.collect (fun td -> td.Methods.AsList)),
                    mkILFields (topTypeDefs |> List.collect (fun td -> td.Fields.AsList)))

            let ilxMainModule = 
                { ilxMainModule with 
                    Manifest = (let m = ilxMainModule.ManifestOfAssembly in Some {m with CustomAttrs = mkILCustomAttrs (m.CustomAttrs.AsList @ savedManifestAttrs) });
                    CustomAttrs = mkILCustomAttrs [ for m in moduls do yield! m.CustomAttrs.AsList ];
                    TypeDefs = mkILTypeDefs (topTypeDef :: List.concat normalTypeDefs);
                    Resources = mkILResources (savedResources @ ilxMainModule.Resources.AsList);
                    NativeResources = savedNativeResources }

            ilxMainModule, rewriteExternalRefsToLocalRefs


    #if DEBUG
    let PrintModule outfile x = 
        use os = File.CreateText(outfile) :> TextWriter
        ILAsciiWriter.output_module os x  
    #endif


    // Find all IL modules that are to be statically linked given the static linking roots.
    let FindAndAddMscorlibTypesForStaticLinkingIntoFSharpCoreLibrary (tcConfig:TcConfig, ilGlobals:ILGlobals, ilxMainModule) = 
        let mscorlib40 = tcConfig.compilingFslib20.Value // + @"\..\.NET Framework 4.0 Pre Beta\mscorlib.dll"
              
        let ilBinaryReader = 
            let ilGlobals = mkILGlobals (IL.mkMscorlibBasedTraits ILScopeRef.Local) (Some ilGlobals.primaryAssemblyName) tcConfig.noDebugData
            let opts = { ILBinaryReader.mkDefault (ilGlobals) with 
                            optimizeForMemory=tcConfig.optimizeForMemory;
                            pdbPath = None; } 
            ILBinaryReader.OpenILModuleReader mscorlib40 opts
              
        let tdefs1 = ilxMainModule.TypeDefs.AsList  |> List.filter (fun td -> not (injectedCompatTypes.Contains(td.Name)))
        let tdefs2 = ilBinaryReader.ILModuleDef.TypeDefs.AsList |> List.filter (fun td -> injectedCompatTypes.Contains(td.Name))
        //printfn "tdefs2 = %A" (tdefs2 |> List.map (fun tdef -> tdef.Name))

        // rewrite the mscorlib references 
        let tdefs2 = 
            let fakeModule = mkILSimpleModule "" "" true (4, 0) false (mkILTypeDefs tdefs2) None None 0 (mkILExportedTypes []) ""
            let fakeModule = 
                  fakeModule |> Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (fun tref -> 
                      if injectedCompatTypes.Contains(tref.Name)  || (tref.Enclosing  |> List.exists (fun x -> injectedCompatTypes.Contains(x))) then 
                          tref
                          //|> Morphs.morphILScopeRefsInILTypeRef (function ILScopeRef.Local -> ilGlobals.mscorlibScopeRef | x -> x) 
                      // The implementations of Tuple use two private methods from System.Environment to get a resource string. Remap it
                      elif tref.Name = "System.Environment" then 
                          ILTypeRef.Create(ILScopeRef.Local, [], "Microsoft.FSharp.Core.PrivateEnvironment")  //|> Morphs.morphILScopeRefsInILTypeRef (function ILScopeRef.Local -> ilGlobals.mscorlibScopeRef | x -> x) 
                      else 
                          tref |> Morphs.morphILScopeRefsInILTypeRef (fun _ -> ilGlobals.traits.ScopeRef) )
                  
            // strip out System.Runtime.TargetedPatchingOptOutAttribute, which doesn't exist for 2.0
            let fakeModule = 
              {fakeModule with 
                TypeDefs = 
                  mkILTypeDefs 
                      ([ for td in fakeModule.TypeDefs do 
                              yield {td with 
                                      Methods =
                                        mkILMethods (List.map (fun (md:ILMethodDef) ->
                                                            {md with                                                            
                                                              CustomAttrs = 
                                                                mkILCustomAttrs (td.CustomAttrs.AsList |> List.filter (fun ilattr ->
                                                                   ilattr.Method.EnclosingType.TypeRef.FullName <> "System.Runtime.TargetedPatchingOptOutAttribute")  )}) 
                                                          (td.Methods.AsList))}])}
            //ILAsciiWriter.output_module stdout fakeModule
            fakeModule.TypeDefs.AsList

        let ilxMainModule = 
            { ilxMainModule with 
                TypeDefs = mkILTypeDefs (tdefs1 @ tdefs2); }
        ilxMainModule

    [<NoEquality; NoComparison>]
    type Node = 
        { name: string;
          data: ILModuleDef; 
          ccu: option<CcuThunk>;
          refs: ILReferences;
          mutable edges: list<Node>; 
          mutable visited: bool }

    // Find all IL modules that are to be statically linked given the static linking roots.
    let FindDependentILModulesForStaticLinking (tcConfig:TcConfig, tcImports:TcImports,ilxMainModule) = 
        if not tcConfig.standalone && tcConfig.extraStaticLinkRoots.IsEmpty then 
            []
        else
            // Recursively find all referenced modules and add them to a module graph 
            let depModuleTable = HashMultiMap(0, HashIdentity.Structural)
            let dummyEntry nm =
                { refs = IL.emptyILRefs ;
                  name=nm;
                  ccu=None;
                  data=ilxMainModule; // any old module
                  edges = []; 
                  visited = true }
            let assumedIndependentSet = set [ "mscorlib";  "System"; "System.Core"; "System.Xml"; "Microsoft.Build.Framework"; "Microsoft.Build.Utilities" ]      

            begin 
                let remaining = ref (computeILRefs ilxMainModule).AssemblyReferences
                while nonNil !remaining do
                    let ilAssemRef = List.head !remaining
                    remaining := List.tail !remaining;
                    if assumedIndependentSet.Contains ilAssemRef.Name || (ilAssemRef.PublicKey = Some ecmaPublicKey) then 
                        depModuleTable.[ilAssemRef.Name] <- dummyEntry ilAssemRef.Name
                    else
                        if not (depModuleTable.ContainsKey ilAssemRef.Name) then 
                            match tcImports.TryFindDllInfo(Range.rangeStartup,ilAssemRef.Name,lookupOnly=false) with 
                            | Some dllInfo ->
                                let ccu = 
                                    match tcImports.FindCcuFromAssemblyRef (Range.rangeStartup, ilAssemRef) with 
                                    | ResolvedCcu ccu -> Some ccu
                                    | UnresolvedCcu(_ccuName) -> None

                                let modul = dllInfo.RawMetadata

                                let refs = 
                                    if ilAssemRef.Name = GetFSharpCoreLibraryName() then 
                                        IL.emptyILRefs 
                                    elif not modul.IsILOnly then 
                                        warning(Error(FSComp.SR.fscIgnoringMixedWhenLinking ilAssemRef.Name,rangeStartup))
                                        IL.emptyILRefs 
                                    else
                                        { AssemblyReferences = dllInfo.ILAssemblyRefs; 
                                          ModuleReferences = [] }

                                depModuleTable.[ilAssemRef.Name] <- 
                                    { refs=refs;
                                      name=ilAssemRef.Name;
                                      ccu=ccu;
                                      data=modul; 
                                      edges = []; 
                                      visited = false };

                                // Push the new work items
                                remaining := refs.AssemblyReferences @ !remaining;

                            | None -> 
                                warning(Error(FSComp.SR.fscAssumeStaticLinkContainsNoDependencies(ilAssemRef.Name),rangeStartup)); 
                                depModuleTable.[ilAssemRef.Name] <- dummyEntry ilAssemRef.Name
                done;
            end;

            ReportTime tcConfig "Find dependencies";

            // Add edges from modules to the modules that depend on them 
            for (KeyValue(_,n)) in depModuleTable do 
                for aref in n.refs.AssemblyReferences do
                    let n2 = depModuleTable.[aref.Name] 
                    n2.edges <- n :: n2.edges
                    
            // Find everything that depends on FSharp.Core
            let roots = 
                [ if tcConfig.standalone && depModuleTable.ContainsKey (GetFSharpCoreLibraryName()) then 
                      yield depModuleTable.[GetFSharpCoreLibraryName()]
                  for n in tcConfig.extraStaticLinkRoots  do
                      match depModuleTable.TryFind n with 
                      | Some x -> yield x
                      | None -> error(Error(FSComp.SR.fscAssemblyNotFoundInDependencySet(n),rangeStartup)); 
                ]
                              
            let remaining = ref roots
            [ while nonNil !remaining do
                let n = List.head !remaining
                remaining := List.tail !remaining;
                if not n.visited then 
                    if verbose then dprintn ("Module "+n.name+" depends on "+GetFSharpCoreLibraryName());
                    n.visited <- true;
                    remaining := n.edges @ !remaining
                    yield (n.ccu, n.data);  ]

    // Add all provider-generated assemblies into the static linking set
    let FindProviderGeneratedILModules (tcImports:TcImports, providerGeneratedAssemblies: (ImportedBinary * _) list) = 
        [ for (importedBinary,provAssemStaticLinkInfo) in providerGeneratedAssemblies do 
              let ilAssemRef  = importedBinary.ILScopeRef.AssemblyRef
              if debugStaticLinking then printfn "adding provider-generated assembly '%s' into static linking set" ilAssemRef.Name
              match tcImports.TryFindDllInfo(Range.rangeStartup,ilAssemRef.Name,lookupOnly=false) with 
              | Some dllInfo ->
                  let ccu = 
                      match tcImports.FindCcuFromAssemblyRef (Range.rangeStartup, ilAssemRef) with 
                      | ResolvedCcu ccu -> Some ccu
                      | UnresolvedCcu(_ccuName) -> None

                  let modul = dllInfo.RawMetadata
                  yield (ccu, dllInfo.ILScopeRef, modul), (ilAssemRef.Name, provAssemStaticLinkInfo)
              | None -> () ]

    // Compute a static linker. This only captures tcImports (a large data structure) if
    // static linking is enabled. Normally this is not the case, which lets us collect tcImports
    // prior to this point.
    let StaticLink (tcConfig:TcConfig, tcImports:TcImports, ilGlobals:ILGlobals) = 

#if EXTENSIONTYPING
        let providerGeneratedAssemblies = 

            [ // Add all EST-generated assemblies into the static linking set
                for KeyValue(_,importedBinary:ImportedBinary) in tcImports.DllTable do
                    if importedBinary.IsProviderGenerated then 
                        match importedBinary.ProviderGeneratedStaticLinkMap with 
                        | None -> ()
                        | Some provAssemStaticLinkInfo -> yield (importedBinary,provAssemStaticLinkInfo) ]
#endif
        if tcConfig.compilingFslib && tcConfig.compilingFslib20.IsSome then 
            (fun (ilxMainModule,_) -> FindAndAddMscorlibTypesForStaticLinkingIntoFSharpCoreLibrary (tcConfig, ilGlobals, ilxMainModule))
          
        elif not tcConfig.standalone && tcConfig.extraStaticLinkRoots.IsEmpty 
#if EXTENSIONTYPING
             && providerGeneratedAssemblies.IsEmpty 
#endif
             then 
            (fun (ilxMainModule,_outfile) -> ilxMainModule)
        else 
            (fun (ilxMainModule,outfile)  ->
              ReportTime tcConfig "Find assembly references";

              let dependentILModules = FindDependentILModulesForStaticLinking (tcConfig, tcImports,ilxMainModule)

              ReportTime tcConfig "Static link";

#if EXTENSIONTYPING
              Morphs.enablemorphCustomAttributeData()
              let providerGeneratedILModules =  FindProviderGeneratedILModules (tcImports, providerGeneratedAssemblies) 

              // Transform the ILTypeRefs references in the IL of all provider-generated assemblies so that the references
              // are now local.
              let providerGeneratedILModules = 
               
                  providerGeneratedILModules |> List.map (fun ((ccu,ilOrigScopeRef,ilModule),(_,localProvAssemStaticLinkInfo)) -> 
                      let ilAssemStaticLinkMap = 
                          dict [ for (_,(_,provAssemStaticLinkInfo)) in providerGeneratedILModules do 
                                     for KeyValue(k,v) in provAssemStaticLinkInfo.ILTypeMap do 
                                         yield (k,v)
                                 for KeyValue(k,v) in localProvAssemStaticLinkInfo.ILTypeMap do
                                     yield (ILTypeRef.Create(ILScopeRef.Local, k.Enclosing, k.Name), v) ]

                      let ilModule = 
                          ilModule |> Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (fun tref -> 
                                  if debugStaticLinking then printfn "deciding whether to rewrite type ref %A" tref.QualifiedName 
                                  let ok,v = ilAssemStaticLinkMap.TryGetValue tref
                                  if ok then 
                                      if debugStaticLinking then printfn "rewriting type ref %A to %A" tref.QualifiedName v.QualifiedName
                                      v
                                  else 
                                      tref)
                      (ccu,ilOrigScopeRef,ilModule))

              // Relocate provider generated type definitions into the expected shape for the [<Generate>] declarations in an assembly
              let providerGeneratedILModules, ilxMainModule = 
                  // Build a dictionary of all remapped IL type defs 
                  let ilOrigTyRefsForProviderGeneratedTypesToRelocate = 
                      let rec walk acc (ProviderGeneratedType(ilOrigTyRef,_,xs) as node) = List.fold walk ((ilOrigTyRef,node)::acc) xs 
                      dict (Seq.fold walk [] tcImports.ProviderGeneratedTypeRoots)

                  // Build a dictionary of all IL type defs, mapping ilOrigTyRef --> ilTypeDef
                  let allTypeDefsInProviderGeneratedAssemblies = 
                      let rec loop ilOrigTyRef (ilTypeDef:ILTypeDef) = 
                          seq { yield (ilOrigTyRef,ilTypeDef); 
                                for ntdef in ilTypeDef.NestedTypes do 
                                    yield! loop (mkILTyRefInTyRef (ilOrigTyRef, ntdef.Name)) ntdef }
                      dict [ 
                          for (_ccu,ilOrigScopeRef,ilModule) in providerGeneratedILModules do 
                              for td in ilModule.TypeDefs do 
                                  yield! loop (mkILTyRef (ilOrigScopeRef, td.Name)) td ]


                  // Debugging output
                  if debugStaticLinking then 
                      for (ProviderGeneratedType(ilOrigTyRef, _, _)) in tcImports.ProviderGeneratedTypeRoots do
                          printfn "Have [<Generate>] root '%s'" ilOrigTyRef.QualifiedName

                  // Build the ILTypeDefs for generated types, starting with the roots 
                  let generatedILTypeDefs = 
                      let rec buildRelocatedGeneratedType (ProviderGeneratedType(ilOrigTyRef, ilTgtTyRef, ch)) = 
                          let isNested = ilTgtTyRef.Enclosing |> nonNil
                          if allTypeDefsInProviderGeneratedAssemblies.ContainsKey ilOrigTyRef then 
                              let ilOrigTypeDef = allTypeDefsInProviderGeneratedAssemblies.[ilOrigTyRef]
                              if debugStaticLinking then printfn "Relocating %s to %s " ilOrigTyRef.QualifiedName ilTgtTyRef.QualifiedName
                              { ilOrigTypeDef with 
                                    Name = ilTgtTyRef.Name
                                    Access = (match ilOrigTypeDef.Access with 
                                              | ILTypeDefAccess.Public when isNested -> ILTypeDefAccess.Nested ILMemberAccess.Public 
                                              | ILTypeDefAccess.Private when isNested -> ILTypeDefAccess.Nested ILMemberAccess.Assembly 
                                              | x -> x)
                                    NestedTypes = mkILTypeDefs (List.map buildRelocatedGeneratedType ch) }
                          else
                              // If there is no matching IL type definition, then make a simple container class
                              if debugStaticLinking then printfn "Generating simple class '%s' because we didn't find an original type '%s' in a provider generated assembly" ilTgtTyRef.QualifiedName ilOrigTyRef.QualifiedName
                              mkILSimpleClass ilGlobals (ilTgtTyRef.Name, (if isNested  then ILTypeDefAccess.Nested ILMemberAccess.Public else ILTypeDefAccess.Public), emptyILMethods, emptyILFields, mkILTypeDefs (List.map buildRelocatedGeneratedType ch) , emptyILProperties, emptyILEvents, emptyILCustomAttrs, ILTypeInit.OnAny) 

                      [ for (ProviderGeneratedType(_, ilTgtTyRef, _) as node) in tcImports.ProviderGeneratedTypeRoots  do
                           yield (ilTgtTyRef, buildRelocatedGeneratedType node) ]
                  
                  // Implant all the generated type definitions into the ilxMainModule (generating a new ilxMainModule)
                  let ilxMainModule = 

                      /// Split the list into left, middle and right parts at the first element satisfying 'p'. If no element matches return
                      /// 'None' for the middle part.
                      let trySplitFind p xs = 
                          let rec loop xs acc = 
                              match xs with 
                              | [] -> List.rev acc, None, [] 
                              | h::t -> if p h then List.rev acc, Some h, t else loop t (h::acc)
                          loop xs []

                      /// Implant the (nested) type definition 'td' at path 'enc' in 'tdefs'. 
                      let rec implantTypeDef isNested (tdefs: ILTypeDefs) (enc:string list) (td: ILTypeDef) = 
                          match enc with 
                          | [] -> addILTypeDef td tdefs
                          | h::t -> 
                               let tdefs = tdefs.AsList
                               let (ltdefs,htd,rtdefs) = 
                                   match tdefs |> trySplitFind (fun td -> td.Name = h) with 
                                   | (ltdefs,None,rtdefs) -> 
                                       let fresh = mkILSimpleClass ilGlobals (h, (if isNested  then ILTypeDefAccess.Nested ILMemberAccess.Public else ILTypeDefAccess.Public), emptyILMethods, emptyILFields, emptyILTypeDefs, emptyILProperties, emptyILEvents, emptyILCustomAttrs, ILTypeInit.OnAny)
                                       (ltdefs, fresh, rtdefs)
                                   | (ltdefs, Some htd, rtdefs) -> 
                                       (ltdefs, htd, rtdefs)
                               let htd = { htd with NestedTypes = implantTypeDef true htd.NestedTypes t td }
                               mkILTypeDefs (ltdefs @ [htd] @ rtdefs)

                      let newTypeDefs = 
                          (ilxMainModule.TypeDefs, generatedILTypeDefs) ||> List.fold (fun acc (ilTgtTyRef,td) -> 
                              if debugStaticLinking then printfn "implanting '%s' at '%s'" td.Name ilTgtTyRef.QualifiedName 
                              implantTypeDef false acc ilTgtTyRef.Enclosing td) 
                      { ilxMainModule with TypeDefs = newTypeDefs } 
                  
                  // Remove any ILTypeDefs from the provider generated modules if they have been relocated because of a [<Generate>] declaration.
                  let providerGeneratedILModules = 
                      providerGeneratedILModules |> List.map (fun (ccu,ilOrigScopeRef,ilModule) -> 
                          let ilTypeDefsAfterRemovingRelocatedTypes = 
                              let rec rw enc (tdefs: ILTypeDefs) = 
                                  mkILTypeDefs
                                   [ for tdef in tdefs do 
                                      let ilOrigTyRef = mkILNestedTyRef (ilOrigScopeRef, enc, tdef.Name)
                                      if  not (ilOrigTyRefsForProviderGeneratedTypesToRelocate.ContainsKey ilOrigTyRef) then
                                          if debugStaticLinking then printfn "Keep provided type %s in place because it wasn't relocated" ilOrigTyRef.QualifiedName
                                          yield { tdef with NestedTypes = rw (enc@[tdef.Name]) tdef.NestedTypes  } ]
                              rw [] ilModule.TypeDefs
                          (ccu, { ilModule with TypeDefs = ilTypeDefsAfterRemovingRelocatedTypes }))

                  providerGeneratedILModules, ilxMainModule
             
              Morphs.disablemorphCustomAttributeData()
#else
              let providerGeneratedILModules = []
#endif

              // Glue all this stuff into ilxMainModule 
              let ilxMainModule,rewriteExternalRefsToLocalRefs = 
                  StaticLinkILModules (tcConfig, ilGlobals, ilxMainModule, dependentILModules @ providerGeneratedILModules)
              
              // Rewrite type and assembly references
              let ilxMainModule =
                  let isMscorlib = ilGlobals.primaryAssemblyName = PrimaryAssembly.Mscorlib.Name
                  let validateTargetPlatform (scopeRef : ILScopeRef) = 
                      let name = getNameOfScopeRef scopeRef
                      if (isMscorlib && name = PrimaryAssembly.DotNetCore.Name) || (not isMscorlib && name = PrimaryAssembly.Mscorlib.Name) then
                          error (Error(FSComp.SR.fscStaticLinkingNoProfileMismatches(), rangeCmdArgs))
                      scopeRef
                  let rewriteAssemblyRefsToMatchLibraries = NormalizeAssemblyRefs tcImports
                  Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (Morphs.morphILScopeRefsInILTypeRef (validateTargetPlatform >> rewriteExternalRefsToLocalRefs >> rewriteAssemblyRefsToMatchLibraries)) ilxMainModule

          #if DEBUG
              // Print it out if requested 
              if tcConfig.writeGeneratedILFiles then (let _ = PrintModule (outpath outfile "ilx.main") ilxMainModule in ());
          #else
              ignore outfile
          #endif

              ilxMainModule)
  
//----------------------------------------------------------------------------
// EMIT IL
//----------------------------------------------------------------------------

type SigningInfo = SigningInfo of (* delaysign:*) bool * (*signer:*)  string option * (*container:*) string option

module FileWriter = 
    let EmitIL (tcConfig:TcConfig,ilGlobals,_errorLogger:ErrorLogger,outfile,pdbfile,ilxMainModule,signingInfo:SigningInfo,exiter:Exiter) =
        let (SigningInfo(delaysign,signer,container)) = signingInfo
        try
    #if DEBUG
            if tcConfig.writeGeneratedILFiles then dprintn "Printing module...";
            if tcConfig.writeGeneratedILFiles  then StaticLinker.PrintModule (outpath outfile "il.txt") ilxMainModule; 
    #endif
            if !progress then dprintn "Writing assembly...";
            try 
                ILBinaryWriter.WriteILBinary 
                  outfile
                  {    ilg = ilGlobals
                       pdbfile=pdbfile;
                       emitTailcalls= tcConfig.emitTailcalls;
                       showTimes=tcConfig.showTimes;

                       signer = 
                         begin
                          // REVIEW: favor the container over the key file - C# appears to do this
                          if isSome container then
                            Some(ILBinaryWriter.ILStrongNameSigner.OpenKeyContainer container.Value)
                          else
                            match signer with 
                            | None -> None
                            | Some(s) ->
                               try 
                                if delaysign then
                                  Some (ILBinaryWriter.ILStrongNameSigner.OpenPublicKeyFile s) 
                                else
                                  Some (ILBinaryWriter.ILStrongNameSigner.OpenKeyPairFile s) 
                               with e -> 
                                   // Note:: don't use errorR here since we really want to fail and not produce a binary
                                   error(Error(FSComp.SR.fscKeyFileCouldNotBeOpened(s),rangeCmdArgs))
                         end;
                       fixupOverlappingSequencePoints = false; 
                       dumpDebugInfo =tcConfig.dumpDebugInfo } 
                  ilxMainModule
                  tcConfig.noDebugData
            with Failure msg -> 
                error(Error(FSComp.SR.fscProblemWritingBinary(outfile,msg), rangeCmdArgs))
        with e -> 
            errorRecoveryNoRange e
#if SQM_SUPPORT
            SqmLoggerWithConfig tcConfig _errorLogger.ErrorNumbers _errorLogger.WarningNumbers
#endif             
            exiter.Exit 1 

    let WriteStatsFile (tcConfig:TcConfig,outfile) = 
      if tcConfig.stats then 
          try 
              use oc = new  StreamWriter((outpath outfile "stats.txt"),append=false,encoding=Encoding.UTF8) :> TextWriter
#if STATISTICS
              Ilread.report oc;
#endif
              Ilxgen.ReportStatistics oc;
          with _ -> ()


let ValidateKeySigningAttributes (tcConfig : TcConfig) tcGlobals topAttrs =
    let delaySignAttrib = AttributeHelpers.TryFindBoolAttribute tcGlobals "System.Reflection.AssemblyDelaySignAttribute" topAttrs.assemblyAttrs
    let signerAttrib = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyKeyFileAttribute" topAttrs.assemblyAttrs
    let containerAttrib = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyKeyNameAttribute" topAttrs.assemblyAttrs
    
    // REVIEW: C# throws a warning when these attributes are used - should we?
    
    // if delaySign is set via an attribute, validate that it wasn't set via an option
    let delaysign = 
        match delaySignAttrib with 
        | Some delaysign -> 
          if tcConfig.delaysign then
            warning(Error(FSComp.SR.fscDelaySignWarning(),rangeCmdArgs)) ;
            tcConfig.delaysign
          else
            delaysign
        | _ -> tcConfig.delaysign
        
         
    // if signer is set via an attribute, validate that it wasn't set via an option
    let signer = 
        match signerAttrib with
        | Some signer -> 
            if tcConfig.signer.IsSome && tcConfig.signer <> Some signer then
                warning(Error(FSComp.SR.fscKeyFileWarning(),rangeCmdArgs)) ;
                tcConfig.signer
            else
                Some signer
        | None -> tcConfig.signer
    
    // if container is set via an attribute, validate that it wasn't set via an option, and that they keyfile wasn't set
    // if keyfile was set, use that instead (silently)
    // REVIEW: This is C# behavior, but it seems kind of sketchy that we fail silently
    let container = 
        match containerAttrib with 
        | Some container -> 
            if tcConfig.container.IsSome && tcConfig.container <> Some container then
              warning(Error(FSComp.SR.fscKeyNameWarning(),rangeCmdArgs)) ;
              tcConfig.container
            else
              Some container
        | None -> tcConfig.container
    
    SigningInfo (delaysign,signer,container)
 
/// Checks if specified file name is absolute path. If yes - returns the name as is, otherwise makes full path using tcConfig.implicitIncludeDir as base.
let expandFileNameIfNeeded (tcConfig : TcConfig) name = 
    if System.IO.Path.IsPathRooted name then name 
    else 
    System.IO.Path.Combine(tcConfig.implicitIncludeDir, name)
   
//----------------------------------------------------------------------------
// main - split up to make sure that we can GC the
// dead data at the end of each phase.  We explicitly communicate arguments
// from one phase to the next.
//-----------------------------------------------------------------------------

[<NoEquality; NoComparison>]
type Args<'a> = Args  of 'a

let main0(argv,bannerAlreadyPrinted,exiter:Exiter, errorLoggerProvider : ErrorLoggerProvider, disposables : DelayedDisposables) = 

    // See Bug 735819 
    let lcidFromCodePage = 
        if (System.Console.OutputEncoding.CodePage <> 65001) &&
           (System.Console.OutputEncoding.CodePage <> System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage) &&
           (System.Console.OutputEncoding.CodePage <> System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.ANSICodePage) then
                System.Threading.Thread.CurrentThread.CurrentUICulture <- new System.Globalization.CultureInfo("en-US")
                Some(1033)
        else
            None

    let tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig,outfile,pdbfile,assemblyName,errorLogger = 
        getTcImportsFromCommandLine(None, argv, defaultFSharpBinariesDir, Directory.GetCurrentDirectory(), lcidFromCodePage, (fun tcConfigB ->
                          match tcConfigB.lcid with
                          | Some(n) -> System.Threading.Thread.CurrentThread.CurrentUICulture <- new System.Globalization.CultureInfo(n)
                          | None -> ()
          
                          if tcConfigB.utf8output then 
                              let prev = System.Console.OutputEncoding
                              System.Console.OutputEncoding <- Encoding.UTF8
                              System.AppDomain.CurrentDomain.ProcessExit.Add(fun _ -> System.Console.OutputEncoding <- prev)
                                    ), (fun tcConfigB -> 
                        // display the banner text, if necessary
                        if not bannerAlreadyPrinted then 
                            Microsoft.FSharp.Compiler.Fscopts.DisplayBannerText tcConfigB
                    ), 
                        false, // optimizeForMemory - fsc.exe can use as much memory as it likes to try to compile as fast as possible
                        exiter,
                        errorLoggerProvider,
                        disposables

    )

    tcGlobals,tcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig,outfile,pdbfile,assemblyName,errorLogger, exiter

// TcGlobals * TcImports * TcImports * CcuThunk * TypedAssembly * TopAttribs * TcConfig * string * string * string* ErrorLogger* Exiter
let main1(tcGlobals,tcImports : TcImports,frameworkTcImports,generatedCcu,typedAssembly,topAttrs,tcConfig : TcConfig, outfile,pdbfile,assemblyName,errorLogger, exiter : Exiter) =


    if tcConfig.typeCheckOnly then exiter.Exit 0
    
    use unwindPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.CodeGen)
    let signingInfo = ValidateKeySigningAttributes tcConfig tcGlobals topAttrs
    
    abortOnError(errorLogger,tcConfig,exiter)

    // Build an updated errorLogger that filters according to the scopedPragmas. Then install
    // it as the updated global error logger and never remove it
    let oldLogger = errorLogger
    let errorLogger = 
        let scopedPragmas = 
            let (TAssembly(impls)) = typedAssembly 
            [ for (TImplFile(_,pragmas,_,_,_)) in impls do yield! pragmas ]
        GetErrorLoggerFilteringByScopedPragmas(true,scopedPragmas,oldLogger)

    let _unwindEL_3 = PushErrorLoggerPhaseUntilUnwind(fun _ -> errorLogger)

    // Try to find an AssemblyVersion attribute 
    let assemVerFromAttrib = 
        match AttributeHelpers.TryFindVersionAttribute tcGlobals "System.Reflection.AssemblyVersionAttribute" "AssemblyVersionAttribute" topAttrs.assemblyAttrs with
        | Some v -> 
           match tcConfig.version with 
           | VersionNone -> Some v
           | _ -> warning(Error(FSComp.SR.fscAssemblyVersionAttributeIgnored(),Range.rangeStartup)); None
        | _ -> None

    // write interface, xmldoc
    begin
      ReportTime tcConfig ("Write Interface File");
      use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Output)    
      if tcConfig.printSignature   then InterfaceFileWriter.WriteInterfaceFile (tcGlobals,tcConfig, InfoReader(tcGlobals,tcImports.GetImportMap()), typedAssembly);
      ReportTime tcConfig ("Write XML document signatures")
      if tcConfig.xmlDocOutputFile.IsSome then 
          XmlDocWriter.computeXmlDocSigs (tcGlobals,generatedCcu) 
      ReportTime tcConfig ("Write XML docs");
      tcConfig.xmlDocOutputFile |> Option.iter ( fun xmlFile -> 
          let xmlFile = expandFileNameIfNeeded tcConfig xmlFile
          XmlDocWriter.writeXmlDoc (assemblyName,generatedCcu,xmlFile)
        )
      ReportTime tcConfig ("Write HTML docs");
    end;


    // Pass on only the minimimum information required for the next phase to ensure GC kicks in.
    // In principle the JIT should be able to do good liveness analysis to clean things up, but the
    // data structures involved here are so large we can't take the risk.
    Args(tcConfig,tcImports,frameworkTcImports,tcGlobals,errorLogger,generatedCcu,outfile,typedAssembly,topAttrs,pdbfile,assemblyName,assemVerFromAttrib,signingInfo,exiter)

  
let main2(Args(tcConfig,tcImports,frameworkTcImports : TcImports,tcGlobals,errorLogger,generatedCcu:CcuThunk,outfile,typedAssembly,topAttrs,pdbfile,assemblyName,assemVerFromAttrib,signingInfo,exiter:Exiter)) = 
      
    ReportTime tcConfig ("Encode Interface Data");
#if DEBUG
    if !verboseStamps then 
        dprintf "---------------------- START MAKE EXPORT REMAPPING ------------\n";
#endif
    let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents
#if DEBUG
    if !verboseStamps then 
        dprintf "---------------------- END MAKE EXPORT REMAPPING ------------\n";
#endif
    
    let sigDataAttributes,sigDataResources = 
        EncodeInterfaceData(tcConfig,tcGlobals,exportRemapping,errorLogger,generatedCcu,outfile,exiter)
        
    if !progress && tcConfig.optSettings.jitOptUser = Some false then 
        dprintf "Note, optimizations are off.\n";
    (* optimize *)
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Optimize)
    
    let optEnv0 = InitialOptimizationEnv tcImports tcGlobals
   
    let importMap = tcImports.GetImportMap()
    let metadataVersion = 
        match tcConfig.metadataVersion with
        | Some(v) -> v
        | _ -> match (frameworkTcImports.DllTable.TryFind tcConfig.primaryAssembly.Name) with | Some(ib) -> ib.RawMetadata.MetadataVersion | _ -> ""
    let optimizedImpls,optimizationData,_ = ApplyAllOptimizations (tcConfig, tcGlobals, (LightweightTcValForUsingInBuildMethodCall tcGlobals), outfile, importMap, false, optEnv0, generatedCcu, typedAssembly)

    abortOnError(errorLogger,tcConfig,exiter)
        
    ReportTime tcConfig ("Encoding OptData");
    let optDataResources = EncodeOptimizationData(tcGlobals,tcConfig,outfile,exportRemapping,(generatedCcu,optimizationData))

    let sigDataResources, _optimizationData = 
        if tcConfig.useSignatureDataFile then 
            let bytes = [| yield! BinaryGenerationUtilities.i32 0x7846ce27
                           yield! BinaryGenerationUtilities.i32 (sigDataResources.Length + optDataResources.Length)
                           for r in (sigDataResources @ optDataResources) do 
                               match r.Location with 
                               |  ILResourceLocation.Local f -> 
                                   let bytes = f() 
                                   yield! BinaryGenerationUtilities.i32 bytes.Length
                                   yield! bytes
                               | _ -> 
                                   failwith "unreachable: expected a local resource" |]
            let sigDataFileName = (Filename.chopExtension outfile)+".fsdata"
            File.WriteAllBytes(sigDataFileName,bytes)
            [], []
        else
            sigDataResources, optDataResources
    
    // Pass on only the minimimum information required for the next phase to ensure GC kicks in.
    // In principle the JIT should be able to do good liveness analysis to clean things up, but the
    // data structures involved here are so large we can't take the risk.
    Args(tcConfig,tcImports,tcGlobals,errorLogger,generatedCcu,outfile,optimizedImpls,topAttrs,pdbfile,assemblyName, (sigDataAttributes, sigDataResources), optDataResources,assemVerFromAttrib,signingInfo,metadataVersion,exiter)

let main2b(Args(tcConfig:TcConfig,tcImports,tcGlobals,errorLogger,generatedCcu:CcuThunk,outfile,optimizedImpls,topAttrs,pdbfile,assemblyName,idata,optDataResources,assemVerFromAttrib,signingInfo,metadataVersion,exiter:Exiter)) = 
  
    // Compute a static linker. 
    let ilGlobals = tcGlobals.ilg
    if tcConfig.standalone && generatedCcu.UsesFSharp20PlusQuotations then    
        error(Error(FSComp.SR.fscQuotationLiteralsStaticLinking0(),rangeStartup));  
    let staticLinker = StaticLinker.StaticLink (tcConfig,tcImports,ilGlobals)

    ReportTime tcConfig "TAST -> ILX";
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind  (BuildPhase.IlxGen)
    let ilxGenerator = CreateIlxAssemblyGenerator (tcConfig,tcImports,tcGlobals, (LightweightTcValForUsingInBuildMethodCall tcGlobals), generatedCcu)

    // Check if System.SerializableAttribute exists in mscorlib.dll, 
    // so that make sure the compiler only emits "serializable" bit into IL metadata when it is available.
    // Note that SerializableAttribute may be relocated in the future but now resides in mscorlib.
    let netFxHasSerializableAttribute = tcImports.SystemRuntimeContainsType "System.SerializableAttribute"
    let codegenResults = GenerateIlxCode (IlWriteBackend, false, false, tcConfig, topAttrs, optimizedImpls, generatedCcu.AssemblyName, netFxHasSerializableAttribute, ilxGenerator)
    let casApplied = new Dictionary<Stamp,bool>()
    let securityAttrs,topAssemblyAttrs = topAttrs.assemblyAttrs |> List.partition (fun a -> TypeChecker.IsSecurityAttribute tcGlobals (tcImports.GetImportMap()) casApplied a rangeStartup)
    // remove any security attributes from the top-level assembly attribute list
    let topAttrs = {topAttrs with assemblyAttrs=topAssemblyAttrs}
    let permissionSets = ilxGenerator.CreatePermissionSets securityAttrs
    let secDecls = if securityAttrs.Length > 0 then mkILSecurityDecls permissionSets else emptyILSecurityDecls


    let ilxMainModule = MainModuleBuilder.CreateMainModule (tcConfig,tcGlobals,pdbfile,assemblyName,outfile,topAttrs,idata,optDataResources,codegenResults,assemVerFromAttrib,metadataVersion,secDecls)
#if DEBUG
    // Print code before bailing out from the compiler due to errors 
    // in the backend of the compiler.  The partially-generated 
    // ILX code often contains useful information. 
    if tcConfig.writeGeneratedILFiles then StaticLinker.PrintModule (outpath outfile "ilx.txt") ilxMainModule;
#endif

    abortOnError(errorLogger,tcConfig,exiter)
    
    Args (tcConfig,errorLogger,staticLinker,ilGlobals,outfile,pdbfile,ilxMainModule,signingInfo,exiter)

let main2c(Args(tcConfig,errorLogger,staticLinker,ilGlobals,outfile,pdbfile,ilxMainModule,signingInfo,exiter:Exiter)) = 
      
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.IlGen)
    
    ReportTime tcConfig "ILX -> IL (Unions)"; 
    let ilxMainModule = EraseIlxUnions.ConvModule ilGlobals ilxMainModule
    ReportTime tcConfig "ILX -> IL (Funcs)"; 
    let ilxMainModule = EraseIlxFuncs.ConvModule ilGlobals ilxMainModule 

    abortOnError(errorLogger,tcConfig,exiter)
    Args(tcConfig,errorLogger,staticLinker,ilGlobals,ilxMainModule,outfile,pdbfile,signingInfo,exiter)
  

let main3(Args(tcConfig,errorLogger:ErrorLogger,staticLinker,ilGlobals,ilxMainModule,outfile,pdbfile,signingInfo,exiter:Exiter)) = 
        
    let ilxMainModule =  
        try  staticLinker (ilxMainModule,outfile)
        with e -> 
            errorRecoveryNoRange e
#if SQM_SUPPORT
            SqmLoggerWithConfig tcConfig errorLogger.ErrorNumbers errorLogger.WarningNumbers
#endif
            exiter.Exit 1

    abortOnError(errorLogger,tcConfig,exiter)
        
    Args (tcConfig,errorLogger,ilGlobals,ilxMainModule,outfile,pdbfile,signingInfo,exiter)

let main4(Args(tcConfig,errorLogger:ErrorLogger,ilGlobals,ilxMainModule,outfile,pdbfile,signingInfo,exiter)) = 
    ReportTime tcConfig "Write .NET Binary"
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Output)    
    let outfile = expandFileNameIfNeeded tcConfig outfile

    let pdbfile = pdbfile |> Option.map ((expandFileNameIfNeeded tcConfig) >> Path.GetFullPath)
    FileWriter.EmitIL (tcConfig,ilGlobals,errorLogger,outfile,pdbfile,ilxMainModule,signingInfo,exiter)

    ReportTime tcConfig "Write Stats File"
    FileWriter.WriteStatsFile (tcConfig,outfile)

    abortOnError(errorLogger,tcConfig,exiter)
    if tcConfig.showLoadedAssemblies then
        for a in System.AppDomain.CurrentDomain.GetAssemblies() do
            dprintfn "%s" a.FullName

#if SQM_SUPPORT
    SqmLoggerWithConfig tcConfig errorLogger.ErrorNumbers errorLogger.WarningNumbers
#endif

    ReportTime tcConfig "Exiting"

let typecheckAndCompile(argv,bannerAlreadyPrinted,exiter:Exiter, errorLoggerProvider) =
    // Don's note: "GC of intermediate data is really, really important here"
    use d = new DelayedDisposables()
    main0(argv,bannerAlreadyPrinted,exiter, errorLoggerProvider, d)
    |> main1
    |> main2
    |> main2b
    |> main2c
    |> main3 
    |> main4

let mainCompile (argv,bannerAlreadyPrinted,exiter:Exiter) = 
    // Enabling batch latency mode currently overrides app config <gcConcurrent>.
    // If batch mode is ever removed or changed, revisit use of <gcConcurrent>.
    System.Runtime.GCSettings.LatencyMode <- System.Runtime.GCLatencyMode.Batch
    typecheckAndCompile(argv, bannerAlreadyPrinted, exiter, DefaultLoggerProvider())

type CompilationOutput = 
    {
        Errors : seq<ErrorOrWarning>
        Warnings : seq<ErrorOrWarning>
    }

type InProcCompiler() = 
    member this.Compile(argv) = 

        let errors = ResizeArray()
        let warnings = ResizeArray()

        let rec loggerProvider = {
            new ErrorLoggerProvider() with
                member log.CreateErrorLoggerThatQuitsAfterMaxErrors(tcConfigBuilder, exiter) = 
                    upcast {
                        new ErrorLoggerThatQuitsAfterMaxErrors(tcConfigBuilder, exiter, "InProcCompilerErrorLoggerThatQuitsAfterMaxErrors") with
                            member this.HandleTooManyErrors(text) = warnings.Add(ErrorOrWarning.Short(false, text))
                            member this.HandleIssue(tcConfigBuilder, err, isWarning) = 
                                let errs = CollectErrorOrWarning(tcConfigBuilder.implicitIncludeDir, tcConfigBuilder.showFullPaths, tcConfigBuilder.flatErrors, tcConfigBuilder.errorStyle, isWarning, err)
                                let container = if isWarning then warnings else errors
                                container.AddRange(errs)
                    }
        }
        let exitCode = ref 0
        let exiter = {
            new Exiter with
                member this.Exit n = exitCode := n; raise StopProcessing
            }
        try 
            typecheckAndCompile(argv, false, exiter, loggerProvider)
        with 
            | StopProcessing -> ()
            | ReportedError _  | WrappedError(ReportedError _,_)  ->
                exitCode := 1
                ()

        let output = { Warnings = warnings; Errors = errors}
        !exitCode = 0, output

/// Collect the output from the stdout and stderr streams, character by character,
/// recording the console color used along the way.
type private OutputCollector() = 
    let output = ResizeArray()
    let outWriter isOut = 
        { new TextWriter() with 
              member x.Write(c:char) = lock output (fun () -> output.Add (isOut, (try Some System.Console.ForegroundColor with _ -> None) ,c)) 
              member x.Encoding = Encoding.UTF8 }
    do System.Console.SetOut (outWriter true)
    do System.Console.SetError (outWriter false)
    member x.GetTextAndClear() = lock output (fun () -> let res = output.ToArray() in output.Clear(); res)

/// Implement the optional resident compilation service
module FSharpResidentCompiler = 

    open System
    open System.Diagnostics
    open System.Runtime.Remoting.Channels
    open System.Runtime.Remoting
    open System.Runtime.Remoting.Lifetime

    /// The compilation server, which runs in the server process. Accessed by clients using .NET remoting.
    type FSharpCompilationServer(exiter:Exiter)  =
        inherit MarshalByRefObject()  

        static let onWindows = 
            match System.Environment.OSVersion.Platform with 
            | PlatformID.Win32NT | PlatformID.Win32S | PlatformID.Win32Windows | PlatformID.WinCE -> true
            | _  -> false

        // The channel/socket name is qualified by the user name (and domain on windows)
        static let domainName = if onWindows then Environment.GetEnvironmentVariable "USERDOMAIN" else ""
        static let userName = Environment.GetEnvironmentVariable (if onWindows then "USERNAME" else "USER") 
        // Use different base channel names on mono and CLR as a CLR remoting process can't talk
        // to a mono server
        static let baseChannelName = if runningOnMono then "FSCChannelMono" else "FSCChannel"
        static let channelName = baseChannelName + "_" +  domainName + "_" + userName
        static let serverName = if runningOnMono then "FSCServerMono" else "FSCSever"
        static let mutable serverExists = true
        
        let outputCollector = new OutputCollector()

        // This background agent ensures all compilation requests sent to the server are serialized
        let agent = MailboxProcessor<_>.Start(fun inbox -> 
                       async { 
                          while true do 
                              let! (pwd,argv, reply: AsyncReplyChannel<_>) = inbox.Receive()
                              if !progress then printfn "server agent: got compilation request, argv = %A" argv
                              let exitCode = 
                                  try 
                                      Environment.CurrentDirectory <- pwd
                                      mainCompile (argv, true, exiter); 
                                      if !progress then printfn "server: finished compilation request, argv = %A" argv
                                      0
                                  with e -> 
                                      if !progress then printfn "server: finished compilation request with errors, argv = %A" argv
                                      errorRecoveryNoRange e
                                      1
                              let output = outputCollector.GetTextAndClear()
                              reply.Reply(output, exitCode)
                              GC.Collect(3)
                              // Exit the server if there are no outstanding requests and the 
                              // current memory usage after collection is over 200MB
                              if inbox.CurrentQueueLength = 0 && GC.GetTotalMemory(true) > 200L * 1024L * 1024L then 
                                  Environment.Exit 0
                       })

        member x.Run() = 
            while serverExists do 
               if !progress then printfn "server: startup thread sleeping..." 
               System.Threading.Thread.Sleep 1000

        abstract Ping : unit -> string
        abstract Compile : string * string[] -> (bool * System.ConsoleColor option * char) [] * int
        default x.Ping() = "ping"
        default x.Compile (pwd,argv) = 
            if !progress then printfn "server: got compilation request, (pwd, argv) = %A" (pwd, argv)
            agent.PostAndReply(fun reply -> (pwd,argv,reply))

        override x.Finalize() =
            serverExists <- false

        // This is called on the server object by .NET remoting to initialize the lifetime characteristics
        // of the server object.
        override x.InitializeLifetimeService() =
            let lease = (base.InitializeLifetimeService() :?> ILease)
            if (lease.CurrentState = LeaseState.Initial)  then
                lease.InitialLeaseTime <- TimeSpan.FromDays(1.0);
                lease.SponsorshipTimeout <- TimeSpan.FromMinutes(2.0);
                lease.RenewOnCallTime <- TimeSpan.FromDays(1.0);
            box lease
            
        static member RunServer(exiter:Exiter) =
            progress := condition "FSHARP_SERVER_PROGRESS"
            if !progress then printfn "server: initializing server object" 
            let server = new FSharpCompilationServer(exiter)
            let chan = new Ipc.IpcChannel(channelName) 
            ChannelServices.RegisterChannel(chan,false);
            RemotingServices.Marshal(server,serverName)  |> ignore

            // On Unix, the file permissions of the implicit socket need to be set correctly to make this
            // private to the user.
            if runningOnMono then 
              try 
                  let monoPosix = System.Reflection.Assembly.Load("Mono.Posix, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756")
                  let monoUnixFileInfo = monoPosix.GetType("Mono.Unix.UnixFileSystemInfo") 
                  let socketName = Path.Combine(FileSystem.GetTempPathShim(), channelName)
                  let fileEntry = monoUnixFileInfo.InvokeMember("GetFileSystemEntry", (BindingFlags.InvokeMethod ||| BindingFlags.Static ||| BindingFlags.Public), null, null, [| box socketName |],System.Globalization.CultureInfo.InvariantCulture)
                  // Add 0x00000180 (UserReadWriteExecute) to the access permissions on Unix
                  monoUnixFileInfo.InvokeMember("set_FileAccessPermissions", (BindingFlags.InvokeMethod ||| BindingFlags.Instance ||| BindingFlags.Public), null, fileEntry, [| box 0x00000180 |],System.Globalization.CultureInfo.InvariantCulture) |> ignore
#if DEBUG
                  printfn "server: good, set permissions on socket name '%s'"  socketName
                  let fileEntry = monoUnixFileInfo.InvokeMember("GetFileSystemEntry", (BindingFlags.InvokeMethod ||| BindingFlags.Static ||| BindingFlags.Public), null, null, [| box socketName |],System.Globalization.CultureInfo.InvariantCulture)
                  let currPermissions = monoUnixFileInfo.InvokeMember("get_FileAccessPermissions", (BindingFlags.InvokeMethod ||| BindingFlags.Instance ||| BindingFlags.Public), null, fileEntry, [| |],System.Globalization.CultureInfo.InvariantCulture) |> unbox<int>
                  if !progress then printfn "server: currPermissions = '%o' (octal)"  currPermissions
#endif
              with e -> 
#if DEBUG
                  printfn "server: failed to set permissions on socket, perhaps on windows? Is is not needed there."  
#endif
                  ()
                  // Fail silently
            server.Run()
            
        static member private ConnectToServer() =
            Activator.GetObject(typeof<FSharpCompilationServer>,"ipc://" + channelName + "/" + serverName) 
            :?> FSharpCompilationServer 

        static member TryCompileUsingServer(argv) =
            let pwd = System.Environment.CurrentDirectory
            let clientOpt = 
                // Detect the absence of the channel via the exception. Probably not the best way.
                // Different exceptions get thrown here on Mono and Windows.
                let client = FSharpCompilationServer.ConnectToServer()
                try 
                    if !progress then printfn "client: attempting to connect to existing service (1)"
                    client.Ping() |> ignore
                    if !progress then printfn "client: connected to existing service"
                    Some client
                with _ ->
                    let procInfo = 
                        if runningOnMono then
                            let shellName, useShellExecute = 
                                match System.Environment.GetEnvironmentVariable("FSC_MONO") with 
                                | null -> 
                                    if onWindows then 
                                        Path.Combine(Path.GetDirectoryName (typeof<Object>.Assembly.Location), @"..\..\..\bin\mono.exe"), false
                                    else
                                        "mono", true
                                | path -> path, false
                                     
                            // e.g. "C:\Program Files\Mono-2.6.1\lib\mono\2.0\mscorlib.dll" --> "C:\Program Files\Mono-2.6.1\bin\mono.exe"
                            ProcessStartInfo(FileName = shellName,
                                             Arguments = typeof<FSharpCompilationServer>.Assembly.Location + " /server",
                                             CreateNoWindow = true,
                                             UseShellExecute = useShellExecute)
                         else
                            ProcessStartInfo(FileName=typeof<FSharpCompilationServer>.Assembly.Location,
                                             Arguments = "/server",
                                             CreateNoWindow = true,
                                             UseShellExecute = false)

                    let cmdProcess = new Process(StartInfo=procInfo)

                    //let exitE = cmdProcess.Exited |> Observable.map (fun x -> x)

                    cmdProcess.Start() |> ignore
                    //exitE.Add(fun _ -> if !progress then eprintfn "client: the server has exited")
                    cmdProcess.EnableRaisingEvents <- true;
                     
                    // Create the client proxy and attempt to connect to the server
                    let rec tryAcccesServer nRemaining =
                        if nRemaining = 0 then 
                            // Failed to connect to server, give up 
                            None
                        else
                            try 
                                if !progress then printfn "client: attempting to connect to existing service (2)"
                                client.Ping() |> ignore
                                if !progress then printfn "client: connected to existing service"
                                Some client
                            // Detect the absence of the channel via the exception. Probably not the best way.
                            // Different exceptions get thrown here on Mono and Windows.
                            with _ (* System.Runtime.Remoting.RemotingException *) ->
                                // Sleep a bit
                                System.Threading.Thread.Sleep 50
                                tryAcccesServer (nRemaining - 1)

                    tryAcccesServer 20

            match clientOpt with
            | Some client -> 
                if !progress then printfn "client: calling client.Compile(%A)" argv
                // Install the global error logger and never remove it. This logger does have all command-line flags considered.
                try 
                    let (output, exitCode) = 
                        try client.Compile (pwd, argv) 
                        with e -> 
                           printfn "server error: %s" (e.ToString())
                           raise (Error (FSComp.SR.fscRemotingError(), rangeStartup))
                        
                    if !progress then printfn "client: returned from client.Compile(%A), res = %d" argv exitCode
                    use holder = 
                        try let originalConsoleColor = Console.ForegroundColor 
                            { new System.IDisposable with member x.Dispose() = Console.ForegroundColor <- originalConsoleColor }
                        with _ -> null
                    let mutable prevConsoleColor = try Console.ForegroundColor with _ -> ConsoleColor.Black
                    for (isOut, consoleColorOpt, c:char) in output do 
                        try match consoleColorOpt with 
                             | Some consoleColor -> 
                                 if prevConsoleColor <> consoleColor then 
                                     Console.ForegroundColor <- consoleColor; 
                             | None -> ()
                        with _ -> ()
                        c |> (if isOut then System.Console.Out.Write else System.Console.Error.Write)
                    Some exitCode
                with err -> 
                   let sb = System.Text.StringBuilder()
                   OutputErrorOrWarning (pwd,true,false,ErrorStyle.DefaultErrors,true) sb (PhasedError.Create(err,BuildPhase.Compile))
                   eprintfn "%s" (sb.ToString())
                   // We continue on and compile in-process - the server appears to have died half way through.
                   None
            | None -> 
                None

let main argv = 
    let inline hasArgument name args = 
        args |> Array.exists (fun x -> x = ("--" + name) || x = ("/" + name))
    let inline stripArgument name args = 
        args |> Array.filter (fun x -> x <> ("--" + name) && x <> ("/" + name))

    // Check for --pause as the very first step so that a compiler can be attached here.
    if hasArgument "pause" argv then 
        System.Console.WriteLine("Press any key to continue...")
        System.Console.ReadKey() |> ignore
      
    if runningOnMono && hasArgument "resident" argv then 
        let argv = stripArgument "resident" argv

        if not (hasArgument "nologo" argv) then 
            printfn "%s" (FSComp.SR.buildProductName(FSharpEnvironment.FSharpTeamVersionNumber))
            printfn "%s" (FSComp.SR.optsCopyright())

        let exitCodeOpt = FSharpResidentCompiler.FSharpCompilationServer.TryCompileUsingServer argv
        match exitCodeOpt with 
        | Some exitCode -> exitCode
        | None -> 
            mainCompile (argv, true, QuitProcessExiter)
            0

    elif runningOnMono && hasArgument "server" argv then 
        // Install the right exiter so we can catch "StopProcessing" without exiting the server
        let exiter = { new Exiter with member x.Exit n = raise StopProcessing }
        FSharpResidentCompiler.FSharpCompilationServer.RunServer(exiter)        
        0
        
    else
        mainCompile (argv, false, QuitProcessExiter)
        0

#endif
