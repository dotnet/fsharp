// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.Editor.TypedAstUtils
open Microsoft.VisualStudio.FSharp.Editor.UntypedAstUtils
open Microsoft.VisualStudio.FSharp.Editor.Symbols
open Microsoft.VisualStudio.FSharp.Editor.Extensions
open Microsoft.VisualStudio.FSharp.Editor.IdentifierUtils


type RawOpenDeclaration =
    { Idents: Idents
      Parent: Idents option }

type OpenDeclWithAutoOpens =
    { Declarations: Idents list
      Parent: Idents option
      IsUsed: bool }

[<NoComparison>]
type internal OpenDeclaration =
    { Declarations: OpenDeclWithAutoOpens list
      DeclarationRange: Range.range
      ScopeRange: Range.range
      IsUsed: bool }

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module OpenDeclWithAutoOpens =
    let updateBySymbolPrefix (symbolPrefix: Idents) (decl: OpenDeclWithAutoOpens) =
        let matched = decl.Declarations |> List.exists (Array.areEqual symbolPrefix)
        //if not decl.IsUsed && matched then debug "OpenDeclarationWithAutoOpens %A is used by %A" decl symbolPrefix
        matched, { decl with IsUsed = decl.IsUsed || matched }

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal OpenDeclaration =
    let updateBySymbolPrefix (symbolPrefix: Idents) (decl: OpenDeclaration) =
        let decls =
            decl.Declarations 
            |> List.map (OpenDeclWithAutoOpens.updateBySymbolPrefix symbolPrefix)
        let matched = decls |> List.exists fst
        let isUsed = decls |> List.exists (fun (_, decl) -> decl.IsUsed)
        //if not decl.IsUsed && isUsed then debug "OpenDeclaration %A is used by %A" decl symbolPrefix
        matched, { decl with Declarations = decls |> List.map snd; IsUsed = isUsed }

