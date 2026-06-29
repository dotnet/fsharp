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
// It prints an existing bullet line (number + verbatim content). Insert your new
// bullet immediately ABOVE that line (use the printed line as the edit anchor).

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

                Directory.GetFiles(dir, "*.md") |> Array.sortBy versionKey |> Array.last

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

    match bulletIdxs with
    | [] -> printfn "Section '### %s' (%s) is empty. Add your bullet on line %d, just below the header." section rel (h + 2)
    | _ ->
        let pick = bulletIdxs.[Random().Next(bulletIdxs.Length)]
        printfn "Insert your new bullet ABOVE this line in %s:" rel
        printfn ""
        printfn "  line %d: %s" (pick + 1) lines.[pick]
        printfn ""
        printfn "Use that exact line as the edit anchor: old_str = the line; new_str = <your bullet>\\n<the line>."
        printfn "(Random position within '### %s' — avoids the top-of-section merge conflicts.)" section
