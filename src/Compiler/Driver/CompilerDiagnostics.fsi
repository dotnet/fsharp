// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Contains logic to prepare, post-process, filter and emit compiler diagnsotics
module internal FSharp.Compiler.CompilerDiagnostics

open System.Text
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

#if DEBUG
module internal CompilerService =
    val showAssertForUnexpectedException: bool ref

/// For extra diagnostics
val mutable showParserStackOnParseError: bool
#endif

/// This exception is an old-style way of reporting a diagnostic
exception HashIncludeNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception HashReferenceNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception HashLoadedSourceHasIssues of informationals: exn list * warnings: exn list * errors: exn list * range

/// This exception is an old-style way of reporting a diagnostic
exception HashLoadedScriptConsideredSource of range

/// This exception is an old-style way of reporting a diagnostic
exception HashDirectiveNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionFull of string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionForHtmlDoc of string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionSuggestAlternative of string * string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionNoDescription of string * range

/// This exception is an old-style way of reporting a diagnostic
exception InternalCommandLineOption of string * range

type PhasedDiagnostic with

    /// Get the location associated with a diagnostic
    member Range: range option

    /// Get the number associated with a diagnostic
    member Number: int

    /// Eagerly format a PhasedDiagnostic return as a new PhasedDiagnostic requiring no formatting of types.
    member EagerlyFormatCore: flattenErrors: bool * suggestNames: bool -> PhasedDiagnostic

    /// Format the core of the diagnostic as a string. Doesn't include the range information.
    member FormatCore: flattenErrors: bool * suggestNames: bool -> string

    /// Indicates if we should report a diagnostic as a warning
    member ReportAsInfo: FSharpDiagnosticOptions * FSharpDiagnosticSeverity -> bool

    /// Indicates if we should report a diagnostic as a warning
    member ReportAsWarning: FSharpDiagnosticOptions * FSharpDiagnosticSeverity -> bool

    /// Indicates if we should report a warning as an error
    member ReportAsError: FSharpDiagnosticOptions * FSharpDiagnosticSeverity -> bool

    /// Output all of a diagnostic to a buffer, including range
    member Output:
        buf: StringBuilder *
        tcConfig: TcConfig *
        severity: FSharpDiagnosticSeverity ->
            unit

    /// Write extra context information for a diagnostic
    member WriteWithContext:
        os: System.IO.TextWriter *
        prefix: string *
        fileLineFunction: (string -> int -> string) *
        tcConfig: TcConfig *
        severity: FSharpDiagnosticSeverity ->
            unit

/// Get an error logger that filters the reporting of warnings based on scoped pragma information
val GetDiagnosticsLoggerFilteringByScopedPragmas:
    checkFile: bool *
    scopedPragmas: ScopedPragma list *
    diagnosticOptions: FSharpDiagnosticOptions *
    diagnosticsLogger: DiagnosticsLogger ->
        DiagnosticsLogger

val SanitizeFileName: fileName: string -> implicitIncludeDir: string -> string

/// Used internally and in LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type FormattedDiagnosticLocation =
    { Range: range
      File: string
      TextRepresentation: string
      IsEmpty: bool }

/// Used internally and in LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type FormattedDiagnosticCanonicalInformation =
    { ErrorNumber: int
      Subcategory: string
      TextRepresentation: string }

/// Used internally and in LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type FormattedDiagnosticDetailedInfo =
    { Location: FormattedDiagnosticLocation option
      Canonical: FormattedDiagnosticCanonicalInformation
      Message: string }

/// Used internally and in LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type FormattedDiagnostic =
    | Short of FSharpDiagnosticSeverity * string
    | Long of FSharpDiagnosticSeverity * FormattedDiagnosticDetailedInfo

/// Used internally and in LegacyHostedCompilerForTesting
val CollectFormattedDiagnostics:
    tcConfig: TcConfig *
    severity: FSharpDiagnosticSeverity *
    PhasedDiagnostic *
    suggestNames: bool ->
        FormattedDiagnostic[]
