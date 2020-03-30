// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// API for declaration lists and method overload lists
namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Range
open FSharp.Compiler.TypedTreeOps

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
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * getAccessibility:(Item -> FSharpAccessibility option) * items:CompletionItem list * reactor:IReactorOperations * currentNamespace:string[] option * isAttributeApplicationContex:bool -> FSharpDeclarationListInfo

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

