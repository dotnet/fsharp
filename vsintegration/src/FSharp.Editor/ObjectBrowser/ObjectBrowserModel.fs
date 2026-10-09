// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler.Symbols

[<RequireQualifiedAccess>]
type internal ObjectListKind =
    | None
    | Projects
    | References
    | Namespaces
    | Types
    | Members
    | BaseTypes
    | Hierarchy

[<RequireQualifiedAccess>]
module internal LibraryList =

    /// The shell has no list type of its own for project references. Class View will not round-trip
    /// its "Project References" folder unless we borrow the same one C#/VB borrow.
    let ProjectReferencesListType = uint32 _LIB_LISTTYPE.LLT_INTERFACEUSEDBYCLASSES

    let ofListType (listType: uint32) =
        match enum<_LIB_LISTTYPE> (int listType) with
        | _LIB_LISTTYPE.LLT_CLASSES -> ObjectListKind.Types
        | _LIB_LISTTYPE.LLT_HIERARCHY -> ObjectListKind.Hierarchy
        | _LIB_LISTTYPE.LLT_MEMBERS -> ObjectListKind.Members
        | _LIB_LISTTYPE.LLT_NAMESPACES -> ObjectListKind.Namespaces
        | _LIB_LISTTYPE.LLT_PACKAGE -> ObjectListKind.Projects
        | _LIB_LISTTYPE.LLT_INTERFACEUSEDBYCLASSES -> ObjectListKind.References
        | _LIB_LISTTYPE.LLT_USESCLASSES -> ObjectListKind.BaseTypes
        | _ -> ObjectListKind.None

    let isClassView flags =
        flags &&& uint32 _LIB_LISTFLAGS.LLF_TRUENESTING <> 0u

    let isFindSymbol flags =
        flags &&& uint32 _LIB_LISTFLAGS.LLF_USESEARCHFILTER <> 0u

    let isObjectBrowser flags =
        let notObjectBrowser =
            uint32 _LIB_LISTFLAGS.LLF_TRUENESTING
            ||| uint32 _LIB_LISTFLAGS.LLF_USESEARCHFILTER
            ||| uint32 _LIB_LISTFLAGS.LLF_RESOURCEVIEW

        flags &&& notObjectBrowser = 0u

    let searchTextOf (criteria: VSOBSEARCHCRITERIA2[]) =
        match criteria with
        | null
        | [||] -> ""
        | criteria -> criteria[0].szName

[<RequireQualifiedAccess>]
type internal ObjectTypeKind =
    | Class
    | Interface
    | Struct
    | Enum
    | Delegate
    | Module
    | Exception

[<RequireQualifiedAccess>]
type internal ObjectMemberKind =
    | Method
    | Property
    | Event
    | Field
    | Constant
    | EnumMember
    | Operator

/// Everything an Object Browser node needs to produce its own children, without going back to the checker.
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type internal ObjectItemData =
    | Project
    /// Straight from the project's metadata references; the FSharpAssembly is resolved only when
    /// the node is expanded, from the owning project's check results.
    | Reference of assemblyName: string * assemblyPath: string voption
    | Folder of childKind: ObjectListKind
    /// Shown while the background check that populates a project is still running.
    | Pending
    | Namespace of types: FSharpEntity[]
    | Type of entity: FSharpEntity * kind: ObjectTypeKind
    | Member of symbol: FSharpSymbol * kind: ObjectMemberKind * isInherited: bool

[<NoComparison; NoEquality>]
type internal ObjectNavPath =
    {
        Library: string
        Namespace: string voption
        Class: string voption
        Member: string voption
    }

[<NoComparison; NoEquality>]
type internal ObjectListItem =
    {
        Data: ObjectItemData
        DisplayText: string
        FullName: string
        GlyphIndex: uint16
        IsHidden: bool
        Accessibility: FSharpAccessibility voption
        NavPath: ObjectNavPath voption
        ProjectId: ProjectId
    }

    member this.SupportsGoToDefinition =
        match this.Data with
        | ObjectItemData.Type _
        | ObjectItemData.Member _ -> true
        | _ -> false

/// The result of one background `ParseAndCheckProject`, reshaped into what the tree consumes.
[<NoComparison; NoEquality>]
type internal ProjectSymbols =
    {
        Types: FSharpEntity[]
        ReferencedAssemblies: FSharpAssembly[]
    }

