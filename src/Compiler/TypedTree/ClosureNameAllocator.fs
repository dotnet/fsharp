// Copyright (c) Microsoft Corporation. All Rights Reserved.

/// <summary>
/// Occurrence-keyed closure name allocation for hot reload delta compiles (Phase C3).
///
/// The replayable name map (<c>FSharpSynthesizedTypeMaps</c> behind
/// <c>ICompilerGeneratedNameMap</c>) keys synthesized names by basic-name SEQUENCE: the
/// i-th request for basic name <c>f</c> replays the i-th recorded name. That keeps
/// closure class names stable for pure body edits, but any change to the lambda SET
/// shifts every subsequent allocation, which is why edits that add or remove lambdas
/// must currently be rejected as rude.
///
/// This module re-keys closure identity by OCCURRENCE instead (the Roslyn
/// <c>EncVariableSlotAllocator.TryGetPreviousLambda/TryGetPreviousClosure</c> analogue,
/// expressed over the C1 typed-tree occurrence model):
///
///  - a fresh-compile occurrence that ALIGNS with a baseline occurrence (same two-pass
///    LCS as the C1 diff, with equal capture sets) reuses the baseline closure class
///    name VERBATIM;
///  - an occurrence with no baseline counterpart — or whose baseline counterpart has an
///    incompatible capture set, or whose baseline name is unknown — gets a FRESH name
///    suffixed with the session generation (the Roslyn <c>DebugId(ordinal, generation)</c>
///    analogue);
///  - a baseline occurrence with no fresh counterpart leaves its name unused forever
///    (removed names never re-enter the chained table, so they can never be reused).
///
/// Generation-suffixed name format (documented in docs/hot-reload-closure-mapping.md):
///
///     {baseName}@hotreload#g{generation}_o{occurrenceOrdinal}
///
/// e.g. <c>f@hotreload#g2_o3</c> for the occurrence with ordinal 3 first allocated in
/// generation 2 of a session. The format extends the existing baseline naming
/// (<c>f@hotreload</c>, <c>f@hotreload-1</c>, ...) produced by FSharpSynthesizedTypeMaps:
/// it shares the <c>@hotreload</c> marker so the names remain recognizably hot-reload
/// managed, while <c>#g…_o…</c> can never collide with the baseline <c>-{ordinal}</c>
/// suffix space (and never parses as a baseline replay ordinal). Occurrence ordinals are
/// unique within a member and generations are strictly increasing within a session, so a
/// (baseName, generation, ordinal) triple is allocated at most once.
///
/// This is a pure data transformation: it consumes the per-method occurrence data the
/// session already chains (baseline EnC CDI occurrence keys + the C1 extraction of the
/// fresh compile) and produces name assignments plus the refreshed occurrence→name
/// table to chain into the next generation. It performs no IO and touches no IlxGen
/// state, so it is fully unit-testable in isolation.
/// </summary>
module internal FSharp.Compiler.ClosureNameAllocator

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
    /// the new member itself is a Phase C4 concern; the allocator fixes the NAME).
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
    { /// One assignment per fresh-compile occurrence, in occurrence (pre-order
      /// ordinal) order — the order in which lowering encounters the closures.
      Assignments: (LambdaOccurrence * ClosureNameAssignment) list

      /// Root-first occurrence ordinal chain → assigned closure class name, for every
      /// fresh-compile occurrence. This is the table to chain forward: baseline names
      /// whose occurrences were removed are deliberately absent (never reused), and
      /// freshly allocated generation-suffixed names are present so the NEXT
      /// generation can reuse them for surviving occurrences.
      RefreshedNamesByOccurrenceChain: Map<int list, string> }

/// <summary>
/// Root-first ordinal chain of a C1 occurrence (enclosing ordinals outermost first,
/// ending with the occurrence's own ordinal) — the same chain
/// <c>EncMethodDebugInformation.tryEncodeOccurrenceKey</c> packs into the EnC CDI
/// "syntax offset" slots. The allocator works on unpacked chains so it carries no
/// dependency on the CDI packing limits; callers translating from decoded CDI keys
/// use <c>EncMethodDebugInformation.decodeOccurrenceKey</c>.
/// </summary>
let occurrenceOrdinalChain (occurrence: LambdaOccurrence) =
    List.rev occurrence.Id.ParentChain @ [ occurrence.Id.Ordinal ]

/// <summary>
/// Formats the generation-suffixed fresh closure class name for an occurrence first
/// allocated in <paramref name="generation"/>:
/// <c>{baseName}@hotreload#g{generation}_o{occurrenceOrdinal}</c>. See the module
/// documentation for the collision-freedom argument.
/// </summary>
let formatGenerationSuffixedClosureName (baseName: string) (generation: int) (occurrenceOrdinal: int) =
    CompilerGeneratedNameSuffix baseName $"hotreload#g{generation}_o{occurrenceOrdinal}"

/// <summary>
/// Recognizes the generation-suffixed fresh closure class names produced by
/// <see cref="formatGenerationSuffixedClosureName"/> (<c>{base}@hotreload#g{N}_o{i}</c>).
/// These names only ever exist for occurrences ADDED in a delta compile (the allocator
/// reuses baseline names verbatim for survivors), so the delta emitter uses this marker
/// to identify closure classes that must be emitted as NEW TypeDef rows.
/// </summary>
let isGenerationSuffixedClosureName (name: string) =
    not (System.String.IsNullOrEmpty name)
    && name.IndexOf("@hotreload#g", System.StringComparison.Ordinal) >= 0

