module internal FSharp.Compiler.GeneratedNames

open System
open System.Text.RegularExpressions

/// Minimal abstraction for compiler-generated name replay/state.
/// Implementations can be hot-reload aware without coupling core compiler paths
/// to a concrete synthesized-name map type.
type ICompilerGeneratedNameMap =
    abstract BeginSession: unit -> unit
    abstract GetOrAddName: basicName: string -> string
    abstract Snapshot: seq<struct (string * string[])>
    abstract LoadSnapshot: snapshot: seq<struct (string * string[])> -> unit

/// Marker of occurrence-keyed closure class names produced by hot reload closure
/// name allocation:
/// `{base}@hotreload#g{generation}_o{occurrenceChain}`. Generation 0 names are minted
/// by flag-on baseline compiles. Generation N >= 1 names are minted for occurrences
/// first allocated by a delta compile of session generation N. The `#g..._o...`
/// suffix space is disjoint from the replayable `-{ordinal}` suffix space of
/// FSharpSynthesizedTypeMaps, so these names never parse as replay ordinals and are
/// never produced by sequence replay.
[<Literal>]
let HotReloadGenerationSuffixedNameInfix = "@hotreload#g"

type SynthesizedPositionalName =
    {
        NormalizedBasicName: string
        Ordinal: int list
    }

type HotReloadReplayName =
    {
        NormalizedBasicName: string
        ReplayOrdinal: int
    }

type HotReloadGenerationName =
    {
        NormalizedBasicName: string
        Generation: int
        OccurrenceOrdinal: int list
    }

let private debugPipeNameRegex =
    lazy Regex(@"^Pipe #[1-9][0-9]* (?:input|stage #[1-9][0-9]*) at line ([1-9][0-9]*)$", RegexOptions.CultureInvariant)

let private tryParseNonNegativeInt (text: string) =
    match Int32.TryParse text with
    | true, value when value >= 0 -> Some value
    | _ -> None

let private tryParsePositiveInt (text: string) =
    match Int32.TryParse text with
    | true, value when value > 0 -> Some value
    | _ -> None

let private tryParseLineOrdinalSuffix (suffix: string) =
    let dashIndex = suffix.IndexOf('-')

    if dashIndex < 0 then
        tryParsePositiveInt suffix |> Option.map (fun line -> line, 0)
    elif dashIndex > 0 && dashIndex < suffix.Length - 1 then
        match tryParsePositiveInt (suffix.Substring(0, dashIndex)), tryParseNonNegativeInt (suffix.Substring(dashIndex + 1)) with
        | Some line, Some ordinal -> Some(line, ordinal)
        | _ -> None
    else
        None

let private tryNormalizeDebugPipeBasicName (name: string) =
    let matchResult = debugPipeNameRegex.Value.Match name

    if matchResult.Success then
        match tryParsePositiveInt matchResult.Groups[1].Value with
        | Some line ->
            let marker = " at line "
            let markerIndex = name.LastIndexOf(marker, StringComparison.Ordinal)

            if markerIndex > 0 then
                Some(name.Substring(0, markerIndex), line)
            else
                None
        | None -> None
    else
        None

let private tryParseOccurrenceOrdinal (text: string) =
    if String.IsNullOrWhiteSpace text then
        None
    else
        let parts = text.Split([| '_' |], StringSplitOptions.None)

        if parts |> Array.exists String.IsNullOrWhiteSpace then
            None
        else
            let parsed = parts |> Array.map tryParseNonNegativeInt

            if parsed |> Array.forall Option.isSome then
                Some(parsed |> Array.map Option.get |> Array.toList)
            else
                None

let private positionalName normalizedBasicName ordinal =
    {
        NormalizedBasicName = normalizedBasicName
        Ordinal = ordinal
    }

let private tryNormalizeDebugPipeName (name: string) =
    tryNormalizeDebugPipeBasicName name
    |> Option.map (fun (normalizedBasicName, line) -> positionalName normalizedBasicName [ line; 0 ])

let TryNormalizeHotReloadGenerationName (name: string) =
    let markerIndex =
        name.IndexOf(HotReloadGenerationSuffixedNameInfix, StringComparison.Ordinal)

    if markerIndex <= 0 then
        None
    else
        let baseName = name.Substring(0, markerIndex)
        let generationStart = markerIndex + HotReloadGenerationSuffixedNameInfix.Length
        let ordinalMarker = "_o"

        let ordinalMarkerIndex =
            name.IndexOf(ordinalMarker, generationStart, StringComparison.Ordinal)

        if
            ordinalMarkerIndex <= generationStart
            || ordinalMarkerIndex + ordinalMarker.Length >= name.Length
            || String.IsNullOrWhiteSpace baseName
            || baseName.IndexOf("@", StringComparison.Ordinal) >= 0
        then
            None
        else
            match
                tryParseNonNegativeInt (name.Substring(generationStart, ordinalMarkerIndex - generationStart)),
                tryParseOccurrenceOrdinal (name.Substring(ordinalMarkerIndex + ordinalMarker.Length))
            with
            | Some generation, Some occurrenceOrdinal ->
                let normalizedBasicName =
                    match tryNormalizeDebugPipeBasicName baseName with
                    | Some(normalizedPipeName, _) -> normalizedPipeName
                    | None -> baseName

                Some
                    {
                        NormalizedBasicName = normalizedBasicName
                        Generation = generation
                        OccurrenceOrdinal = occurrenceOrdinal
                    }
            | _ -> None

