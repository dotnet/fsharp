// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open System
open FSharp.Compiler 
open FSharp.Compiler.Range
open FSharp.Compiler.TcGlobals 
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.ErrorLogger

[<RequireQualifiedAccess>]
type public FSharpErrorSeverity = 
    | Warning 
    | Error

/// Object model for diagnostics
[<Class>]
type public FSharpErrorInfo = 
    member FileName: string
    member Start: pos
    member End: pos
    member StartLineAlternate: int
    member EndLineAlternate: int
    member StartColumn: int
    member EndColumn: int

    member Range: range
    member Severity: FSharpErrorSeverity
    member Message: string
    member Subcategory: string
    member ErrorNumber: int

    static member internal CreateFromExceptionAndAdjustEof: PhasedDiagnostic * isError: bool * range * lastPosInFile: (int*int) * suggestNames: bool -> FSharpErrorInfo
    static member internal CreateFromException: PhasedDiagnostic * isError: bool * range * suggestNames: bool -> FSharpErrorInfo

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type public FSharpXmlDoc =
    /// No documentation is available
    | None

    /// The text for documentation 
    | Text of string

    /// Indicates that the text for the documentation can be found in a .xml documentation file, using the given signature key
    | XmlDocFileSignature of (*File:*) string * (*Signature:*)string

type public Layout = Internal.Utilities.StructuredFormat.Layout

/// A single data tip display element
[<RequireQualifiedAccess>]
type public FSharpToolTipElementData<'T> = 
    {
      MainDescription:  'T 

      XmlDoc: FSharpXmlDoc

      /// typar instantiation text, to go after xml
      TypeMapping: 'T list

      /// Extra text, goes at the end
      Remarks: 'T option

      /// Parameter name
      ParamName : string option
    }

/// A single tool tip display element
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type public FSharpToolTipElement<'T> = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of string
    static member Single : 'T * FSharpXmlDoc * ?typeMapping: 'T list * ?paramName: string * ?remarks : 'T  -> FSharpToolTipElement<'T>

/// A single data tip display element with where text is expressed as string
type public FSharpToolTipElement = FSharpToolTipElement<string>

/// A single data tip display element with where text is expressed as <see cref="Layout"/>
type public FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>

/// Information for building a tool tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
type public FSharpToolTipText<'T> = 

    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement<'T> list  

type public FSharpToolTipText = FSharpToolTipText<string>

type public FSharpStructuredToolTipText = FSharpToolTipText<Layout>

[<RequireQualifiedAccess>]
type public CompletionItemKind =
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | CustomOperation
    | Other

type UnresolvedSymbol =
    {
      FullName: string

      DisplayName: string

      Namespace: string[]
    }

type internal CompletionItem =
    {
      ItemWithInst: ItemWithInst

      Kind: CompletionItemKind

      IsOwnMember: bool

      MinorPriority: int

      Type: TyconRef option 

      Unresolved: UnresolvedSymbol option
    }
    member Item : Item

module public Tooltips =

    val ToFSharpToolTipElement: FSharpStructuredToolTipElement -> FSharpToolTipElement

    val ToFSharpToolTipText: FSharpStructuredToolTipText -> FSharpToolTipText

    val Map: f: ('T1 -> 'T2) -> a: Async<'T1> -> Async<'T2>

