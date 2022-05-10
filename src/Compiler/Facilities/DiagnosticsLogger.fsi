// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.DiagnosticsLogger

open System
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Features
open FSharp.Compiler.Text

/// Represents the style being used to format errors
[<RequireQualifiedAccess>]
type DiagnosticStyle =
    | Default
    | Emacs
    | Test
    | VisualStudio
    | Gcc

/// Thrown when we want to add some range information to a .NET exception
exception WrappedError of exn * range

/// Thrown when immediate, local error recovery is not possible. This indicates
/// we've reported an error but need to make a non-local transfer of control.
/// Error recovery may catch this and continue (see 'errorRecovery')
///
/// The exception that caused the report is carried as data because in some
/// situations (LazyWithContext) we may need to re-report the original error
/// when a lazy thunk is re-evaluated.
exception ReportedError of exn option

val findOriginalException: err: exn -> exn

type Suggestions = (string -> unit) -> unit

val NoSuggestions: Suggestions

/// Thrown when we stop processing the F# Interactive entry or #load.
exception StopProcessingExn of exn option

val (|StopProcessing|_|): exn: exn -> unit option

val StopProcessing<'T> : exn

/// Represents a diagnostic exeption whose text comes via SR.*
exception DiagnosticWithText of number: int * message: string * range: range

/// Creates a diagnostic exeption whose text comes via SR.*
val Error: (int * string) * range -> exn

exception InternalError of message: string * range: range

exception UserCompilerMessage of message: string * number: int * range: range

exception LibraryUseOnly of range: range

exception Deprecated of message: string * range: range

exception Experimental of message: string * range: range

exception PossibleUnverifiableCode of range: range

exception UnresolvedReferenceNoRange of assemblyName: string

exception UnresolvedReferenceError of assemblyName: string * range: range

exception UnresolvedPathReferenceNoRange of assemblyName: string * path: string

exception UnresolvedPathReference of assemblyName: string * path: string * range: range

exception DiagnosticWithSuggestions of
    number: int *
    message: string *
    range: range *
    identifier: string *
    suggestions: Suggestions

/// Creates a DiagnosticWithSuggestions whose text comes via SR.*
val ErrorWithSuggestions: (int * string) * range * string * Suggestions -> exn

val inline protectAssemblyExploration: dflt: 'a -> f: (unit -> 'a) -> 'a

val inline protectAssemblyExplorationF: dflt: (string * string -> 'a) -> f: (unit -> 'a) -> 'a

