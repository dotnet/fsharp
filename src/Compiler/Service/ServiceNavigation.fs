// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.EditorServices

open System
open System.Collections.Generic
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range

/// Represents the different kinds of items that can appear in the navigation bar
[<RequireQualifiedAccess>]
type NavigationItemKind =
    | Namespace
    | ModuleFile
    | Exception
    | Module
    | Type
    | Method
    | Property
    | Field
    | Other

[<RequireQualifiedAccess>]
type NavigationEntityKind =
    | Namespace
    | Module
    | Class
    | Exception
    | Interface
    | Record
    | Enum
    | Union

/// Represents an item to be displayed in the navigation bar
[<Sealed>]
type NavigationItem
    (
        uniqueName: string,
        logicalName: string,
        kind: NavigationItemKind,
        glyph: FSharpGlyph,
        range: range,
        bodyRange: range,
        singleTopLevel: bool,
        enclosingEntityKind: NavigationEntityKind,
        isAbstract: bool,
        access: SynAccess option
    ) =

    member _.bodyRange = bodyRange

    member _.UniqueName = uniqueName

    member _.LogicalName = logicalName

    member _.Glyph = glyph

    member _.Kind = kind

    member _.Range = range

    member _.BodyRange = bodyRange

    member _.IsSingleTopLevel = singleTopLevel

    member _.EnclosingEntityKind = enclosingEntityKind

    member _.IsAbstract = isAbstract

    member _.Access = access

    member _.WithUniqueName(uniqueName: string) =
        NavigationItem(uniqueName, logicalName, kind, glyph, range, bodyRange, singleTopLevel, enclosingEntityKind, isAbstract, access)

    static member Create(name, kind, glyph, range, bodyRange, singleTopLevel, enclosingEntityKind, isAbstract, access) =
        NavigationItem("", name, kind, glyph, range, bodyRange, singleTopLevel, enclosingEntityKind, isAbstract, access)

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type NavigationTopLevelDeclaration =
    {
        Declaration: NavigationItem
        Nested: NavigationItem[]
    }

/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type NavigationItems(declarations: NavigationTopLevelDeclaration[]) =
    member _.Declarations = declarations

