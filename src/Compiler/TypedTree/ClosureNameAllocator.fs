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

/// <summary>
/// Root-first ordinal chain of a lambda occurrence (enclosing ordinals outermost first,
/// ending with the occurrence's own ordinal) — the same chain
/// <c>EncMethodDebugInformation.tryEncodeOccurrenceKey</c> packs into the EnC CDI
/// "syntax offset" slots. The allocator works on unpacked chains so it carries no
/// dependency on the CDI packing limits; callers translating from decoded CDI keys
/// use <c>EncMethodDebugInformation.decodeOccurrenceKey</c>.
/// </summary>
let occurrenceOrdinalChain (occurrence: LambdaOccurrence) =
    List.rev (occurrence.Id.Ordinal :: occurrence.Id.ParentChain)

/// <summary>
/// Renders a root-first occurrence ordinal chain into the name-suffix key:
/// <c>[3]</c> -> <c>"3"</c>, <c>[0; 3]</c> -> <c>"0_3"</c>. Parseable back by
/// splitting on <c>'_'</c>; ordinals are plain non-negative decimals so the rendering
/// is unambiguous.
/// </summary>
let formatOccurrenceChainKey (ordinalChain: int list) =
    ordinalChain |> Seq.map string |> String.concat "_"

/// <summary>
/// Formats the generation-suffixed closure class name for an occurrence first
/// allocated in <paramref name="generation"/> (0 = the baseline compile itself):
/// <c>{baseName}@hotreload#g{generation}_o{occurrenceChain}</c>. See the module
/// documentation for the collision-freedom argument.
/// </summary>
let formatGenerationSuffixedClosureName (baseName: string) (generation: int) (ordinalChain: int list) =
    CompilerGeneratedNameSuffix baseName $"hotreload#g{generation}_o{formatOccurrenceChainKey ordinalChain}"

/// <summary>
/// Recognizes the generation-suffixed closure class names produced by
/// <see cref="formatGenerationSuffixedClosureName"/> (<c>{base}@hotreload#g{N}_o{chain}</c>).
/// Under occurrence-derived baseline naming generation-0 names exist in the
/// baseline itself; names of LATER generations only ever exist for occurrences ADDED in
/// a delta compile (the allocator reuses baseline names verbatim for survivors), so the
/// delta emitter uses this marker together with the absence of a baseline TypeDef token
/// to identify closure classes that must be emitted as NEW TypeDef rows.
/// </summary>
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
    let baselineIndexByFreshIndex = Dictionary<int, int>()

    for i, j in alignLambdaOccurrenceIndexPairs olds news do
        baselineIndexByFreshIndex[j] <- i

    let baselineOccurrenceChains =
        baselineOccurrences
        |> Seq.map occurrenceOrdinalChain
        |> ImmutableHashSet.CreateRange

    let refreshedNames = ImmutableDictionary.CreateBuilder<int list, string>()

    let assignments =
        news
        |> Seq.mapi (fun j freshOcc ->
            let freshChain = occurrenceOrdinalChain freshOcc
            let mutable baselineIndex = 0
            let mutable baselineName = Unchecked.defaultof<string>

            let reusableBaselineName =
                // Persisted CDI can describe a closure absent from the in-memory tree.
                if
                    baselineNamesByOccurrenceChain.TryGetValue(freshChain, &baselineName)
                    && not (baselineOccurrenceChains.Contains freshChain)
                then
                    ValueSome baselineName
                elif baselineIndexByFreshIndex.TryGetValue(j, &baselineIndex) then
                    let baselineOcc = olds[baselineIndex]

                    // Capture changes require a fresh closure even when occurrence shapes align.
                    if
                        baselineOcc.Captures = freshOcc.Captures
                        && baselineNamesByOccurrenceChain.TryGetValue(occurrenceOrdinalChain baselineOcc, &baselineName)
                    then
                        ValueSome baselineName
                    else
                        ValueNone
                else
                    ValueNone

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
            |> Seq.filter (fun (_, count) -> count > 1)
            |> Seq.map fst
            |> fun names -> ImmutableHashSet.CreateRange(StringComparer.Ordinal, names)

        (Map.empty, allMembers)
        ||> List.fold (fun acc (symbol: SymbolId, occurrences) ->
            match symbol.CompiledName, occurrences with
            | Some methName, _ :: _ when not (ambiguousNames.Contains methName) ->
                let entries =
                    (Map.empty, occurrences)
                    ||> List.fold (fun entries occurrence ->
                        let mutable name = Unchecked.defaultof<string>

                        if closureNamesByStamp.TryGetValue(occurrence.RootExprStamp, &name) then
                            Map.add (occurrenceOrdinalChain occurrence) name entries
                        else
                            entries)

                if Map.isEmpty entries then
                    acc
                else
                    Map.add methName entries acc
            | _ -> acc)