val inline protectAssemblyExplorationNoReraise: dflt1: 'a -> dflt2: 'a -> f: (unit -> 'a) -> 'a

val AttachRange: m: range -> exn: exn -> exn

type Exiter =
    abstract member Exit: int -> 'T

val QuitProcessExiter: Exiter

/// Closed enumeration of build phases.
[<RequireQualifiedAccess>]
type BuildPhase =
    | DefaultPhase
    | Compile
    | Parameter
    | Parse
    | TypeCheck
    | CodeGen
    | Optimize
    | IlxGen
    | IlGen
    | Output
    | Interactive

/// Literal build phase subcategory strings.
module BuildPhaseSubcategory =
    [<Literal>]
    val DefaultPhase: string = ""

    [<Literal>]
    val Compile: string = "compile"

    [<Literal>]
    val Parameter: string = "parameter"

    [<Literal>]
    val Parse: string = "parse"

    [<Literal>]
    val TypeCheck: string = "typecheck"

    [<Literal>]
    val CodeGen: string = "codegen"

    [<Literal>]
    val Optimize: string = "optimize"

    [<Literal>]
    val IlxGen: string = "ilxgen"

    [<Literal>]
    val IlGen: string = "ilgen"

    [<Literal>]
    val Output: string = "output"

    [<Literal>]
    val Interactive: string = "interactive"

    [<Literal>]
    val Internal: string = "internal"

type PhasedDiagnostic =
    { Exception: exn
      Phase: BuildPhase }

    /// Construct a phased error
    static member Create: exn: exn * phase: BuildPhase -> PhasedDiagnostic

    /// Return true if the textual phase given is from the compile part of the build process.
    /// This set needs to be equal to the set of subcategories that the language service can produce.
    static member IsSubcategoryOfCompile: subcategory: string -> bool

    member DebugDisplay: unit -> string

    /// Return true if this phase is one that's known to be part of the 'compile'. This is the initial phase of the entire compilation that
    /// the language service knows about.
    member IsPhaseInCompile: unit -> bool

    /// This is the textual subcategory to display in error and warning messages (shows only under --vserrors):
    ///
    ///     file1.fs(72): subcategory warning FS0072: This is a warning message
    ///
    member Subcategory: unit -> string

[<AbstractClass>]
type DiagnosticsLogger =

    new: nameForDebugging: string -> DiagnosticsLogger

    member DebugDisplay: unit -> string

    abstract member DiagnosticSink: phasedError: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

    abstract member ErrorCount: int

val DiscardErrorsLogger: DiagnosticsLogger

val AssertFalseDiagnosticsLogger: DiagnosticsLogger

type CapturingDiagnosticsLogger =
    inherit DiagnosticsLogger

    new: nm: string -> CapturingDiagnosticsLogger

    member CommitDelayedDiagnostics: errorLogger: DiagnosticsLogger -> unit

    override DiagnosticSink: phasedError: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

    member Diagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list

    override ErrorCount: int

[<Class>]
type CompileThreadStatic =

    static member BuildPhase: BuildPhase with get, set

    static member BuildPhaseUnchecked: BuildPhase

    static member DiagnosticsLogger: DiagnosticsLogger with get, set

[<AutoOpen>]
module DiagnosticsLoggerExtensions =

    val tryAndDetectDev15: bool

    /// Instruct the exception not to reset itself when thrown again.
    val PreserveStackTrace: exn: 'a -> unit

    /// Reraise an exception if it is one we want to report to Watson.
    val ReraiseIfWatsonable: exn: exn -> unit

    type DiagnosticsLogger with

        member ErrorR: exn: exn -> unit

        member Warning: exn: exn -> unit

        member Error: exn: exn -> 'b

        member SimulateError: ph: PhasedDiagnostic -> 'a

        member ErrorRecovery: exn: exn -> m: range -> unit

        member StopProcessingRecovery: exn: exn -> m: range -> unit

        member ErrorRecoveryNoRange: exn: exn -> unit

/// NOTE: The change will be undone when the returned "unwind" object disposes
val PushThreadBuildPhaseUntilUnwind: phase: BuildPhase -> IDisposable

/// NOTE: The change will be undone when the returned "unwind" object disposes
val PushDiagnosticsLoggerPhaseUntilUnwind:
    errorLoggerTransformer: (DiagnosticsLogger -> #DiagnosticsLogger) -> IDisposable

val SetThreadBuildPhaseNoUnwind: phase: BuildPhase -> unit

val SetThreadDiagnosticsLoggerNoUnwind: errorLogger: DiagnosticsLogger -> unit

/// Reports an error diagnostic and continues
val errorR: exn: exn -> unit

/// Reports a warning diagnostic
val warning: exn: exn -> unit

/// Reports an error and raises a ReportedError exception
val error: exn: exn -> 'a

/// Reports an informational diagnostic
val informationalWarning: exn: exn -> unit

val simulateError: p: PhasedDiagnostic -> 'a

val diagnosticSink: phasedError: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

val errorSink: pe: PhasedDiagnostic -> unit

val warnSink: pe: PhasedDiagnostic -> unit

val errorRecovery: exn: exn -> m: range -> unit

val stopProcessingRecovery: exn: exn -> m: range -> unit

val errorRecoveryNoRange: exn: exn -> unit

val report: f: (unit -> 'a) -> 'a

val deprecatedWithError: s: string -> m: range -> unit

val libraryOnlyError: m: range -> unit

val libraryOnlyWarning: m: range -> unit

val deprecatedOperator: m: range -> unit

val mlCompatWarning: s: string -> m: range -> unit

val mlCompatError: s: string -> m: range -> unit

val suppressErrorReporting: f: (unit -> 'a) -> 'a

val conditionallySuppressErrorReporting: cond: bool -> f: (unit -> 'a) -> 'a

/// The result type of a computational modality to colelct warnings and possibly fail
[<NoEquality; NoComparison>]
type OperationResult<'T> =
    | OkResult of warnings: exn list * 'T
    | ErrorResult of warnings: exn list * exn

type ImperativeOperationResult = OperationResult<unit>

val ReportWarnings: warns: #exn list -> unit

val CommitOperationResult: res: OperationResult<'a> -> 'a

val RaiseOperationResult: res: OperationResult<unit> -> unit

val ErrorD: err: exn -> OperationResult<'a>

val WarnD: err: exn -> OperationResult<unit>

val CompleteD: OperationResult<unit>

val ResultD: x: 'a -> OperationResult<'a>

val CheckNoErrorsAndGetWarnings: res: OperationResult<'a> -> (exn list * 'a) option

val (++): res: OperationResult<'a> -> f: ('a -> OperationResult<'b>) -> OperationResult<'b>

/// Stop on first error. Accumulate warnings and continue.
val IterateD: f: ('a -> OperationResult<unit>) -> xs: 'a list -> OperationResult<unit>

val WhileD: gd: (unit -> bool) -> body: (unit -> OperationResult<unit>) -> OperationResult<unit>

val MapD: f: ('a -> OperationResult<'b>) -> xs: 'a list -> OperationResult<'b list>

type TrackErrorsBuilder =

    new: unit -> TrackErrorsBuilder

    member Bind: res: OperationResult<'h> * k: ('h -> OperationResult<'i>) -> OperationResult<'i>

    member Combine: expr1: OperationResult<'c> * expr2: ('c -> OperationResult<'d>) -> OperationResult<'d>

    member Delay: fn: (unit -> 'b) -> (unit -> 'b)

    member For: seq: 'e list * k: ('e -> OperationResult<unit>) -> OperationResult<unit>

    member Return: res: 'g -> OperationResult<'g>

    member ReturnFrom: res: 'f -> 'f

    member Run: fn: (unit -> 'a) -> 'a

    member While: gd: (unit -> bool) * k: (unit -> OperationResult<unit>) -> OperationResult<unit>

    member Zero: unit -> OperationResult<unit>

val trackErrors: TrackErrorsBuilder

val OptionD: f: ('a -> OperationResult<unit>) -> xs: 'a option -> OperationResult<unit>

val IterateIdxD: f: (int -> 'a -> OperationResult<unit>) -> xs: 'a list -> OperationResult<unit>

/// Stop on first error. Accumulate warnings and continue.
val Iterate2D: f: ('a -> 'b -> OperationResult<unit>) -> xs: 'a list -> ys: 'b list -> OperationResult<unit>

val TryD: f: (unit -> OperationResult<'a>) -> g: (exn -> OperationResult<'a>) -> OperationResult<'a>

val RepeatWhileD: nDeep: int -> body: (int -> OperationResult<bool>) -> OperationResult<unit>

val inline AtLeastOneD: f: ('a -> OperationResult<bool>) -> l: 'a list -> OperationResult<bool>

val inline AtLeastOne2D: f: ('a -> 'b -> OperationResult<bool>) -> xs: 'a list -> ys: 'b list -> OperationResult<bool>

val inline MapReduceD:
    mapper: ('a -> OperationResult<'b>) -> zero: 'b -> reducer: ('b -> 'b -> 'b) -> l: 'a list -> OperationResult<'b>

val inline MapReduce2D:
    mapper: ('a -> 'b -> OperationResult<'c>) ->
    zero: 'c ->
    reducer: ('c -> 'c -> 'c) ->
    xs: 'a list ->
    ys: 'b list ->
        OperationResult<'c>

module OperationResult =
    val inline ignore: res: OperationResult<'a> -> OperationResult<unit>

// For --flaterrors flag that is only used by the IDE
val stringThatIsAProxyForANewlineInFlatErrors: String

val NewlineifyErrorString: message: string -> string

/// fixes given string by replacing all control chars with spaces.
/// NOTE: newlines are recognized and replaced with stringThatIsAProxyForANewlineInFlatErrors (ASCII 29, the 'group separator'),
/// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
val NormalizeErrorString: text: string -> string

val checkLanguageFeatureError: langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> unit

val checkLanguageFeatureErrorRecover: langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> unit

val tryLanguageFeatureErrorOption:
    langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> exn option

val languageFeatureNotSupportedInLibraryError:
    langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> 'a

type StackGuard =
    new: maxDepth: int -> StackGuard

    /// Execute the new function, on a new thread if necessary
    member Guard: f: (unit -> 'T) -> 'T

    static member GetDepthOption: string -> int

/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope =
    new: errorLogger: DiagnosticsLogger * buildPhase: BuildPhase -> CompilationGlobalsScope

    interface IDisposable

    member DiagnosticsLogger: DiagnosticsLogger

    member BuildPhase: BuildPhase
