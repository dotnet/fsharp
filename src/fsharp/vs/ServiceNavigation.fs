// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons 

/// Represents the differnt kinds of items that can appear in the navigation bar
type FSharpNavigationDeclarationItemKind =
    | NamespaceDecl
    | ModuleFileDecl
    | ExnDecl
    | ModuleDecl
    | TypeDecl
    | MethodDecl
    | PropertyDecl
    | FieldDecl
    | OtherDecl

/// Represents an item to be displayed in the navigation bar
[<Sealed>]
type FSharpNavigationDeclarationItem(uniqueName: string, name: string, kind: FSharpNavigationDeclarationItemKind, glyph: GlyphMajor, range: range, 
                                     bodyRange: range, singleTopLevel: bool, access: SynAccess option) = 
    
    member x.bodyRange = bodyRange
    member x.UniqueName = uniqueName
    member x.Name = name
    member x.Glyph = int glyph * 6
    member x.GlyphMajor = glyph 
    member x.Kind = kind
    member x.Range = range
    member x.BodyRange = bodyRange 
    member x.IsSingleTopLevel = singleTopLevel
    member x.Access = access
    member x.WithUniqueName(uniqueName: string) =
      FSharpNavigationDeclarationItem(uniqueName, name, kind, glyph, range, bodyRange, singleTopLevel, access)
    static member Create(name: string, kind, glyph: GlyphMajor, range: range, bodyRange: range, singleTopLevel: bool, access: SynAccess option) = 
      FSharpNavigationDeclarationItem("", name, kind, glyph, range, bodyRange, singleTopLevel, access)

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type FSharpNavigationTopLevelDeclaration = 
    { Declaration: FSharpNavigationDeclarationItem
      Nested: FSharpNavigationDeclarationItem[] }
      
/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type FSharpNavigationItems(declarations:FSharpNavigationTopLevelDeclaration[]) =
    member x.Declarations = declarations

