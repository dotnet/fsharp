// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Cycle group processing for cross-file mutual recursion (Level B).
module internal FSharp.Compiler.CycleGroupProcessing

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Xml

/// Compute the longest common prefix of a non-empty list of LongIdent.
/// Returns the prefix (possibly empty if files share no common namespace).
let private commonPrefix (longIds: LongIdent list) : LongIdent =
    match longIds with
    | [] -> []
    | first :: rest ->
        let mutable prefix = first
        for li in rest do
            // Take prefix common between current `prefix` and `li`
            let pairs = List.zip (List.truncate (min prefix.Length li.Length) prefix)
                                 (List.truncate (min prefix.Length li.Length) li)
            let common =
                pairs
                |> List.takeWhile (fun (a: Ident, b: Ident) -> a.idText = b.idText)
                |> List.map fst
            prefix <- common
        prefix

/// Given a top-level SynModuleOrNamespace and a common prefix to strip,
/// produce a SynModuleDecl.NestedModule whose name is the remaining tail.
/// Example: input `module Foo.Bar.Baz = decls` with prefix `[Foo; Bar]`
/// becomes `SynModuleDecl.NestedModule(name=[Baz], decls=decls)`.
let private rewriteAsNestedModule (prefix: LongIdent) (modOrNs: SynModuleOrNamespace) : SynModuleDecl option =
    let (SynModuleOrNamespace(longId, _isRec, kind, decls, xmlDoc, attribs, accessibility, range, _trivia)) = modOrNs
    let prefixLen = prefix.Length

    // If the original was a namespace (not a module), we can't represent it as a NestedModule.
    // For now, only handle named modules. Namespaces in cycle groups are an edge case.
    match kind with
    | SynModuleOrNamespaceKind.NamedModule ->
        // Strip the common prefix from the longId; what remains becomes the nested module name
        let remainingId = List.skip prefixLen longId
        match remainingId with
        | [] ->
            // The module name was entirely the prefix; nothing to nest. Skip.
            None
        | name ->
            let componentInfo =
                SynComponentInfo(
                    attribs,
                    None,  // typeParams
                    [],    // constraints
                    name,
                    xmlDoc,
                    false, // preferPostfix
                    accessibility,
                    range
                )
            let nestedModuleTrivia : SynModuleDeclNestedModuleTrivia = {
                ModuleKeyword = None
                EqualsRange = None
            }
            Some(SynModuleDecl.NestedModule(componentInfo, false, decls, false, range, nestedModuleTrivia))
    | _ ->
        // Namespaces, anon modules, global namespace — pass through as a non-recursive nested decl.
        // This isn't ideal but avoids crashing.
        None

