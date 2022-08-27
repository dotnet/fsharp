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

val inline protectAssemblyExploration: dflt: 'T -> f: (unit -> 'T) -> 'T

val inline protectAssemblyExplorationF: dflt: (string * string -> 'T) -> f: (unit -> 'T) -> 'T

val inline protectAssemblyExplorationNoReraise: dflt1: 'T -> dflt2: 'T -> f: (unit -> 'T) -> 'T

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

    abstract member DiagnosticSink: diagnostic: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

    abstract member ErrorCount: int

val DiscardErrorsLogger: DiagnosticsLogger

val AssertFalseDiagnosticsLogger: DiagnosticsLogger

type CapturingDiagnosticsLogger =
    inherit DiagnosticsLogger

    new: nm: string -> CapturingDiagnosticsLogger

    member CommitDelayedDiagnostics: diagnosticsLogger: DiagnosticsLogger -> unit

    override DiagnosticSink: diagnostic: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

    member Diagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity) list

    override ErrorCount: int

[<Class>]
type DiagnosticsThreadStatics =

    static member BuildPhase: BuildPhase with get, set

    static member BuildPhaseUnchecked: BuildPhase

    static member DiagnosticsLogger: DiagnosticsLogger with get, set

[<AutoOpen>]
module DiagnosticsLoggerExtensions =

    val tryAndDetectDev15: bool

    /// Instruct the exception not to reset itself when thrown again.
    val PreserveStackTrace: exn: 'T -> unit

    /// Reraise an exception if it is one we want to report to Watson.
    val ReraiseIfWatsonable: exn: exn -> unit

    type DiagnosticsLogger with

        member ErrorR: exn: exn -> unit

        member Warning: exn: exn -> unit

        member Error: exn: exn -> 'T

        member SimulateError: diagnostic: PhasedDiagnostic -> 'T

        member ErrorRecovery: exn: exn -> m: range -> unit

        member StopProcessingRecovery: exn: exn -> m: range -> unit

        member ErrorRecoveryNoRange: exn: exn -> unit

/// NOTE: The change will be undone when the returned "unwind" object disposes
val PushThreadBuildPhaseUntilUnwind: phase: BuildPhase -> IDisposable