/// <summary>
/// Allocates closure class names for the lambda occurrences of one member in a delta
/// compile.
/// </summary>
/// <param name="baselineOccurrences">The member's occurrence sequence in the previous
/// generation (extracted by C1 from the implementation files the session stores and
/// chains).</param>
/// <param name="baselineNamesByOccurrenceChain">The previous generation's
/// occurrence-chain→closure-class-name table for the member (generation 1: derived
/// from the baseline EnC CDI occurrence keys and the baseline name-map state stored
/// in the session; generation N+1: the RefreshedNamesByOccurrenceChain produced by
/// generation N's allocation). Occurrences absent from this table are treated as
/// unmappable and allocate fresh — fail closed, a name is never guessed.</param>
/// <param name="freshOccurrences">The member's occurrence sequence in the fresh
/// (edited) compile, from the C1 extraction.</param>
/// <param name="freshNameBase">Basic name lowering would use for the member's closure
/// classes (the enclosing let-bound value's compiled name).</param>
/// <param name="generation">The session generation the delta compile is producing
/// (the first delta after the baseline is generation 1).</param>
let allocateMemberClosureNames
    (baselineOccurrences: LambdaOccurrence list)
    (baselineNamesByOccurrenceChain: Map<int list, string>)
    (freshOccurrences: LambdaOccurrence list)
    (freshNameBase: string)
    (generation: int)
    : MemberClosureNameAllocation =

    let olds = Array.ofList baselineOccurrences
    let news = Array.ofList freshOccurrences

    // Same alignment as the C1 lambda-edit classification: pass 1 on the full
    // structural digest, pass 2 (shape-only) pairing reordered survivors and
    // capture-incompatible occurrences.
    let baselineIndexByFreshIndex =
        alignLambdaOccurrenceIndexPairs olds news
        |> List.map (fun (i, j) -> j, i)
        |> Map.ofList

    let assignments =
        news
        |> Array.mapi (fun j freshOcc ->
            let reusableBaselineName =
                match Map.tryFind j baselineIndexByFreshIndex with
                | Some i ->
                    let baselineOcc = olds[i]

                    // A shape-only (pass 2) pair with a different capture set is NOT
                    // compatible: per Roslyn semantics the previous closure member is
                    // stubbed and a new one is synthesized fresh, so the baseline name
                    // must not be reused. Capture lists are deterministically ordered
                    // (C1), so structural list equality is the set comparison.
                    if baselineOcc.Captures = freshOcc.Captures then
                        Map.tryFind (occurrenceOrdinalChain baselineOcc) baselineNamesByOccurrenceChain
                    else
                        None
                | None -> None

            let assignment =
                match reusableBaselineName with
                | Some name -> ClosureNameAssignment.Reused name
                | None ->
                    ClosureNameAssignment.Fresh(
                        formatGenerationSuffixedClosureName freshNameBase generation freshOcc.Id.Ordinal
                    )

            freshOcc, assignment)
        |> List.ofArray

    // Chain-forward table: every FRESH occurrence's chain maps to its assigned name.
    // Baseline occurrences without a fresh counterpart contribute nothing — their
    // names fall out of the chain and are never reused by construction (later
    // generations can only allocate generation-suffixed names, and the generation
    // counter never repeats within a session).
    let refreshedNames =
        assignments
        |> List.map (fun (occ, assignment) -> occurrenceOrdinalChain occ, assignment.Name)
        |> Map.ofList

    { Assignments = assignments
      RefreshedNamesByOccurrenceChain = refreshedNames }

/// <summary>
/// Joins a baseline compile's stamp -> emitted-closure-name recording (captured at the
/// IlxGen closure call site) with the lambda occurrence extraction of the SAME typed
/// tree, producing the per-member occurrence-chain -> closure-class-name tables the
/// allocator consumes in later generations. Keying is by IL method (compiled) name with
/// the same fail-closed rules as the C2 CDI emission (the token resolution happens where
/// baseline MethodDef tokens are known):
///  - members without a compiled name, or whose compiled name is claimed by more than
///    one member binding in the assembly, are dropped (a table can never describe the
///    wrong method);
///  - a member is dropped entirely when ANY of its occurrences has no recorded name
///    (closure formation diverged from extraction) — a partial table could force fresh
///    names onto surviving closures, so the member instead stays on sequence replay.
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
            |> List.choose (fun (symbol, _) -> symbol.CompiledName)
            |> List.countBy id
            |> List.filter (fun (_, count) -> count > 1)
            |> List.map fst
            |> Set.ofList

        (Map.empty, allMembers)
        ||> List.fold (fun acc (symbol: SymbolId, occurrences) ->
            match symbol.CompiledName, occurrences with
            | Some methName, _ :: _ when not (Set.contains methName ambiguousNames) ->
                let entries =
                    occurrences
                    |> List.map (fun occ ->
                        Map.tryFind occ.RootExprStamp closureNamesByStamp
                        |> Option.map (fun name -> occurrenceOrdinalChain occ, name))

                if entries |> List.forall Option.isSome then
                    Map.add methName (entries |> List.map Option.get |> Map.ofList) acc
                else
                    acc
            | _ -> acc)
