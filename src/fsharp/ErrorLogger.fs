// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.ErrorLogger


open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Range
open System

//------------------------------------------------------------------------
// General error recovery mechanism
//-----------------------------------------------------------------------

/// Thrown when we want to add some range information to a .NET exception
exception WrappedError of exn * range

/// Thrown when immediate, local error recovery is not possible. This indicates
/// we've reported an error but need to make a non-local transfer of control.
/// Error recovery may catch this and continue (see 'errorRecovery')
///
/// The exception that caused the report is carried as data because in some
/// situations (LazyWithContext) we may need to re-report the original error
/// when a lazy thunk is re-evaluated.
exception ReportedError of exn option with
    override this.Message =
        match this :> exn with
        | ReportedError (Some exn) -> exn.Message
        | _ -> "ReportedError"

let rec findOriginalException err = 
    match err with 
    | ReportedError (Some err) -> err
    | WrappedError(err,_)  -> findOriginalException err
    | _ -> err


/// Thrown when we stop processing the F# Interactive entry or #load.
exception StopProcessingExn of exn option
let (|StopProcessing|_|) exn = match exn with StopProcessingExn _ -> Some () | _ -> None
let StopProcessing<'T> = StopProcessingExn None

(* common error kinds *)
exception NumberedError of (int * string) * range with   // int is e.g. 191 in FS0191
    override this.Message =
        match this :> exn with
        | NumberedError((_,msg),_) -> msg
        | _ -> "impossible"
exception Error of (int * string) * range with   // int is e.g. 191 in FS0191  // eventually remove this type, it is a transitional artifact of the old unnumbered error style
    override this.Message =
        match this :> exn with
        | Error((_,msg),_) -> msg
        | _ -> "impossible"
exception InternalError of string * range
exception UserCompilerMessage of string * int * range
exception LibraryUseOnly of range
exception Deprecated of string * range
exception Experimental of string * range
exception PossibleUnverifiableCode of range

// Range/NoRange Duals
exception UnresolvedReferenceNoRange of (*assemblyname*) string 
exception UnresolvedReferenceError of (*assemblyname*) string * range
exception UnresolvedPathReferenceNoRange of (*assemblyname*) string * (*path*) string
exception UnresolvedPathReference of (*assemblyname*) string * (*path*) string * range


let inline protectAssemblyExploration dflt f = 
    try 
       f()
     with 
        | UnresolvedPathReferenceNoRange _ -> dflt
        | _ -> reraise()

let inline protectAssemblyExplorationF dflt f = 
    try 
       f()
     with 
        | UnresolvedPathReferenceNoRange (asmName, path) -> dflt(asmName,path)
        | _ -> reraise()

let inline protectAssemblyExplorationNoReraise dflt1 dflt2 f  = 
    try 
       f()
     with 
        | UnresolvedPathReferenceNoRange _ -> dflt1
        | _ -> dflt2

// Attach a range if this is a range dual exception.
let rec AttachRange m (exn:exn) = 
    if m = range0 then exn
    else 
        match exn with
        // Strip TargetInvocationException wrappers
        | :? System.Reflection.TargetInvocationException -> AttachRange m exn.InnerException
        | UnresolvedReferenceNoRange(a) -> UnresolvedReferenceError(a,m)
        | UnresolvedPathReferenceNoRange(a,p) -> UnresolvedPathReference(a,p,m)
        | Failure(msg) -> InternalError(msg^" (Failure)",m)
        | :? System.ArgumentException as exn -> InternalError(exn.Message + " (ArgumentException)",m)
        | notARangeDual -> notARangeDual

//----------------------------------------------------------------------------
// Error logger interface

type Exiter = 
    abstract Exit : int -> 'T 


let QuitProcessExiter =  
    { new Exiter with  
        member x.Exit(n) =                     
            try  
                System.Environment.Exit(n) 
            with _ ->  
                ()             
            failwithf "%s" <| FSComp.SR.elSysEnvExitDidntExit() } 

