// Picks a RANDOM insertion point within a release-notes section.
//
// Why: every PR used to prepend its bullet at the top of "### Fixed", so two
// concurrent PRs always touched the same lines and merge-conflicted. Inserting
// at a random line within the section instead spreads edits out, dropping the
// pairwise conflict rate from ~100% to ~0-2%.
//
// Usage (run from anywhere in the repo):
//   dotnet fsi .github/skills/release-notes/pick-insert-line.fsx --sink FSharp.Compiler.Service --section Fixed
//   dotnet fsi .github/skills/release-notes/pick-insert-line.fsx --file docs/release-notes/.FSharp.Core/11.0.100.md --section Added
//
// --sink auto-selects the NEWEST version file on disk, which can diverge from the
// version the repo variable VNEXT points at (e.g. on servicing branches). When the
// target version is known (it is in the skill flow), prefer passing --file with the
// VNEXT-derived path so the bullet always lands in the intended file.
//
// It prints an anchor line (number + verbatim content) and whether to insert ABOVE or
// BELOW it. Use the printed line as the edit anchor and place your bullet accordingly.

open System
open System.IO

let args = Environment.GetCommandLineArgs() |> List.ofArray

let getArg name =
    let rec find =
        function
        | a :: b :: _ when a = name -> Some b
        | _ :: rest -> find rest
        | [] -> None

    find args

let sink = getArg "--sink"
let fileArg = getArg "--file"
let section = (getArg "--section" |> Option.defaultValue "Fixed").Trim()

let repoRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", ".."))

let resolveFile () =
    match fileArg with
    | Some f -> Path.GetFullPath f
    | None ->
        match sink with
        | None -> failwith "Provide --file <path> or --sink <Name> (e.g. FSharp.Compiler.Service)."
        | Some s ->
            let dir = Path.Combine(repoRoot, "docs", "release-notes", "." + s)

            if not (Directory.Exists dir) then
                failwithf "Sink folder not found: %s" dir

            match s with
            | "Language" -> Path.Combine(dir, "preview.md")
            | _ ->
                // Newest version-like file: parse name into ints; non-ints (e.g. "vNext") sort highest.
                let versionKey (p: string) =
                    Path.GetFileNameWithoutExtension(p).Split('.')
                    |> Array.map (fun x ->
                        match Int32.TryParse x with
                        | true, v -> v
                        | _ -> Int32.MaxValue)
                    |> List.ofArray

                let chosen =
                    Directory.GetFiles(dir, "*.md") |> Array.sortBy versionKey |> Array.last

                // This is the newest file ON DISK, which may not match the VNEXT-targeted
                // version. Surface it (on stderr, so stdout stays the clean anchor output)
                // so a mismatch is noticed; pass --file to override.
                eprintfn
                    "note: --sink auto-selected newest on-disk file '%s'. If VNEXT targets another version, pass --file."
                    (Path.GetFileName chosen)

                chosen

let file = resolveFile ()

if not (File.Exists file) then
    failwithf "File not found: %s" file

let lines = File.ReadAllLines file

let rel =
    if file.StartsWith(repoRoot, StringComparison.Ordinal) then
        Path.GetRelativePath(repoRoot, file)
    else
        file
let isHeader (l: string) = l.TrimStart().StartsWith("### ")

let headerMatches (l: string) =
    let t = l.Trim()
    t.StartsWith("### ")
    && t.Substring(4).Trim().Equals(section, StringComparison.OrdinalIgnoreCase)

match lines |> Array.tryFindIndex headerMatches with
| None ->
    printfn "Section '### %s' not found in %s." section rel
    printfn "Add the '### %s' section (Keep A Changelog) and put your bullet under it." section
    exit 0
| Some h ->
    let sectionEnd =
        seq { h + 1 .. lines.Length - 1 }
        |> Seq.tryFind (fun i -> isHeader lines.[i])
        |> Option.defaultValue lines.Length

    let bulletIdxs =
        [ for i in h + 1 .. sectionEnd - 1 do
              if lines.[i].TrimStart().StartsWith("* ") then
                  yield i ]

    // Candidate insertion points, each a (non-blank anchor line, side) pair. Anchoring
    // only on non-blank lines (the header or an existing bullet) keeps the edit anchor
    // unique. Offering BOTH sides of every bullet plus the header means even a 0-1 bullet
    // section yields several distinct positions, so two concurrent PRs rarely pick the
    // same spot. Dedup by the resulting gap so we never present two options that land in
    // the same place (which would still merge-conflict).
    let candidates =
        [ yield h, "below"
          for b in bulletIdxs do
              yield b, "above"
              yield b, "below" ]
        |> List.distinctBy (fun (idx, side) -> if side = "above" then idx else idx + 1)

    let anchorIdx, side = candidates.[Random().Next(candidates.Length)]
    printfn "Insert your new bullet %s this line in %s:" (side.ToUpperInvariant()) rel
    printfn ""
    printfn "  line %d: %s" (anchorIdx + 1) lines.[anchorIdx]
    printfn ""

    if side = "above" then
        printfn "Use that exact line as the edit anchor: old_str = the line; new_str = <your bullet>\\n<the line>."
    else
        printfn "Use that exact line as the edit anchor: old_str = the line; new_str = <the line>\\n<your bullet>."

    printfn "(Random position within '### %s' — avoids the top-of-section merge conflicts.)" section
