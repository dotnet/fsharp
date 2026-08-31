// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System

open Xunit

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Editor

/// Exercises FSharpObjectList against a scripted host: no Visual Studio shell, no checker.
module ObjectBrowserListTests =

    let private emptySymbols =
        {
            Types = [||]
            ReferencedAssemblies = [||]
        }

    type private ScriptedHost(symbols: ProjectSymbols voption) =
        member val Counter = 0u with get, set

        interface IObjectBrowserHost with
            member _.NavInfoFactory = FSharpNavInfoFactory(Guid.NewGuid())
            member _.CommandTarget = null
            member this.UpdateCounter _ = this.Counter
            member _.ProjectName _ = "TestProject"
            member _.TryGetProjectIcon _ = ValueNone
            member _.ReferenceItems _ = Array.empty
            member _.TryGetProjectSymbols _ = symbols
            member _.GoToSource _ = ()
            member _.FillDescription(_, _, _) = false
            member _.Search(_, _, _) = Array.empty

    let private projectId = ProjectId.CreateNewId()

    let private projectsList (host: ScriptedHost) flags =
        FSharpObjectList(
            ObjectListKind.Projects,
            flags,
            host,
            ValueNone,
            ValueNone,
            fun () -> [| ObjectBrowserItems.projectItem projectId "TestProject" |]
        )
        :> IVsSimpleObjectList2

    let private classViewFlags = uint32 _LIB_LISTFLAGS.LLF_TRUENESTING

    let private childList (list: IVsSimpleObjectList2) index listType flags =
        let mutable child = null
        let result = list.GetList2(index, listType, flags, null, &child)
        result, child

    let private itemCount (list: IVsSimpleObjectList2) =
        let mutable count = 0u
        list.GetItemCount(&count) |> ignore
        count

    let private textOf (list: IVsSimpleObjectList2) index =
        let mutable text = null

        list.GetTextWithOwnership(index, VSTREETEXTOPTIONS.TTO_DISPLAYTEXT, &text)
        |> ignore

        text

    [<Fact>]
    let ``Project References folder expands into the project's references`` () =
        // The folder's children belong to the project item the folder stands in for; resolving them
        // through the wrong ancestor used to return E_INVALIDARG here.
        let host = ScriptedHost(ValueSome emptySymbols)
        let root = projectsList host classViewFlags

        let hierarchyResult, hierarchy =
            childList root 0u (uint32 _LIB_LISTTYPE.LLT_HIERARCHY) classViewFlags

        Assert.Equal(VSConstants.S_OK, hierarchyResult)
        Assert.Equal(1u, itemCount hierarchy)
        Assert.Equal(SR.ObjectBrowserProjectReferences(), textOf hierarchy 0u)

        let referencesResult, references =
            childList hierarchy 0u (uint32 _LIB_LISTTYPE.LLT_PACKAGE) classViewFlags

        Assert.Equal(VSConstants.S_OK, referencesResult)
        Assert.Equal(0u, itemCount references)

    [<Fact>]
    let ``Unchecked project renders a single pending placeholder`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host classViewFlags

        let result, namespaces =
            childList root 0u (uint32 _LIB_LISTTYPE.LLT_NAMESPACES) classViewFlags

        Assert.Equal(VSConstants.S_OK, result)
        Assert.Equal(1u, itemCount namespaces)
        Assert.Equal(SR.ObjectBrowserPending(), textOf namespaces 0u)

        let mutable expandable = 1
        namespaces.GetExpandable3(0u, 0u, &expandable) |> ignore
        Assert.Equal(0, expandable)

        let mutable canNavigate = 1

        namespaces.CanGoToSource(0u, VSOBJGOTOSRCTYPE.GS_DEFINITION, &canNavigate)
        |> ignore

        Assert.Equal(0, canNavigate)

    [<Fact>]
    let ``Checked project with no types renders empty child lists`` () =
        let host = ScriptedHost(ValueSome emptySymbols)
        let root = projectsList host classViewFlags

        let result, namespaces =
            childList root 0u (uint32 _LIB_LISTTYPE.LLT_NAMESPACES) classViewFlags

        Assert.Equal(VSConstants.S_OK, result)
        Assert.Equal(0u, itemCount namespaces)

    [<Fact>]
    let ``UpdateCounter rebuilds items only when the counter moves`` () =
        let host = ScriptedHost ValueNone
        let mutable produced = 0

        let list =
            FSharpObjectList(
                ObjectListKind.Projects,
                0u,
                host,
                ValueNone,
                ValueNone,
                fun () ->
                    produced <- produced + 1
                    Array.empty
            )
            :> IVsSimpleObjectList2

        Assert.Equal(1, produced)

        let mutable version = 0u
        list.UpdateCounter(&version) |> ignore
        Assert.Equal(1, produced)

        host.Counter <- 1u
        list.UpdateCounter(&version) |> ignore
        Assert.Equal(1u, version)
        Assert.Equal(2, produced)

    [<Fact>]
    let ``Invalid index is rejected without touching the host`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host 0u

        let mutable text = null
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetTextWithOwnership(99u, VSTREETEXTOPTIONS.TTO_DISPLAYTEXT, &text))

        let result, _ = childList root 99u (uint32 _LIB_LISTTYPE.LLT_NAMESPACES) 0u
        Assert.Equal(VSConstants.E_INVALIDARG, result)

    /// The shell hands 0xFFFFFFFF to mean "no item". Converting it to a signed int yields -1, which
    /// passes a signed bounds check and then indexes out of the array.
    [<Theory>]
    [<InlineData(0xFFFFFFFFu)>]
    [<InlineData(0xFFFFFFFEu)>]
    [<InlineData(1u)>]
    let ``Out-of-range indices are rejected rather than indexing the item array`` (index: uint32) =
        let host = ScriptedHost(ValueSome emptySymbols)
        let root = projectsList host classViewFlags
        Assert.Equal(1u, itemCount root)

        let mutable field = 0u
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetCategoryField2(index, int LIB_CATEGORY.LC_VISIBILITY, &field))

        let mutable text = null
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetTextWithOwnership(index, VSTREETEXTOPTIONS.TTO_DISPLAYTEXT, &text))

        let display = Array.zeroCreate<VSTREEDISPLAYDATA> 1
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetDisplayData(index, display))

        let mutable fileName = null
        let mutable line = 0u
        Assert.Equal(VSConstants.E_NOTIMPL, root.GetSourceContextWithOwnership(index, &fileName, &line))

        let mutable navInfo = null
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetNavInfo(index, &navInfo))

        let mutable node = null
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetNavInfoNode(index, &node))

        let mutable menuGuid = Guid.Empty
        let mutable menuId = 0
        let mutable target = null
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetContextMenu(index, &menuGuid, &menuId, &target))

        let mutable expandable = 1
        Assert.Equal(VSConstants.S_OK, root.GetExpandable3(index, 0u, &expandable))
        Assert.Equal(0, expandable)

        let mutable canNavigate = 1
        Assert.Equal(VSConstants.S_OK, root.CanGoToSource(index, VSOBJGOTOSRCTYPE.GS_DEFINITION, &canNavigate))
        Assert.Equal(0, canNavigate)

        Assert.Equal(VSConstants.E_INVALIDARG, root.GoToSource(index, VSOBJGOTOSRCTYPE.GS_DEFINITION))

        let result, _ =
            childList root index (uint32 _LIB_LISTTYPE.LLT_NAMESPACES) classViewFlags

        Assert.Equal(VSConstants.E_INVALIDARG, result)

    [<Fact>]
    let ``GetDisplayData rejects a buffer it cannot write into`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host 0u

        Assert.Equal(VSConstants.E_INVALIDARG, root.GetDisplayData(0u, null))
        Assert.Equal(VSConstants.E_INVALIDARG, root.GetDisplayData(0u, Array.empty))

    [<Fact>]
    let ``LocateNavInfoNode finds items by display text and reports misses`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host 0u

        let mutable index = 0u
        Assert.Equal(VSConstants.S_OK, root.LocateNavInfoNode(FSharpNavInfoNode("TestProject", _LIB_LISTTYPE.LLT_PACKAGE), &index))
        Assert.Equal(0u, index)

        Assert.Equal(VSConstants.S_FALSE, root.LocateNavInfoNode(FSharpNavInfoNode("Unknown", _LIB_LISTTYPE.LLT_PACKAGE), &index))
        Assert.Equal(UInt32.MaxValue, index)

    [<Fact>]
    let ``Project category fields report a visible project node`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host 0u

        let mutable field = 0u
        Assert.Equal(VSConstants.S_OK, root.GetCategoryField2(0u, int LIB_CATEGORY.LC_VISIBILITY, &field))
        Assert.Equal(uint32 _LIBCAT_VISIBILITY.LCV_VISIBLE, field)

        Assert.Equal(VSConstants.S_OK, root.GetCategoryField2(0u, int LIB_CATEGORY.LC_LISTTYPE, &field))
        Assert.Equal(uint32 _LIB_LISTTYPE.LLT_NAMESPACES ||| uint32 _LIB_LISTTYPE.LLT_CLASSES, field)

    [<Fact>]
    let ``Class View root also offers the hierarchy folder`` () =
        let host = ScriptedHost ValueNone
        let root = projectsList host classViewFlags

        let mutable field = 0u
        root.GetCategoryField2(0u, int LIB_CATEGORY.LC_LISTTYPE, &field) |> ignore

        Assert.Equal(
            uint32 _LIB_LISTTYPE.LLT_NAMESPACES
            ||| uint32 _LIB_LISTTYPE.LLT_CLASSES
            ||| uint32 _LIB_LISTTYPE.LLT_HIERARCHY,
            field
        )