module NavigationImpl =
    let unionRangesChecked r1 r2 =
        if equals r1 range.Zero then r2
        elif equals r2 range.Zero then r1
        else unionRanges r1 r2

    let rangeOfDecls2 f decls =
        match decls |> List.map (f >> (fun (d: NavigationItem) -> d.bodyRange)) with
        | hd :: tl -> tl |> List.fold unionRangesChecked hd
        | [] -> range.Zero

    let rangeOfDecls = rangeOfDecls2 fst

    let moduleRange (idm: range) others =
        unionRangesChecked idm.EndRange (rangeOfDecls2 (fun (a, _, _) -> a) others)

    let fldspecRange fldspec =
        match fldspec with
        | SynUnionCaseKind.Fields(flds) ->
            flds
            |> List.fold (fun st (SynField(range = m)) -> unionRangesChecked m st) range.Zero
        | SynUnionCaseKind.FullType(ty, _) -> ty.Range

    let bodyRange mBody decls =
        unionRangesChecked (rangeOfDecls decls) mBody

    /// Get information for implementation file
    let getNavigationFromImplFile (modules: SynModuleOrNamespace list) =
        // Map for dealing with name conflicts
        let names = Dictionary()

        let addItemName name =
            let count =
                match names.TryGetValue name with
                | true, count -> count + 1
                | _ -> 1

            names[name] <- count
            count

        let uniqueName name idx =
            let total = names[name]
            sprintf "%s_%d_of_%d" name idx total

        // Create declaration (for the left dropdown)
        let createDeclLid (baseName, lid, kind, baseGlyph, m, mBody, nested, enclosingEntityKind, access) =
            let name = (if baseName <> "" then baseName + "." else "") + textOfLid lid
            let item = NavigationItem.Create(name, kind, baseGlyph, m, mBody, false, enclosingEntityKind, false, access)
            item, addItemName name, nested

        let createDecl (baseName, id: Ident, kind, baseGlyph, m, mBody, nested, enclosingEntityKind, isAbstract, access) =
            let name = (if baseName <> "" then baseName + "." else "") + id.idText
            let item = NavigationItem.Create(name, kind, baseGlyph, m, mBody, false, enclosingEntityKind, isAbstract, access)
            item, addItemName name, nested

        let createTypeDecl (baseName, lid, baseGlyph, m, mBody, nested, enclosingEntityKind, access) =
            createDeclLid (baseName, lid, NavigationItemKind.Type, baseGlyph, m, mBody, nested, enclosingEntityKind, access)

        // Create member-kind-of-thing for the right dropdown
        let createMemberLid (lid, kind, baseGlyph, m, enclosingEntityKind, isAbstract, access) =
            let item = NavigationItem.Create(textOfLid lid, kind, baseGlyph, m, m, false, enclosingEntityKind, isAbstract, access)
            item, addItemName (textOfLid lid)

        let createMember (id: Ident, kind, baseGlyph, m, enclosingEntityKind, isAbstract, access) =
            let item = NavigationItem.Create(id.idText, kind, baseGlyph, m, m, false, enclosingEntityKind, isAbstract, access)
            item, addItemName (id.idText)

        // Process let-binding
        let processBinding isMember enclosingEntityKind isAbstract synBinding =
            let (SynBinding(valData = valData; headPat = synPat; expr = synExpr)) = synBinding
            let (SynValData(memberFlags = memberOpt)) = valData

            let m =
                match synExpr with
                | SynExpr.Typed(e, _, _) -> e.Range // fix range for properties with type annotations
                | _ -> synExpr.Range

            match synPat, memberOpt with
            | SynPat.LongIdent(longDotId = SynLongIdent(lid, _, _); accessibility = access), Some(flags) when isMember ->
                let icon, kind =
                    match flags.MemberKind with
                    | SynMemberKind.ClassConstructor
                    | SynMemberKind.Constructor
                    | SynMemberKind.Member ->
                        let glyph =
                            if flags.IsOverrideOrExplicitImpl then
                                FSharpGlyph.OverridenMethod
                            else
                                FSharpGlyph.Method

                        glyph, NavigationItemKind.Method
                    | SynMemberKind.PropertyGetSet
                    | SynMemberKind.PropertySet
                    | SynMemberKind.PropertyGet -> FSharpGlyph.Property, NavigationItemKind.Property

                let lidShow, rangeMerge =
                    match lid with
                    | _thisVar :: nm :: _ -> (List.tail lid, nm.idRange)
                    | hd :: _ -> (lid, hd.idRange)
                    | _ -> (lid, m)

                let m = unionRanges rangeMerge m

                [
                    createMemberLid (lidShow, kind, icon, m, enclosingEntityKind, isAbstract, access)
                ]

            | SynPat.LongIdent(longDotId = SynLongIdent(lid, _, _); accessibility = access), _ ->
                let m = unionRanges (List.head lid).idRange m

                [
                    createMemberLid (lid, NavigationItemKind.Field, FSharpGlyph.Field, m, enclosingEntityKind, isAbstract, access)
                ]

            | SynPat.Named(SynIdent(id, _), _, access, _), _
            | SynPat.As(_, SynPat.Named(SynIdent(id, _), _, access, _), _), _ ->
                let glyph = if isMember then FSharpGlyph.Method else FSharpGlyph.Field
                let m = unionRanges id.idRange m

                [
                    createMember (id, NavigationItemKind.Field, glyph, m, enclosingEntityKind, isAbstract, access)
                ]
            | _ -> []

        // Process a class declaration or F# type declaration
        let rec processExnDefnRepr baseName nested synExnRepr =
            let (SynExceptionDefnRepr(_, ucase, _, _, access, m)) = synExnRepr
            let (SynUnionCase(ident = SynIdent(id, _); caseType = fldspec)) = ucase
            let mBody = fldspecRange fldspec

            [
                createDecl (baseName, id, NavigationItemKind.Exception, FSharpGlyph.Exception, m, mBody, nested, NavigationEntityKind.Exception, false, access)
            ]

        // Process a class declaration or F# type declaration
        and processExnDefn baseName synExnDefn =
            let (SynExceptionDefn(repr, _, membDefns, _)) = synExnDefn
            let nested = processMembers membDefns NavigationEntityKind.Exception |> snd
            processExnDefnRepr baseName nested repr

        and processTycon baseName synTypeDefn =
            let (SynTypeDefn(typeInfo = typeInfo; typeRepr = repr; members = membDefns; range = m)) = synTypeDefn
            let (SynComponentInfo(longId = lid; accessibility = access)) = typeInfo

            let topMembers = processMembers membDefns NavigationEntityKind.Class |> snd

            match repr with
            | SynTypeDefnRepr.Exception repr -> processExnDefnRepr baseName [] repr

            | SynTypeDefnRepr.ObjectModel(_, membDefns, mBody) ->
                // F# class declaration
                let members = processMembers membDefns NavigationEntityKind.Class |> snd
                let nested = members @ topMembers
                let mBody = bodyRange mBody nested

                [
                    createTypeDecl (baseName, lid, FSharpGlyph.Class, m, mBody, nested, NavigationEntityKind.Class, access)
                ]

            | SynTypeDefnRepr.Simple(simple, _) ->
                // F# type declaration
                match simple with
                | SynTypeDefnSimpleRepr.Union(_, cases, mBody) ->
                    let cases =
                        [
                            for SynUnionCase(ident = SynIdent(id, _); caseType = fldspec) in cases ->
                                let mBody = unionRanges (fldspecRange fldspec) id.idRange
                                createMember (id, NavigationItemKind.Other, FSharpGlyph.Struct, mBody, NavigationEntityKind.Union, false, access)
                        ]

                    let nested = cases @ topMembers
                    let mBody = bodyRange mBody nested

                    [
                        createTypeDecl (baseName, lid, FSharpGlyph.Union, m, mBody, nested, NavigationEntityKind.Union, access)
                    ]

                | SynTypeDefnSimpleRepr.Enum(cases, mBody) ->
                    let cases =
                        [
                            for SynEnumCase(ident = SynIdent(id, _); range = m) in cases ->
                                createMember (id, NavigationItemKind.Field, FSharpGlyph.EnumMember, m, NavigationEntityKind.Enum, false, access)
                        ]

                    let nested = cases @ topMembers
                    let mBody = bodyRange mBody nested

                    [
                        createTypeDecl (baseName, lid, FSharpGlyph.Enum, m, mBody, nested, NavigationEntityKind.Enum, access)
                    ]

                | SynTypeDefnSimpleRepr.Record(_, fields, mBody) ->
                    let fields =
                        [
                            for SynField(idOpt = id; range = m) in fields do
                                match id with
                                | Some ident -> yield createMember (ident, NavigationItemKind.Field, FSharpGlyph.Field, m, NavigationEntityKind.Record, false, access)
                                | _ -> ()
                        ]

                    let nested = fields @ topMembers
                    let mBody = bodyRange mBody nested

                    [
                        createTypeDecl (baseName, lid, FSharpGlyph.Type, m, mBody, nested, NavigationEntityKind.Record, access)
                    ]

                | SynTypeDefnSimpleRepr.TypeAbbrev(_, _, mBody) ->
                    let mBody = bodyRange mBody topMembers

                    [
                        createTypeDecl (baseName, lid, FSharpGlyph.Typedef, m, mBody, topMembers, NavigationEntityKind.Class, access)
                    ]

                //| SynTypeDefnSimpleRepr.General of TyconKind * (SynType * Range * ident option) list * (valSpfn * MemberFlags) list * fieldDecls * bool * bool * Range
                //| SynTypeDefnSimpleRepr.LibraryOnlyILAssembly of ILType * Range
                //| TyconCore_repr_hidden of Range
                | _ -> []

        // Returns class-members for the right dropdown
        and processMembers members enclosingEntityKind =
            let members =
                members
                |> List.map (fun md ->
                    md.Range,
                    (match md with
                     | SynMemberDefn.LetBindings(binds, _, _, _) -> List.collect (processBinding false enclosingEntityKind false) binds
                     | SynMemberDefn.GetSetMember(Some bind, None, _, _)
                     | SynMemberDefn.GetSetMember(None, Some bind, _, _)
                     | SynMemberDefn.Member(bind, _) -> processBinding true enclosingEntityKind false bind
                     | SynMemberDefn.ValField(fieldInfo = SynField(idOpt = Some rcid; accessibility = access; range = range)) ->
                         [
                             createMember (rcid, NavigationItemKind.Field, FSharpGlyph.Field, range, enclosingEntityKind, false, access)
                         ]
                     | SynMemberDefn.AutoProperty(ident = id; accessibility = access; propKind = propKind) ->
                         let getterAccessibility, setterAccessibility = access.GetSetAccessNoCheck()

                         [
                             match propKind with
                             | SynMemberKind.PropertyGetSet ->
                                 yield createMember (id, NavigationItemKind.Field, FSharpGlyph.Field, id.idRange, enclosingEntityKind, false, getterAccessibility)
                                 yield createMember (id, NavigationItemKind.Field, FSharpGlyph.Field, id.idRange, enclosingEntityKind, false, setterAccessibility)
                             | SynMemberKind.PropertySet ->
                                 yield createMember (id, NavigationItemKind.Field, FSharpGlyph.Field, id.idRange, enclosingEntityKind, false, setterAccessibility)
                             | _ -> yield createMember (id, NavigationItemKind.Field, FSharpGlyph.Field, id.idRange, enclosingEntityKind, false, getterAccessibility)
                         ]
                     | SynMemberDefn.AbstractSlot(slotSig = SynValSig(ident = SynIdent(id, _); synType = ty; accessibility = access)) ->
                         [
                             createMember (id, NavigationItemKind.Method, FSharpGlyph.OverridenMethod, ty.Range, enclosingEntityKind, true, access.SingleAccess())
                         ]
                     | SynMemberDefn.NestedType _ -> failwith "tycon as member????" //processTycon tycon
                     | SynMemberDefn.Interface(members = Some(membs)) -> processMembers membs enclosingEntityKind |> snd
                     | SynMemberDefn.GetSetMember(Some getBinding, Some setBinding, _, _) ->
                         [
                             yield! processBinding true enclosingEntityKind false getBinding
                             yield! processBinding true enclosingEntityKind false setBinding
                         ]
                     | _ -> []))

            let m2 = members |> Seq.map fst |> Seq.fold unionRangesChecked range.Zero
            let items = members |> List.collect snd
            m2, items

        // Process declarations in a module that belong to the right drop-down (let bindings)
        let processNestedDeclarations decls =
            [
                for decl in decls do
                    match decl with
                    | SynModuleDecl.Let(_, binds, _) ->
                        for bind in binds do
                            yield! processBinding false NavigationEntityKind.Module false bind
                    | _ -> ()
            ]

        // Process declarations nested in a module that should be displayed in the left dropdown
        // (such as type declarations, nested modules etc.)
        let rec processNavigationTopLevelDeclarations (baseName, decls) =
            [
                for decl in decls do
                    match decl with
                    | SynModuleDecl.ModuleAbbrev(id, lid, m) ->
                        let mBody = rangeOfLid lid
                        createDecl (baseName, id, NavigationItemKind.Module, FSharpGlyph.Module, m, mBody, [], NavigationEntityKind.Namespace, false, None)

                    | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = lid; accessibility = access); decls = decls; range = m) ->
                        // Find let bindings (for the right dropdown)
                        let nested = processNestedDeclarations (decls)

                        let newBaseName =
                            (if (String.IsNullOrEmpty(baseName)) then
                                 ""
                             else
                                 baseName + ".")
                            + (textOfLid lid)

                        let other = processNavigationTopLevelDeclarations (newBaseName, decls)

                        let mBody = unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid lid) other)
                        createDeclLid (baseName, lid, NavigationItemKind.Module, FSharpGlyph.Module, m, mBody, nested, NavigationEntityKind.Module, access)
                        // Get nested modules and types (for the left dropdown)
                        yield! other

                    | SynModuleDecl.Types(tydefs, _) ->
                        for tydef in tydefs do
                            yield! processTycon baseName tydef
                    | SynModuleDecl.Exception(defn, _) -> yield! processExnDefn baseName defn
                    | _ -> ()
            ]

        // Collect all the items
        let items =
            // Show base name for this module only if it's not the root one
            let singleTopLevel = (modules.Length = 1)

            [
                for modul in modules do
                    let (SynModuleOrNamespace(id, _isRec, kind, decls, _, _, access, m, _)) = modul
                    let baseName = if (not singleTopLevel) then textOfLid id else ""
                    // Find let bindings (for the right dropdown)
                    let nested = processNestedDeclarations (decls)
                    // Get nested modules and types (for the left dropdown)
                    let other = processNavigationTopLevelDeclarations (baseName, decls)

                    // Create explicitly - it can be 'single top level' thing that is hidden
                    if not (List.isEmpty id) then
                        let kind =
                            if kind.IsModule then
                                NavigationItemKind.ModuleFile
                            else
                                NavigationItemKind.Namespace

                        let mBody = unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid id) other)
                        let nm = textOfLid id

                        let item =
                            NavigationItem.Create(nm, kind, FSharpGlyph.Module, m, mBody, singleTopLevel, NavigationEntityKind.Module, false, access)

                        let decl = (item, addItemName (nm), nested)
                        decl

                    yield! other
            ]

        let items =
            [|
                for (d, idx, nested) in items do
                    let nested =
                        nested
                        |> Array.ofList
                        |> Array.map (fun (decl, idx) -> decl.WithUniqueName(uniqueName d.LogicalName idx))

                    nested |> Array.sortInPlaceWith (fun a b -> compare a.LogicalName b.LogicalName)

                    {
                        Declaration = d.WithUniqueName(uniqueName d.LogicalName idx)
                        Nested = nested
                    }
            |]

        items
        |> Array.sortInPlaceWith (fun a b -> compare a.Declaration.LogicalName b.Declaration.LogicalName)

        NavigationItems(items)

    /// Get information for signature file
    let getNavigationFromSigFile (modules: SynModuleOrNamespaceSig list) =
        // Map for dealing with name conflicts
        let mutable nameMap = Map.empty

        let addItemName name =
            let count = defaultArg (nameMap |> Map.tryFind name) 0
            nameMap <- (Map.add name (count + 1) nameMap)
            (count + 1)

        let uniqueName name idx =
            let total = Map.find name nameMap
            sprintf "%s_%d_of_%d" name idx total

        // Create declaration (for the left dropdown)
        let createDeclLid (baseName, lid, kind, baseGlyph, m, mBody, nested, enclosingEntityKind, access) =
            let name = (if baseName <> "" then baseName + "." else "") + (textOfLid lid)
            let item = NavigationItem.Create(name, kind, baseGlyph, m, mBody, false, enclosingEntityKind, false, access)
            item, addItemName name, nested

        let createTypeDecl (baseName, lid, baseGlyph, m, mBody, nested, enclosingEntityKind, access) =
            createDeclLid (baseName, lid, NavigationItemKind.Type, baseGlyph, m, mBody, nested, enclosingEntityKind, access)

        let createDecl (baseName, id: Ident, kind, baseGlyph, m, mBody, nested, enclosingEntityKind, isAbstract, access) =
            let name = (if baseName <> "" then baseName + "." else "") + id.idText
            let item = NavigationItem.Create(name, kind, baseGlyph, m, mBody, false, enclosingEntityKind, isAbstract, access)
            item, addItemName name, nested

        let createMember (id: Ident, kind, baseGlyph, m, enclosingEntityKind, isAbstract, access) =
            let item = NavigationItem.Create(id.idText, kind, baseGlyph, m, m, false, enclosingEntityKind, isAbstract, access)
            item, addItemName (id.idText)

        let rec processExnRepr baseName nested inp =
            let (SynExceptionDefnRepr(_, SynUnionCase(ident = SynIdent(id, _); caseType = fldspec), _, _, access, m)) = inp
            let mBody = fldspecRange fldspec

            [
                createDecl (baseName, id, NavigationItemKind.Exception, FSharpGlyph.Exception, m, mBody, nested, NavigationEntityKind.Exception, false, access)
            ]

        and processExnSig baseName inp =
            let (SynExceptionSig(exnRepr = repr; members = memberSigs)) = inp
            let nested = processSigMembers memberSigs
            processExnRepr baseName nested repr

        and processTycon baseName inp =
            let (SynTypeDefnSig(typeInfo = SynComponentInfo(longId = lid; accessibility = access); typeRepr = repr; members = membDefns; range = m)) =
                inp

            let topMembers = processSigMembers membDefns

            [
                match repr with
                | SynTypeDefnSigRepr.Exception repr -> yield! processExnRepr baseName [] repr
                | SynTypeDefnSigRepr.ObjectModel(_, membDefns, mBody) ->
                    // F# class declaration
                    let members = processSigMembers membDefns
                    let nested = members @ topMembers
                    let mBody = bodyRange mBody nested
                    createTypeDecl (baseName, lid, FSharpGlyph.Class, m, mBody, nested, NavigationEntityKind.Class, access)
                | SynTypeDefnSigRepr.Simple(simple, _) ->
                    // F# type declaration
                    match simple with
                    | SynTypeDefnSimpleRepr.Union(_, cases, mBody) ->
                        let cases =
                            [
                                for SynUnionCase(ident = SynIdent(id, _); caseType = fldspec) in cases ->
                                    let m = unionRanges (fldspecRange fldspec) id.idRange
                                    createMember (id, NavigationItemKind.Other, FSharpGlyph.Struct, m, NavigationEntityKind.Union, false, access)
                            ]

                        let nested = cases @ topMembers
                        let mBody = bodyRange mBody nested
                        createTypeDecl (baseName, lid, FSharpGlyph.Union, m, mBody, nested, NavigationEntityKind.Union, access)
                    | SynTypeDefnSimpleRepr.Enum(cases, mBody) ->
                        let cases =
                            [
                                for SynEnumCase(ident = SynIdent(id, _); range = m) in cases ->
                                    createMember (id, NavigationItemKind.Field, FSharpGlyph.EnumMember, m, NavigationEntityKind.Enum, false, access)
                            ]

                        let nested = cases @ topMembers
                        let mBody = bodyRange mBody nested
                        createTypeDecl (baseName, lid, FSharpGlyph.Enum, m, mBody, nested, NavigationEntityKind.Enum, access)
                    | SynTypeDefnSimpleRepr.Record(_, fields, mBody) ->
                        let fields =
                            [
                                for SynField(idOpt = id; range = m) in fields do
                                    match id with
                                    | Some ident -> yield createMember (ident, NavigationItemKind.Field, FSharpGlyph.Field, m, NavigationEntityKind.Record, false, access)
                                    | _ -> ()
                            ]

                        let nested = fields @ topMembers
                        let mBody = bodyRange mBody nested
                        createTypeDecl (baseName, lid, FSharpGlyph.Type, m, mBody, nested, NavigationEntityKind.Record, access)
                    | SynTypeDefnSimpleRepr.TypeAbbrev(_, _, mBody) ->
                        let mBody = bodyRange mBody topMembers
                        createTypeDecl (baseName, lid, FSharpGlyph.Typedef, m, mBody, topMembers, NavigationEntityKind.Class, access)

                    //| SynTypeDefnSimpleRepr.General of TyconKind * (SynType * range * ident option) list * (valSpfn * MemberFlags) list * fieldDecls * bool * bool * range
                    //| SynTypeDefnSimpleRepr.LibraryOnlyILAssembly of ILType * range
                    //| TyconCore_repr_hidden of range
                    | _ -> ()
            ]

        and processSigMembers (members: SynMemberSig list) =
            [
                for memb in members do
                    match memb with
                    | SynMemberSig.Member(memberSig = SynValSig.SynValSig(ident = SynIdent(id, _); accessibility = access; range = m); flags = { MemberKind = propKind }) ->
                        let getterAccessibility, setterAccessibility = access.GetSetAccessNoCheck()

                        match propKind with
                        | SynMemberKind.PropertyGetSet ->
                            yield createMember (id, NavigationItemKind.Method, FSharpGlyph.Method, m, NavigationEntityKind.Class, false, getterAccessibility)
                            yield createMember (id, NavigationItemKind.Method, FSharpGlyph.Method, m, NavigationEntityKind.Class, false, setterAccessibility)
                        | SynMemberKind.PropertySet ->
                            yield createMember (id, NavigationItemKind.Method, FSharpGlyph.Method, m, NavigationEntityKind.Class, false, setterAccessibility)
                        | _ -> yield createMember (id, NavigationItemKind.Method, FSharpGlyph.Method, m, NavigationEntityKind.Class, false, getterAccessibility)
                    | SynMemberSig.ValField(SynField(idOpt = Some rcid; fieldType = ty; accessibility = access), _) ->
                        yield createMember (rcid, NavigationItemKind.Field, FSharpGlyph.Field, ty.Range, NavigationEntityKind.Class, false, access)
                    | _ -> ()
            ]

        // Process declarations in a module that belong to the right drop-down (let bindings)
        let processNestedSigDeclarations decls =
            [
                for decl in decls do
                    match decl with
                    | SynModuleSigDecl.Val(SynValSig.SynValSig(ident = SynIdent(id, _); accessibility = access; range = m), _) ->
                        createMember (id, NavigationItemKind.Method, FSharpGlyph.Method, m, NavigationEntityKind.Module, false, access.SingleAccess())
                    | _ -> ()
            ]

        // Process declarations nested in a module that should be displayed in the left dropdown
        // (such as type declarations, nested modules etc.)
        let rec processNavigationTopLevelSigDeclarations (baseName, decls) =
            [
                for decl in decls do
                    match decl with
                    | SynModuleSigDecl.ModuleAbbrev(id, lid, m) ->
                        let mBody = rangeOfLid lid
                        createDecl (baseName, id, NavigationItemKind.Module, FSharpGlyph.Module, m, mBody, [], NavigationEntityKind.Module, false, None)

                    | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo(longId = lid; accessibility = access); moduleDecls = decls; range = m) ->
                        // Find let bindings (for the right dropdown)
                        let nested = processNestedSigDeclarations (decls)

                        let newBaseName =
                            (if String.IsNullOrEmpty(baseName) then
                                 ""
                             else
                                 baseName + ".")
                            + (textOfLid lid)

                        let other = processNavigationTopLevelSigDeclarations (newBaseName, decls)

                        // Get nested modules and types (for the left dropdown)
                        let mBody = unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid lid) other)
                        createDeclLid (baseName, lid, NavigationItemKind.Module, FSharpGlyph.Module, m, mBody, nested, NavigationEntityKind.Module, access)
                        yield! other

                    | SynModuleSigDecl.Types(tydefs, _) ->
                        for tydef in tydefs do
                            yield! processTycon baseName tydef
                    | SynModuleSigDecl.Exception(defn, _) -> yield! processExnSig baseName defn
                    | _ -> ()
            ]

        // Collect all the items
        let items =
            // Show base name for this module only if it's not the root one
            let singleTopLevel = (modules.Length = 1)

            [
                for modulSig in modules do
                    let (SynModuleOrNamespaceSig(id, _isRec, kind, decls, _, _, access, m, _)) = modulSig
                    let baseName = if (not singleTopLevel) then textOfLid id else ""
                    // Find let bindings (for the right dropdown)
                    let nested = processNestedSigDeclarations (decls)
                    // Get nested modules and types (for the left dropdown)
                    let other = processNavigationTopLevelSigDeclarations (baseName, decls)

                    // Create explicitly - it can be 'single top level' thing that is hidden
                    let kind =
                        if kind.IsModule then
                            NavigationItemKind.ModuleFile
                        else
                            NavigationItemKind.Namespace

                    let mBody = unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid id) other)

                    let item =
                        NavigationItem.Create(textOfLid id, kind, FSharpGlyph.Module, m, mBody, singleTopLevel, NavigationEntityKind.Module, false, access)

                    let decl = (item, addItemName (textOfLid id), nested)
                    decl
                    yield! other
            ]

        let items =
            [|
                for (d, idx, nested) in items do
                    let nested =
                        nested
                        |> Array.ofList
                        |> Array.map (fun (decl, idx) -> decl.WithUniqueName(uniqueName d.LogicalName idx))

                    nested |> Array.sortInPlaceWith (fun a b -> compare a.LogicalName b.LogicalName)

                    let nested =
                        nested
                        |> Array.distinctBy (fun x -> x.Range, x.BodyRange, x.LogicalName, x.Kind)

                    {
                        Declaration = d.WithUniqueName(uniqueName d.LogicalName idx)
                        Nested = nested
                    }
            |]

        items
        |> Array.sortInPlaceWith (fun a b -> compare a.Declaration.LogicalName b.Declaration.LogicalName)

        NavigationItems(items)