/// Synthesize a single implementation file from a list of cycle group files.
/// Strategy: detect common namespace prefix, wrap all modules in `namespace rec <prefix>`
/// so they become mutually recursive within that recursive namespace.
let synthesizeCycleGroupImpl (groupId: int) (files: ParsedImplFileInput list) : ParsedImplFileInput =
    match files with
    | [] -> failwith "synthesizeCycleGroupImpl: empty file list"
    | [ single ] -> single  // Single-file group is just that file
    | _ ->
        let firstFile = List.head files
        let (ParsedImplFileInput(_, isScript, _, _, _, _, trivia, _)) = firstFile

        let syntheticFileName = sprintf "_cyclegroup_%d.fs" groupId

        let firstQualName =
            let (ParsedImplFileInput(qualifiedNameOfFile = qn)) = firstFile
            qn

        let allHashDirectives =
            files |> List.collect (fun (ParsedImplFileInput(hashDirectives = hds)) -> hds)

        // Collect all top-level SynModuleOrNamespace from all files
        let allTopLevels =
            files |> List.collect (fun (ParsedImplFileInput(contents = cs)) -> cs)

        // Find the common namespace prefix among all named modules
        let namedModuleLongIds =
            allTopLevels
            |> List.choose (fun (SynModuleOrNamespace(longId = lid; kind = k)) ->
                match k with
                | SynModuleOrNamespaceKind.NamedModule -> Some lid
                | _ -> None)

        let prefix = commonPrefix namedModuleLongIds

        // Determine the wrapping namespace structure.
        // If all modules share a common prefix (e.g., Fantomas.Core), wrap in
        // `namespace rec Fantomas.Core` containing each as a nested module.
        // Otherwise fall back to wrapping in `namespace rec global`.
        let mergedRange =
            allTopLevels
            |> List.map (fun (SynModuleOrNamespace(range = r)) -> r)
            |> List.fold unionRanges range0

        let nestedDecls =
            allTopLevels |> List.choose (rewriteAsNestedModule prefix)

        let mergedContent =
            let kind, longId =
                if prefix.IsEmpty then
                    SynModuleOrNamespaceKind.GlobalNamespace, []
                else
                    SynModuleOrNamespaceKind.DeclaredNamespace, prefix
            let nsTrivia : SynModuleOrNamespaceTrivia = {
                LeadingKeyword = SynModuleOrNamespaceLeadingKeyword.Namespace mergedRange
            }
            SynModuleOrNamespace(
                longId,
                true,  // isRecursive — KEY for mutual recursion
                kind,
                nestedDecls,
                PreXmlDoc.Empty,
                [],  // attribs
                None, // accessibility
                mergedRange,
                nsTrivia
            )

        let isLastCompiland, isExe =
            files
            |> List.fold (fun (last, exe) (ParsedImplFileInput(flags = (l, e))) ->
                (last || l), (exe || e)) (false, false)

        let allIdentifiers =
            files
            |> List.fold (fun acc (ParsedImplFileInput(identifiers = ids)) -> Set.union acc ids) Set.empty

        let result =
            ParsedImplFileInput(
                syntheticFileName,
                isScript,
                firstQualName,
                allHashDirectives,
                [ mergedContent ],
                (isLastCompiland, isExe),
                trivia,
                allIdentifiers
            )

        // Optional debug dump
        let debugPathOpt =
            match System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_DEBUG" with
            | null -> None
            | "" -> None
            | v -> Some v
        match debugPathOpt with
        | Some p ->
            use w = System.IO.File.AppendText(p)
            w.WriteLine(sprintf "=== Synthesized cycle group %d ===" groupId)
            w.WriteLine(sprintf "  prefix: %s" (prefix |> List.map (fun i -> i.idText) |> String.concat "."))
            w.WriteLine(sprintf "  files: %d, top-level decls: %d" files.Length nestedDecls.Length)
            for d in nestedDecls do
                match d with
                | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = lid)) ->
                    w.WriteLine(sprintf "    nested module: %s" (lid |> List.map (fun i -> i.idText) |> String.concat "."))
                | _ ->
                    w.WriteLine(sprintf "    other decl: %A" d)
        | None -> ()

        result

/// Synthesize a single signature file from a list of cycle group sig files.
/// Same strategy as the impl version.
let synthesizeCycleGroupSig (groupId: int) (files: ParsedSigFileInput list) : ParsedSigFileInput =
    match files with
    | [] -> failwith "synthesizeCycleGroupSig: empty file list"
    | [ single ] -> single
    | _ ->
        let firstFile = List.head files
        let (ParsedSigFileInput(_, _, _, _, trivia, _)) = firstFile

        let syntheticFileName = sprintf "_cyclegroup_sig_%d.fsi" groupId

        let firstQualName =
            let (ParsedSigFileInput(qualifiedNameOfFile = qn)) = firstFile
            qn

        let allHashDirectives =
            files |> List.collect (fun (ParsedSigFileInput(hashDirectives = hds)) -> hds)

        let allTopLevels =
            files |> List.collect (fun (ParsedSigFileInput(contents = cs)) -> cs)

        // For sig files, we keep top-level structure simpler: just mark each as recursive.
        // (Signature files are less commonly cycle-prone.)
        let recursiveContents =
            allTopLevels
            |> List.map (fun (SynModuleOrNamespaceSig(longId, _isRec, kind, decls, xmlDoc, attribs, accessibility, range, trivia)) ->
                SynModuleOrNamespaceSig(longId, true, kind, decls, xmlDoc, attribs, accessibility, range, trivia))

        let allIdentifiers =
            files
            |> List.fold (fun acc (ParsedSigFileInput(identifiers = ids)) -> Set.union acc ids) Set.empty

        ParsedSigFileInput(
            syntheticFileName,
            firstQualName,
            allHashDirectives,
            recursiveContents,
            trivia,
            allIdentifiers
        )
