// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System

open Xunit

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Editor

module ObjectBrowserNavInfoTests =

    let private libraryId = Guid "{56BF9F1D-EE02-46C7-9A6A-7C66674863E9}"

    let private factory = FSharpNavInfoFactory libraryId

    let private nodes (enumerate: IVsEnumNavInfoNodes) =
        let buffer = Array.zeroCreate<IVsNavInfoNode> 1

        let rec next acc =
            let mutable fetched = 0u

            if enumerate.Next(1u, buffer, &fetched) = VSConstants.S_OK && fetched = 1u then
                let mutable name = null
                let mutable listType = 0u
                buffer[0].get_Name (&name) |> ignore
                buffer[0].get_Type (&listType) |> ignore
                next ((name, enum<_LIB_LISTTYPE> (int listType)) :: acc)
            else
                List.rev acc

        next []

    let private canonicalNodes (navInfo: IVsNavInfo) =
        let mutable enumerate = null
        navInfo.EnumCanonicalNodes(&enumerate) |> ignore
        nodes enumerate

    let private presentationNodes flags (navInfo: IVsNavInfo) =
        let mutable enumerate = null
        navInfo.EnumPresentationNodes(flags, &enumerate) |> ignore
        nodes enumerate

    let private objectBrowserNodes = presentationNodes (uint32 _LIB_LISTFLAGS.LLF_NONE)

    let private classViewNodes =
        presentationNodes (uint32 _LIB_LISTFLAGS.LLF_TRUENESTING)

    [<Fact>]
    let ``Canonical nodes split dotted namespaces and classes`` () =
        let navInfo =
            factory.Create("MyProject", ValueNone, ValueSome "A.B", ValueSome "Outer.Inner", ValueSome "Item")

        let expected =
            [
                "MyProject", _LIB_LISTTYPE.LLT_PACKAGE
                "A", _LIB_LISTTYPE.LLT_NAMESPACES
                "B", _LIB_LISTTYPE.LLT_NAMESPACES
                "Outer", _LIB_LISTTYPE.LLT_CLASSES
                "Inner", _LIB_LISTTYPE.LLT_CLASSES
                "Item", _LIB_LISTTYPE.LLT_MEMBERS
            ]

        Assert.Equal<(string * _LIB_LISTTYPE) list>(expected, canonicalNodes navInfo)

    [<Fact>]
    let ``Presentation nodes keep dotted names whole`` () =
        let navInfo =
            factory.Create("MyProject", ValueNone, ValueSome "A.B", ValueSome "Outer.Inner", ValueNone)

        let expected =
            [
                "MyProject", _LIB_LISTTYPE.LLT_PACKAGE
                "A.B", _LIB_LISTTYPE.LLT_NAMESPACES
                "Outer.Inner", _LIB_LISTTYPE.LLT_CLASSES
            ]

        Assert.Equal<(string * _LIB_LISTTYPE) list>(expected, objectBrowserNodes navInfo)

    [<Fact>]
    let ``Class View keeps the reference owner prefix that Object Browser drops`` () =
        let navInfo =
            factory.Create("FSharp.Core.dll", ValueSome "MyProject", ValueSome "Microsoft.FSharp.Core", ValueSome "Operators", ValueNone)

        let classView = classViewNodes navInfo

        Assert.Equal<string list>(
            [
                "MyProject"
                SR.ObjectBrowserProjectReferences()
                "FSharp.Core.dll"
                "Microsoft.FSharp.Core"
                "Operators"
            ],
            classView |> List.map fst
        )

        Assert.Equal<string list>([ "FSharp.Core.dll"; "Microsoft.FSharp.Core"; "Operators" ], objectBrowserNodes navInfo |> List.map fst)

    [<Fact>]
    let ``Canonical nodes never contain the hierarchy placeholder`` () =
        let navInfo =
            factory.Create("FSharp.Core.dll", ValueSome "MyProject", ValueSome "Microsoft.FSharp.Core", ValueNone, ValueNone)

        Assert.DoesNotContain(_LIB_LISTTYPE.LLT_HIERARCHY, canonicalNodes navInfo |> List.map snd)

    [<Fact>]
    let ``Symbol type is the deepest presentation node`` () =
        let navInfo =
            factory.Create("MyProject", ValueNone, ValueSome "A", ValueSome "T", ValueSome "M")

        let mutable symbolType = 0u
        navInfo.GetSymbolType(&symbolType) |> ignore

        Assert.Equal(uint32 _LIB_LISTTYPE.LLT_MEMBERS, symbolType)

    [<Fact>]
    let ``Library guid round-trips`` () =
        let navInfo = factory.Create("MyProject")
        let mutable guid = Guid.Empty
        navInfo.GetLibGuid(&guid) |> ignore

        Assert.Equal(libraryId, guid)

    [<Theory>]
    [<InlineData(0x04, "Types")>]
    [<InlineData(0x01, "Hierarchy")>]
    [<InlineData(0x08, "Members")>]
    [<InlineData(0x02, "Namespaces")>]
    [<InlineData(0x10, "Projects")>]
    [<InlineData(0x800, "References")>]
    [<InlineData(0x80, "BaseTypes")>]
    let ``List types map to the kinds the tree dispatches on`` (listType: int, expected: string) =
        Assert.Equal(expected, string (LibraryList.ofListType (uint32 listType)))

    [<Fact>]
    let ``Only the plain flags mean Object Browser`` () =
        Assert.True(LibraryList.isObjectBrowser (uint32 _LIB_LISTFLAGS.LLF_NONE))
        Assert.False(LibraryList.isObjectBrowser (uint32 _LIB_LISTFLAGS.LLF_TRUENESTING))
        Assert.True(LibraryList.isClassView (uint32 _LIB_LISTFLAGS.LLF_TRUENESTING))
        Assert.True(LibraryList.isFindSymbol (uint32 _LIB_LISTFLAGS.LLF_USESEARCHFILTER))

    [<Fact>]
    let ``Node enumerator never writes past the buffer it was handed`` () =
        let navInfo =
            factory.Create("MyProject", ValueNone, ValueSome "A", ValueSome "T", ValueSome "M")

        let mutable enumerate = null
        navInfo.EnumCanonicalNodes(&enumerate) |> ignore

        // celt claims room for four nodes, the array holds one.
        let buffer = Array.zeroCreate<IVsNavInfoNode> 1
        let mutable fetched = 0u
        enumerate.Next(4u, buffer, &fetched) |> ignore

        Assert.Equal(1u, fetched)

    [<Fact>]
    let ``Node enumerator honours the COM Skip and Clone contracts`` () =
        let navInfo =
            factory.Create("MyProject", ValueNone, ValueSome "A", ValueSome "T", ValueSome "M")

        let mutable enumerate = null
        navInfo.EnumCanonicalNodes(&enumerate) |> ignore

        // Skipping past the end reports S_FALSE; skipping within reports S_OK.
        Assert.Equal(VSConstants.S_OK, enumerate.Skip 2u)
        Assert.Equal(VSConstants.S_FALSE, enumerate.Skip 99u)

        enumerate.Reset() |> ignore
        enumerate.Skip 3u |> ignore

        // A clone continues from the same position instead of restarting.
        let mutable clone = null
        enumerate.Clone(&clone) |> ignore

        let buffer = Array.zeroCreate<IVsNavInfoNode> 1
        let mutable fetched = 0u
        clone.Next(1u, buffer, &fetched) |> ignore

        let mutable name = null
        buffer[0].get_Name (&name) |> ignore
        Assert.Equal("M", name)
