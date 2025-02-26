// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

open System
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.DiagnosticsLogger

module public ExtendedData =
    /// Information about the context of a type equation in type-mismatch-like diagnostic
    [<RequireQualifiedAccess; Experimental("This FCS API is experimental and subject to change.")>]
    type public DiagnosticContextInfo =
        /// No context was given
        | NoContext
        /// The type equation comes from an IF expression
        | IfExpression
        /// The type equation comes from an omitted else branch
        | OmittedElseBranch
        /// The type equation comes from a type check of the result of an else branch
        | ElseBranchResult
        /// The type equation comes from the verification of record fields
        | RecordFields
        /// The type equation comes from the verification of a tuple in record fields
        | TupleInRecordFields
        /// The type equation comes from a list or array constructor
        | CollectionElement
        /// The type equation comes from a return in a computation expression
        | ReturnInComputationExpression
        /// The type equation comes from a yield in a computation expression
        | YieldInComputationExpression
        /// The type equation comes from a runtime type test
        | RuntimeTypeTest
        /// The type equation comes from an downcast where a upcast could be used
        | DowncastUsedInsteadOfUpcast
        /// The type equation comes from a return type of a pattern match clause (not the first clause)
        | FollowingPatternMatchClause
        /// The type equation comes from a pattern match guard
        | PatternMatchGuard
        /// The type equation comes from a sequence expression
        | SequenceExpression

    /// Contextually-relevant data to each particular diagnostic
    [<Interface; Experimental("This FCS API is experimental and subject to change.")>]
    type public IFSharpDiagnosticExtendedData =
        interface
        end

    /// Additional data for diagnostics about obsolete attributes.
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public ObsoleteDiagnosticExtendedData =
        interface IFSharpDiagnosticExtendedData

        /// Represents the DiagnosticId of the diagnostic
        member DiagnosticId: string option

        /// Represents the URL format of the diagnostic
        member UrlFormat: string option

    /// Additional data for diagnostics about experimental attributes.
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public ExperimentalExtendedData =
        interface IFSharpDiagnosticExtendedData

        /// Represents the DiagnosticId of the diagnostic
        member DiagnosticId: string option

        /// Represents the URL format of the diagnostic
        member UrlFormat: string option

    /// Additional data for type-mismatch-like (usually with ErrorNumber = 1) diagnostics
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public TypeMismatchDiagnosticExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Represents F# type expected in the current context
        member ExpectedType: FSharpType
        /// Represents F# type type actual in the current context
        member ActualType: FSharpType
        /// The context in which the type mismatch was found
        member ContextInfo: DiagnosticContextInfo
        /// Represents the information needed to format types
        member DisplayContext: FSharpDisplayContext

    /// Additional data for 'This expression is a function value, i.e. is missing arguments' diagnostic
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public ExpressionIsAFunctionExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Represents F# type of the expression
        member ActualType: FSharpType

    /// Additional data for diagnostics about a field whose declarations differ in signature and implementation
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public FieldNotContainedDiagnosticExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Represents F# field in signature file
        member SignatureField: FSharpField
        /// Represents F# field in implementation file
        member ImplementationField: FSharpField

    /// Additional data for diagnostics about a value whose declarations differ in signature and implementation
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type public ValueNotContainedDiagnosticExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Represents F# value in signature file
        member SignatureValue: FSharpMemberOrFunctionOrValue
        /// Represents F# value in implementation file
        member ImplementationValue: FSharpMemberOrFunctionOrValue

    /// Additional data for 'argument names in the signature and implementation do not match' diagnostic
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type ArgumentsInSigAndImplMismatchExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Argument name in signature file
        member SignatureName: string
        /// Argument name in implementation file
        member ImplementationName: string
        /// Argument identifier range within signature file
        member SignatureRange: range
        /// Argument identifier range within implementation file
        member ImplementationRange: range

    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferExtendedData =
        interface IFSharpDiagnosticExtendedData
        /// Range of the signature type identifier.
        member SignatureRange: range
        /// Range of the implementation type identifier.
        member ImplementationRange: range

open ExtendedData

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

    /// Gets the contextually-relevant data to each particular diagnostic for things like code fixes
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
