// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

open System
open FSharp.Compiler.Text
open FSharp.Compiler.DiagnosticsLogger

/// Represents a diagnostic produced by the F# compiler
[<Class>]
type public FSharpDiagnostic =

    /// Gets the file name for the diagnostic
    member FileName: string

    /// Gets the start position for the diagnostic
    member Start: Position

    /// Gets the end position for the diagnostic
    member End: Position

    /// Gets the start column for the diagnostic
    member StartColumn: int

    /// Gets the end column for the diagnostic
    member EndColumn: int

    /// Gets the start line for the diagnostic
    member StartLine: int

    /// Gets the end line for the diagnostic
    member EndLine: int

    /// Gets the range for the diagnostic
    member Range: range

    /// Gets the severity for the diagnostic
    member Severity: FSharpDiagnosticSeverity

    /// Gets the message for the diagnostic
    member Message: string

    /// Gets the sub-category for the diagnostic
    member Subcategory: string

    /// Gets the number for the diagnostic
    member ErrorNumber: int

    /// Gets the number prefix for the diagnostic, usually "FS" but may differ for analyzers
    member ErrorNumberPrefix: string

    /// Gets the full error number text e.g "FS0031"
    member ErrorNumberText: string

    /// Creates a diagnostic, e.g. for reporting from an analyzer
    static member Create:
        severity: FSharpDiagnosticSeverity *
        message: string *
        number: int *
        range: range *
        ?numberPrefix: string *
        ?subcategory: string ->
            FSharpDiagnostic

    static member internal CreateFromExceptionAndAdjustEof:
        diagnostic: PhasedDiagnostic *
        severity: FSharpDiagnosticSeverity *
        range *
        lastPosInFile: (int * int) *
        suggestNames: bool ->
            FSharpDiagnostic

    static member internal CreateFromException:
        diagnostic: PhasedDiagnostic * severity: FSharpDiagnosticSeverity * range * suggestNames: bool ->
            FSharpDiagnostic

    /// Newlines are recognized and replaced with (ASCII 29, the 'group separator'),
    /// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
    static member NewlineifyErrorString: message: string -> string

    /// Newlines are recognized and replaced with (ASCII 29, the 'group separator'),
    /// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
    static member NormalizeErrorString: text: string -> string

//----------------------------------------------------------------------------
// Internal only

// Implementation details used by other code in the compiler
[<Sealed>]
type internal DiagnosticsScope =

    interface IDisposable

    new: unit -> DiagnosticsScope

    member Diagnostics: FSharpDiagnostic list

    static member Protect<'T> : range -> (unit -> 'T) -> (string -> 'T) -> 'T

/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationDiagnosticLogger =
    inherit DiagnosticsLogger

    /// Create the diagnostics logger
    new:
        debugName: string * options: FSharpDiagnosticOptions * ?preprocess: (PhasedDiagnostic -> PhasedDiagnostic) ->
            CompilationDiagnosticLogger

    /// Get the captured diagnostics
    member GetDiagnostics: unit -> (PhasedDiagnostic * FSharpDiagnosticSeverity)[]

module internal DiagnosticHelpers =

    val ReportDiagnostic:
        FSharpDiagnosticOptions *
        allErrors: bool *
        mainInputFileName: string *
        fileInfo: (int * int) *
        diagnostic: PhasedDiagnostic *
        severity: FSharpDiagnosticSeverity *
        suggestNames: bool ->
            FSharpDiagnostic list

    val CreateDiagnostics:
        FSharpDiagnosticOptions *
        allErrors: bool *
        mainInputFileName: string *
        seq<PhasedDiagnostic * FSharpDiagnosticSeverity> *
        suggestNames: bool ->
            FSharpDiagnostic[]
