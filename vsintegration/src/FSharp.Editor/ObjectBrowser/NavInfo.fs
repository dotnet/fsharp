// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop

[<Sealed>]
type internal FSharpNavInfoNode(name: string, listType: _LIB_LISTTYPE) =

    member _.ListType = listType

    interface IVsNavInfoNode with

        member _.get_Name(pbstrName: byref<string>) =
            pbstrName <- name
            VSConstants.S_OK

        member _.get_Type(pllt: byref<uint32>) =
            pllt <- uint32 listType
            VSConstants.S_OK

[<Sealed>]
type internal FSharpNavInfoNodeEnum private (nodes: FSharpNavInfoNode[], startPosition: int) =

    let mutable position = startPosition

    new(nodes) = FSharpNavInfoNodeEnum(nodes, 0)

    interface IVsEnumNavInfoNodes with

        member _.Clone(ppEnum: byref<IVsEnumNavInfoNodes>) =
            ppEnum <- FSharpNavInfoNodeEnum(nodes, position)
            VSConstants.S_OK

        member _.Next(celt, rgelt: IVsNavInfoNode[], pceltFetched: byref<uint32>) =
            // celt is the caller's promise about rgelt; never write past what it actually handed us.
            let capacity =
                match rgelt with
                | null -> 0
                | rgelt -> min (int celt) rgelt.Length

            let fetched = min capacity (nodes.Length - position)

            for i in 0 .. fetched - 1 do
                rgelt[i] <- nodes[position + i]

            position <- position + fetched
            pceltFetched <- uint32 fetched

            if fetched = int celt then
                VSConstants.S_OK
            else
                VSConstants.S_FALSE

        member _.Reset() =
            position <- 0
            VSConstants.S_OK

        member _.Skip(celt) =
            let requested = position + int celt
            position <- min nodes.Length requested

            if requested > nodes.Length then
                VSConstants.S_FALSE
            else
                VSConstants.S_OK

/// An ordered path identifying one Object Browser or Class View node:
/// [reference owner, "Project References"] -> library -> namespace -> class -> member.
[<Sealed>]
type internal FSharpNavInfo
    (
        libraryGuid: Guid,
        libraryName: string,
        referenceOwnerName: string voption,
        namespaceName: string voption,
        className: string voption,
        memberName: string voption
    ) =

    let nodesFor name listType expandDottedNames =
        match name with
        | ValueSome name when not (String.IsNullOrEmpty name) ->
            if expandDottedNames then
                name.Split('.') |> Array.map (fun part -> FSharpNavInfoNode(part, listType))
            else
                [| FSharpNavInfoNode(name, listType) |]
        | _ -> Array.empty

    let createNodes expandDottedNames =
        [|
            match referenceOwnerName with
            | ValueSome owner ->
                FSharpNavInfoNode(owner, _LIB_LISTTYPE.LLT_PACKAGE)
                FSharpNavInfoNode(SR.ObjectBrowserProjectReferences(), _LIB_LISTTYPE.LLT_HIERARCHY)
            | ValueNone -> ()

            FSharpNavInfoNode(libraryName, _LIB_LISTTYPE.LLT_PACKAGE)
            yield! nodesFor namespaceName _LIB_LISTTYPE.LLT_NAMESPACES expandDottedNames
            yield! nodesFor className _LIB_LISTTYPE.LLT_CLASSES expandDottedNames
            yield! nodesFor memberName _LIB_LISTTYPE.LLT_MEMBERS false
        |]

    let presentationNodes = createNodes false

    let canonicalNodes =
        createNodes true
        |> Array.filter (fun node -> node.ListType <> _LIB_LISTTYPE.LLT_HIERARCHY)

    /// Class View paths start with a (package, hierarchy) pair that Object Browser must not be shown.
    let objectBrowserNodes =
        if
            presentationNodes.Length >= 2
            && presentationNodes[1].ListType = _LIB_LISTTYPE.LLT_HIERARCHY
        then
            presentationNodes[2..]
        else
            presentationNodes

    let symbolType =
        if presentationNodes.Length = 0 then
            0u
        else
            uint32 presentationNodes[presentationNodes.Length - 1].ListType

    let getLibGuid (pGuid: byref<Guid>) =
        pGuid <- libraryGuid
        VSConstants.S_OK

    let getSymbolType (pdwType: byref<uint32>) =
        pdwType <- symbolType
        VSConstants.S_OK

    let enumCanonicalNodes (ppEnum: byref<IVsEnumNavInfoNodes>) =
        ppEnum <- FSharpNavInfoNodeEnum(canonicalNodes)
        VSConstants.S_OK

    let enumPresentationNodes dwFlags (ppEnum: byref<IVsEnumNavInfoNodes>) =
        ppEnum <-
            FSharpNavInfoNodeEnum(
                if dwFlags = uint32 _LIB_LISTFLAGS.LLF_NONE then
                    objectBrowserNodes
                else
                    presentationNodes
            )

        VSConstants.S_OK

    interface IVsNavInfo with
        member _.GetLibGuid(pGuid) = getLibGuid &pGuid
        member _.GetSymbolType(pdwType) = getSymbolType &pdwType
        member _.EnumCanonicalNodes(ppEnum) = enumCanonicalNodes &ppEnum

        member _.EnumPresentationNodes(dwFlags, ppEnum) = enumPresentationNodes dwFlags &ppEnum

    interface IVsNavInfo2 with
        member _.GetLibGuid(pGuid) = getLibGuid &pGuid
        member _.GetSymbolType(pdwType) = getSymbolType &pdwType
        member _.EnumCanonicalNodes(ppEnum) = enumCanonicalNodes &ppEnum

        member _.EnumPresentationNodes(dwFlags, ppEnum) = enumPresentationNodes dwFlags &ppEnum

        member _.GetPreferredLanguage(pLanguage: byref<uint32>) =
            pLanguage <- uint32 __SymbolToolLanguage.SymbolToolLanguage_None

[<Sealed>]
type internal FSharpNavInfoFactory(libraryGuid: Guid) =

    member _.LibraryGuid = libraryGuid

    member _.Create(libraryName, referenceOwnerName, namespaceName, className, memberName) : IVsNavInfo =
        FSharpNavInfo(libraryGuid, libraryName, referenceOwnerName, namespaceName, className, memberName)

    member this.Create(libraryName) =
        this.Create(libraryName, ValueNone, ValueNone, ValueNone, ValueNone)

    member this.Create(libraryName, namespaceName) =
        this.Create(libraryName, ValueNone, namespaceName, ValueNone, ValueNone)

    member this.Create(libraryName, namespaceName, className) =
        this.Create(libraryName, ValueNone, namespaceName, className, ValueNone)
