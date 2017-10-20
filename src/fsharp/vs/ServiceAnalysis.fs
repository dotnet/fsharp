// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range

module UnusedOpens =
    open Microsoft.FSharp.Compiler.PrettyNaming

    /// Represents single open statement.
    type OpenStatement =
        { /// Full namespace or module identifier as it's presented in source code.
          Idents: Set<string>
          /// Range of open statement itself.
          Range: range
          /// Enclosing module or namespace range (that is, the scope on in which this open statement is visible).
          ModuleRange: range }

    let getOpenStatements (openDeclarations: OpenDeclaration list) : OpenStatement list = 
        openDeclarations
        |> List.choose (fun openDeclaration ->
             match openDeclaration with
             | OpenDeclaration.Open (longId, moduleRefs, scopem) when not (List.isEmpty longId) ->
                 
                 Some { Idents = moduleRefs |> List.map (fun x -> x.CompiledName) |> Set.ofList
                        Range =
                            let first = List.head longId
                            let last = List.last longId
                            mkRange scopem.FileName first.idRange.Start last.idRange.End
                        ModuleRange = scopem }
             | _ -> None // for now
           )

    let getAutoOpenAccessPath (ent:FSharpEntity) =
        // Some.Namespace+AutoOpenedModule+Entity

        // HACK: I can't see a way to get the EnclosingEntity of an Entity
        // Some.Namespace + Some.Namespace.AutoOpenedModule are both valid
        ent.TryFullName |> Option.bind(fun _ ->
            if (not ent.IsNamespace) && ent.QualifiedName.Contains "+" then 
                Some ent.QualifiedName.[0..ent.QualifiedName.IndexOf "+" - 1]
            else
                None)

    let entityNamespace (entOpt: FSharpEntity option) =
        match entOpt with
        | Some ent ->
            if ent.IsFSharpModule then
                [ yield Some ent.QualifiedName
                  yield Some ent.LogicalName
                  yield Some ent.AccessPath
                  yield Some ent.FullName
                  yield Some ent.DisplayName
                  yield ent.TryGetFullDisplayName()
                  if ent.HasFSharpModuleSuffix then
                    yield Some (ent.AccessPath + "." + ent.DisplayName)]
            else
                [ yield ent.Namespace
                  yield Some ent.AccessPath
                  yield getAutoOpenAccessPath ent
                  for path in ent.AllCompilationPaths do
                    yield Some path 
                ]
        | None -> []

    let symbolIsFullyQualified (getSourceLineStr: int -> string) (sym: FSharpSymbolUse) (fullName: string) =
        let lineStr = getSourceLineStr sym.RangeAlternate.StartLine
        match QuickParse.GetCompleteIdentifierIsland true lineStr sym.RangeAlternate.EndColumn with
        | Some (island, _, _) -> island = fullName
        | None -> false

    type NamespaceUse =
        { Ident: string
          Location: range }

    let getUnusedOpens (symbolUses: FSharpSymbolUse[], openDeclarations: OpenDeclaration list, getSourceLineStr: int -> string) : range list =
        let getPartNamespace (symbolUse: FSharpSymbolUse) (fullName: string) =
            // given a symbol range such as `Text.ISegment` and a full name of `MonoDevelop.Core.Text.ISegment`, return `MonoDevelop.Core`
            let length = symbolUse.RangeAlternate.EndColumn - symbolUse.RangeAlternate.StartColumn
            let lengthDiff = fullName.Length - length - 2
            if lengthDiff <= 0 || lengthDiff > fullName.Length - 1 then None
            else Some fullName.[0..lengthDiff]

        let getPossibleNamespaces (symbolUse: FSharpSymbolUse) : string list =
            let isQualified = symbolIsFullyQualified getSourceLineStr symbolUse

            (match symbolUse with
             | SymbolUse.Entity (ent, cleanFullNames) when not (cleanFullNames |> List.exists isQualified) ->
                 Some (cleanFullNames, Some ent)
             | SymbolUse.Field f when not (isQualified f.FullName) ->
                 Some ([f.FullName], Some f.DeclaringEntity)
             | SymbolUse.MemberFunctionOrValue mfv when not (isQualified mfv.FullName) ->
                 Some ([mfv.FullName], mfv.EnclosingEntity)
             | SymbolUse.Operator op when not (isQualified op.FullName) ->
                 Some ([op.FullName], op.EnclosingEntity)
             | SymbolUse.ActivePattern ap when not (isQualified ap.FullName) ->
                 Some ([ap.FullName], ap.EnclosingEntity)
             | SymbolUse.ActivePatternCase apc when not (isQualified apc.FullName) ->
                 Some ([apc.FullName], apc.Group.EnclosingEntity)
             | SymbolUse.UnionCase uc when not (isQualified uc.FullName) ->
                 Some ([uc.FullName], Some uc.ReturnType.TypeDefinition)
             | SymbolUse.Parameter p when not (isQualified p.FullName) && p.Type.HasTypeDefinition ->
                 Some ([p.FullName], Some p.Type.TypeDefinition)
             | _ -> None)
            |> Option.map (fun (fullNames, declaringEntity) ->
                 [ for name in fullNames do
                     let partNamespace = getPartNamespace symbolUse name
                     yield partNamespace
                   yield! entityNamespace declaringEntity ])
            |> Option.toList
            |> List.concat
            |> List.choose id

        let namespacesInUse : NamespaceUse list =
            let importantSymbolUses =
                symbolUses
                |> Array.filter (fun (symbolUse: FSharpSymbolUse) -> 
                     not symbolUse.IsFromDefinition &&
                     match symbolUse.Symbol with
                     | :? FSharpEntity as e -> not e.IsNamespace
                     | _ -> true
                   )

            importantSymbolUses
            |> Array.toList
            |> List.collect (fun su ->
                let lineStr = getSourceLineStr su.RangeAlternate.StartLine
                let partialName = QuickParse.GetPartialLongNameEx(lineStr, su.RangeAlternate.EndColumn - 1)
                let qualifier = partialName.QualifyingIdents |> String.concat "."
                getPossibleNamespaces su 
                |> List.distinct
                |> List.choose (fun ns ->
                     if qualifier = "" then Some ns
                     elif ns = qualifier then None
                     elif ns.EndsWith qualifier then Some ns.[..(ns.Length - qualifier.Length) - 2]
                     else None)
                |> List.map (fun ns ->
                     { Ident = ns
                       Location = su.RangeAlternate }))

        let filter list: OpenStatement list =
            let rec filterInner acc (list: OpenStatement list) (seenOpenStatements: OpenStatement list) = 
                
                let notUsed (os: OpenStatement) =
                    if os.Idents |> Set.exists (fun x -> x.StartsWith MangledGlobalName) then false
                    else
                        let notUsedAnywhere = 
                            not (namespacesInUse |> List.exists (fun nsu -> 
                                rangeContainsRange os.ModuleRange nsu.Location && os.Idents |> Set.contains nsu.Ident))
                        if notUsedAnywhere then true
                        else
                            let alreadySeen =
                                seenOpenStatements
                                |> List.exists (fun seenNs ->
                                    // if such open statement has already been marked as used in this or outer module, we skip it 
                                    // (that is, do not mark as used so far)
                                    rangeContainsRange seenNs.ModuleRange os.ModuleRange && 
                                    not (os.Idents |> Set.intersect seenNs.Idents |> Set.isEmpty))
                            alreadySeen
                
                match list with
                | os :: xs when notUsed os -> 
                    filterInner (os :: acc) xs (os :: seenOpenStatements)
                | os :: xs ->
                    filterInner acc xs (os :: seenOpenStatements)
                | [] -> List.rev acc
            
            filterInner [] list []

        openDeclarations |> getOpenStatements |> filter |> List.map (fun os -> os.Range)