module NavigationImpl =
    let unionRangesChecked r1 r2 = if r1 = range.Zero then r2 elif r2 = range.Zero then r1 else unionRanges r1 r2
    
    let rangeOfDecls2 f decls = 
      match (decls |> List.map (f >> (fun (d:FSharpNavigationDeclarationItem) -> d.bodyRange))) with 
      | hd::tl -> tl |> List.fold unionRangesChecked hd
      | [] -> range.Zero
    
    let rangeOfDecls = rangeOfDecls2 fst

    let moduleRange (idm:range) others = 
      unionRangesChecked idm.EndRange (rangeOfDecls2 (fun (a, _, _) -> a) others)
    
    let fldspecRange fldspec =
      match fldspec with
      | UnionCaseFields(flds) -> flds |> List.fold (fun st (Field(_, _, _, _, _, _, _, m)) -> unionRangesChecked m st) range.Zero
      | UnionCaseFullType(ty, _) -> ty.Range
      
    let bodyRange mb decls =
      unionRangesChecked (rangeOfDecls decls) mb
          
    /// Get information for implementation file      
    let getNavigationFromImplFile (modules: SynModuleOrNamespace list) =
        // Map for dealing with name conflicts
        let nameMap = ref Map.empty 

        let addItemName name = 
            let count = defaultArg (!nameMap |> Map.tryFind name) 0
            nameMap := (Map.add name (count + 1) (!nameMap))
            (count + 1)
        
        let uniqueName name idx = 
            let total = Map.find name (!nameMap)
            sprintf "%s_%d_of_%d" name idx total

        // Create declaration (for the left dropdown)                
        let createDeclLid(baseName, lid, kind, baseGlyph, m, bodym, nested, access) =
            let name = (if baseName <> "" then baseName + "." else "") + (textOfLid lid)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph, m, bodym, false, access), (addItemName name), nested
            
        let createDecl(baseName, id:Ident, kind, baseGlyph, m, bodym, nested, access) =
            let name = (if baseName <> "" then baseName + "." else "") + (id.idText)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph, m, bodym, false, access), (addItemName name), nested
         
        // Create member-kind-of-thing for the right dropdown
        let createMemberLid(lid, kind, baseGlyph, m, access) =
            FSharpNavigationDeclarationItem.Create(textOfLid lid, kind, baseGlyph, m, m, false, access), (addItemName(textOfLid lid))

        let createMember(id:Ident, kind, baseGlyph, m, access) =
            FSharpNavigationDeclarationItem.Create(id.idText, kind, baseGlyph, m, m, false, access), (addItemName(id.idText))

        // Process let-binding
        let processBinding isMember (Binding(_, _, _, _, _, _, SynValData(memebrOpt, _, _), synPat, _, synExpr, _, _)) =
            let m = 
                match synExpr with 
                | SynExpr.Typed(e, _, _) -> e.Range // fix range for properties with type annotations
                | _ -> synExpr.Range

            match synPat, memebrOpt with
            | SynPat.LongIdent(longDotId=LongIdentWithDots(lid,_); accessibility=access), Some(flags) when isMember -> 
                let icon, kind =
                  match flags.MemberKind with
                  | MemberKind.ClassConstructor
                  | MemberKind.Constructor
                  | MemberKind.Member -> 
                        (if flags.IsOverrideOrExplicitImpl then GlyphMajor.Method2 else GlyphMajor.Method), MethodDecl
                  | MemberKind.PropertyGetSet
                  | MemberKind.PropertySet
                  | MemberKind.PropertyGet -> GlyphMajor.Property, PropertyDecl
                let lidShow, rangeMerge = 
                  match lid with 
                  | _thisVar::nm::_ -> (List.tail lid, nm.idRange) 
                  | hd::_ -> (lid, hd.idRange) 
                  | _ -> (lid, m)
                [ createMemberLid(lidShow, kind, icon, unionRanges rangeMerge m, access) ]
            | SynPat.LongIdent(LongIdentWithDots(lid,_), _, _, _, access, _), _ -> 
                [ createMemberLid(lid, FieldDecl, GlyphMajor.FieldBlue, unionRanges (List.head lid).idRange m, access) ]
            | SynPat.Named(_, id, _, access, _), _ -> 
                let glyph = if isMember then GlyphMajor.Method else GlyphMajor.FieldBlue
                [ createMember(id, FieldDecl, glyph, unionRanges id.idRange m, access) ]
            | _ -> []
        
        // Process a class declaration or F# type declaration
        let rec processExnDefnRepr baseName nested (SynExceptionDefnRepr(_, (UnionCase(_, id, fldspec, _, _, _)), _, _, access, m)) =
            // Exception declaration
            [ createDecl(baseName, id, ExnDecl, GlyphMajor.Exception, m, fldspecRange fldspec, nested, access) ] 

        // Process a class declaration or F# type declaration
        and processExnDefn baseName (SynExceptionDefn(repr, membDefns, _)) =  
            let nested = processMembers membDefns |> snd
            processExnDefnRepr baseName nested repr

        and processTycon baseName (TypeDefn(ComponentInfo(_, _, _, lid, _, _, access, _), repr, membDefns, m)) =
            let topMembers = processMembers membDefns |> snd
            match repr with
            | SynTypeDefnRepr.Exception repr -> processExnDefnRepr baseName [] repr
            | SynTypeDefnRepr.ObjectModel(_, membDefns, mb) ->
                // F# class declaration
                let members = processMembers membDefns |> snd
                let nested = members@topMembers
                ([ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Class, m, bodyRange mb nested, nested, access) ]: ((FSharpNavigationDeclarationItem * int * _) list))
            | SynTypeDefnRepr.Simple(simple, _) ->
                // F# type declaration
                match simple with
                | SynTypeDefnSimpleRepr.Union(_, cases, mb) ->
                    let cases = 
                        [ for (UnionCase(_, id, fldspec, _, _, _)) in cases -> 
                            createMember(id, OtherDecl, GlyphMajor.ValueType, unionRanges (fldspecRange fldspec) id.idRange, access) ]
                    let nested = cases@topMembers              
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Union, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.Enum(cases, mb) -> 
                    let cases = 
                        [ for (EnumCase(_, id, _, _, m)) in cases ->
                            createMember(id, FieldDecl, GlyphMajor.EnumMember, m, access) ]
                    let nested = cases@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Enum, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.Record(_, fields, mb) ->
                    let fields = 
                        [ for (Field(_, _, id, _, _, _, _, m)) in fields do
                            if (id.IsSome) then
                              yield createMember(id.Value, FieldDecl, GlyphMajor.FieldBlue, m, access) ]
                    let nested = fields@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Type, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.TypeAbbrev(_, _, mb) ->
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Typedef, m, bodyRange mb topMembers, topMembers, access) ]
                          
                //| SynTypeDefnSimpleRepr.General of TyconKind * (SynType * range * ident option) list * (valSpfn * MemberFlags) list * fieldDecls * bool * bool * range 
                //| SynTypeDefnSimpleRepr.LibraryOnlyILAssembly of ILType * range
                //| TyconCore_repr_hidden of range
                | _ -> [] 
                  
        // Returns class-members for the right dropdown                  
        and processMembers members: (range * list<FSharpNavigationDeclarationItem * int>) = 
            let members = 
                members 
                |> List.groupBy (fun x -> x.Range)
                |> List.map (fun (range, members) ->
                    range,
                    (match members with
                     | [memb] ->
                         match memb with
                         | SynMemberDefn.LetBindings(binds, _, _, _) -> List.collect (processBinding false) binds
                         | SynMemberDefn.Member(bind, _) -> processBinding true bind
                         | SynMemberDefn.ValField(Field(_, _, Some(rcid), ty, _, _, access, _), _) ->
                             [ createMember(rcid, FieldDecl, GlyphMajor.FieldBlue, ty.Range, access) ]
                         | SynMemberDefn.AutoProperty(_attribs,_isStatic,id,_tyOpt,_propKind,_,_xmlDoc, access,_synExpr, _, _) -> 
                             [ createMember(id, FieldDecl, GlyphMajor.FieldBlue, id.idRange, access) ]
                         | SynMemberDefn.AbstractSlot(ValSpfn(_, id, _, ty, _, _, _, _, access, _, _), _, _) ->
                             [ createMember(id, MethodDecl, GlyphMajor.Method2, ty.Range, access) ]
                         | SynMemberDefn.NestedType _ -> failwith "tycon as member????" //processTycon tycon                
                         | SynMemberDefn.Interface(_, Some(membs), _) ->
                             processMembers membs |> snd
                         | _ -> [] 
                     // can happen if one is a getter and one is a setter
                     | [SynMemberDefn.Member(memberDefn=Binding(headPat=SynPat.LongIdent(lid1, Some(info1),_,_,_,_)) as binding1)
                        SynMemberDefn.Member(memberDefn=Binding(headPat=SynPat.LongIdent(lid2, Some(info2),_,_,_,_)) as binding2)] ->
                         // ensure same long id
                         assert((lid1.Lid,lid2.Lid) ||> List.forall2 (fun x y -> x.idText = y.idText))
                         // ensure one is getter, other is setter
                         assert((info1.idText = "set" && info2.idText = "get") ||
                                (info2.idText = "set" && info1.idText = "get"))
                         // both binding1 and binding2 have same range, so just try the first one, else try the second one
                         match processBinding true binding1 with
                         | [] -> processBinding true binding2
                         | x -> x
                     | _ -> [])) 
            
            (members |> Seq.map fst |> Seq.fold unionRangesChecked range.Zero),
            (members |> List.map snd |> List.concat)

        // Process declarations in a module that belong to the right drop-down (let bindings)
        let processNestedDeclarations decls = decls |> List.collect (function
            | SynModuleDecl.Let(_, binds, _) -> List.collect (processBinding false) binds
            | _ -> [])        

        // Process declarations nested in a module that should be displayed in the left dropdown
        // (such as type declarations, nested modules etc.)                            
        let rec processFSharpNavigationTopLevelDeclarations(baseName, decls) = decls |> List.collect (function
            | SynModuleDecl.ModuleAbbrev(id, lid, m) ->
                [ createDecl(baseName, id, ModuleDecl, GlyphMajor.Module, m, rangeOfLid lid, [], None) ]
                
            | SynModuleDecl.NestedModule(ComponentInfo(_, _, _, lid, _, _, access, _), _isRec, decls, _, m) ->
                // Find let bindings (for the right dropdown)
                let nested = processNestedDeclarations(decls)
                let newBaseName = (if (baseName = "") then "" else baseName+".") + (textOfLid lid)
                
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelDeclarations(newBaseName, decls)
                createDeclLid(baseName, lid, ModuleDecl, GlyphMajor.Module, m, unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid lid) other), nested, access) :: other
                  
            | SynModuleDecl.Types(tydefs, _) -> tydefs |> List.collect (processTycon baseName)                                    
            | SynModuleDecl.Exception (defn,_) -> processExnDefn baseName defn
            | _ -> [])

        // Collect all the items  
        let items = 
            // Show base name for this module only if it's not the root one
            let singleTopLevel = (modules.Length = 1)
            modules |> List.collect (fun (SynModuleOrNamespace(id, _isRec, isModule, decls, _, _, access, m)) ->
                let baseName = if (not singleTopLevel) then textOfLid id else ""
                // Find let bindings (for the right dropdown)
                let nested = processNestedDeclarations(decls)
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelDeclarations(baseName, decls)

                // Create explicitly - it can be 'single top level' thing that is hidden
                match id with
                | [] -> other
                | _ ->
                    let decl =
                        FSharpNavigationDeclarationItem.Create
                            (textOfLid id, (if isModule then ModuleFileDecl else NamespaceDecl),
                                GlyphMajor.Module, m, 
                                unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid id) other), 
                                singleTopLevel, access), (addItemName(textOfLid id)), nested
                    decl::other)
                  
        let items = 
            items 
            |> Array.ofList 
            |> Array.map (fun (d, idx, nest) -> 
                let nest = nest |> Array.ofList |> Array.map (fun (decl, idx) -> decl.WithUniqueName(uniqueName d.Name idx))
                nest |> Array.sortInPlaceWith (fun a b -> compare a.Name b.Name)
                { Declaration = d.WithUniqueName(uniqueName d.Name idx); Nested = nest } )                  
        items |> Array.sortInPlaceWith (fun a b -> compare a.Declaration.Name b.Declaration.Name)
        new FSharpNavigationItems(items)

    /// Get information for signature file      
    let getNavigationFromSigFile (modules: SynModuleOrNamespaceSig list) =
        // Map for dealing with name conflicts
        let nameMap = ref Map.empty 
        let addItemName name = 
            let count = defaultArg (!nameMap |> Map.tryFind name) 0
            nameMap := (Map.add name (count + 1) (!nameMap))
            (count + 1)
        let uniqueName name idx = 
            let total = Map.find name (!nameMap)
            sprintf "%s_%d_of_%d" name idx total

        // Create declaration (for the left dropdown)                
        let createDeclLid(baseName, lid, kind, baseGlyph, m, bodym, nested, access) =
            let name = (if baseName <> "" then baseName + "." else "") + (textOfLid lid)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph, m, bodym, false, access), (addItemName name), nested
            
        let createDecl(baseName, id:Ident, kind, baseGlyph, m, bodym, nested, access) =
            let name = (if baseName <> "" then baseName + "." else "") + (id.idText)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph, m, bodym, false, access), (addItemName name), nested
         
        let createMember(id:Ident, kind, baseGlyph, m, access) =
            FSharpNavigationDeclarationItem.Create(id.idText, kind, baseGlyph, m, m, false, access), (addItemName(id.idText))

        let rec processExnRepr baseName nested (SynExceptionDefnRepr(_, (UnionCase(_, id, fldspec, _, _, _)), _, _, access, m)) =
            // Exception declaration
            [ createDecl(baseName, id, ExnDecl, GlyphMajor.Exception, m, fldspecRange fldspec, nested, access) ] 
        
        and processExnSig baseName (SynExceptionSig(repr, memberSigs, _)) =  
            let nested = processSigMembers memberSigs
            processExnRepr baseName nested repr

        and processTycon baseName (TypeDefnSig(ComponentInfo(_, _, _, lid, _, _, access, _), repr, membDefns, m)) =
            let topMembers = processSigMembers membDefns
            match repr with
            | SynTypeDefnSigRepr.Exception repr -> processExnRepr baseName [] repr
            | SynTypeDefnSigRepr.ObjectModel(_, membDefns, mb) ->
                // F# class declaration
                let members = processSigMembers membDefns
                let nested = members @ topMembers
                ([ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Class, m, bodyRange mb nested, nested, access) ]: ((FSharpNavigationDeclarationItem * int * _) list))
            | SynTypeDefnSigRepr.Simple(simple, _) ->
                // F# type declaration
                match simple with
                | SynTypeDefnSimpleRepr.Union(_, cases, mb) ->
                    let cases = 
                        [ for (UnionCase(_, id, fldspec, _, _, _)) in cases -> 
                            createMember(id, OtherDecl, GlyphMajor.ValueType, unionRanges (fldspecRange fldspec) id.idRange, access) ]
                    let nested = cases@topMembers              
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Union, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.Enum(cases, mb) -> 
                    let cases = 
                        [ for (EnumCase(_, id, _, _, m)) in cases ->
                            createMember(id, FieldDecl, GlyphMajor.EnumMember, m, access) ]
                    let nested = cases@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Enum, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.Record(_, fields, mb) ->
                    let fields = 
                        [ for (Field(_, _, id, _, _, _, _, m)) in fields do
                            if (id.IsSome) then
                              yield createMember(id.Value, FieldDecl, GlyphMajor.FieldBlue, m, access) ]
                    let nested = fields@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Type, m, bodyRange mb nested, nested, access) ]
                | SynTypeDefnSimpleRepr.TypeAbbrev(_, _, mb) ->
                    [ createDeclLid(baseName, lid, TypeDecl, GlyphMajor.Typedef, m, bodyRange mb topMembers, topMembers, access) ]
                          
                //| SynTypeDefnSimpleRepr.General of TyconKind * (SynType * range * ident option) list * (valSpfn * MemberFlags) list * fieldDecls * bool * bool * range 
                //| SynTypeDefnSimpleRepr.LibraryOnlyILAssembly of ILType * range
                //| TyconCore_repr_hidden of range
                | _ -> [] 
                  
        and processSigMembers (members: SynMemberSig list): list<FSharpNavigationDeclarationItem * int> = 
            [ for memb in members do
                 match memb with
                 | SynMemberSig.Member(SynValSig.ValSpfn(_, id, _, _, _, _, _, _, access, _, m), _, _) ->
                     yield createMember(id, MethodDecl, GlyphMajor.Method, m, access)
                 | SynMemberSig.ValField(Field(_, _, Some(rcid), ty, _, _, access, _), _) ->
                     yield createMember(rcid, FieldDecl, GlyphMajor.FieldBlue, ty.Range, access)
                 | _ -> () ]

        // Process declarations in a module that belong to the right drop-down (let bindings)
        let processNestedSigDeclarations decls = decls |> List.collect (function
            | SynModuleSigDecl.Val(SynValSig.ValSpfn(_, id, _, _, _, _, _, _, access, _, m), _) ->
                [ createMember(id, MethodDecl, GlyphMajor.Method, m, access) ]
            | _ -> [] )        

        // Process declarations nested in a module that should be displayed in the left dropdown
        // (such as type declarations, nested modules etc.)                            
        let rec processFSharpNavigationTopLevelSigDeclarations(baseName, decls) = decls |> List.collect (function
            | SynModuleSigDecl.ModuleAbbrev(id, lid, m) ->
                [ createDecl(baseName, id, ModuleDecl, GlyphMajor.Module, m, rangeOfLid lid, [], None) ]
                
            | SynModuleSigDecl.NestedModule(ComponentInfo(_, _, _, lid, _, _, access, _), _, decls, m) ->                
                // Find let bindings (for the right dropdown)
                let nested = processNestedSigDeclarations(decls)
                let newBaseName = (if (baseName = "") then "" else baseName+".") + (textOfLid lid)
                
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelSigDeclarations(newBaseName, decls)
                createDeclLid(baseName, lid, ModuleDecl, GlyphMajor.Module, m, unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid lid) other), nested, access)::other
                  
            | SynModuleSigDecl.Types(tydefs, _) -> tydefs |> List.collect (processTycon baseName)                                    
            | SynModuleSigDecl.Exception (defn,_) -> processExnSig baseName defn
            | _ -> [])
                  
        // Collect all the items  
        let items = 
            // Show base name for this module only if it's not the root one
            let singleTopLevel = (modules.Length = 1)
            modules |> List.collect (fun (SynModuleOrNamespaceSig(id, _isRec, isModule, decls, _, _, access, m)) ->
                let baseName = if (not singleTopLevel) then textOfLid id else ""
                // Find let bindings (for the right dropdown)
                let nested = processNestedSigDeclarations(decls)
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelSigDeclarations(baseName, decls)
                
                // Create explicitly - it can be 'single top level' thing that is hidden
                let decl =
                    FSharpNavigationDeclarationItem.Create
                        (textOfLid id, (if isModule then ModuleFileDecl else NamespaceDecl),
                            GlyphMajor.Module, m, 
                            unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid id) other), 
                            singleTopLevel, access), (addItemName(textOfLid id)), nested
                decl::other)
        
        let items = 
            items 
            |> Array.ofList 
            |> Array.map (fun (d, idx, nest) -> 
                let nest = nest |> Array.ofList |> Array.map (fun (decl, idx) -> decl.WithUniqueName(uniqueName d.Name idx))
                nest |> Array.sortInPlaceWith (fun a b -> compare a.Name b.Name)
                { Declaration = d.WithUniqueName(uniqueName d.Name idx); Nested = nest } )                  
        items |> Array.sortInPlaceWith (fun a b -> compare a.Declaration.Name b.Declaration.Name)
        new FSharpNavigationItems(items)

    let getNavigation (parsedInput: ParsedInput) =
        match parsedInput with
        | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, modules)) -> getNavigationFromSigFile modules
        | ParsedInput.ImplFile(ParsedImplFileInput(_, _, _, _, _, modules, _)) -> getNavigationFromImplFile modules

    let empty = FSharpNavigationItems([||])

