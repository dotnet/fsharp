// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.InteropServices.ComTypes

open Microsoft.VisualStudio
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

/// Class View context menu ids. The shell owns these menus, but `VsMenus` publishes only the
/// Solution Explorer half of vsshlids.h, so the Class View ids have to be spelled out.
[<RequireQualifiedAccess>]
module private ClassViewMenus =

    [<Literal>]
    let Project = 0x0432

    [<Literal>]
    let Item = 0x0433

    [<Literal>]
    let GroupingFolder = 0x0435

    [<Literal>]
    let Member = 0x0438

/// Search list flags the shell passes. `_LIB_LISTFLAGS` stops at `LLF_RESOURCEVIEW`, so these two
/// have no interop counterpart to reference.
[<RequireQualifiedAccess>]
module private SearchListFlags =

    [<Literal>]
    let ExpandMembers = 0x0400u

    [<Literal>]
    let WithExpansion = 0x0800u

/// One node list in Object Browser or Class View. Items are a snapshot: they are rebuilt only when
/// the manager's counter for this kind of list moves, which is also how a project whose symbols were
/// still being computed replaces its placeholder with real content.
[<Sealed>]
type internal FSharpObjectList
    (
        kind: ObjectListKind,
        flags: uint32,
        host: IObjectBrowserHost,
        parentList: FSharpObjectList voption,
        parentItem: ObjectListItem voption,
        produce: unit -> ObjectListItem[]
    ) =

    /// An assembly's contents never change with edits, so lists sourced from a reference node ride
    /// the package counter and are not re-walked on every keystroke.
    let counterKind =
        match parentItem with
        | ValueSome item when item.Data.IsReference -> ObjectListKind.Projects
        | _ -> kind

    let mutable version = host.UpdateCounter counterKind
    let mutable items = produce ()

    let parentKind =
        match parentList with
        | ValueSome list -> list.Kind
        | ValueNone -> ObjectListKind.None

    /// The shell passes 0xFFFFFFFF for "no item", and `int` on it is -1, which passes a signed
    /// bounds check and then indexes out of the array. Comparing unsigned rejects that and every
    /// other out-of-range index without singling the sentinel out.
    let tryItem (index: uint32) =
        if index < uint32 items.Length then
            ValueSome items[int index]
        else
            ValueNone

    let withItem index (f: ObjectListItem -> int) =
        match tryItem index with
        | ValueSome item -> f item
        | ValueNone -> VSConstants.E_INVALIDARG

    let listType () =
        match kind with
        | ObjectListKind.BaseTypes -> ValueSome(uint32 _LIB_LISTTYPE.LLT_CLASSES ||| uint32 _LIB_LISTTYPE.LLT_MEMBERS)

        | ObjectListKind.Hierarchy ->
            ValueSome(
                match parentKind with
                | ObjectListKind.Types
                | ObjectListKind.BaseTypes -> uint32 _LIB_LISTTYPE.LLT_CLASSES
                | _ -> uint32 _LIB_LISTTYPE.LLT_PACKAGE
            )

        | ObjectListKind.Members -> ValueSome 0u
        | ObjectListKind.Namespaces -> ValueSome(uint32 _LIB_LISTTYPE.LLT_CLASSES)

        | ObjectListKind.Projects ->
            let namespacesAndTypes =
                uint32 _LIB_LISTTYPE.LLT_NAMESPACES ||| uint32 _LIB_LISTTYPE.LLT_CLASSES

            ValueSome(
                if LibraryList.isClassView flags && parentKind.IsNone then
                    namespacesAndTypes ||| uint32 _LIB_LISTTYPE.LLT_HIERARCHY
                else
                    namespacesAndTypes
            )

        | ObjectListKind.References -> ValueSome(uint32 _LIB_LISTTYPE.LLT_NAMESPACES ||| uint32 _LIB_LISTTYPE.LLT_CLASSES)

        | ObjectListKind.Types ->
            let searching =
                flags &&& (SearchListFlags.ExpandMembers ||| SearchListFlags.WithExpansion)
                <> 0u

            ValueSome(
                if searching then
                    uint32 _LIB_LISTTYPE.LLT_MEMBERS
                else
                    uint32 _LIB_LISTTYPE.LLT_MEMBERS ||| uint32 _LIB_LISTTYPE.LLT_HIERARCHY
            )

        | ObjectListKind.None -> ValueNone

    let accessField (item: ObjectListItem) =
        match item.Accessibility with
        | ValueNone
        | ValueSome(Tokenizer.Public) -> uint32 _LIBCAT_MEMBERACCESS.LCMA_PUBLIC
        | ValueSome(Tokenizer.Internal) -> uint32 _LIBCAT_MEMBERACCESS.LCMA_PACKAGE
        | ValueSome(Tokenizer.Protected) -> uint32 _LIBCAT_MEMBERACCESS.LCMA_PROTECTED
        | ValueSome(Tokenizer.Private) -> uint32 _LIBCAT_MEMBERACCESS.LCMA_PRIVATE

    let classTypeField (item: ObjectListItem) =
        match item.Data with
        | ObjectItemData.Type(_, typeKind) ->
            ValueSome(
                match typeKind with
                | ObjectTypeKind.Interface -> uint32 _LIBCAT_CLASSTYPE.LCCT_INTERFACE
                | ObjectTypeKind.Struct -> uint32 _LIBCAT_CLASSTYPE.LCCT_STRUCT
                | ObjectTypeKind.Enum -> uint32 _LIBCAT_CLASSTYPE.LCCT_ENUM
                | ObjectTypeKind.Delegate -> uint32 _LIBCAT_CLASSTYPE.LCCT_DELEGATE
                | ObjectTypeKind.Module -> uint32 _LIBCAT_CLASSTYPE.LCCT_MODULE
                | ObjectTypeKind.Exception -> uint32 _LIBCAT_CLASSTYPE.LCCT_EXCEPTION
                | ObjectTypeKind.Class -> uint32 _LIBCAT_CLASSTYPE.LCCT_CLASS
            )
        | ObjectItemData.Namespace _ -> ValueSome(uint32 _LIBCAT_CLASSTYPE.LCCT_NSPC)
        | _ -> ValueNone

    let memberTypeField (item: ObjectListItem) =
        match item.Data with
        | ObjectItemData.Member(_, memberKind, _) ->
            ValueSome(
                match memberKind with
                | ObjectMemberKind.Constant -> uint32 _LIBCAT_MEMBERTYPE.LCMT_CONSTANT
                | ObjectMemberKind.EnumMember -> uint32 _LIBCAT_MEMBERTYPE.LCMT_ENUMITEM
                | ObjectMemberKind.Event -> uint32 _LIBCAT_MEMBERTYPE.LCMT_EVENT
                | ObjectMemberKind.Field -> uint32 _LIBCAT_MEMBERTYPE.LCMT_FIELD
                | ObjectMemberKind.Operator -> uint32 _LIBCAT_MEMBERTYPE.LCMT_OPERATOR
                | ObjectMemberKind.Property -> uint32 _LIBCAT_MEMBERTYPE.LCMT_PROPERTY
                | ObjectMemberKind.Method -> uint32 _LIBCAT_MEMBERTYPE.LCMT_METHOD
            )
        | _ -> ValueNone

    let memberInheritanceField (item: ObjectListItem) =
        match item.Data with
        | ObjectItemData.Member(_, _, true) -> uint32 _LIBCAT_MEMBERINHERITANCE.LCMI_INHERITED
        | _ -> uint32 _LIBCAT_MEMBERINHERITANCE.LCMI_IMMEDIATE

    let physicalContainerField () =
        match parentKind with
        | ObjectListKind.Projects -> ValueSome(uint32 _LIBCAT_PHYSICALCONTAINERTYPE.LCPT_PROJECT)
        | ObjectListKind.References -> ValueSome(uint32 _LIBCAT_PHYSICALCONTAINERTYPE.LCPT_PROJECTREFERENCE)
        | _ -> ValueNone

    let hierarchyTypeField () =
        if kind.IsHierarchy then
            match parentKind with
            | ObjectListKind.Projects -> uint32 _LIBCAT_HIERARCHYTYPE.LCHT_PROJECTREFERENCES
            | _ -> uint32 _LIBCAT_HIERARCHYTYPE.LCHT_BASESANDINTERFACES
        else
            uint32 _LIBCAT_HIERARCHYTYPE.LCHT_UNKNOWN

    let categoryField (item: ObjectListItem) category =
        if category = int LIB_CATEGORY.LC_LISTTYPE then
            listType ()
        elif
            category = int LIB_CATEGORY.LC_MEMBERACCESS
            || category = int LIB_CATEGORY.LC_CLASSACCESS
        then
            ValueSome(accessField item)
        elif category = int LIB_CATEGORY.LC_CLASSTYPE then
            classTypeField item
        elif category = int LIB_CATEGORY.LC_MEMBERTYPE then
            memberTypeField item
        elif category = int _LIB_CATEGORY2.LC_MEMBERINHERITANCE then
            ValueSome(memberInheritanceField item)
        elif category = int _LIB_CATEGORY2.LC_HIERARCHYTYPE then
            ValueSome(hierarchyTypeField ())
        elif category = int _LIB_CATEGORY2.LC_PHYSICALCONTAINERTYPE then
            physicalContainerField ()
        elif category = int LIB_CATEGORY.LC_VISIBILITY then
            ValueSome(
                if item.IsHidden then
                    uint32 _LIBCAT_VISIBILITY.LCV_HIDDEN
                else
                    uint32 _LIBCAT_VISIBILITY.LCV_VISIBLE
            )
        else
            ValueNone

    let navInfoOf (item: ObjectListItem) =
        match item.NavPath with
        | ValueNone -> ValueNone
        | ValueSome path ->
            // Class View parents references under their owning project; Object Browser does not.
            let referenceOwner =
                match item.Data with
                | ObjectItemData.Reference _ when LibraryList.isClassView flags -> ValueSome(host.ProjectName item.ProjectId)
                | _ -> ValueNone

            ValueSome(host.NavInfoFactory.Create(path.Library, referenceOwner, path.Namespace, path.Class, path.Member))

    let childItems (item: ObjectListItem) childKind =
        let library =
            match item.NavPath with
            | ValueSome path -> path.Library
            | ValueNone -> host.ProjectName item.ProjectId

        let ofProject build =
            match host.TryGetProjectSymbols item.ProjectId with
            | ValueSome symbols -> build symbols
            | ValueNone -> [| ObjectBrowserItems.pendingItem item.ProjectId |]

        match childKind, item.Data with
        | ObjectListKind.Namespaces, ObjectItemData.Project ->
            ofProject (fun symbols -> ObjectBrowserItems.namespaceItems item.ProjectId library symbols.Types)

        | ObjectListKind.Namespaces, ObjectItemData.Reference(name, _) ->
            ofProject (fun symbols ->
                match ObjectBrowserItems.tryFindReferencedAssembly symbols name with
                | ValueSome assembly ->
                    ObjectBrowserItems.namespaceItems item.ProjectId library (ObjectBrowserItems.typesOfAssembly assembly)
                | ValueNone -> Array.empty)

        | ObjectListKind.Types, ObjectItemData.Project ->
            ofProject (fun symbols -> ObjectBrowserItems.globalTypeItems item.ProjectId library symbols.Types)

        | ObjectListKind.Types, ObjectItemData.Reference(name, _) ->
            ofProject (fun symbols ->
                match ObjectBrowserItems.tryFindReferencedAssembly symbols name with
                | ValueSome assembly ->
                    ObjectBrowserItems.globalTypeItems item.ProjectId library (ObjectBrowserItems.typesOfAssembly assembly)
                | ValueNone -> Array.empty)

        | ObjectListKind.Types, ObjectItemData.Namespace types -> ObjectBrowserItems.typeItems item.ProjectId library types

        | ObjectListKind.Members, ObjectItemData.Type(entity, _) -> ObjectBrowserItems.memberItems item.ProjectId library entity

        | ObjectListKind.BaseTypes, ObjectItemData.Type(entity, _) -> ObjectBrowserItems.baseTypeItems item.ProjectId library entity

        | ObjectListKind.References, ObjectItemData.Project -> host.ReferenceItems item.ProjectId

        | ObjectListKind.Hierarchy, ObjectItemData.Project ->
            [|
                ObjectBrowserItems.folderItem item.ProjectId (SR.ObjectBrowserProjectReferences()) ObjectListKind.References
            |]

        | ObjectListKind.Hierarchy, ObjectItemData.Type _ ->
            [|
                ObjectBrowserItems.folderItem item.ProjectId (SR.ObjectBrowserBaseTypes()) ObjectListKind.BaseTypes
            |]

        | _ -> Array.empty

    member _.Kind = kind

    interface IVsCoTaskMemFreeMyStrings

    interface IVsSimpleObjectList2 with

        member _.GetFlags(pFlags: byref<uint32>) =
            pFlags <- flags
            VSConstants.S_OK

        member _.GetCapabilities2(pgrfCapabilities: byref<uint32>) =
            pgrfCapabilities <- uint32 _LIB_LISTCAPABILITIES2.LLC_ALLOWELEMENTSEARCH
            VSConstants.S_OK

        member _.UpdateCounter(pCurUpdate: byref<uint32>) =
            let current = host.UpdateCounter counterKind

            if current <> version then
                version <- current
                items <- produce ()

            pCurUpdate <- current
            VSConstants.S_OK

        member _.GetItemCount(pCount: byref<uint32>) =
            pCount <- uint32 items.Length
            VSConstants.S_OK

        member _.GetDisplayData(index, pData: VSTREEDISPLAYDATA[]) =
            match pData with
            | null
            | [||] -> VSConstants.E_INVALIDARG
            | pData ->

                withItem index (fun item ->
                    // Zero the image list so the Object Browser uses its own, avoiding DPI scaling
                    // issues — except for projects, whose real icon only their hierarchy knows.
                    let imageList, glyph =
                        match item.Data with
                        | ObjectItemData.Project ->
                            match host.TryGetProjectIcon item.ProjectId with
                            | ValueSome(handle, iconIndex) -> handle, iconIndex
                            | ValueNone -> IntPtr.Zero, item.GlyphIndex
                        | _ -> IntPtr.Zero, item.GlyphIndex

                    pData[0].hImageList <- imageList
                    pData[0].Image <- glyph
                    pData[0].SelectedImage <- glyph

                    if item.IsHidden then
                        pData[0].State <- pData[0].State ||| uint32 _VSTREEDISPLAYSTATE.TDS_GRAYTEXT

                    VSConstants.S_OK)

        member _.GetTextWithOwnership(index, tto, pbstrText: byref<string>) =
            match tryItem index with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome item ->
                pbstrText <-
                    if tto = VSTREETEXTOPTIONS.TTO_SEARCHTEXT then
                        item.FullName
                    else
                        item.DisplayText

                VSConstants.S_OK

        member _.GetTipTextWithOwnership(_index, _eTipType, pbstrText: byref<string>) =
            pbstrText <- null
            VSConstants.E_NOTIMPL

        member _.GetCategoryField2(index, category, pfCatField: byref<uint32>) =
            match tryItem index with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome item ->
                match categoryField item category with
                | ValueSome field ->
                    pfCatField <- field
                    VSConstants.S_OK
                | ValueNone ->
                    pfCatField <- 0u
                    VSConstants.E_NOTIMPL

        member _.GetBrowseObject(_index, ppdispBrowseObj: byref<obj>) =
            ppdispBrowseObj <- null
            VSConstants.E_NOTIMPL

        member _.GetUserContext(_index, ppunkUserCtx: byref<obj>) =
            ppunkUserCtx <- null
            VSConstants.E_NOTIMPL

        member _.ShowHelp(_index) = VSConstants.E_NOTIMPL

        member _.GetSourceContextWithOwnership(index, pbstrFilename: byref<string>, pulLineNum: byref<uint32>) =
            match tryItem index |> ValueOption.bind ObjectBrowserItems.tryDeclarationRange with
            | ValueSome range ->
                pbstrFilename <- range.FileName
                pulLineNum <- uint32 (max 0 (range.StartLine - 1))
                VSConstants.S_OK
            | ValueNone ->
                pbstrFilename <- null
                pulLineNum <- 0u
                VSConstants.E_NOTIMPL

        member _.CountSourceItems(_index, ppHier: byref<IVsHierarchy>, pItemid: byref<uint32>, pcItems: byref<uint32>) =
            ppHier <- null
            pItemid <- 0u
            pcItems <- 0u
            VSConstants.E_NOTIMPL

        member _.GetMultipleSourceItems(_index, _grfGSI, _cItems, _rgItemSel) = VSConstants.E_NOTIMPL

        member _.CanGoToSource(index, srcType, pfOK: byref<int>) =
            let canNavigate =
                srcType = VSOBJGOTOSRCTYPE.GS_DEFINITION
                && (match tryItem index with
                    | ValueSome item -> item.SupportsGoToDefinition
                    | ValueNone -> false)

            pfOK <- if canNavigate then 1 else 0
            VSConstants.S_OK

        member _.GoToSource(index, _srcType) =
            withItem index (fun item ->
                host.GoToSource item
                VSConstants.S_OK)

        member _.GetContextMenu(index, pclsidActive: byref<Guid>, pnMenuId: byref<int>, ppCmdTrgtActive: byref<IOleCommandTarget>) =
            match tryItem index with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome item ->
                pclsidActive <- VsMenus.guidSHLMainMenu

                pnMenuId <-
                    match item.Data with
                    | ObjectItemData.Project
                    | ObjectItemData.Reference _ -> ClassViewMenus.Project
                    | ObjectItemData.Folder _
                    | ObjectItemData.Pending -> ClassViewMenus.GroupingFolder
                    | ObjectItemData.Member _ -> ClassViewMenus.Member
                    | _ -> ClassViewMenus.Item

                ppCmdTrgtActive <- host.CommandTarget
                VSConstants.S_OK

        member _.QueryDragDrop(_index, _pDataObject, _grfKeyState, pdwEffect: byref<uint32>) =
            pdwEffect <- 0u
            VSConstants.E_NOTIMPL

        member _.DoDragDrop(_index, _pDataObject, _grfKeyState, pdwEffect: byref<uint32>) =
            pdwEffect <- 0u
            VSConstants.E_NOTIMPL

        member _.CanRename(_index, _pszNewName, pfOK: byref<int>) =
            pfOK <- 0
            VSConstants.E_NOTIMPL

        member _.DoRename(_index, _pszNewName, _grfFlags) = VSConstants.E_NOTIMPL

        member _.CanDelete(_index, pfOK: byref<int>) =
            pfOK <- 0
            VSConstants.E_NOTIMPL

        member _.DoDelete(_index, _grfFlags) = VSConstants.E_NOTIMPL

        member _.FillDescription2(index, grfOptions, pobDesc) =
            match tryItem index with
            | ValueSome item when host.FillDescription(item, grfOptions, pobDesc) -> VSConstants.S_OK
            | _ -> VSConstants.E_FAIL

        member _.EnumClipboardFormats(_index, _grfFlags, _celt, _rgcfFormats, _pcActual) = VSConstants.E_NOTIMPL

        member _.GetClipboardFormat(_index, _grfFlags, _pFormatetc: FORMATETC[], _pMedium: STGMEDIUM[]) = VSConstants.E_NOTIMPL

        member _.GetExtendedClipboardVariant(_index, _grfFlags, _pcfFormat, pvarFormat: byref<obj>) =
            pvarFormat <- null
            VSConstants.E_NOTIMPL

        member _.GetProperty(index, propid, pvar: byref<obj>) =
            match tryItem index with
            | ValueSome item when propid = int _VSOBJLISTELEMPROPID.VSOBJLISTELEMPROPID_FULLNAME ->
                pvar <- box item.FullName
                VSConstants.S_OK
            | ValueSome item when propid = int _VSOBJLISTELEMPROPID.VSOBJLISTELEMPROPID_LEAFNAME ->
                pvar <- box item.DisplayText
                VSConstants.S_OK
            | _ ->
                pvar <- null
                VSConstants.E_NOTIMPL

        member _.GetNavInfo(index, ppNavInfo: byref<IVsNavInfo>) =
            match tryItem index with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome item ->
                match navInfoOf item with
                | ValueSome navInfo ->
                    ppNavInfo <- navInfo
                    VSConstants.S_OK
                | ValueNone -> VSConstants.E_NOTIMPL

        member _.GetNavInfoNode(index, ppNavInfoNode: byref<IVsNavInfoNode>) =
            match tryItem index with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome item ->
                // BaseTypes and References borrow other list types on the wire, but their nav-info
                // nodes must carry the canonical class/package types or LocateNavInfoNode round-trips fail.
                let nodeType =
                    match kind with
                    | ObjectListKind.BaseTypes
                    | ObjectListKind.Types -> _LIB_LISTTYPE.LLT_CLASSES
                    | ObjectListKind.References
                    | ObjectListKind.Projects -> _LIB_LISTTYPE.LLT_PACKAGE
                    | ObjectListKind.Members -> _LIB_LISTTYPE.LLT_MEMBERS
                    | ObjectListKind.Namespaces -> _LIB_LISTTYPE.LLT_NAMESPACES
                    | ObjectListKind.Hierarchy -> _LIB_LISTTYPE.LLT_HIERARCHY
                    | ObjectListKind.None -> enum 0

                ppNavInfoNode <- FSharpNavInfoNode(item.DisplayText, nodeType)
                VSConstants.S_OK

        member _.LocateNavInfoNode(pNavInfoNode, pulIndex: byref<uint32>) =
            let mutable name = null
            pulIndex <- UInt32.MaxValue

            if pNavInfoNode.get_Name (&name) <> VSConstants.S_OK then
                VSConstants.E_FAIL
            else
                match name with
                | null -> VSConstants.E_FAIL
                | name ->
                    match Array.FindIndex(items, fun item -> String.Equals(item.DisplayText, name, StringComparison.Ordinal)) with
                    | -1 -> VSConstants.S_FALSE
                    | index ->
                        pulIndex <- uint32 index
                        VSConstants.S_OK

        member _.GetExpandable3(index, _listTypeExcluded, pfExpandable: byref<int>) =
            let expandable =
                match tryItem index with
                | ValueNone -> false
                | ValueSome item ->
                    // Answering precisely for a type would force a full member import per visible
                    // row during paint; an expander that opens onto an empty list is cheaper.
                    match item.Data with
                    | ObjectItemData.Member _
                    | ObjectItemData.Pending -> false
                    | _ -> true

            pfExpandable <- if expandable then 1 else 0
            VSConstants.S_OK

        member this.GetList2(index, requestedListType, listFlags, pobSrch, ppList: byref<IVsSimpleObjectList2>) =
            // A hierarchy folder stands in for the node above it: its children belong to that node.
            let childListType, sourceItem =
                match kind, tryItem index with
                | _, ValueNone -> requestedListType, ValueNone

                | ObjectListKind.Hierarchy, ValueSome _ ->
                    let listType =
                        if requestedListType = uint32 _LIB_LISTTYPE.LLT_CLASSES then
                            uint32 _LIB_LISTTYPE.LLT_USESCLASSES
                        else
                            LibraryList.ProjectReferencesListType

                    listType, parentItem

                | ObjectListKind.BaseTypes, ValueSome item when requestedListType = uint32 _LIB_LISTTYPE.LLT_CLASSES ->
                    uint32 _LIB_LISTTYPE.LLT_USESCLASSES, ValueSome item

                | _, ValueSome item -> requestedListType, ValueSome item

            match sourceItem with
            | ValueNone -> VSConstants.E_INVALIDARG
            | ValueSome sourceItem ->
                let childKind = LibraryList.ofListType childListType

                let produceChildren =
                    if LibraryList.isFindSymbol listFlags then
                        let searchText = LibraryList.searchTextOf pobSrch
                        fun () -> host.Search(childKind, sourceItem, searchText)
                    else
                        fun () -> childItems sourceItem childKind

                ppList <- FSharpObjectList(childKind, listFlags, host, ValueSome this, ValueSome sourceItem, produceChildren)
                VSConstants.S_OK

        // The shell calls this while merely collapsing or refreshing a node and keeps using the list
        // afterwards, so releasing the items here makes the node report itself unexpandable.
        member _.OnClose(_ptca: VSTREECLOSEACTIONS[]) = VSConstants.E_NOTIMPL
