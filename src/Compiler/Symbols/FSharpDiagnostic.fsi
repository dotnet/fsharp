// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

open System
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.DiagnosticsLogger

[<RequireQualifiedAccess; Experimental("This FCS API is experimental and subject to change.")>]
type public DiagnosticContextInfo =
    /// No context was given.
    | NoContext
    /// The type equation comes from an IF expression.
    | IfExpression
    /// The type equation comes from an omitted else branch.
    | OmittedElseBranch
    /// The type equation comes from a type check of the result of an else branch.
    | ElseBranchResult
    /// The type equation comes from the verification of record fields.
    | RecordFields
    /// The type equation comes from the verification of a tuple in record fields.
    | TupleInRecordFields
    /// The type equation comes from a list or array constructor
    | CollectionElement
    /// The type equation comes from a return in a computation expression.
    | ReturnInComputationExpression
    /// The type equation comes from a yield in a computation expression.
    | YieldInComputationExpression
    /// The type equation comes from a runtime type test.
    | RuntimeTypeTest
    /// The type equation comes from an downcast where a upcast could be used.
    | DowncastUsedInsteadOfUpcast
    /// The type equation comes from a return type of a pattern match clause (not the first clause).
    | FollowingPatternMatchClause
    /// The type equation comes from a pattern match guard.
    | PatternMatchGuard
    /// The type equation comes from a sequence expression.
    | SequenceExpression

[<Interface; Experimental("This FCS API is experimental and subject to change.")>]
type public IFSharpDiagnosticExtendedData =
    interface
    end

[<Class; Experimental("This FCS API is experimental and subject to change.")>]
type public TypeMismatchDiagnosticExtendedData =
    interface IFSharpDiagnosticExtendedData
    member ExpectedType: FSharpType
    member ActualType: FSharpType
    member ContextInfo: DiagnosticContextInfo
    member DisplayContext: FSharpDisplayContext

[<Class; Experimental("This FCS API is experimental and subject to change.")>]
type public ExpressionIsAFunctionExtendedData =
    interface IFSharpDiagnosticExtendedData
    member ActualType: FSharpType

[<Class; Experimental("This FCS API is experimental and subject to change.")>]
type public FieldNotContainedDiagnosticExtendedData =
    interface IFSharpDiagnosticExtendedData
    member SignatureField: FSharpField
    member ImplementationField: FSharpField

[<Class; Experimental("This FCS API is experimental and subject to change.")>]
type public ValueNotContainedDiagnosticExtendedData =
    interface IFSharpDiagnosticExtendedData
    member SignatureValue: FSharpMemberOrFunctionOrValue
    member ImplementationValue: FSharpMemberOrFunctionOrValue

[<Class; Experimental("This FCS API is experimental and subject to change.")>]
type ArgumentsInSigAndImplMismatchExtendedData =
    interface IFSharpDiagnosticExtendedData
    member SignatureName: string
    member ImplementationName: string
    member SignatureRange: range
    member ImplementationRange: range

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

    [<Experimental("This FCS API is experimental and subject to change.")>]
    member ExtendedData: IFSharpDiagnosticExtendedData option

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
        suggestNames: bool *
        flatErrors: bool *
        symbolEnv: SymbolEnv option ->
            FSharpDiagnostic

    static member internal CreateFromException:
        diagnostic: PhasedDiagnostic *
        severity: FSharpDiagnosticSeverity *
        range *
        suggestNames: bool *
        flatErrors: bool *
        symbolEnv: SymbolEnv option ->
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

    new: bool -> DiagnosticsScope

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
        suggestNames: bool *
        flatErrors: bool *
        symbolEnv: SymbolEnv option ->
            FSharpDiagnostic list

    val CreateDiagnostics:
        FSharpDiagnosticOptions *
        allErrors: bool *
        mainInputFileName: string *
        seq<PhasedDiagnostic * FSharpDiagnosticSeverity> *
        suggestNames: bool *
        flatErrors: bool *
        symbolEnv: SymbolEnv option ->
            FSharpDiagnostic[]
