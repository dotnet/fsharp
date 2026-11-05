// Copyright (c) Microsoft Corporation. All Rights Reserved.

/// Assigns stable closure names from typed-tree occurrence chains.
/// Compatible surviving occurrences retain their baseline names. New occurrences receive
/// names of the form {baseName}@hotreload#g{generation}_o{rootFirstOrdinalChain}.
/// Generation zero names identify baseline closures. Later generations never reuse removed names.
/// The replay-map marker remains shared, but the generation suffix cannot collide with replay ordinals.
module internal FSharp.Compiler.ClosureNameAllocator

open System
open System.Collections.Generic
open System.Collections.Immutable

open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff

/// <summary>
/// The name assigned to one fresh-compile lambda occurrence by the allocator.
/// </summary>
[<RequireQualifiedAccess>]
type ClosureNameAssignment =
    /// The occurrence aligned with a compatible baseline occurrence: its closure
    /// class must lower to exactly the baseline name so the delta updates the
    /// existing closure type in place.
    | Reused of name: string

    /// The occurrence has no compatible baseline counterpart: its closure class is a
    /// new synthesized member and gets a generation-suffixed fresh name (emission of
    /// the new member itself is the delta emitter's concern; the allocator fixes the NAME).
    | Fresh of name: string

    /// The assigned closure class name, regardless of provenance.
    member this.Name =
        match this with
        | Reused name
        | Fresh name -> name

/// <summary>
/// The allocation produced for one member: per-occurrence name assignments for the
/// fresh compile, plus the refreshed occurrence-chain→name table to store in the
/// next-generation baseline (the input table for the NEXT delta's allocation).
/// </summary>
type MemberClosureNameAllocation =
    {
        /// One assignment per fresh-compile occurrence, in occurrence (pre-order
        /// ordinal) order — the order in which lowering encounters the closures.
        Assignments: ImmutableArray<struct (LambdaOccurrence * ClosureNameAssignment)>

        /// Root-first occurrence ordinal chain → assigned closure class name, for every
        /// fresh-compile occurrence. This is the table to chain forward: baseline names
        /// whose occurrences were removed are deliberately absent (never reused), and
        /// freshly allocated generation-suffixed names are present so the NEXT
        /// generation can reuse them for surviving occurrences.
        RefreshedNamesByOccurrenceChain: ImmutableDictionary<int list, string>
    }

/// Root-first occurrence chain, shared with the EnC custom debug information encoder.
let occurrenceOrdinalChain (occurrence: LambdaOccurrence) =
    List.rev (occurrence.Id.Ordinal :: occurrence.Id.ParentChain)

/// Formats a root-first ordinal chain with underscore separators.
let formatOccurrenceChainKey (ordinalChain: int list) =
    ordinalChain |> Seq.map string |> String.concat "_"

/// Formats a fresh closure name. Generation zero identifies the baseline compile.
let formatGenerationSuffixedClosureName (baseName: string) (generation: int) (ordinalChain: int list) =
    CompilerGeneratedNameSuffix baseName $"hotreload#g{generation}_o{formatOccurrenceChainKey ordinalChain}"

/// Identifies the generation suffix used for occurrence-derived closure names.
let isGenerationSuffixedClosureName (name: string) =
    GeneratedNames.IsHotReloadGenerationSuffixedName name

