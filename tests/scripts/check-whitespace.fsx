open System.IO
open System

let root = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..\\..\\"))

// Single * wildcards are permitted.  Where you might use ** to search recursively,
// use SearchOption.AllDirectories.  To only search a single directory, use
// SearchOption.TopDirectoryOnly.
let matches =
    [|
        //"*.fs", SearchOption.AllDirectories
    |]

let files =
    matches
    |> Seq.collect(fun (path, searchOption) ->
        seq { yield! Directory.EnumerateFiles(root, path, searchOption) })

let hasTrailingWhitespace fileName =
    let lines = File.ReadAllLines(fileName)
    lines
    |> Array.exists(fun line -> line.TrimEnd().Length <> line.Length)

let filesWithTrailingWhitespace =
    files
    |> Seq.fold(fun filesWithWhitespace file ->
       match hasTrailingWhitespace file with
       | true -> file::filesWithWhitespace
       | false -> filesWithWhitespace) []

filesWithTrailingWhitespace
|> List.iter(printfn "Whitespace found: %s")

match List.isEmpty filesWithTrailingWhitespace with
| true -> Environment.Exit(0)
| false -> Environment.Exit(1)