/// The part of the library manager that object lists depend on. Breaks the cycle between the manager,
/// which creates the root list, and the lists, which create their own children.
type internal IObjectBrowserHost =
    abstract NavInfoFactory: FSharpNavInfoFactory
    abstract CommandTarget: IOleCommandTarget
    abstract UpdateCounter: ObjectListKind -> uint32
    abstract ProjectName: ProjectId -> string
    /// The project's own icon, as its hierarchy supplies it to Solution Explorer: an image list
    /// handle and an index into it. The Object Browser's default image list has no F# glyph, so
    /// without this a project node can only be drawn with another language's icon.
    abstract TryGetProjectIcon: ProjectId -> struct (nativeint * uint16) voption
    /// ValueNone while the project's symbols are still being computed; a counter bump follows when they land.
    abstract TryGetProjectSymbols: ProjectId -> ProjectSymbols voption
    /// Reference rows for a project, read from its metadata references without any check.
    abstract ReferenceItems: ProjectId -> ObjectListItem[]
    abstract GoToSource: ObjectListItem -> unit
    abstract FillDescription: ObjectListItem * uint32 * IVsObjectBrowserDescription3 -> bool
    abstract Search: ObjectListKind * ObjectListItem * string -> ObjectListItem[]

[<RequireQualifiedAccess>]
module internal ObjectBrowserGlyph =

    /// Below GlyphGroupError a group is a base index and the accessibility is an offset into it;
    /// above it the groups are single images.
    let private toIndex (group: StandardGlyphGroup) (item: StandardGlyphItem) =
        if group < StandardGlyphGroup.GlyphGroupError then
            uint16 (int group + int item)
        else
            uint16 group

    let private accessibilityItem (access: FSharpAccessibility voption) =
        match access with
        | ValueNone -> StandardGlyphItem.GlyphItemPublic
        | ValueSome(Tokenizer.Public) -> StandardGlyphItem.GlyphItemPublic
        | ValueSome(Tokenizer.Internal) -> StandardGlyphItem.GlyphItemInternal
        | ValueSome(Tokenizer.Protected) -> StandardGlyphItem.GlyphItemProtected
        | ValueSome(Tokenizer.Private) -> StandardGlyphItem.GlyphItemPrivate

    let private typeGroup kind =
        match kind with
        | ObjectTypeKind.Interface -> StandardGlyphGroup.GlyphGroupInterface
        | ObjectTypeKind.Struct -> StandardGlyphGroup.GlyphGroupStruct
        | ObjectTypeKind.Enum -> StandardGlyphGroup.GlyphGroupEnum
        | ObjectTypeKind.Delegate -> StandardGlyphGroup.GlyphGroupDelegate
        | ObjectTypeKind.Module -> StandardGlyphGroup.GlyphGroupModule
        | ObjectTypeKind.Exception -> StandardGlyphGroup.GlyphGroupException
        | ObjectTypeKind.Class -> StandardGlyphGroup.GlyphGroupClass

    let private memberGroup kind =
        match kind with
        | ObjectMemberKind.Property -> StandardGlyphGroup.GlyphGroupProperty
        | ObjectMemberKind.Event -> StandardGlyphGroup.GlyphGroupEvent
        | ObjectMemberKind.Field -> StandardGlyphGroup.GlyphGroupField
        | ObjectMemberKind.Constant -> StandardGlyphGroup.GlyphGroupConstant
        | ObjectMemberKind.EnumMember -> StandardGlyphGroup.GlyphGroupEnumMember
        | ObjectMemberKind.Operator -> StandardGlyphGroup.GlyphGroupOperator
        | ObjectMemberKind.Method -> StandardGlyphGroup.GlyphGroupMethod

    let forType kind access =
        toIndex (typeGroup kind) (accessibilityItem access)

    let forMember kind access =
        toIndex (memberGroup kind) (accessibilityItem access)

    let forNamespace =
        toIndex StandardGlyphGroup.GlyphGroupNamespace StandardGlyphItem.GlyphItemPublic

    /// Only reached when a project's hierarchy will not hand over its own icon. `StandardGlyphGroup`
    /// predates F# and has no F# project glyph (nor does `KnownMonikers`), so this is the C# icon —
    /// wrong language, but at least it reads as a project. See dotnet/roslyn#85102.
    let forProject =
        toIndex StandardGlyphGroup.GlyphCoolProject StandardGlyphItem.GlyphItemPublic

    let forAssembly =
        toIndex StandardGlyphGroup.GlyphAssembly StandardGlyphItem.GlyphItemPublic

    let forFolder =
        toIndex StandardGlyphGroup.GlyphClosedFolder StandardGlyphItem.GlyphItemPublic

    let forPending =
        toIndex StandardGlyphGroup.GlyphInformation StandardGlyphItem.GlyphItemPublic