// Implementation details used by other code in the compiler    
module internal SymbolHelpers = 

    val isFunction : TcGlobals -> TType -> bool

    val ParamNameAndTypesOfUnaryCustomOperation : TcGlobals -> MethInfo -> ParamNameAndType list

    val GetXmlDocSigOfEntityRef : InfoReader -> range -> EntityRef -> (string option * string) option

    val GetXmlDocSigOfScopedValRef : TcGlobals -> TyconRef -> ValRef -> (string option * string) option

    val GetXmlDocSigOfILFieldInfo : InfoReader -> range -> ILFieldInfo -> (string option * string) option

    val GetXmlDocSigOfRecdFieldInfo : RecdFieldInfo -> (string option * string) option

    val GetXmlDocSigOfUnionCaseInfo : UnionCaseInfo -> (string option * string) option

    val GetXmlDocSigOfMethInfo : InfoReader -> range -> MethInfo -> (string option * string) option

    val GetXmlDocSigOfValRef : TcGlobals -> ValRef -> (string option * string) option

    val GetXmlDocSigOfProp : InfoReader -> range -> PropInfo -> (string option * string) option

    val GetXmlDocSigOfEvent : InfoReader -> range -> EventInfo -> (string option * string) option

    val GetXmlCommentForItem : InfoReader -> range -> Item -> FSharpXmlDoc

    val FormatStructuredDescriptionOfItem : isDecl:bool -> InfoReader -> range -> DisplayEnv -> ItemWithInst -> FSharpStructuredToolTipElement

    val RemoveDuplicateItems : TcGlobals -> ItemWithInst list -> ItemWithInst list

    val RemoveExplicitlySuppressed : TcGlobals -> ItemWithInst list -> ItemWithInst list

    val RemoveDuplicateCompletionItems : TcGlobals -> CompletionItem list -> CompletionItem list

    val RemoveExplicitlySuppressedCompletionItems : TcGlobals -> CompletionItem list -> CompletionItem list

    val GetF1Keyword : TcGlobals -> Item -> string option

    val rangeOfItem : TcGlobals -> bool option -> Item -> range option

    val fileNameOfItem : TcGlobals -> string option -> range -> Item -> string

    val FullNameOfItem : TcGlobals -> Item -> string

    val ccuOfItem : TcGlobals -> Item -> CcuThunk option

    val mutable ToolTipFault : string option

    val IsAttribute : InfoReader -> Item -> bool

    val IsExplicitlySuppressed : TcGlobals -> Item -> bool

    val FlattenItems : TcGlobals -> range -> Item -> Item list

#if !NO_EXTENSIONTYPING
    val (|ItemIsProvidedType|_|) : TcGlobals -> Item -> TyconRef option

    val (|ItemIsWithStaticArguments|_|): range -> TcGlobals -> Item -> Tainted<ExtensionTyping.ProvidedParameterInfo>[] option

    val (|ItemIsProvidedTypeWithStaticArguments|_|): range -> TcGlobals -> Item -> Tainted<ExtensionTyping.ProvidedParameterInfo>[] option
#endif

    val SimplerDisplayEnv : DisplayEnv -> DisplayEnv

//----------------------------------------------------------------------------
// Internal only

// Implementation details used by other code in the compiler    
[<Sealed>]
type internal ErrorScope = 
    interface IDisposable
    new : unit -> ErrorScope
    member Diagnostics : FSharpErrorInfo list
    static member Protect<'a> : range -> (unit->'a) -> (string->'a) -> 'a

/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationErrorLogger = 
    inherit ErrorLogger

    /// Create the error logger
    new: debugName:string * options: FSharpErrorSeverityOptions -> CompilationErrorLogger
            
    /// Get the captured errors
    member GetErrors: unit -> (PhasedDiagnostic * FSharpErrorSeverity)[]

/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type internal CompilationGlobalsScope =
    new : ErrorLogger * BuildPhase -> CompilationGlobalsScope
    interface IDisposable

module internal ErrorHelpers = 
    val ReportError: FSharpErrorSeverityOptions * allErrors: bool * mainInputFileName: string * fileInfo: (int * int) * (PhasedDiagnostic * FSharpErrorSeverity) * suggestNames: bool -> FSharpErrorInfo list
    val CreateErrorInfos: FSharpErrorSeverityOptions * allErrors: bool * mainInputFileName: string * seq<(PhasedDiagnostic * FSharpErrorSeverity)> * suggestNames: bool -> FSharpErrorInfo[]