/// Closed enumeration of build phases.
type BuildPhase =
    | DefaultPhase 
    | Compile 
    |  Parameter | Parse | TypeCheck 
    | CodeGen 
    |  Optimize |  IlxGen |  IlGen |  Output 
    | Interactive // An error seen during interactive execution
    
/// Literal build phase subcategory strings.
module BuildPhaseSubcategory =
    [<Literal>] 
    let DefaultPhase = ""
    [<Literal>] 
    let Compile = "compile"
    [<Literal>] 
    let Parameter = "parameter"
    [<Literal>] 
    let Parse = "parse"
    [<Literal>] 
    let TypeCheck = "typecheck"
    [<Literal>] 
    let CodeGen = "codegen"
    [<Literal>] 
    let Optimize = "optimize"
    [<Literal>] 
    let IlxGen = "ilxgen"
    [<Literal>] 
    let IlGen = "ilgen"        
    [<Literal>] 
    let Output = "output"        
    [<Literal>] 
    let Interactive = "interactive"        
    [<Literal>] 
    let Internal = "internal"          // Compiler ICE

[<System.Diagnostics.DebuggerDisplay("{DebugDisplay()}")>]
type PhasedError = { Exception:exn; Phase:BuildPhase } with
    /// Construct a phased error
    static member Create(exn:exn,phase:BuildPhase) : PhasedError =
        System.Diagnostics.Debug.Assert(phase<>BuildPhase.DefaultPhase, sprintf "Compile error seen with no phase to attribute it to.%A %s %s" phase exn.Message exn.StackTrace )        
        {Exception = exn; Phase=phase}
    member this.DebugDisplay() =
        sprintf "%s: %s" (this.Subcategory()) this.Exception.Message
    /// This is the textual subcategory to display in error and warning messages (shows only under --vserrors):
    ///
    ///     file1.fs(72): subcategory warning FS0072: This is a warning message
    ///
    member pe.Subcategory() =
        match pe.Phase with
        | DefaultPhase -> BuildPhaseSubcategory.DefaultPhase
        | Compile -> BuildPhaseSubcategory.Compile
        | Parameter -> BuildPhaseSubcategory.Parameter
        | Parse -> BuildPhaseSubcategory.Parse
        | TypeCheck -> BuildPhaseSubcategory.TypeCheck
        | CodeGen -> BuildPhaseSubcategory.CodeGen
        | Optimize -> BuildPhaseSubcategory.Optimize
        | IlxGen -> BuildPhaseSubcategory.IlxGen
        | IlGen -> BuildPhaseSubcategory.IlGen
        | Output -> BuildPhaseSubcategory.Output
        | Interactive -> BuildPhaseSubcategory.Interactive
    /// Return true if the textual phase given is from the compile part of the build process.
    /// This set needs to be equal to the set of subcategories that the language service can produce. 
    static member IsSubcategoryOfCompile(subcategory:string) =
        // This code logic is duplicated in DocumentTask.cs in the language service.
        match subcategory with 
        | BuildPhaseSubcategory.Compile 
        | BuildPhaseSubcategory.Parameter 
        | BuildPhaseSubcategory.Parse 
        | BuildPhaseSubcategory.TypeCheck -> true
        | null 
        | BuildPhaseSubcategory.DefaultPhase 
        | BuildPhaseSubcategory.CodeGen 
        | BuildPhaseSubcategory.Optimize 
        | BuildPhaseSubcategory.IlxGen 
        | BuildPhaseSubcategory.IlGen 
        | BuildPhaseSubcategory.Output 
        | BuildPhaseSubcategory.Interactive -> false
        | BuildPhaseSubcategory.Internal 
            // Getting here means the compiler has ICE-d. Let's not pile on by showing the unknownSubcategory assert below.
            // Just treat as an unknown-to-LanguageService error.
            -> false
        | unknownSubcategory -> 
            System.Diagnostics.Debug.Assert(false, sprintf "Subcategory '%s' could not be correlated with a build phase." unknownSubcategory)
            // Recovery is to treat this as a 'build' error. Downstream, the project system and language service will treat this as
            // if it came from the build and not the language service.
            false
    /// Return true if this phase is one that's known to be part of the 'compile'. This is the initial phase of the entire compilation that
    /// the language service knows about.                
    member pe.IsPhaseInCompile() = 
        let isPhaseInCompile = 
            match pe.Phase with
            | Compile | Parameter | Parse | TypeCheck -> true
            | _ -> false
        // Sanity check ensures that Phase matches Subcategory            
