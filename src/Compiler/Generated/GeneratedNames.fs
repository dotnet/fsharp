module internal FSharp.Compiler.GeneratedNames

open System

/// Minimal abstraction for compiler-generated name replay/state.
/// Implementations can be hot-reload aware without coupling core compiler paths
/// to a concrete synthesized-name map type.
type ICompilerGeneratedNameMap =
    abstract BeginSession: unit -> unit
    abstract GetOrAddName: basicName: string -> string
    abstract Snapshot: seq<struct (string * string[])>
    abstract LoadSnapshot: snapshot: seq<struct (string * string[])> -> unit

/// Marker of occurrence-keyed closure class names produced by the hot reload closure
/// name allocation (Phase C3/C6, docs/hot-reload-closure-mapping.md):
/// `{base}@hotreload#g{generation}_o{occurrenceChain}`. Generation 0 names are minted
/// by flag-on BASELINE compiles (a pure function of lambda occurrence identity);
/// generation N >= 1 names are minted for occurrences first allocated by a delta
/// compile of session generation N. The `#g…_o…` suffix space is disjoint from the
/// replayable `-{ordinal}` suffix space of FSharpSynthesizedTypeMaps, so these names
/// never parse as replay ordinals and are never produced by sequence replay.
[<Literal>]
let HotReloadGenerationSuffixedNameInfix = "@hotreload#g"

/// Recognizes occurrence-keyed (generation-suffixed) closure class names
/// (`{base}@hotreload#g{N}_o{chain}`), any generation.
let IsHotReloadGenerationSuffixedName (name: string) =
    not (String.IsNullOrEmpty name)
    && name.IndexOf(HotReloadGenerationSuffixedNameInfix, StringComparison.Ordinal) >= 0

/// Parses the generation of an occurrence-keyed closure class name:
/// `f@hotreload#g2_o3` -> Some 2. None when the name is not generation-suffixed
/// (or malformed).
let TryGetHotReloadNameGeneration (name: string) : int option =
    if String.IsNullOrEmpty name then
        None
    else
        match name.IndexOf(HotReloadGenerationSuffixedNameInfix, StringComparison.Ordinal) with
        | -1 -> None
        | markerIndex ->
            let digitsStart = markerIndex + HotReloadGenerationSuffixedNameInfix.Length
            let digitsEnd = name.IndexOf("_o", digitsStart, StringComparison.Ordinal)

            if digitsEnd <= digitsStart then
                None
            else
                match Int32.TryParse(name.Substring(digitsStart, digitsEnd - digitsStart)) with
                | true, generation when generation >= 0 -> Some generation
                | _ -> None