/// <summary>
/// Allocates closure class names for the lambda occurrences of one member in a delta
/// compile.
/// </summary>
/// <param name="baselineOccurrences">The member's occurrence sequence in the previous
/// generation (extracted by the occurrence model from the implementation files the session stores and
/// chains).</param>
/// <param name="baselineNamesByOccurrenceChain">The previous generation's
/// occurrence-chain→closure-class-name table for the member (generation 1: derived
/// from the baseline EnC CDI occurrence keys and the baseline name-map state stored
/// in the session; generation N+1: the RefreshedNamesByOccurrenceChain produced by
/// generation N's allocation). Occurrences absent from this table are treated as
/// unmappable and allocate fresh — fail closed, a name is never guessed.</param>
/// <param name="freshOccurrences">The member's occurrence sequence in the fresh
/// (edited) compile, from the occurrence extraction.</param>
/// <param name="freshNameBase">Basic name lowering would use for the member's closure
/// classes (the enclosing let-bound value's compiled name).</param>
/// <param name="generation">The session generation the delta compile is producing
/// (the first delta after the baseline is generation 1).</param>
let allocateMemberClosureNames
    (baselineOccurrences: LambdaOccurrence list)
    (baselineNamesByOccurrenceChain: IReadOnlyDictionary<int list, string>)
    (freshOccurrences: LambdaOccurrence list)
    (freshNameBase: string)
    (generation: int)
    : MemberClosureNameAllocation =

    let olds = Array.ofList baselineOccurrences
    let news = Array.ofList freshOccurrences

    // Same alignment as the lambda-edit classification: pass 1 on the full
    // structural digest, pass 2 (shape-only) pairing reordered survivors and
    // capture-incompatible occurrences.
    let baselineIndexByFreshIndex =
        let indices = Dictionary<int, int>()

        for i, j in alignLambdaOccurrenceIndexPairs olds news do
            indices[j] <- i

        indices

    let baselineOccurrenceChains =
        baselineOccurrences
        |> Seq.map occurrenceOrdinalChain
        |> ImmutableHashSet.CreateRange

    let refreshedNames =
        ImmutableDictionary.CreateBuilder<int list, string>(HashIdentity.Structural)

    let assignments =
        news
        |> Seq.mapi (fun j freshOcc ->
            let freshChain = occurrenceOrdinalChain freshOcc

            let reusableBaselineName =
                match baselineIndexByFreshIndex.TryGetValue j, baselineNamesByOccurrenceChain.TryGetValue freshChain with
                // Disk-started sessions can have CDI+TypeDef evidence for a closure whose
                // occurrence is absent from the in-memory baseline tree. Reuse only the
                // exact persisted chain; normal capture compatibility still gates pairs
                // where both baseline and fresh occurrences are available.
                | _, (true, name) when not (baselineOccurrenceChains.Contains freshChain) -> ValueSome name
                | (true, i), _ ->
                    let baselineOcc = olds[i]

                    // A shape-only (pass 2) pair with a different capture set is NOT
                    // compatible: per Roslyn semantics the previous closure member is
                    // stubbed and a new one is synthesized fresh, so the baseline name
                    // must not be reused. Capture lists are deterministically ordered,
                    // so structural list equality is the set comparison.
                    if baselineOcc.Captures = freshOcc.Captures then
                        match baselineNamesByOccurrenceChain.TryGetValue(occurrenceOrdinalChain baselineOcc) with
                        | true, name -> ValueSome name
                        | _ -> ValueNone
                    else
                        ValueNone
                | (false, _), _ -> ValueNone

            let assignment =
                match reusableBaselineName with
                | ValueSome name -> ClosureNameAssignment.Reused name
                | ValueNone -> ClosureNameAssignment.Fresh(formatGenerationSuffixedClosureName freshNameBase generation freshChain)

            refreshedNames[freshChain] <- assignment.Name
            struct (freshOcc, assignment))
        |> ImmutableArray.CreateRange

    {
        Assignments = assignments
        RefreshedNamesByOccurrenceChain = refreshedNames.ToImmutable()
    }

/// <summary>
/// Joins a baseline compile's stamp -> emitted-closure-name recording (captured at the
/// IlxGen closure call site) with the lambda occurrence extraction of the SAME typed
/// tree, producing the per-member occurrence-chain -> closure-class-name tables the
/// allocator consumes in later generations. Keying is by IL method (compiled) name with
/// the same fail-closed rules as the baseline CDI emission (the token resolution happens where
/// baseline MethodDef tokens are known):
///  - members without a compiled name, or whose compiled name is claimed by more than
///    one member binding in the assembly, are dropped (a table can never describe the
///    wrong method);
///  - occurrences without a recorded name are omitted (they did not lower to a closure
///    class, or closure formation diverged from extraction), while recorded occurrences
///    remain replayable by their exact occurrence chain.
/// Members without lambda occurrences (including extraction-unsupported members, which
/// report an empty occurrence list) carry no table.
/// </summary>
let computeBaselineClosureNameRows
    (g: TcGlobals)
    (implFiles: CheckedImplFile list)
    (closureNamesByStamp: Map<int64, string>)
    : Map<string, Map<int list, string>> =

    if Map.isEmpty closureNamesByStamp then
        Map.empty
    else
        let allMembers = implFiles |> List.collect (collectMemberLambdaOccurrences g)

        let ambiguousNames =
            allMembers
            |> Seq.choose (fun (symbol, _) -> symbol.CompiledName)
            |> Seq.countBy id
            |> Seq.choose (fun (name, count) -> if count > 1 then Some name else None)
            |> fun names -> ImmutableHashSet.CreateRange(StringComparer.Ordinal, names)

        (Map.empty, allMembers)
        ||> List.fold (fun acc (symbol: SymbolId, occurrences) ->
            match symbol.CompiledName, occurrences with
            | Some methName, _ :: _ when not (ambiguousNames.Contains methName) ->
                let entries =
                    (Map.empty, occurrences)
                    ||> List.fold (fun names occ ->
                        match closureNamesByStamp.TryGetValue occ.RootExprStamp with
                        | true, name -> Map.add (occurrenceOrdinalChain occ) name names
                        | _ -> names)

                if not (Map.isEmpty entries) then
                    Map.add methName entries acc
                else
                    acc
            | _ -> acc)