#if DEBUG
        if isPhaseInCompile then 
            System.Diagnostics.Debug.Assert(PhasedError.IsSubcategoryOfCompile(pe.Subcategory()), "Subcategory did not match isPhaesInCompile=true")
        else
            System.Diagnostics.Debug.Assert(not(PhasedError.IsSubcategoryOfCompile(pe.Subcategory())), "Subcategory did not match isPhaseInCompile=false")
#endif            
        isPhaseInCompile

[<AbstractClass>]
[<System.Diagnostics.DebuggerDisplay("{DebugDisplay()}")>]
type ErrorLogger(nameForDebugging:string) = 
    abstract ErrorCount: int
    // The 'Impl' factoring enables a developer to place a breakpoint at the non-Impl 
    // code just below and get a breakpoint for all error logger implementations.
    abstract WarnSinkImpl: PhasedError -> unit
    abstract ErrorSinkImpl: PhasedError -> unit
    member this.WarnSink err = 
        this.WarnSinkImpl err
    member this.ErrorSink err =
        this.ErrorSinkImpl err
    member this.DebugDisplay() = sprintf "ErrorLogger(%s)" nameForDebugging
    // Record the reported error/warning numbers for SQM purpose
    abstract ErrorNumbers : int list
    abstract WarningNumbers : int list
    default this.ErrorNumbers = []
    default this.WarningNumbers = []

let DiscardErrorsLogger = 
    { new ErrorLogger("DiscardErrorsLogger") with 
            member x.WarnSinkImpl(e) = 
                ()
            member x.ErrorSinkImpl(e) = 
                ()
            member x.ErrorCount = 
                0 }

let AssertFalseErrorLogger =
    { new ErrorLogger("AssertFalseErrorLogger") with 
            member x.WarnSinkImpl(e) = 
                assert false; ()
            member x.ErrorSinkImpl(e) = 
                assert false; ()
            member x.ErrorCount = 
                assert false; 0 }

/// When no errorLogger is installed (on the thread) use this one.
let uninitializedErrorLoggerFallback = ref AssertFalseErrorLogger

/// Type holds thread-static globals for use by the compile.
type internal CompileThreadStatic =
    [<System.ThreadStatic;DefaultValue>]
    static val mutable private buildPhase  : BuildPhase
    
    [<System.ThreadStatic;DefaultValue>]
    static val mutable private errorLogger : ErrorLogger

    static member BuildPhaseUnchecked with get() = CompileThreadStatic.buildPhase (* This can be a null value *)
    static member BuildPhase
        with get() = if box CompileThreadStatic.buildPhase <> null then CompileThreadStatic.buildPhase else (assert false; BuildPhase.DefaultPhase)
        and set v = CompileThreadStatic.buildPhase <- v
            
    static member ErrorLogger
        with get() = if box CompileThreadStatic.errorLogger <> null then CompileThreadStatic.errorLogger else !uninitializedErrorLoggerFallback
        and set v = CompileThreadStatic.errorLogger <- v


[<AutoOpen>]
module ErrorLoggerExtensions = 
    open System.Reflection

    // Instruct the exception not to reset itself when thrown again.
    // Design Note: This enables the compiler to prompt the user to send mail to fsbugs@microsoft.com, 
    // by catching the exception, prompting and then propagating the exception with reraise. 
    let PreserveStackTrace(exn) =
        try 
            let preserveStackTrace = typeof<System.Exception>.GetMethod("InternalPreserveStackTrace", BindingFlags.Instance ||| BindingFlags.NonPublic)
            preserveStackTrace.Invoke(exn, null) |> ignore
        with e->
           // This is probably only the mono case.
           System.Diagnostics.Debug.Assert(false, "Could not preserve stack trace for watson exception.")
           ()


    // Reraise an exception if it is one we want to report to Watson.
    let ReraiseIfWatsonable(exn:exn) =
