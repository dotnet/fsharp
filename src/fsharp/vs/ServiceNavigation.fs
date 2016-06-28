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
type FSharpNavigationDeclarationItem(uniqueName: string, name: string, kind: FSharpNavigationDeclarationItemKind, glyph: int, range: range, bodyRange: range, singleTopLevel:bool) = 
    
    member x.bodyRange = bodyRange
    
    member x.UniqueName = uniqueName
    member x.Name = name
    member x.Glyph = glyph
    member x.Kind = kind
    member x.Range = range
    member x.BodyRange = bodyRange 
    member x.IsSingleTopLevel = singleTopLevel
    member x.WithUniqueName(uniqueName: string) =
      FSharpNavigationDeclarationItem(uniqueName, name, kind, glyph, range, bodyRange, singleTopLevel)
    static member Create(name: string, kind, glyph: int, range: range, bodyRange: range, singleTopLevel:bool) = 
      FSharpNavigationDeclarationItem("", name, kind, glyph, range, bodyRange, singleTopLevel)

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
      | hd::tl -> tl |> List.fold (unionRangesChecked) hd
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
    let getNavigationFromImplFile (modules:SynModuleOrNamespace list) =

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
        let createDeclLid(baseName, lid, kind, baseGlyph, m, bodym, nested) =
            let name = (if baseName <> "" then baseName + "." else "") + (textOfLid lid)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph * 6, m, bodym, false), (addItemName name), nested
            
        let createDecl(baseName, (id:Ident), kind, baseGlyph, m, bodym, nested) =
            let name = (if baseName <> "" then baseName + "." else "") + (id.idText)
            FSharpNavigationDeclarationItem.Create
              (name, kind, baseGlyph * 6, m, bodym, false), (addItemName name), nested
         
        // Create member-kind-of-thing for the right dropdown
        let createMemberLid(lid, kind, baseGlyph, m) =
            FSharpNavigationDeclarationItem.Create(textOfLid lid, kind, baseGlyph * 6, m, m, false), (addItemName(textOfLid lid))

        let createMember((id:Ident), kind, baseGlyph, m) =
            FSharpNavigationDeclarationItem.Create(id.idText, kind, baseGlyph * 6, m, m, false), (addItemName(id.idText))
            

        // Process let-binding
        let processBinding isMember (Binding(_, _, _, _, _, _, SynValData(memebrOpt, _, _), synPat, _, synExpr, _, _)) =
            let m = match synExpr with 
                    | SynExpr.Typed(e, _, _) -> e.Range // fix range for properties with type annotations
                    | _ -> synExpr.Range
            match synPat, memebrOpt with
            | SynPat.LongIdent(LongIdentWithDots(lid,_), _,_, _, _, _), Some(flags) when isMember -> 
                let icon, kind =
                  match flags.MemberKind with
                  | MemberKind.ClassConstructor
                  | MemberKind.Constructor
                  | MemberKind.Member -> 
                        (if flags.IsOverrideOrExplicitImpl then iIconGroupMethod2 else iIconGroupMethod), MethodDecl
                  | MemberKind.PropertyGetSet
                  | MemberKind.PropertySet
                  | MemberKind.PropertyGet -> iIconGroupProperty, PropertyDecl
                let lidShow, rangeMerge = 
                  match lid with 
                  | _thisVar::nm::_ -> (List.tail lid, nm.idRange) 
                  | hd::_ -> (lid, hd.idRange) 
                  | _ -> (lid, m)
                [ createMemberLid(lidShow, kind, icon, unionRanges rangeMerge m) ]
            | SynPat.LongIdent(LongIdentWithDots(lid,_), _,_, _, _, _), _ -> [ createMemberLid(lid, FieldDecl, iIconGroupConstant, unionRanges (List.head lid).idRange m) ]
            | _ -> []
        
        // Process a class declaration or F# type declaration
        let rec processExnDefnRepr baseName nested (SynExceptionDefnRepr(_, (UnionCase(_, id, fldspec, _, _, _)), _, _, _, m)) =
            // Exception declaration
            [ createDecl(baseName, id, ExnDecl, iIconGroupException, m, fldspecRange fldspec, nested) ] 

        // Process a class declaration or F# type declaration
        and processExnDefn baseName (SynExceptionDefn(repr, membDefns, _)) =  
            let nested = processMembers membDefns |> snd
            processExnDefnRepr baseName nested repr

        and processTycon baseName (TypeDefn(ComponentInfo(_, _, _, lid, _, _, _, _), repr, membDefns, m)) =
            let topMembers = processMembers membDefns |> snd
            match repr with
            | SynTypeDefnRepr.Exception repr -> processExnDefnRepr baseName [] repr
            | SynTypeDefnRepr.ObjectModel(_, membDefns, mb) ->
                // F# class declaration
                let members = processMembers membDefns |> snd
                let nested = members@topMembers
                ([ createDeclLid(baseName, lid, TypeDecl, iIconGroupClass, m, bodyRange mb nested, nested) ]: ((FSharpNavigationDeclarationItem * int * _) list))
            | SynTypeDefnRepr.Simple(simple, _) ->
                // F# type declaration
                match simple with
                | SynTypeDefnSimpleRepr.Union(_, cases, mb) ->
                    let cases = 
                        [ for (UnionCase(_, id, fldspec, _, _, _)) in cases -> 
                            createMember(id, OtherDecl, iIconGroupValueType, unionRanges (fldspecRange fldspec) id.idRange) ]
                    let nested = cases@topMembers              
                    [ createDeclLid(baseName, lid, TypeDecl, iIconGroupUnion, m, bodyRange mb nested, nested) ]
                | SynTypeDefnSimpleRepr.Enum(cases, mb) -> 
                    let cases = 
                        [ for (EnumCase(_, id, _, _, m)) in cases ->
                            createMember(id, FieldDecl, iIconGroupEnumMember, m) ]
                    let nested = cases@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, iIconGroupEnum, m, bodyRange mb nested, nested) ]
                | SynTypeDefnSimpleRepr.Record(_, fields, mb) ->
                    let fields = 
                        [ for (Field(_, _, id, _, _, _, _, m)) in fields do
                            if (id.IsSome) then
                              yield createMember(id.Value, FieldDecl, iIconGroupFieldBlue, m) ]
                    let nested = fields@topMembers
                    [ createDeclLid(baseName, lid, TypeDecl, iIconGroupType, m, bodyRange mb nested, nested) ]
                | SynTypeDefnSimpleRepr.TypeAbbrev(_, _, mb) ->
                    [ createDeclLid(baseName, lid, TypeDecl, iIconGroupTypedef, m, bodyRange mb topMembers, topMembers) ]
                          
                //| SynTypeDefnSimpleRepr.General of TyconKind * (SynType * range * ident option) list * (valSpfn * MemberFlags) list * fieldDecls * bool * bool * range 
                //| SynTypeDefnSimpleRepr.LibraryOnlyILAssembly of ILType * range
                //| TyconCore_repr_hidden of range
                | _ -> [] 
                  
        // Returns class-members for the right dropdown                  
        and processMembers members: (range * list<FSharpNavigationDeclarationItem * int>) = 
            let members = members |> List.map (fun memb ->
               (memb.Range,
                match memb with
                | SynMemberDefn.LetBindings(binds, _, _, _) -> List.collect (processBinding false) binds
                | SynMemberDefn.Member(bind, _) -> processBinding true bind
                | SynMemberDefn.ValField(Field(_, _, Some(rcid), ty, _, _, _, _), _) ->
                    [ createMember(rcid, FieldDecl, iIconGroupFieldBlue, ty.Range) ]
                | SynMemberDefn.AutoProperty(_attribs,_isStatic,id,_tyOpt,_propKind,_,_xmlDoc,_access,_synExpr, _, _) -> 
                    [ createMember(id, FieldDecl, iIconGroupFieldBlue, id.idRange) ]
                | SynMemberDefn.AbstractSlot(ValSpfn(_, id, _, ty, _, _, _, _, _, _, _), _, _) ->
                    [ createMember(id, MethodDecl, iIconGroupMethod2, ty.Range) ]
                | SynMemberDefn.NestedType _ -> failwith "tycon as member????" //processTycon tycon                
                | SynMemberDefn.Interface(_, Some(membs), _) ->
                    processMembers membs |> snd
                | _ -> []  )) 
            ((members |> Seq.map fst |> Seq.fold unionRangesChecked range.Zero),
             (members |> List.map snd |> List.concat))

        // Process declarations in a module that belong to the right drop-down (let bindings)
        let processNestedDeclarations decls = decls |> List.collect (function
            | SynModuleDecl.Let(_, binds, _) -> List.collect (processBinding false) binds
            | _ -> [] )        

        // Process declarations nested in a module that should be displayed in the left dropdown
        // (such as type declarations, nested modules etc.)                            
        let rec processFSharpNavigationTopLevelDeclarations(baseName, decls) = decls |> List.collect (function
            | SynModuleDecl.ModuleAbbrev(id, lid, m) ->
                [ createDecl(baseName, id, ModuleDecl, iIconGroupModule, m, rangeOfLid lid, []) ]
                
            | SynModuleDecl.NestedModule(ComponentInfo(_, _, _, lid, _, _, _, _), _isRec, decls, _, m) ->                
                // Find let bindings (for the right dropdown)
                let nested = processNestedDeclarations(decls)
                let newBaseName = (if (baseName = "") then "" else baseName+".") + (textOfLid lid)
                
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelDeclarations(newBaseName, decls)
                createDeclLid(baseName, lid, ModuleDecl, iIconGroupModule, m, unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid lid) other), nested)::other
                  
            | SynModuleDecl.Types(tydefs, _) -> tydefs |> List.collect (processTycon baseName)                                    
            | SynModuleDecl.Exception (defn,_) -> processExnDefn baseName defn
            | _ -> [] )            
                  
        // Collect all the items  
        let items = 
            // Show base name for this module only if it's not the root one
            let singleTopLevel = (modules.Length = 1)
            modules |> List.collect (fun (SynModuleOrNamespace(id, _isRec, isModule, decls, _, _, _, m)) ->
                let baseName = if (not singleTopLevel) then textOfLid id else ""
                // Find let bindings (for the right dropdown)
                let nested = processNestedDeclarations(decls)
                // Get nested modules and types (for the left dropdown)
                let other = processFSharpNavigationTopLevelDeclarations(baseName, decls)
                
                // Create explicitly - it can be 'single top level' thing that is hidden
                let decl =
                    FSharpNavigationDeclarationItem.Create
                        (textOfLid id, (if isModule then ModuleFileDecl else NamespaceDecl),
                            iIconGroupModule * 6, m, 
                            unionRangesChecked (rangeOfDecls nested) (moduleRange (rangeOfLid id) other), 
                            singleTopLevel), (addItemName(textOfLid id)), nested
                decl::other )
                  
        let items = 
            items 
            |> Array.ofList 
            |> Array.map (fun (d, idx, nest) -> 
                let nest = nest |> Array.ofList |> Array.map (fun (decl, idx) -> decl.WithUniqueName(uniqueName d.Name idx))
                nest |> Array.sortInPlaceWith (fun a b -> compare a.Name b.Name)
                { Declaration = d.WithUniqueName(uniqueName d.Name idx); Nested = nest } )                  
        items |> Array.sortInPlaceWith (fun a b -> compare a.Declaration.Name b.Declaration.Name)
        new FSharpNavigationItems(items)

    let empty = new FSharpNavigationItems([| |])