/// NOTE: The change will be undone when the returned "unwind" object disposes
val PushDiagnosticsLoggerPhaseUntilUnwind:
    diagnosticsLoggerTransformer: (DiagnosticsLogger -> #DiagnosticsLogger) -> IDisposable

val SetThreadBuildPhaseNoUnwind: phase: BuildPhase -> unit

val SetThreadDiagnosticsLoggerNoUnwind: diagnosticsLogger: DiagnosticsLogger -> unit

/// Reports an error diagnostic and continues
val errorR: exn: exn -> unit

/// Reports a warning diagnostic
val warning: exn: exn -> unit

/// Reports an error and raises a ReportedError exception
val error: exn: exn -> 'T

/// Reports an informational diagnostic
val informationalWarning: exn: exn -> unit

val simulateError: diagnostic: PhasedDiagnostic -> 'T

val diagnosticSink: diagnostic: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

val errorSink: diagnostic: PhasedDiagnostic -> unit

val warnSink: diagnostic: PhasedDiagnostic -> unit

val errorRecovery: exn: exn -> m: range -> unit

val stopProcessingRecovery: exn: exn -> m: range -> unit

val errorRecoveryNoRange: exn: exn -> unit

val deprecatedWithError: s: string -> m: range -> unit

val libraryOnlyError: m: range -> unit

val libraryOnlyWarning: m: range -> unit

val deprecatedOperator: m: range -> unit

val mlCompatWarning: s: string -> m: range -> unit

val mlCompatError: s: string -> m: range -> unit

val suppressErrorReporting: f: (unit -> 'T) -> 'T

val conditionallySuppressErrorReporting: cond: bool -> f: (unit -> 'T) -> 'T

/// The result type of a computational modality to collect warnings and possibly fail
[<NoEquality; NoComparison>]
type OperationResult<'T> =
    | OkResult of warnings: exn list * result: 'T
    | ErrorResult of warnings: exn list * error: exn

type ImperativeOperationResult = OperationResult<unit>

val ReportWarnings: warns: #exn list -> unit

val CommitOperationResult: res: OperationResult<'T> -> 'T

val RaiseOperationResult: res: OperationResult<unit> -> unit

val ErrorD: err: exn -> OperationResult<'T>

val WarnD: err: exn -> OperationResult<unit>

val CompleteD: OperationResult<unit>

val ResultD: x: 'T -> OperationResult<'T>

val CheckNoErrorsAndGetWarnings: res: OperationResult<'T> -> (exn list * 'T) option

val (++): res: OperationResult<'T> -> f: ('T -> OperationResult<'b>) -> OperationResult<'b>

/// Stop on first error. Accumulate warnings and continue.
val IterateD: f: ('T -> OperationResult<unit>) -> xs: 'T list -> OperationResult<unit>

val WhileD: gd: (unit -> bool) -> body: (unit -> OperationResult<unit>) -> OperationResult<unit>

val MapD: f: ('T -> OperationResult<'b>) -> xs: 'T list -> OperationResult<'b list>

type TrackErrorsBuilder =

    new: unit -> TrackErrorsBuilder

    member Bind: res: OperationResult<'h> * k: ('h -> OperationResult<'i>) -> OperationResult<'i>

    member Combine: expr1: OperationResult<'c> * expr2: ('c -> OperationResult<'d>) -> OperationResult<'d>

    member Delay: fn: (unit -> 'b) -> (unit -> 'b)

    member For: seq: 'e list * k: ('e -> OperationResult<unit>) -> OperationResult<unit>

    member Return: res: 'g -> OperationResult<'g>

    member ReturnFrom: res: 'f -> 'f

    member Run: fn: (unit -> 'T) -> 'T

    member While: gd: (unit -> bool) * k: (unit -> OperationResult<unit>) -> OperationResult<unit>

    member Zero: unit -> OperationResult<unit>

val trackErrors: TrackErrorsBuilder

val OptionD: f: ('T -> OperationResult<unit>) -> xs: 'T option -> OperationResult<unit>

val IterateIdxD: f: (int -> 'T -> OperationResult<unit>) -> xs: 'T list -> OperationResult<unit>

/// Stop on first error. Accumulate warnings and continue.
val Iterate2D: f: ('T -> 'b -> OperationResult<unit>) -> xs: 'T list -> ys: 'b list -> OperationResult<unit>

val TryD: f: (unit -> OperationResult<'T>) -> g: (exn -> OperationResult<'T>) -> OperationResult<'T>

val RepeatWhileD: nDeep: int -> body: (int -> OperationResult<bool>) -> OperationResult<unit>

val inline AtLeastOneD: f: ('T -> OperationResult<bool>) -> l: 'T list -> OperationResult<bool>

val inline AtLeastOne2D: f: ('T -> 'b -> OperationResult<bool>) -> xs: 'T list -> ys: 'b list -> OperationResult<bool>

val inline MapReduceD:
    mapper: ('T -> OperationResult<'b>) -> zero: 'b -> reducer: ('b -> 'b -> 'b) -> l: 'T list -> OperationResult<'b>

val inline MapReduce2D:
    mapper: ('T -> 'T2 -> OperationResult<'c>) ->
    zero: 'c ->
    reducer: ('c -> 'c -> 'c) ->
    xs: 'T list ->
    ys: 'T2 list ->
        OperationResult<'c>

module OperationResult =
    val inline ignore: res: OperationResult<'T> -> OperationResult<unit>

// For --flaterrors flag that is only used by the IDE
val stringThatIsAProxyForANewlineInFlatErrors: String

val NewlineifyErrorString: message: string -> string

/// fixes given string by replacing all control chars with spaces.
/// NOTE: newlines are recognized and replaced with stringThatIsAProxyForANewlineInFlatErrors (ASCII 29, the 'group separator'),
/// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
val NormalizeErrorString: text: string -> string

val checkLanguageFeatureError: langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> unit

val checkLanguageFeatureAndRecover: langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> unit

val tryLanguageFeatureErrorOption:
    langVersion: LanguageVersion -> langFeature: LanguageFeature -> m: range -> exn option

val languageFeatureNotSupportedInLibraryError:
    langFeature: LanguageFeature -> m: range -> 'T

type StackGuard =
    new: maxDepth: int -> StackGuard

    /// Execute the new function, on a new thread if necessary
    member Guard: f: (unit -> 'T) -> 'T

    static member GetDepthOption: string -> int

/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope =
    new: diagnosticsLogger: DiagnosticsLogger * buildPhase: BuildPhase -> CompilationGlobalsScope

    interface IDisposable

    member DiagnosticsLogger: DiagnosticsLogger

    member BuildPhase: BuildPhase