module internal OpenDeclarationGetter =
    open System 
    open System.Diagnostics

    let private getAutoOpenModules entities =
        List.foldBack( fun (e: AssemblySymbol) acc -> 
             match e.Kind LookupType.Precise with
             |  EntityKind.Module { IsAutoOpen = true } -> e.CleanedIdents :: acc
             | _ -> acc) entities []


    let private getActivePatterns (entities: AssemblySymbol list) =
        List.foldBack( fun (e: AssemblySymbol) acc -> 
             match e.Kind  LookupType.Precise with
             | EntityKind.FunctionOrValue true -> e.CleanedIdents::acc
             | _ -> acc) entities  []

    let parseTooltip (FSharpToolTipText elems): RawOpenDeclaration list =
        elems
        |> List.collect (fun e -> 
            let rawStrings =
                match e with
                | FSharpToolTipElement.Group elems -> 
                    elems |> List.map (fun e -> e.MainDescription)
                | _ -> []
            
            let removePrefix prefix (str: string) =
                if str.StartsWith prefix then Some (str.Substring(prefix.Length).Trim()) else None

            (* Tooltip at this point is a bunch of lines. One or two lines correspond to each open statement.
               If an open statement is fully qualified, then a single line corresponds to it:
               "module My.Module" or "namespace My.Namespace" (for namespaces).
               If it's a relative module open statement, then two lines correspond to it:
               "module Module"
               "from My".
               If a relative module open statement is actually opens several modules (being a suffix of several parent 
               modules / namespaces), then several "module/namespace + from" line pairs correspond to it:
               "module Module"
               "from My"
               "module Module"
               "from My2"
            *)
            rawStrings
            |> List.choose (fun (s: string) ->
                 maybe {
                    let! name, from = 
                        match String.getNonEmptyLines s with
                        | [|name; from|] -> Some (name, Some from)
                        | [|name|] -> Some (name, None)
                        | _ -> None

                    let! name = 
                        name 
                        |> removePrefix "namespace"
                        |> Option.orElse (name |> removePrefix "module")
                     
                    let from = from |> Option.bind (removePrefix "from")
                    let fullName = (from |> Option.map (fun from -> from + ".") |> Option.defaultValue "") + name
                    return { RawOpenDeclaration.Idents = fullName.Split '.'
                             Parent = from |> Option.map (fun from -> from.Split '.') }
                }))

    let updateOpenDeclsWithSymbolPrefix symbolPrefix symbolRange openDecls = 
        openDecls 
        |> List.fold (fun (acc, foundMatchedDecl) openDecl -> 
            // We already found a matched open declaration or the symbol range is out or the next open declaration.
            // Do nothing, just add the open decl to the accumulator as is.
            if foundMatchedDecl || not (Range.rangeContainsRange openDecl.ScopeRange symbolRange) then
                openDecl :: acc, foundMatchedDecl
            else
                let matched, updatedDecl = openDecl |> OpenDeclaration.updateBySymbolPrefix symbolPrefix
                updatedDecl :: acc, matched
            ) ([], false)
        |> fst
        |> List.rev

    let setOpenDeclsIsUsedFlag idents (openDecls: OpenDeclaration list) =
        openDecls
        |> List.map (fun decl -> 
            let declarations = 
                decl.Declarations 
                |> List.map (fun d -> 
                    if d.IsUsed then d
                    else { d with IsUsed = d.Declarations |> List.exists (Array.areEqual idents) })
            let isUsed = declarations |> List.exists (fun d -> d.IsUsed)
            { decl with Declarations = declarations; IsUsed = isUsed })

    let spreadIsUsedFlagToParents (openDecls: OpenDeclaration list) =
        let res = 
            openDecls 
            |> List.fold (fun res decl ->
                if not decl.IsUsed then res
                else
                    let parents = decl.Declarations |> List.choose (fun decl -> decl.Parent)
                    parents 
                    |> List.fold (fun res parent -> res |> setOpenDeclsIsUsedFlag parent)
                       res
               ) openDecls
        //debug "[OpenDeclarationsGetter] spreadIsUsedFlagToParents: Before = %A, After = %A" openDecls res
        res

    type Line = int
    type EndColumn = int

    let private processOpenDeclaration (longIdent: Ident list) =
        let isExactlyOne =
            match longIdent with
            | [_] -> true
            | _ -> false
        longIdent
        |> List.mapi (fun i ident -> 
            // Only filter out if global is a prefix of the open declaration
            match i, ident.idText with
            | 0, "`global`" when not isExactlyOne ->
                let r = ident.idRange
                // Make sure that we don't filter out ``global`` and the like
                if r.StartLine = r.EndLine && r.EndColumn - r.StartColumn = 6 then
                    None
                else Some ident
            | _ -> 
                Some ident)
        |> List.choose id
        |> longIdentToArray

    let getEffectiveOpenDeclarationsAtLocation (pos: pos) (ast: ParsedInput) =
        let openStatements =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput(_, _, _, _, _, modules, _)) ->
                let rec walkModuleOrNamespace openStatements (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc -> 
                        function
                        | SynModuleDecl.NestedModule (_, _, nestedModuleDecls, _, nestedModuleRange) -> 
                            if rangeContainsPos moduleRange pos then
                                walkModuleOrNamespace acc (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | SynModuleDecl.Open (LongIdentWithDots(longIdent, _), openDeclRange) ->
                            let identArray = processOpenDeclaration longIdent
                            if openDeclRange.EndLine < pos.Line || (openDeclRange.EndLine = pos.Line && openDeclRange.EndColumn < pos.Column) then 
                                String.Join(".", identArray) :: acc
                            else acc
                        | _ -> acc) openStatements 

                modules
                |> List.fold (fun acc (SynModuleOrNamespace(_, _, _, decls, _, _, _, moduleRange)) ->
                       if rangeContainsPos moduleRange pos then
                           walkModuleOrNamespace acc (decls, moduleRange) @ acc
                       else acc) []
            | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, modules)) -> 
                let rec walkModuleOrNamespaceSig openStatements (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc -> 
                        function
                        | SynModuleSigDecl.NestedModule (_, _, nestedModuleDecls, nestedModuleRange) -> 
                            if rangeContainsPos moduleRange pos then
                                walkModuleOrNamespaceSig acc (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | SynModuleSigDecl.Open (longIdent, openDeclRange) ->
                            let identArray = processOpenDeclaration longIdent
                            if openDeclRange.EndLine < pos.Line || (openDeclRange.EndLine = pos.Line && openDeclRange.EndColumn < pos.Column) then 
                                String.Join(".", identArray) :: acc
                            else acc
                        | _ -> acc) openStatements

                modules
                |> List.fold (fun acc (SynModuleOrNamespaceSig(_, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespaceSig acc (decls, moduleRange) @ acc
                        else acc) []
        seq {
            yield! List.rev openStatements
            yield getModuleOrNamespacePath pos ast
        }
        |> Seq.distinct
        |> Seq.toList

    let private removeModuleSuffixes (allEntities: IDictionary<_,_> option) (symbolUses: SymbolUse[]) : SymbolUse [] =
        match allEntities with
        | Some entities ->
            symbolUses 
            |> Array.map (fun symbolUse ->
                let fullNames =
                    symbolUse.FullNames
                    |> Array.map (fun fullName ->
                        match entities |> Dict.tryFind (String.Join (".", (fullName:string []))) with
                        | Some [cleanIdents] ->
                            //debug "[SourceCodeClassifier] One clean FullName %A -> %A" fullName cleanIdents
                            cleanIdents
                        | Some (firstCleanIdents :: _ as cleanIdentsList) ->
                            if cleanIdentsList |> List.exists (Array.areEqual fullName) then
                                //debug "[SourceCodeClassifier] An exact match found among several clean idents: %A" fullName
                                fullName
                            else
                                //debug "[SourceCodeClassifier] No match found among several clean idents, return the first one FullName %A -> %A" 
                                  //    fullName firstCleanIdents
                                firstCleanIdents
                        | _ -> 
                            //debug "[SourceCodeClassifier] NOT Cleaned FullName %A" fullName
                            fullName)
                { symbolUse with FullNames = fullNames })
        | None -> symbolUses

    open System.Collections.Generic

    let private getSymbolUsesPotentiallyRequireOpenDecls symbolsUses =
        symbolsUses
        |> Array.filter (fun symbolUse ->
            let res = 
                match symbolUse.SymbolUse.Symbol with
                | TypedAstPatterns.Pattern | TypedAstPatterns.RecordField _
                | TypedAstPatterns.FSharpEntity ( TypedAstPatterns.Class 
                                                |   ( TypedAstPatterns.AbbreviatedType _ 
                                                    | TypedAstPatterns.FSharpType 
                                                    | TypedAstPatterns.ValueType 
                                                    | TypedAstPatterns.FSharpModule 
                                                    | TypedAstPatterns.Array
                                                    ), _, _)
                | TypedAstPatterns.FSharpEntity (_, _, TypedAstPatterns.Tuple)
                | TypedAstPatterns.MemberFunctionOrValue (TypedAstPatterns.Constructor _ | TypedAstPatterns.ExtensionMember) -> true
                | TypedAstPatterns.MemberFunctionOrValue func -> not func.IsMember
                | _ -> false
            res) 

    // Filter out symbols which ranges are fully included into a bigger symbols. 
    // For example, for this code: Ns.Module.Module1.Type.NestedType.Method() FCS returns the following symbols: 
    // Ns, Module, Module1, Type, NestedType, Method.
    // We want to filter all of them but the longest one (Method).
    let private filterNestedSymbolUses (longIdents: IDictionary<_,_>) symbolUses =
        symbolUses
        |> Array.map (fun sUse ->
            match longIdents.TryGetValue sUse.SymbolUse.RangeAlternate.End with
            | true, longIdent -> sUse, Some longIdent
            | _ -> sUse, None)
        |> Seq.groupBy (fun (_, longIdent) -> longIdent)
        |> Seq.map (fun (longIdent, sUses) -> longIdent, sUses |> Seq.map fst)
        |> Seq.collect (fun (longIdent, symbolUses) ->
            match longIdent with
            | Some _ -> 
                (* Find all longest SymbolUses which has unique roots. For example:
                           
                    module Top
                    module M =
                        type System.IO.Path with
                            member static ExtensionMethod() = ()

                    open M
                    open System
                    let _ = IO.Path.ExtensionMethod()

                    The last line contains three SymbolUses: "System.IO", "System.IO.Path" and "Top.M.ExtensionMethod". 
                    So, we filter out "System.IO" since it's a prefix of "System.IO.Path".

                *)
                let res =
                    symbolUses
                    |> Seq.sortBy (fun symbolUse -> -symbolUse.SymbolUse.RangeAlternate.EndColumn)
                    |> Seq.fold (fun (prev, acc) next ->
                         match prev with
                         | Some prev -> 
                            if prev.FullNames
                               |> Array.exists (fun prevFullName ->
                                    next.FullNames
                                    |> Array.exists (fun nextFullName ->
                                         nextFullName.Length < prevFullName.Length
                                         && prevFullName |> Array.startsWith nextFullName)) then 
                                Some prev, acc
                            else Some next, next :: acc
                         | None -> Some next, next :: acc)
                       (None, [])
                    |> snd
                    |> List.rev
                    |> List.toSeq
                res
            | None -> symbolUses)
        |> Seq.toArray

    let private getFullPrefix (longIdents: IDictionary<_,_>) fullName (endPos: Range.pos): Idents option =
        match longIdents.TryGetValue endPos with
        | true, longIdent ->
            let rec loop matchFound longIdents symbolIdents =
                match longIdents, symbolIdents with
                | [], _ -> symbolIdents
                | _, [] -> []
                | lh :: lt, sh :: st ->
                    if lh <> sh then
                        if matchFound then symbolIdents else loop matchFound lt symbolIdents
                    else loop true lt st
                        
            let prefix = 
                loop false (longIdent |> Array.rev |> List.ofArray) (fullName |> Array.rev |> List.ofArray)
                |> List.rev
                |> List.toArray
                            
//            debug "[SourceCodeClassifier] QualifiedSymbol: FullName = %A, Symbol end pos = (%d, %d), Res = %A" 
//                  fullName endPos.Line endPos.Column prefix
            Some prefix
        | _ -> None
