module LanguageServer.SemanticTokensHelpers

open System
open System.Text.RegularExpressions

open Microsoft.VisualStudio.LanguageServer.Protocol

// A decoded semantic token with absolute positions and resolved type/modifier names.
type DecodedToken =
    {
        Line: int
        StartChar: int
        Length: int
        TokenType: string
        TokenModifiers: string list
    }

// Decode the LSP semantic tokens data array (relative encoding, 5 ints per token)
// into a list of tokens with absolute positions and resolved type/modifier names.
let decodeSemanticTokens (legend: SemanticTokensLegend) (data: int array) : DecodedToken list =
    if data = null || data.Length = 0 then
        []
    else
        assert (data.Length % 5 = 0)

        let tokenCount = data.Length / 5
        let mutable prevLine = 0
        let mutable prevStartChar = 0

        [ for i in 0 .. tokenCount - 1 do
            let offset = i * 5
            let deltaLine = data[offset]
            let deltaStart = data[offset + 1]
            let length = data[offset + 2]
            let tokenTypeIdx = data[offset + 3]
            let modifierBits = data[offset + 4]

            let line = prevLine + deltaLine

            let startChar =
                if deltaLine = 0 then
                    prevStartChar + deltaStart
                else
                    deltaStart

            prevLine <- line
            prevStartChar <- startChar

            let tokenType =
                if tokenTypeIdx >= 0 && tokenTypeIdx < legend.TokenTypes.Length then
                    legend.TokenTypes[tokenTypeIdx]
                else
                    $"<unknown:{tokenTypeIdx}>"

            let modifiers =
                [ for bit in 0 .. legend.TokenModifiers.Length - 1 do
                    if modifierBits &&& (1 <<< bit) <> 0 then
                        legend.TokenModifiers[bit] ]

            { Line = line
              StartChar = startChar
              Length = length
              TokenType = tokenType
              TokenModifiers = modifiers } ]

// Marker pattern: (*N*) where N is one or more characters.
let private markerPattern = Regex(@"\(\*\d+\*\)", RegexOptions.Compiled)

// Strip all (*N*) markers from source text and return the cleaned source
// plus a map from marker name to (line, character) in the cleaned source.
let stripMarkers (source: string) : string * Map<string, (int * int)> =
    let mutable cleanedParts = System.Text.StringBuilder()
    let mutable markerPositions = Map.empty
    let mutable srcIdx = 0
    let mutable line = 0
    let mutable col = 0

    let matches = markerPattern.Matches(source) |> Seq.cast<Match> |> Seq.toList

    for m in matches do
        // Append text before this marker
        for i in srcIdx .. m.Index - 1 do
            cleanedParts.Append(source[i]) |> ignore

            if source[i] = '\n' then
                line <- line + 1
                col <- 0
            else
                col <- col + 1

        // Record marker position (position right after the marker in cleaned text)
        markerPositions <- markerPositions |> Map.add m.Value (line, col)
        srcIdx <- m.Index + m.Length

    // Append remaining text
    for i in srcIdx .. source.Length - 1 do
        cleanedParts.Append(source[i]) |> ignore

    cleanedParts.ToString(), markerPositions

// Find the token at a specific (line, character) position.
let findTokenAtPosition (line: int) (character: int) (tokens: DecodedToken list) : DecodedToken option =
    tokens
    |> List.tryFind (fun t -> t.Line = line && t.StartChar <= character && character < t.StartChar + t.Length)

// Find the token right after a marker position. The marker position should point
// to the start of the token immediately following the marker.
let findTokenAtMarker (markerPositions: Map<string, (int * int)>) (marker: string) (tokens: DecodedToken list) : DecodedToken option =
    match markerPositions |> Map.tryFind marker with
    | None -> failwith $"Marker '{marker}' not found in marker positions map"
    | Some(line, col) -> findTokenAtPosition line col tokens

// Find all tokens on a given line.
let findTokensOnLine (line: int) (tokens: DecodedToken list) : DecodedToken list =
    tokens |> List.filter (fun t -> t.Line = line)

// Validate that decoded tokens are in monotonically increasing order,
// have positive lengths, and don't overlap on the same line.
let validateTokenOrdering (tokens: DecodedToken list) : unit =
    let mutable prevLine = -1
    let mutable prevEnd = -1

    for t in tokens do
        if t.Line < prevLine then
            failwith $"Tokens not in line order: token at line {t.Line} appears after line {prevLine}"

        if t.Line = prevLine && t.StartChar < prevEnd then
            failwith
                $"Overlapping tokens on line {t.Line}: token at col {t.StartChar} overlaps with previous token ending at col {prevEnd}"

        if t.Length <= 0 then
            failwith $"Token at ({t.Line},{t.StartChar}) has non-positive length {t.Length}"

        prevLine <- t.Line
        prevEnd <- t.StartChar + t.Length