[<RequireQualifiedAccess>]
module Navigation =
    let getNavigation (parsedInput: ParsedInput) =
        match parsedInput with
        | ParsedInput.SigFile file -> NavigationImpl.getNavigationFromSigFile file.Contents
        | ParsedInput.ImplFile file -> NavigationImpl.getNavigationFromImplFile file.Contents

    let empty = NavigationItems([||])

[<RequireQualifiedAccess>]
type NavigableItemKind =
    | Module
    | ModuleAbbreviation
    | Exception
    | Type
    | ModuleValue
    | Field
    | Property
    | Constructor
    | Member
    | EnumCase
    | UnionCase

    override x.ToString() = sprintf "%+A" x

[<RequireQualifiedAccess>]
type NavigableContainerType =
    | File
    | Namespace
    | Module
    | Type
    | Exception

type NavigableContainer =
    | File of fileName: string
    | Container of containerType: NavigableContainerType * nameParts: string list * parent: NavigableContainer

    member x.FullName =
        let rec loop acc =
            function
            | File _ -> acc
            | Container(_, nameParts, parent) -> loop (nameParts @ acc) parent

        loop [] x |> textOfPath

    member x.Type =
        match x with
        | File _ -> NavigableContainerType.File
        | Container(t, _, _) -> t

    member x.Name =
        match x with
        | File name -> name
        | Container(nameParts = []) -> ""
        | Container(nameParts = ns) -> ns |> List.last

