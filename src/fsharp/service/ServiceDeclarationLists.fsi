// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// API for declaration lists and method overload lists
namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Symbols
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// A single data tip display element
[<RequireQualifiedAccess>]
type public ToolTipElementData = 
    {
      MainDescription: TaggedText[]

      XmlDoc: FSharpXmlDoc

      /// typar instantiation text, to go after xml
      TypeMapping: TaggedText[] list

      /// Extra text, goes at the end
      Remarks: TaggedText[] option

      /// Parameter name
      ParamName: string option
    }

/// A single tool tip display element
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type public ToolTipElement = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of elements: ToolTipElementData list

    /// An error occurred formatting this element
    | CompositionError of errorText: string

    static member Single: layout: TaggedText[] * xml: FSharpXmlDoc * ?typeMapping: TaggedText[] list * ?paramName: string * ?remarks: TaggedText[]  -> ToolTipElement

/// Information for building a tool tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
type public ToolTipText = 

    /// A list of data tip elements to display.
    | ToolTipText of ToolTipElement list  

[<RequireQualifiedAccess>]
type public CompletionItemKind =
    | Field
    | Property
    | Method of isExtension: bool
    | Event
    | Argument
    | CustomOperation
    | Other

type public UnresolvedSymbol =
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
    member Item: Item

[<Sealed>]
/// Represents a declaration in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type public DeclarationListItem =
    /// Get the display name for the declaration.
    member Name: string

    /// Get the name for the declaration as it's presented in source code.
    member NameInCode: string

    /// Get the description
    member Description: ToolTipText

    member Glyph: FSharpGlyph

    member Accessibility: FSharpAccessibility

    member Kind: CompletionItemKind

    member IsOwnMember: bool

    member MinorPriority: int

    member FullName: string

    member IsResolved: bool

    member NamespaceToOpen: string option


[<Sealed>]
/// Represents a set of declarations in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type public DeclarationListInfo =

    member Items: DeclarationListItem[]

    member IsForType: bool

    member IsError: bool

    // Implementation details used by other code in the compiler    
    static member internal Create:
        infoReader:InfoReader * 
        m:range * 
        denv:DisplayEnv * 
        getAccessibility:(Item -> FSharpAccessibility) * 
        items:CompletionItem list * 
        currentNamespace:string[] option * 
        isAttributeApplicationContext:bool 
            -> DeclarationListInfo

    static member internal Error: message:string -> DeclarationListInfo

    static member Empty: DeclarationListInfo

/// Represents one parameter for one method (or other item) in a group. 
[<Sealed>]
type public MethodGroupItemParameter = 

    /// The name of the parameter.
    member ParameterName: string

    /// A key that can be used for sorting the parameters, used to help sort overloads.
    member CanonicalTypeTextForSorting: string

    /// The representation for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member Display: TaggedText[]

    /// Is the parameter optional
    member IsOptional: bool

/// Represents one method (or other item) in a method group. The item may represent either a method or 
/// a single, non-overloaded item such as union case or a named function value.
[<Sealed>]
type public MethodGroupItem = 

    /// The documentation for the item
    member XmlDoc: FSharpXmlDoc

    /// The description representation for the method (or other item)
    member Description: ToolTipText

    /// The tagged text for the return type for the method (or other item)
    member ReturnTypeText: TaggedText[]

    /// The parameters of the method in the overload set
    member Parameters: MethodGroupItemParameter[]

    /// Does the method support an arguments list?  This is always true except for static type instantiations like TP<42,"foo">.
    member HasParameters: bool

    /// Does the method support a params list arg?
    member HasParamArrayArg: bool

    /// Does the type name or method support a static arguments list, like TP<42,"foo"> or conn.CreateCommand<42, "foo">(arg1, arg2)?
    member StaticParameters: MethodGroupItemParameter[]

/// Represents a group of methods (or other items) returned by GetMethods.  
[<Sealed>]
type public MethodGroup = 

    internal new: string * MethodGroupItem[] -> MethodGroup

    /// The shared name of the methods (or other items) in the group
    member MethodName: string

    /// The methods (or other items) in the group
    member Methods: MethodGroupItem[] 

    static member internal Create: InfoReader * range * DisplayEnv * ItemWithInst list -> MethodGroup

module internal DeclarationListHelpers =
    val FormatStructuredDescriptionOfItem: isDecl:bool -> InfoReader -> range -> DisplayEnv -> ItemWithInst -> ToolTipElement

    val RemoveDuplicateCompletionItems: TcGlobals -> CompletionItem list -> CompletionItem list

    val RemoveExplicitlySuppressedCompletionItems: TcGlobals -> CompletionItem list -> CompletionItem list

    val mutable ToolTipFault: string option