#if FX_REDUCED_EXCEPTIONS
        ignore exn
        ()
#else
        match  exn with 
        // These few SystemExceptions which we don't report to Watson are because we handle these in some way in Build.fs
        | :? System.Reflection.TargetInvocationException -> ()
        | :? System.NotSupportedException  -> ()
        | :? System.IO.IOException -> () // This covers FileNotFoundException and DirectoryNotFoundException
        | :? System.UnauthorizedAccessException -> ()
        | Failure _ // This gives reports for compiler INTERNAL ERRORs
        | :? System.SystemException -> 
            PreserveStackTrace(exn)
            raise exn
        | _ -> ()
#endif

    type ErrorLogger with  
        member x.ErrorR  exn = match exn with StopProcessing | ReportedError _ -> raise exn | _ -> x.ErrorSink(PhasedError.Create(exn,CompileThreadStatic.BuildPhase))
        member x.Warning exn = match exn with StopProcessing | ReportedError _ -> raise exn | _ -> x.WarnSink(PhasedError.Create(exn,CompileThreadStatic.BuildPhase))
        member x.Error   exn = x.ErrorR exn; raise (ReportedError (Some exn))
        member x.PhasedError   (ph:PhasedError) = 
            x.ErrorSink ph
            raise (ReportedError (Some ph.Exception))
        member x.ErrorRecovery (exn:exn) (m:range) =
            // Never throws ReportedError.
            // Throws StopProcessing and exceptions raised by the ErrorSink(exn) handler.
            match exn with
            (* Don't send ThreadAbortException down the error channel *)
#if FX_REDUCED_EXCEPTIONS
#else
            | :? System.Threading.ThreadAbortException | WrappedError((:? System.Threading.ThreadAbortException),_) ->  ()
#endif
            | ReportedError _  | WrappedError(ReportedError _,_)  -> ()
            | StopProcessing | WrappedError(StopProcessing,_) -> raise exn
            | _ ->
                try  
                    x.ErrorR (AttachRange m exn) // may raise exceptions, e.g. an fsi error sink raises StopProcessing.
                    ReraiseIfWatsonable(exn)
                with
                  | ReportedError _ | WrappedError(ReportedError _,_)  -> ()
        member x.StopProcessingRecovery (exn:exn) (m:range) =
            // Do standard error recovery.
            // Additionally ignore/catch StopProcessing. [This is the only catch handler for StopProcessing].
            // Additionally ignore/catch ReportedError.
            // Can throw other exceptions raised by the ErrorSink(exn) handler.         
            match exn with
            | StopProcessing | WrappedError(StopProcessing,_) -> () // suppress, so skip error recovery.
            | _ ->
                try  x.ErrorRecovery exn m
                with
                  | StopProcessing | WrappedError(StopProcessing,_) -> () // catch, e.g. raised by ErrorSink.
                  | ReportedError _ | WrappedError(ReportedError _,_)  -> () // catch, but not expected unless ErrorRecovery is changed.
        member x.ErrorRecoveryNoRange (exn:exn) =
            x.ErrorRecovery exn range0

/// NOTE: The change will be undone when the returned "unwind" object disposes
let PushThreadBuildPhaseUntilUnwind (phase:BuildPhase) =
    let oldBuildPhase = CompileThreadStatic.BuildPhaseUnchecked
    CompileThreadStatic.BuildPhase <- phase
    { new System.IDisposable with 
         member x.Dispose() = CompileThreadStatic.BuildPhase <- oldBuildPhase (* maybe null *) }

/// NOTE: The change will be undone when the returned "unwind" object disposes
let PushErrorLoggerPhaseUntilUnwind(errorLoggerTransformer : ErrorLogger -> #ErrorLogger) =
    let oldErrorLogger = CompileThreadStatic.ErrorLogger
    let newErrorLogger = errorLoggerTransformer oldErrorLogger
    let newInstalled = ref true
    let newIsInstalled() = if !newInstalled then () else (assert false; (); (*failwith "error logger used after unwind"*)) // REVIEW: ok to throw?
    let chkErrorLogger = { new ErrorLogger("PushErrorLoggerPhaseUntilUnwind") with
                             member x.WarnSinkImpl(e)  = newIsInstalled(); newErrorLogger.WarnSink(e)
                             member x.ErrorSinkImpl(e) = newIsInstalled(); newErrorLogger.ErrorSink(e)
                             member x.ErrorCount   = newIsInstalled(); newErrorLogger.ErrorCount }
    CompileThreadStatic.ErrorLogger <- chkErrorLogger
    { new System.IDisposable with 
         member x.Dispose() =       
            CompileThreadStatic.ErrorLogger <- oldErrorLogger
            newInstalled := false }

let SetThreadBuildPhaseNoUnwind(phase:BuildPhase) = CompileThreadStatic.BuildPhase <- phase
let SetThreadErrorLoggerNoUnwind(errorLogger)     = CompileThreadStatic.ErrorLogger <- errorLogger
let SetUninitializedErrorLoggerFallback errLogger = uninitializedErrorLoggerFallback := errLogger

// Global functions are still used by parser and TAST ops.
let errorR  exn = CompileThreadStatic.ErrorLogger.ErrorR exn
let warning exn = CompileThreadStatic.ErrorLogger.Warning exn
let error   exn = CompileThreadStatic.ErrorLogger.Error exn
// for test only
let phasedError (p : PhasedError) = CompileThreadStatic.ErrorLogger.PhasedError p

let errorSink pe = CompileThreadStatic.ErrorLogger.ErrorSink pe
let warnSink pe = CompileThreadStatic.ErrorLogger.WarnSink pe
let errorRecovery exn m = CompileThreadStatic.ErrorLogger.ErrorRecovery exn m
let stopProcessingRecovery exn m = CompileThreadStatic.ErrorLogger.StopProcessingRecovery exn m
let errorRecoveryNoRange exn = CompileThreadStatic.ErrorLogger.ErrorRecoveryNoRange exn


let report f = 
    f() 

let deprecatedWithError s m = errorR(Deprecated(s,m))

// Note: global state, but only for compiling FSharp.Core.dll
let mutable reportLibraryOnlyFeatures = true
let libraryOnlyError m = if reportLibraryOnlyFeatures then errorR(LibraryUseOnly(m))
let libraryOnlyWarning m = if reportLibraryOnlyFeatures then warning(LibraryUseOnly(m))
let deprecatedOperator m = deprecatedWithError (FSComp.SR.elDeprecatedOperator()) m
let mlCompatWarning s m = warning(UserCompilerMessage(FSComp.SR.mlCompatMessage s, 62, m))

let suppressErrorReporting f =
    let errorLogger = CompileThreadStatic.ErrorLogger
    try
        let errorLogger = 
            { new ErrorLogger("suppressErrorReporting") with 
                member x.WarnSinkImpl(_exn) = ()
                member x.ErrorSinkImpl(_exn) = ()
                member x.ErrorCount = 0 }
        SetThreadErrorLoggerNoUnwind(errorLogger)
        f()
    finally
        SetThreadErrorLoggerNoUnwind(errorLogger)

let conditionallySuppressErrorReporting cond f = if cond then suppressErrorReporting f else f()

//------------------------------------------------------------------------
// Errors as data: Sometimes we have to reify errors as data, e.g. if backtracking 
//
// REVIEW: consider using F# computation expressions here

[<NoEquality; NoComparison>]
type OperationResult<'T> = 
    | OkResult of (* warnings: *) exn list * 'T
    | ErrorResult of (* warnings: *) exn list * exn
    
type ImperativeOperationResult = OperationResult<unit>

let ReportWarnings warns = 
    match warns with 
    | [] -> () // shortcut in common case
    | _ -> List.iter warning warns

let CommitOperationResult res = 
    match res with 
    | OkResult (warns,res) -> ReportWarnings warns; res
    | ErrorResult (warns,err) -> ReportWarnings warns; error err

let RaiseOperationResult res : unit = CommitOperationResult res

let ErrorD err = ErrorResult([],err)
let WarnD err = OkResult([err],())
let CompleteD = OkResult([],())
let ResultD x = OkResult([],x)
let CheckNoErrorsAndGetWarnings res  = match res with OkResult (warns,_) -> Some warns | ErrorResult _ -> None 

/// The bind in the monad. Stop on first error. Accumulate warnings and continue. 
let (++) res f = 
    match res with 
    | OkResult([],res) -> (* tailcall *) f res 
    | OkResult(warns,res) -> 
        begin match f res with 
        | OkResult(warns2,res2) -> OkResult(warns@warns2, res2)
        | ErrorResult(warns2,err) -> ErrorResult(warns@warns2, err)
        end
    | ErrorResult(warns,err) -> 
        ErrorResult(warns,err)
        
/// Stop on first error. Accumulate warnings and continue. 
let rec IterateD f xs = match xs with [] -> CompleteD | h :: t -> f h ++ (fun () -> IterateD f t)
let rec WhileD gd body = if gd() then body() ++ (fun () -> WhileD gd body) else CompleteD
let MapD f xs = let rec loop acc xs = match xs with [] -> ResultD (List.rev acc) | h :: t -> f h ++ (fun x -> loop (x::acc) t) in loop [] xs

type TrackErrorsBuilder() =
    member x.Bind(res,k) = res ++ k
    member x.Return(res) = ResultD(res)
    member x.ReturnFrom(res) = res
    member x.For(seq,k) = IterateD k seq
    member x.While(gd,k) = WhileD gd k
    member x.Zero()  = CompleteD

let trackErrors = TrackErrorsBuilder()
    
/// Stop on first error. Accumulate warnings and continue. 
let OptionD f xs = match xs with None -> CompleteD | Some(h) -> f h 

/// Stop on first error. Report index 
let IterateIdxD f xs = 
    let rec loop xs i = match xs with [] -> CompleteD | h :: t -> f i h ++ (fun () -> loop t (i+1))
    loop xs 0

/// Stop on first error. Accumulate warnings and continue. 
let rec Iterate2D f xs ys = 
    match xs,ys with 
    | [],[] -> CompleteD 
    | h1 :: t1, h2::t2 -> f h1 h2 ++ (fun () -> Iterate2D f t1 t2) 
    | _ -> failwith "Iterate2D"

let TryD f g = 
    match f() with
    | ErrorResult(warns,err) ->  (OkResult(warns,())) ++ (fun () -> g err)
    | res -> res

let rec RepeatWhileD ndeep body = body ndeep ++ (function true -> RepeatWhileD (ndeep+1) body | false -> CompleteD) 
let AtLeastOneD f l = MapD f l ++ (fun res -> ResultD (List.exists id res))


// Code below is for --flaterrors flag that is only used by the IDE

let stringThatIsAProxyForANewlineInFlatErrors = new System.String[|char 29 |]

let NewlineifyErrorString (message:string) = message.Replace(stringThatIsAProxyForANewlineInFlatErrors, Environment.NewLine)

/// fixes given string by replacing all control chars with spaces.
/// NOTE: newlines are recognized and replaced with stringThatIsAProxyForANewlineInFlatErrors (ASCII 29, the 'group separator'), 
/// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
let NormalizeErrorString (text : string) =    
    if text = null then nullArg "text"
    let text = text.Trim()

    let buf = System.Text.StringBuilder()
    let mutable i = 0
    while i < text.Length do
        let delta = 
            match text.[i] with
            | '\r' when i + 1 < text.Length && text.[i + 1] = '\n' ->
                // handle \r\n sequence - replace it with one single space
                buf.Append(stringThatIsAProxyForANewlineInFlatErrors) |> ignore
                2
            | '\n' ->
                buf.Append(stringThatIsAProxyForANewlineInFlatErrors) |> ignore
                1
            | c ->
                // handle remaining chars: control - replace with space, others - keep unchanged
                let c = if Char.IsControl(c) then ' ' else c
                buf.Append(c) |> ignore
                1
        i <- i + delta
    buf.ToString()