type NavigableItem =
    {
        Name: string
        NeedsBackticks: bool
        Range: range
        IsSignature: bool
        Kind: NavigableItemKind
        Container: NavigableContainer
    }

[<RequireQualifiedAccess>]
module NavigateTo =
    let GetNavigableItems (parsedInput: ParsedInput) : NavigableItem[] =

        let convertToDisplayName name =
            if PrettyNaming.IsLogicalOpName name then
                PrettyNaming.ConvertValLogicalNameToDisplayNameCore name
            else
                name

        let result = ResizeArray()

        let addLongIdent kind (lid: LongIdent) (isSignature: bool) (container: NavigableContainer) =
            if not lid.IsEmpty then
                let name = textOfLid lid

                {
                    Name = name
                    NeedsBackticks = pathOfLid lid |> List.exists PrettyNaming.DoesIdentifierNeedBackticks
                    Range = rangeOfLid lid
                    IsSignature = isSignature
                    Kind = kind
                    Container = container
                }
                |> result.Add

        let addIdent kind (id: Ident) (isSignature: bool) (container: NavigableContainer) =
            if not (String.IsNullOrEmpty id.idText) then
                let name = convertToDisplayName id.idText

                {
                    Name = name
                    NeedsBackticks = PrettyNaming.DoesIdentifierNeedBackticks name
                    Range = id.idRange
                    IsSignature = isSignature
                    Kind = kind
                    Container = container
                }
                |> result.Add

        let addModule lid isSig container =
            addLongIdent NavigableItemKind.Module lid isSig container

        let addModuleAbbreviation (id: Ident) isSig container =
            addIdent NavigableItemKind.ModuleAbbreviation id isSig container

        let addExceptionRepr exnRepr isSig container =
            let (SynExceptionDefnRepr(_, SynUnionCase(ident = SynIdent(id, _)), _, _, _, _)) = exnRepr
            addIdent NavigableItemKind.Exception id isSig container
            NavigableContainer.Container(NavigableContainerType.Exception, [ id.idText ], container)

        let addComponentInfo containerType kind info isSig container =
            let (SynComponentInfo(longId = lid)) = info
            addLongIdent kind lid isSig container

            NavigableContainer.Container(containerType, pathOfLid lid, container)

        let addValSig kind synValSig isSig container =
            let (SynValSig(ident = SynIdent(id, _))) = synValSig
            addIdent kind id isSig container

        let addField synField isSig container =
            let (SynField(idOpt = id)) = synField

            match id with
            | Some id -> addIdent NavigableItemKind.Field id isSig container
            | _ -> ()

        let addEnumCase inp isSig =
            let (SynEnumCase(ident = SynIdent(id, _))) = inp
            addIdent NavigableItemKind.EnumCase id isSig

        let addUnionCase synUnionCase isSig container =
            let (SynUnionCase(ident = SynIdent(id, _))) = synUnionCase
            addIdent NavigableItemKind.UnionCase id isSig container

        let mapMemberKind mk =
            match mk with
            | SynMemberKind.ClassConstructor // ?
            | SynMemberKind.Constructor -> NavigableItemKind.Constructor
            | SynMemberKind.PropertyGet
            | SynMemberKind.PropertySet
            | SynMemberKind.PropertyGetSet -> NavigableItemKind.Property
            | SynMemberKind.Member -> NavigableItemKind.Member

        let addBinding synBinding itemKind container =
            let (SynBinding(valData = valData; headPat = headPat)) = synBinding
            let (SynValData(memberFlags = memberFlagsOpt)) = valData

            let kind =
                match itemKind with
                | Some x -> x
                | _ ->
                    match memberFlagsOpt with
                    | Some mf -> mapMemberKind mf.MemberKind
                    | _ -> NavigableItemKind.ModuleValue

            match headPat with
            | SynPat.LongIdent(longDotId = SynLongIdent([ _; id ], _, _)) ->
                // instance members
                addIdent kind id false container
            | SynPat.LongIdent(longDotId = SynLongIdent([ id ], _, _)) ->
                // functions
                addIdent kind id false container
            | SynPat.Named(SynIdent(id, _), _, _, _)
            | SynPat.As(_, SynPat.Named(SynIdent(id, _), _, _, _), _) ->
                // values
                addIdent kind id false container
            | _ -> ()

        let addMember valSig (memberFlags: SynMemberFlags) isSig container =
            let ctor = mapMemberKind memberFlags.MemberKind
            addValSig ctor valSig isSig container

        let rec walkSigFileInput (file: ParsedSigFileInput) =

            for item in file.Contents do
                walkSynModuleOrNamespaceSig item (NavigableContainer.File file.FileName)

        and walkSynModuleOrNamespaceSig (inp: SynModuleOrNamespaceSig) container =
            let (SynModuleOrNamespaceSig(longId = lid; kind = kind; decls = decls)) = inp
            let isModule = kind.IsModule

            if isModule then
                addModule lid true container

            let ctype =
                if isModule then
                    NavigableContainerType.Module
                else
                    NavigableContainerType.Namespace

            for decl in decls do
                walkSynModuleSigDecl decl (NavigableContainer.Container(ctype, pathOfLid lid, container))

        and walkSynModuleSigDecl (decl: SynModuleSigDecl) container =
            match decl with
            | SynModuleSigDecl.ModuleAbbrev(lhs, _, _range) -> addModuleAbbreviation lhs true container
            | SynModuleSigDecl.Exception(exnSig = SynExceptionSig(exnRepr = representation)) -> addExceptionRepr representation true container |> ignore
            | SynModuleSigDecl.NamespaceFragment fragment -> walkSynModuleOrNamespaceSig fragment container
            | SynModuleSigDecl.NestedModule(moduleInfo = componentInfo; moduleDecls = nestedDecls) ->
                let container = addComponentInfo NavigableContainerType.Module NavigableItemKind.Module componentInfo true container

                for decl in nestedDecls do
                    walkSynModuleSigDecl decl container
            | SynModuleSigDecl.Types(types, _) ->
                for ty in types do
                    walkSynTypeDefnSig ty container
            | SynModuleSigDecl.Val(valSig, _range) -> addValSig NavigableItemKind.ModuleValue valSig true container
            | SynModuleSigDecl.HashDirective _
            | SynModuleSigDecl.Open _ -> ()

        and walkSynTypeDefnSig (inp: SynTypeDefnSig) container =
            let (SynTypeDefnSig(typeInfo = componentInfo; typeRepr = repr; members = members)) = inp
            let container = addComponentInfo NavigableContainerType.Type NavigableItemKind.Type componentInfo true container

            for m in members do
                walkSynMemberSig m container

            match repr with
            | SynTypeDefnSigRepr.ObjectModel(_, membersSigs, _) ->
                for m in membersSigs do
                    walkSynMemberSig m container
            | SynTypeDefnSigRepr.Simple(repr, _) -> walkSynTypeDefnSimpleRepr repr true container
            | SynTypeDefnSigRepr.Exception _ -> ()

        and walkSynMemberSig (synMemberSig: SynMemberSig) container =
            match synMemberSig with
            | SynMemberSig.Member(memberSig = valSig; flags = memberFlags) -> addMember valSig memberFlags true container
            | SynMemberSig.ValField(synField, _) -> addField synField true container
            | SynMemberSig.NestedType(synTypeDef, _) -> walkSynTypeDefnSig synTypeDef container
            | SynMemberSig.Inherit _
            | SynMemberSig.Interface _ -> ()

        and walkImplFileInput (inp: ParsedImplFileInput) =
            for item in inp.Contents do
                walkSynModuleOrNamespace item (NavigableContainer.File inp.FileName)

        and walkSynModuleOrNamespace inp container =
            let (SynModuleOrNamespace(longId = lid; kind = kind; decls = decls)) = inp
            let isModule = kind.IsModule

            if isModule then
                addModule lid false container

            let ctype =
                if isModule then
                    NavigableContainerType.Module
                else
                    NavigableContainerType.Namespace

            for decl in decls do
                walkSynModuleDecl decl (NavigableContainer.Container(ctype, pathOfLid lid, container))

        and walkSynModuleDecl (decl: SynModuleDecl) container =
            match decl with
            | SynModuleDecl.Exception(SynExceptionDefn(repr, _, synMembers, _), _) ->
                let container = addExceptionRepr repr false container

                for m in synMembers do
                    walkSynMemberDefn m container
            | SynModuleDecl.Let(_, bindings, _) ->
                for binding in bindings do
                    addBinding binding None container
            | SynModuleDecl.ModuleAbbrev(lhs, _, _) -> addModuleAbbreviation lhs false container
            | SynModuleDecl.NamespaceFragment(fragment) -> walkSynModuleOrNamespace fragment container
            | SynModuleDecl.NestedModule(moduleInfo = componentInfo; decls = modules) ->
                let container = addComponentInfo NavigableContainerType.Module NavigableItemKind.Module componentInfo false container

                for m in modules do
                    walkSynModuleDecl m container
            | SynModuleDecl.Types(typeDefs, _range) ->
                for t in typeDefs do
                    walkSynTypeDefn t container
            | SynModuleDecl.Attributes _
            | SynModuleDecl.Expr _
            | SynModuleDecl.HashDirective _
            | SynModuleDecl.Open _ -> ()

        and walkSynTypeDefn inp container =
            let (SynTypeDefn(typeInfo = componentInfo; typeRepr = representation; members = members)) = inp
            let container = addComponentInfo NavigableContainerType.Type NavigableItemKind.Type componentInfo false container
            walkSynTypeDefnRepr representation container

            for m in members do
                walkSynMemberDefn m container

        and walkSynTypeDefnRepr (typeDefnRepr: SynTypeDefnRepr) container =
            match typeDefnRepr with
            | SynTypeDefnRepr.ObjectModel(_, members, _) ->
                for m in members do
                    walkSynMemberDefn m container
            | SynTypeDefnRepr.Simple(repr, _) -> walkSynTypeDefnSimpleRepr repr false container
            | SynTypeDefnRepr.Exception _ -> ()

        and walkSynTypeDefnSimpleRepr (repr: SynTypeDefnSimpleRepr) isSig container =
            match repr with
            | SynTypeDefnSimpleRepr.Enum(enumCases, _) ->
                for c in enumCases do
                    addEnumCase c isSig container
            | SynTypeDefnSimpleRepr.Record(_, fields, _) ->
                for f in fields do
                    // TODO: add specific case for record field?
                    addField f isSig container
            | SynTypeDefnSimpleRepr.Union(_, unionCases, _) ->
                for uc in unionCases do
                    addUnionCase uc isSig container
            | SynTypeDefnSimpleRepr.General _
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _
            | SynTypeDefnSimpleRepr.None _
            | SynTypeDefnSimpleRepr.TypeAbbrev _
            | SynTypeDefnSimpleRepr.Exception _ -> ()

        and walkSynMemberDefn (memberDefn: SynMemberDefn) container =
            match memberDefn with
            | SynMemberDefn.AbstractSlot(slotSig = synValSig; flags = memberFlags) -> addMember synValSig memberFlags false container
            | SynMemberDefn.AutoProperty(ident = id) -> addIdent NavigableItemKind.Property id false container
            | SynMemberDefn.Interface(members = members) ->
                match members with
                | Some members ->
                    for m in members do
                        walkSynMemberDefn m container
                | None -> ()
            | SynMemberDefn.Member(binding, _) -> addBinding binding None container
            | SynMemberDefn.GetSetMember(getBinding, setBinding, _, _) ->
                Option.iter (fun b -> addBinding b None container) getBinding
                Option.iter (fun b -> addBinding b None container) setBinding
            | SynMemberDefn.NestedType(typeDef, _, _) -> walkSynTypeDefn typeDef container
            | SynMemberDefn.ValField(fieldInfo = field) -> addField field false container
            | SynMemberDefn.LetBindings(bindings, _, _, _) ->
                bindings
                |> List.iter (fun binding -> addBinding binding (Some NavigableItemKind.Field) container)
            | SynMemberDefn.Open _
            | SynMemberDefn.ImplicitInherit _
            | SynMemberDefn.Inherit _
            | SynMemberDefn.ImplicitCtor _ -> ()

        match parsedInput with
        | ParsedInput.SigFile input -> walkSigFileInput input
        | ParsedInput.ImplFile input -> walkImplFileInput input

        result.ToArray()
