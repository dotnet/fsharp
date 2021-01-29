// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// API for declaration lists and method overload lists
namespace FSharp.Compiler.Editing

open System
open FSharp.Compiler
open FSharp.Compiler.Analysis
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TextLayout
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

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
    | Group of elements: FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of errorText: string

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
type public FSharpCompletionItemKind =
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | CustomOperation
    | Other

type FSharpUnresolvedSymbol =
    {
      FullName: string

      DisplayName: string

      Namespace: string[]
    }

type internal CompletionItem =
    {
      ItemWithInst: ItemWithInst

      Kind: FSharpCompletionItemKind

      IsOwnMember: bool

      MinorPriority: int

      Type: TyconRef option 

      Unresolved: FSharpUnresolvedSymbol option
    }
    member Item : Item

module public FSharpToolTip =

    val ToFSharpToolTipElement: FSharpStructuredToolTipElement -> FSharpToolTipElement

    val ToFSharpToolTipText: FSharpStructuredToolTipText -> FSharpToolTipText

[<Sealed>]
/// Represents a declaration in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type public FSharpDeclarationListItem =
    /// Get the display name for the declaration.
    member Name : string

    /// Get the name for the declaration as it's presented in source code.
    member NameInCode : string

    [<Obsolete("This operation is no longer asynchronous, please use the non-async version")>]
    member StructuredDescriptionTextAsync : Async<FSharpStructuredToolTipText>

    /// Get the description text.
    member StructuredDescriptionText : FSharpStructuredToolTipText

    [<Obsolete("This operation is no longer asynchronous, please use the non-async version")>]
    member DescriptionTextAsync : Async<FSharpToolTipText>

    member DescriptionText : FSharpToolTipText

    member Glyph : FSharpGlyph

    member Accessibility : FSharpAccessibility option

    member Kind : FSharpCompletionItemKind

    member IsOwnMember : bool

    member MinorPriority : int

    member FullName : string

    member IsResolved : bool

    member NamespaceToOpen : string option


[<Sealed>]
/// Represents a set of declarations in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type public FSharpDeclarationListInfo =

    member Items : FSharpDeclarationListItem[]

    member IsForType : bool

    member IsError : bool

    // Implementation details used by other code in the compiler    
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * getAccessibility:(Item -> FSharpAccessibility option) * items:CompletionItem list * currentNamespace:string[] option * isAttributeApplicationContext:bool -> FSharpDeclarationListInfo

    static member internal Error : message:string -> FSharpDeclarationListInfo

    static member Empty : FSharpDeclarationListInfo

/// Represents one parameter for one method (or other item) in a group. 
[<Sealed>]
type public FSharpMethodGroupItemParameter = 

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
type public FSharpMethodGroupItem = 

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
type public FSharpMethodGroup = 

    internal new : string * FSharpMethodGroupItem[] -> FSharpMethodGroup

    /// The shared name of the methods (or other items) in the group
    member MethodName: string

    /// The methods (or other items) in the group
    member Methods: FSharpMethodGroupItem[] 

    static member internal Create : InfoReader * range * DisplayEnv * ItemWithInst list -> FSharpMethodGroup

module internal DeclarationListHelpers =
    val FormatStructuredDescriptionOfItem : isDecl:bool -> InfoReader -> range -> DisplayEnv -> ItemWithInst -> FSharpStructuredToolTipElement

    val RemoveDuplicateCompletionItems : TcGlobals -> CompletionItem list -> CompletionItem list

    val RemoveExplicitlySuppressedCompletionItems : TcGlobals -> CompletionItem list -> CompletionItem list

    val mutable ToolTipFault : string option

