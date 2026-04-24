// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Cycle group processing for cross-file mutual recursion (Level B).
module internal FSharp.Compiler.CycleGroupProcessing

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Xml
open FSharp.Compiler.SymbolCollection
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Import

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
/// produce a list of SynModuleDecl entries representing its content within
/// the synthesized cycle-group namespace.
///
/// For NamedModule: produces a single NestedModule wrapping the original decls.
/// For DeclaredNamespace at the prefix level: the decls are spliced in directly
/// (the namespace IS the synthetic wrapper, so its content is already at the right level).
/// For other kinds: skip (rare edge case).
let private rewriteAsNestedDecls (prefix: LongIdent) (modOrNs: SynModuleOrNamespace) : SynModuleDecl list =
    let (SynModuleOrNamespace(longId, _isRec, kind, decls, xmlDoc, attribs, accessibility, range, _trivia)) = modOrNs
    let prefixLen = prefix.Length

    match kind with
    | SynModuleOrNamespaceKind.NamedModule ->
        // Strip the common prefix from the longId; what remains becomes the nested module name
        let remainingId = List.skip prefixLen longId
        match remainingId with
        | [] -> []  // Module name was entirely the prefix; skip
        | name ->
            let componentInfo =
                SynComponentInfo(
                    attribs,
                    None,
                    [],
                    name,
                    xmlDoc,
                    false,
                    accessibility,
                    range
                )
            let nestedModuleTrivia : SynModuleDeclNestedModuleTrivia = {
                ModuleKeyword = None
                EqualsRange = None
            }
            [ SynModuleDecl.NestedModule(componentInfo, false, decls, false, range, nestedModuleTrivia) ]

    | SynModuleOrNamespaceKind.DeclaredNamespace ->
        // If the namespace matches the common prefix exactly, splice its decls
        // directly into the synthesized wrapper (they're already at the right level).
        // If the namespace extends BEYOND the prefix (e.g., prefix=[Fantomas;Core] but
        // file declares `namespace Fantomas.Core.Extras`), wrap the decls in a nested
        // module with the remaining segments as the name.
        let remainingId = List.skip prefixLen longId
        match remainingId with
        | [] ->
            // Namespace == prefix; splice decls directly
            decls
        | extra ->
            // Wrap in a nested module representing the namespace tail
            let componentInfo =
                SynComponentInfo(attribs, None, [], extra, xmlDoc, false, accessibility, range)
            let nestedModuleTrivia : SynModuleDeclNestedModuleTrivia = {
                ModuleKeyword = None
                EqualsRange = None
            }
            [ SynModuleDecl.NestedModule(componentInfo, false, decls, false, range, nestedModuleTrivia) ]

    | _ ->
        // AnonModule / GlobalNamespace — splice decls directly
        decls

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

        // Find common prefix considering BOTH named modules AND declared namespaces
        // (Fantomas-style: some files declare `namespace Fantomas.Core` and provide
        // content directly, others declare `module Fantomas.Core.Foo`).
        let allLongIds =
            allTopLevels
            |> List.choose (fun (SynModuleOrNamespace(longId = lid; kind = k)) ->
                match k with
                | SynModuleOrNamespaceKind.NamedModule -> Some lid
                | SynModuleOrNamespaceKind.DeclaredNamespace -> Some lid
                | _ -> None)

        let prefix = commonPrefix allLongIds

        let mergedRange =
            allTopLevels
            |> List.map (fun (SynModuleOrNamespace(range = r)) -> r)
            |> List.fold unionRanges range0

        let nestedDecls =
            allTopLevels |> List.collect (rewriteAsNestedDecls prefix)

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


/// High-level entry point: apply --file-order-auto+ behavior to a list of parsed inputs.
let applyAutoFileOrder
    (g: TcGlobals)
    (amap: ImportMap)
    (tcEnv: TcEnv)
    (inputs: ParsedInput list)
    : ParsedInput list * TcEnv =

    if List.isEmpty inputs then
        (inputs, tcEnv)
    else
        // Step 1: run enter phase to populate TcEnv with stubs and gather FileDeclarations
        let parsedInputs =
            inputs
            |> List.toArray
            |> Array.map (fun (input: ParsedInput) -> (input.FileName, input))

        let tcEnvPrepopulated, fileDecls = runEnterPhase g amap tcEnv parsedInputs

        // Step 2: compute dependency-ordered compilation units
        let units = computeCompilationUnits fileDecls
        let inputsArray = inputs |> List.toArray

        // Step 3: process each unit (single files pass through, cycle groups synthesize)
        let mutable nextGroupId = 0
        let processedInputs =
            units
            |> Array.toList
            |> List.collect (fun unit ->
                match unit with
                | SingleFile idx -> [ inputsArray.[idx] ]
                | CycleGroup indices ->
                    let groupFiles = indices |> List.map (fun idx -> inputsArray.[idx])
                    // Cycle groups containing .fsi files fall back to original order
                    // (sig/impl pairing complications — see Track 03 plan).
                    let hasSigFile =
                        groupFiles |> List.exists (fun f ->
                            match f with
                            | ParsedInput.SigFile _ -> true
                            | _ -> false)
                    if hasSigFile then
                        groupFiles
                    else
                        let impls =
                            groupFiles |> List.choose (fun f ->
                                match f with
                                | ParsedInput.ImplFile i -> Some i
                                | _ -> None)
                        let groupId = nextGroupId
                        nextGroupId <- nextGroupId + 1
                        if impls.IsEmpty then []
                        else [ ParsedInput.ImplFile(synthesizeCycleGroupImpl groupId impls) ])

        // Step 4: fix up IsLastCompiland on the actual last file
        let reorderedInputs =
            let lastIdx = processedInputs.Length - 1
            processedInputs |> List.mapi (fun i input ->
                match input with
                | ParsedInput.ImplFile(ParsedImplFileInput(fileName, isScript, qualName, hashDirectives, contents, (_, isExe), trivia, idents)) ->
                    let isLast = (i = lastIdx)
                    ParsedInput.ImplFile(ParsedImplFileInput(fileName, isScript, qualName, hashDirectives, contents, (isLast, isExe), trivia, idents))
                | sigFile -> sigFile)

        (reorderedInputs, tcEnvPrepopulated)
