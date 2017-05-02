// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type internal FSharpXmlDoc =
    /// No documentation is available
    | None

    /// The text for documentation 
    | Text of string

    /// Indicates that the text for the documentation can be found in a .xml documentation file, using the given signature key
    | XmlDocFileSignature of (*File:*) string * (*Signature:*)string

type internal Layout = Internal.Utilities.StructuredFormat.Layout

/// A single data tip display element
[<RequireQualifiedAccess>]
type FSharpToolTipElementData<'T> = 
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
type internal FSharpToolTipElement<'T> = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of string
    static member Single : 'T * FSharpXmlDoc * ?typeMapping: 'T list * ?paramName: string * ?remarks : 'T  -> FSharpToolTipElement<'T>

/// A single data tip display element with where text is expressed as string
type FSharpToolTipElement = FSharpToolTipElement<string>

/// A single data tip display element with where text is expressed as <see cref="Layout"/>
type internal FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>

/// Information for building a tool tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
type internal FSharpToolTipText<'T> = 
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement<'T> list  

type FSharpToolTipText = FSharpToolTipText<string>
type internal FSharpStructuredToolTipText = FSharpToolTipText<Layout>

module internal Tooltips =
    val ToFSharpToolTipElement: FSharpStructuredToolTipElement -> FSharpToolTipElement
    val ToFSharpToolTipText: FSharpStructuredToolTipText -> FSharpToolTipText
    val Map: f: ('T1 -> 'T2) -> a: Async<'T1> -> Async<'T2>

[<RequireQualifiedAccess>]
type internal CompletionItemKind =
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | Other

/// Indicates the accessibility of a symbol, as seen by the F# language
and [<Class>] internal FSharpAccessibility = 
    new: Accessibility * ?isProtected: bool -> FSharpAccessibility

    /// Indicates the symbol has public accessibility
    member IsPublic : bool

    /// Indicates the symbol has private accessibility
    member IsPrivate : bool

    /// Indicates the symbol has internal accessibility
    member IsInternal : bool

    /// The underlying Accessibility
    member Contents : Accessibility

[<Sealed>]
/// Represents a declaration in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListItem =
    /// Get the display name for the declaration.
    member Name : string
    /// Get the name for the declaration as it's presented in source code.
    member NameInCode : string
    /// Get the description text for the declaration. Computing this property may require using compiler
    /// resources and may trigger execution of a type provider method to retrieve documentation.
    ///
    /// May return "Loading..." if timeout occurs
    member StructuredDescriptionText : FSharpStructuredToolTipText
    member DescriptionText : FSharpToolTipText

    /// Get the description text, asynchronously.  Never returns "Loading...".
    member StructuredDescriptionTextAsync : Async<FSharpStructuredToolTipText>
    member DescriptionTextAsync : Async<FSharpToolTipText>
    member Glyph : FSharpGlyph
    member Accessibility : FSharpAccessibility option
    member Kind : CompletionItemKind
    member IsOwnMember : bool
    member MinorPriority : int
    member FullName : string
    member IsResolved : bool
    member NamespaceToOpen : string option

type UnresolvedSymbol =
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

[<Sealed>]
/// Represents a set of declarations in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListInfo =
    member Items : FSharpDeclarationListItem[]
    member IsForType : bool
    member IsError : bool

    // Implementation details used by other code in the compiler    
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * getAccessibility:(Item -> FSharpAccessibility option) * items:CompletionItem list * reactor:IReactorOperations * currentNamespace:string[] option * isAttributeApplicationContex:bool * checkAlive:(unit -> bool) -> FSharpDeclarationListInfo
    static member internal Error : message:string -> FSharpDeclarationListInfo
    static member Empty : FSharpDeclarationListInfo

/// Represents one parameter for one method (or other item) in a group. 
[<Sealed>]
type internal FSharpMethodGroupItemParameter = 

    /// The name of the parameter.
    member ParameterName: string

    /// A key that can be used for sorting the parameters, used to help sort overloads.
    member CanonicalTypeTextForSorting: string

    /// The structured representation for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member StructuredDisplay: Layout

    /// The text to display for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member Display: string

    /// Is the parameter optional
    member IsOptional: bool

/// Represents one method (or other item) in a method group. The item may represent either a method or 
/// a single, non-overloaded item such as union case or a named function value.
[<Sealed>]
type internal FSharpMethodGroupItem = 

    /// The documentation for the item
    member XmlDoc : FSharpXmlDoc

    /// The structured description representation for the method (or other item)
    member StructuredDescription : FSharpStructuredToolTipText

    /// The formatted description text for the method (or other item)
    member Description : FSharpToolTipText

    /// The The structured description representation for the method (or other item)
    member StructuredReturnTypeText: Layout

    /// The formatted type text for the method (or other item)
    member ReturnTypeText: string

    /// The parameters of the method in the overload set
    member Parameters: FSharpMethodGroupItemParameter[]

    /// Does the method support an arguments list?  This is always true except for static type instantiations like TP<42,"foo">.
    member HasParameters: bool

    /// Does the method support a params list arg?
    member HasParamArrayArg: bool

    /// Does the type name or method support a static arguments list, like TP<42,"foo"> or conn.CreateCommand<42, "foo">(arg1, arg2)?
    member StaticParameters: FSharpMethodGroupItemParameter[]

/// Represents a group of methods (or other items) returned by GetMethods.  
[<Sealed>]
type internal FSharpMethodGroup = 

    internal new : string * FSharpMethodGroupItem[] -> FSharpMethodGroup

    /// The shared name of the methods (or other items) in the group
    member MethodName: string

    /// The methods (or other items) in the group
    member Methods: FSharpMethodGroupItem[] 

    static member internal Create : InfoReader * range * DisplayEnv * ItemWithInst list -> FSharpMethodGroup

// implementation details used by other code in the compiler    
module internal ItemDescriptionsImpl = 
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
    val GlyphOfItem : DisplayEnv * Item -> FSharpGlyph
    val IsAttribute : InfoReader -> Item -> bool
    val IsExplicitlySuppressed : TcGlobals -> Item -> bool
    val FlattenItems : TcGlobals -> range -> Item -> Item list
    val (|ItemIsProvidedType|_|) : TcGlobals -> Item -> TyconRef option

module EnvMisc2 =
    val maxMembers : int
    /// dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
    /// This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
    val dataTipSpinWaitTime : int