/// Recognizes well-formed occurrence-keyed generation-suffixed closure class names:
/// `{base}@hotreload#g{N}_o{chain}`, any generation.
let IsHotReloadGenerationSuffixedName (name: string) =
    not (String.IsNullOrEmpty name)
    && (TryNormalizeHotReloadGenerationName name |> Option.isSome)

/// Parses the generation of a well-formed occurrence-keyed closure class name:
/// `f@hotreload#g2_o3` -> Some 2. None when the name is not generation-suffixed
/// or any part of the name is malformed.
let TryGetHotReloadNameGeneration (name: string) : int option =
    if String.IsNullOrEmpty name then
        None
    else
        TryNormalizeHotReloadGenerationName name
        |> Option.map _.Generation

let TryNormalizeHotReloadReplayName (name: string) =
    let marker = "@hotreload"
    let markerIndex = name.LastIndexOf(marker, StringComparison.Ordinal)

    if markerIndex <= 0 then
        None
    else
        let suffixStart = markerIndex + marker.Length
        let suffix = name.Substring suffixStart
        let baseName = name.Substring(0, markerIndex)

        if
            String.IsNullOrWhiteSpace baseName
            || baseName.IndexOf("@", StringComparison.Ordinal) >= 0
        then
            None
        else
            let ordinalOpt =
                if suffix = "" then
                    Some 0
                elif suffix.StartsWith("-", StringComparison.Ordinal) then
                    tryParsePositiveInt (suffix.Substring 1)
                else
                    None

            ordinalOpt
            |> Option.map (fun ordinal ->
                let normalizedBasicName =
                    match tryNormalizeDebugPipeBasicName baseName with
                    | Some(normalizedPipeName, _) -> normalizedPipeName
                    | None -> baseName

                {
                    NormalizedBasicName = normalizedBasicName
                    ReplayOrdinal = ordinal
                })

let private tryNormalizeHotReloadOrdinalName (name: string) =
    TryNormalizeHotReloadReplayName name
    |> Option.map (fun replayName ->
        let ordinal =
            let markerIndex = name.LastIndexOf("@hotreload", StringComparison.Ordinal)
            let baseName = name.Substring(0, markerIndex)

            match tryNormalizeDebugPipeBasicName baseName with
            | Some(_, line) -> [ line; replayName.ReplayOrdinal ]
            | None -> [ replayName.ReplayOrdinal ]

        positionalName replayName.NormalizedBasicName ordinal)

let private tryNormalizeLineOrdinalName (name: string) =
    let atIndex = name.LastIndexOf('@')

    if atIndex <= 0 || atIndex = name.Length - 1 then
        None
    else
        let baseName = name.Substring(0, atIndex)
        let suffix = name.Substring(atIndex + 1)

        match tryParseLineOrdinalSuffix suffix with
        | None -> None
        | Some(line, ordinal) ->
            match tryNormalizeDebugPipeBasicName baseName with
            | Some(normalizedPipeName, pipeLine) when pipeLine = line -> Some(positionalName normalizedPipeName [ line; ordinal ])
            | Some _ -> None
            | None ->
                if
                    String.IsNullOrWhiteSpace baseName
                    || baseName.IndexOf("@", StringComparison.Ordinal) >= 0
                    || baseName.StartsWith("Pipe #", StringComparison.Ordinal)
                then
                    None
                else
                    Some(positionalName baseName [ line; ordinal ])

let tryNormalizeSynthesizedTypeNameForPositionalPairing (name: string) =
    if String.IsNullOrWhiteSpace name then
        None
    else
        match tryNormalizeHotReloadOrdinalName name with
        | Some normalized -> Some normalized
        | None ->
            match tryNormalizeLineOrdinalName name with
            | Some normalized -> Some normalized
            | None -> tryNormalizeDebugPipeName name

let SynthesizedNameMapKey (basicName: string) =
    match tryNormalizeSynthesizedTypeNameForPositionalPairing basicName with
    | Some normalized -> normalized.NormalizedBasicName
    | None -> basicName