[<RequireQualifiedAccess>]
module NavigateTo =
    open System

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
    type ContainerType =
        | File
        | Namespace
        | Module
        | Type
        | Exception

    type Container =
        { Type: ContainerType
          Name: string }

    type NavigableItem = 
        { Name: string
          Range: range
          IsSignature: bool
          Kind: NavigableItemKind
          Container: Container }
        
    let getNavigableItems (parsedInput: ParsedInput) : NavigableItem [] = 
        let rec lastInLid (lid: LongIdent) = 
            match lid with
            | [x] -> Some x
            | _ :: xs -> lastInLid xs
            | _ -> None // empty lid is possible in case of broken ast

        let formatLongIdent (lid: LongIdent) = lid |> List.map (fun id -> id.idText) |> String.concat "."
        let result = ResizeArray()
        
        let addIdent kind (id: Ident) (isSignature: bool) (container: Container) = 
            if not (String.IsNullOrEmpty id.idText) then
                let item = 
                    { Name = id.idText
                      Range = id.idRange
                      IsSignature = isSignature
                      Kind = kind 
                      Container = container }
                result.Add item
    
        let addModule lid isSig container = 
            match lastInLid lid with
            | Some id -> addIdent NavigableItemKind.Module id isSig container
            | _ -> ()
    
        let addModuleAbbreviation (id: Ident) isSig container =
            addIdent NavigableItemKind.ModuleAbbreviation id isSig container
        
        let addExceptionRepr (SynExceptionDefnRepr(_, UnionCase(_, id, _, _, _, _), _, _, _, _)) isSig container = 
            addIdent NavigableItemKind.Exception id isSig container
            { Type = ContainerType.Exception; Name = id.idText }
    
        let addComponentInfo kind (ComponentInfo(_, _, _, lid, _, _, _, _)) isSig container = 
            match lastInLid lid with
            | Some id -> addIdent kind id isSig container
            | _ -> ()
            { Type = ContainerType.Type; Name = formatLongIdent lid }
    
        let addValSig kind (ValSpfn(_, id, _, _, _, _, _, _, _, _, _)) isSig container = 
            addIdent kind id isSig container
        
        let addField(SynField.Field(_, _, id, _, _, _, _, _)) isSig container = 
            match id with
            | Some id -> addIdent NavigableItemKind.Field id isSig container
            | _ -> ()
        
        let addEnumCase(EnumCase(_, id, _, _, _)) isSig = 
            addIdent NavigableItemKind.EnumCase id isSig
        
        let addUnionCase(UnionCase(_, id, _, _, _, _)) isSig container = 
            addIdent NavigableItemKind.UnionCase id isSig container
    
        let mapMemberKind mk = 
            match mk with
            | MemberKind.ClassConstructor // ?
            | MemberKind.Constructor -> NavigableItemKind.Constructor
            | MemberKind.PropertyGet
            | MemberKind.PropertySet
            | MemberKind.PropertyGetSet -> NavigableItemKind.Property
            | MemberKind.Member -> NavigableItemKind.Member
    
        let addBinding (Binding(_, _, _, _, _, _, valData, headPat, _, _, _, _)) itemKind container =
            let (SynValData(memberFlagsOpt, _, _)) = valData
            let kind =
                match itemKind with
                | Some x -> x
                | _ ->
                    match memberFlagsOpt with
                    | Some mf -> mapMemberKind mf.MemberKind
                    | _ -> NavigableItemKind.ModuleValue
    
            match headPat with
            | SynPat.LongIdent(LongIdentWithDots([_; id], _), _, _, _, _access, _) ->
                // instance members
                addIdent kind id false container
            | SynPat.LongIdent(LongIdentWithDots([id], _), _, _, _, _, _) ->
                // functions
                addIdent kind id false container
            | SynPat.Named(_, id, _, _, _) ->
                // values
                addIdent kind id false container
            | _ -> ()
    
        let addMember valSig (memberFlags: MemberFlags) isSig container = 
            let ctor = mapMemberKind memberFlags.MemberKind
            addValSig ctor valSig isSig container
    
        let rec walkSigFileInput (ParsedSigFileInput(fileName, _, _, _, moduleOrNamespaceList)) = 
            for item in moduleOrNamespaceList do
                walkSynModuleOrNamespaceSig item { Type = ContainerType.File; Name = fileName }
    
        and walkSynModuleOrNamespaceSig (SynModuleOrNamespaceSig(lid, _, isModule, decls, _, _, _, _)) container = 
            if isModule then
                addModule lid true container
            let container = 
                { Type = if isModule then ContainerType.Module else ContainerType.Namespace
                  Name = formatLongIdent lid }
            for decl in decls do
                walkSynModuleSigDecl decl container
    
        and walkSynModuleSigDecl(decl: SynModuleSigDecl) container = 
            match decl with
            | SynModuleSigDecl.ModuleAbbrev(lhs, _, _range) ->
                addModuleAbbreviation lhs true container
            | SynModuleSigDecl.Exception(SynExceptionSig(representation, _, _), _) ->
                addExceptionRepr representation true container |> ignore
            | SynModuleSigDecl.NamespaceFragment fragment ->
                walkSynModuleOrNamespaceSig fragment container
            | SynModuleSigDecl.NestedModule(componentInfo, _, nestedDecls, _) ->
                let container = addComponentInfo NavigableItemKind.Module componentInfo true container
                for decl in nestedDecls do
                    walkSynModuleSigDecl decl container
            | SynModuleSigDecl.Types(types, _) ->
                for ty in types do
                    walkSynTypeDefnSig ty container
            | SynModuleSigDecl.Val(valSig, _range) ->
                addValSig NavigableItemKind.ModuleValue valSig true container
            | SynModuleSigDecl.HashDirective _
            | SynModuleSigDecl.Open _ -> ()
    
        and walkSynTypeDefnSig (TypeDefnSig(componentInfo, repr, members, _)) container = 
            let container = addComponentInfo NavigableItemKind.Type componentInfo true container
            for m in members do
                walkSynMemberSig m container
            match repr with
            | SynTypeDefnSigRepr.ObjectModel(_, membersSigs, _) ->
                for m in membersSigs do
                    walkSynMemberSig m container
            | SynTypeDefnSigRepr.Simple(repr, _) ->
                walkSynTypeDefnSimpleRepr repr true container
            | SynTypeDefnSigRepr.Exception _ -> ()
    
        and walkSynMemberSig (synMemberSig: SynMemberSig) container = 
            match synMemberSig with
            | SynMemberSig.Member(valSig, memberFlags, _) ->
                addMember valSig memberFlags true container
            | SynMemberSig.ValField(synField, _) ->
                addField synField true container
            | SynMemberSig.NestedType(synTypeDef, _) ->
                walkSynTypeDefnSig synTypeDef container
            | SynMemberSig.Inherit _
            | SynMemberSig.Interface _ -> ()
    
        and walkImplFileInpit (ParsedImplFileInput(fileName, _, _, _, _, moduleOrNamespaceList, _)) = 
            let container = { Type = ContainerType.File; Name = fileName }
            for item in moduleOrNamespaceList do
                walkSynModuleOrNamespace item container
    
        and walkSynModuleOrNamespace(SynModuleOrNamespace(lid, _, isModule, decls, _, _, _, _)) container =
            if isModule then
                addModule lid false container
            let container = 
                { Type = if isModule then ContainerType.Module else ContainerType.Namespace
                  Name = formatLongIdent lid }
            for decl in decls do
                walkSynModuleDecl decl container
    
        and walkSynModuleDecl(decl: SynModuleDecl) container =
            match decl with
            | SynModuleDecl.Exception(SynExceptionDefn(repr, synMembers, _), _) -> 
                let container = addExceptionRepr repr false container
                for m in synMembers do
                    walkSynMemberDefn m container
            | SynModuleDecl.Let(_, bindings, _) ->
                for binding in bindings do
                    addBinding binding None container
            | SynModuleDecl.ModuleAbbrev(lhs, _, _) ->
                addModuleAbbreviation lhs false container
            | SynModuleDecl.NamespaceFragment(fragment) ->
                walkSynModuleOrNamespace fragment container
            | SynModuleDecl.NestedModule(componentInfo, _, modules, _, _) ->
                let container = addComponentInfo NavigableItemKind.Module componentInfo false container
                for m in modules do
                    walkSynModuleDecl m container
            | SynModuleDecl.Types(typeDefs, _range) ->
                for t in typeDefs do
                    walkSynTypeDefn t container
            | SynModuleDecl.Attributes _
            | SynModuleDecl.DoExpr _
            | SynModuleDecl.HashDirective _
            | SynModuleDecl.Open _ -> ()
    
        and walkSynTypeDefn(TypeDefn(componentInfo, representation, members, _)) container = 
            walkSynTypeDefnRepr representation container
            let container = addComponentInfo NavigableItemKind.Type componentInfo false container
            for m in members do
                walkSynMemberDefn m container
    
        and walkSynTypeDefnRepr(typeDefnRepr: SynTypeDefnRepr) container = 
            match typeDefnRepr with
            | SynTypeDefnRepr.ObjectModel(_, members, _) ->
                for m in members do
                    walkSynMemberDefn m container
            | SynTypeDefnRepr.Simple(repr, _) -> 
                walkSynTypeDefnSimpleRepr repr false container
            | SynTypeDefnRepr.Exception _ -> ()
    
        and walkSynTypeDefnSimpleRepr(repr: SynTypeDefnSimpleRepr) isSig container = 
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
            | SynMemberDefn.AbstractSlot(synValSig, memberFlags, _) ->
                addMember synValSig memberFlags false container
            | SynMemberDefn.AutoProperty(_, _, id, _, _, _, _, _, _, _, _) ->
                addIdent NavigableItemKind.Property id false container
            | SynMemberDefn.Interface(_, members, _) ->
                match members with
                | Some members ->
                    for m in members do
                        walkSynMemberDefn m container
                | None -> ()
            | SynMemberDefn.Member(binding, _) ->
                addBinding binding None container
            | SynMemberDefn.NestedType(typeDef, _, _) -> 
                walkSynTypeDefn typeDef container
            | SynMemberDefn.ValField(field, _) ->
                addField field false container
            | SynMemberDefn.LetBindings (bindings, _, _, _) -> 
                bindings |> List.iter (fun binding -> addBinding binding (Some NavigableItemKind.Field) container)
            | SynMemberDefn.Open _
            | SynMemberDefn.ImplicitInherit _
            | SynMemberDefn.Inherit _
            | SynMemberDefn.ImplicitCtor _ -> ()

        match parsedInput with
        | ParsedInput.SigFile input -> walkSigFileInput input
        | ParsedInput.ImplFile input -> walkImplFileInpit input
    
        result.ToArray()