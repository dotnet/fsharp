// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.ErrorLogger

//----------------------------------------------------------------------------
// Object model for diagnostics


[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type FSharpErrorSeverity = 
#else
type internal FSharpErrorSeverity = 
#endif
| Warning 
    | Error

[<Class>]
#if COMPILER_PUBLIC_API
type FSharpErrorInfo = 
#else
type internal FSharpErrorInfo = 
#endif
    member FileName: string
    member StartLineAlternate:int
    member EndLineAlternate:int
    member StartColumn:int
    member EndColumn:int
    member Severity:FSharpErrorSeverity
    member Message:string
    member Subcategory:string
    member ErrorNumber:int
    static member internal CreateFromExceptionAndAdjustEof : PhasedDiagnostic * isError: bool * range * lastPosInFile:(int*int) -> FSharpErrorInfo
    static member internal CreateFromException : PhasedDiagnostic * isError: bool * range -> FSharpErrorInfo

//----------------------------------------------------------------------------
// Object model for quick info

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type FSharpXmlDoc =
#else
type internal FSharpXmlDoc =
#endif
    /// No documentation is available
    | None

    /// The text for documentation 
    | Text of string

    /// Indicates that the text for the documentation can be found in a .xml documentation file, using the given signature key
    | XmlDocFileSignature of (*File:*) string * (*Signature:*)string

#if COMPILER_PUBLIC_API
type Layout = Internal.Utilities.StructuredFormat.Layout
#else
type internal Layout = Internal.Utilities.StructuredFormat.Layout
#endif

/// A single data tip display element
[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type FSharpToolTipElementData<'T> = 
#else
type internal FSharpToolTipElementData<'T> = 
#endif
    { MainDescription:  'T 
      XmlDoc: FSharpXmlDoc
      /// typar insantiation text, to go after xml
      TypeMapping: 'T list
      /// Extra text, goes at the end
      Remarks: 'T option
      /// Parameter name
      ParamName : string option }

/// A single tool tip display element
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type FSharpToolTipElement<'T> = 
#else
type internal FSharpToolTipElement<'T> = 
#endif
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of string
    static member Single : 'T * FSharpXmlDoc * ?typeMapping: 'T list * ?paramName: string * ?remarks : 'T  -> FSharpToolTipElement<'T>

/// A single data tip display element with where text is expressed as string
#if COMPILER_PUBLIC_API
type FSharpToolTipElement = FSharpToolTipElement<string>
#else
type internal FSharpToolTipElement = FSharpToolTipElement<string>
#endif


/// A single data tip display element with where text is expressed as <see cref="Layout"/>
#if COMPILER_PUBLIC_API
type FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>
#else
type internal FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>
#endif

/// Information for building a tool tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
#if COMPILER_PUBLIC_API
type FSharpToolTipText<'T> = 
#else
type internal FSharpToolTipText<'T> = 
#endif
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement<'T> list  

#if COMPILER_PUBLIC_API
type FSharpToolTipText = FSharpToolTipText<string>
type FSharpStructuredToolTipText = FSharpToolTipText<Layout>
#else
type internal FSharpToolTipText = FSharpToolTipText<string>
type internal FSharpStructuredToolTipText = FSharpToolTipText<Layout>
#endif

//----------------------------------------------------------------------------
// Object model for completion list entries (one of several in the API...)


[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type CompletionItemKind =
#else
type internal CompletionItemKind =
#endif
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | Other

type internal UnresolvedSymbol =
    { DisplayName: string
      Namespace: string[] }

type internal CompletionItem =
    { ItemWithInst: ItemWithInst
      Kind: CompletionItemKind
      IsOwnMember: bool
      MinorPriority: int
      Type: TyconRef option 
      Unresolved: UnresolvedSymbol option }
    member Item : Item

#if COMPILER_PUBLIC_API
module Tooltips =
#else
module internal Tooltips =
#endif
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
    member GetErrors: unit -> (PhasedDiagnostic * FSharpErrorSeverity) list

/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type internal CompilationGlobalsScope =
    new : ErrorLogger * BuildPhase -> CompilationGlobalsScope
    interface IDisposable

module internal ErrorHelpers = 
    val ReportError: FSharpErrorSeverityOptions * allErrors: bool * mainInputFileName: string * fileInfo: (int * int) * (PhasedDiagnostic * FSharpErrorSeverity) -> FSharpErrorInfo list
    val CreateErrorInfos: FSharpErrorSeverityOptions * allErrors: bool * mainInputFileName: string * seq<(PhasedDiagnostic * FSharpErrorSeverity)> -> FSharpErrorInfo[]
