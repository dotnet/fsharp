module FSharpDiagServer.DiagnosticsFormatter

open FSharp.Compiler.Diagnostics

let private formatOne (getLines: string -> string[]) (d: FSharpDiagnostic) =
    let sev =
        match d.Severity with
        | FSharpDiagnosticSeverity.Error -> "ERROR"
        | _ -> "WARNING"

    let lines = getLines d.Range.FileName

    let src =
        if d.StartLine >= 1 && d.StartLine <= lines.Length then
            $" | {lines.[d.StartLine - 1].Trim()}"
        else
            ""

    $"{sev} {d.ErrorNumberText} ({d.StartLine},{d.Start.Column}-{d.EndLine},{d.End.Column}) {d.Message.Replace('\n', ' ').Replace('\r', ' ')}{src}"

let private withLineReader f =
    let cache = System.Collections.Generic.Dictionary<string, string[]>()

    let getLines path =
        match cache.TryGetValue(path) with
        | true, l -> l
        | _ ->
            let l =
                try
                    System.IO.File.ReadAllLines(path)
                with _ ->
                    [||] in

            cache.[path] <- l
            l

    f getLines

let private relevant (diags: FSharpDiagnostic array) =
    diags
    |> Array.filter (fun d ->
        d.Severity = FSharpDiagnosticSeverity.Error
        || d.Severity = FSharpDiagnosticSeverity.Warning)

let formatFile (diags: FSharpDiagnostic array) =
    let diags = relevant diags

    if diags.Length = 0 then
        "OK"
    else
        withLineReader (fun getLines -> diags |> Array.map (formatOne getLines) |> String.concat "\n")

let formatProject (repoRoot: string) (diags: FSharpDiagnostic array) =
    let diags = relevant diags

    if diags.Length = 0 then
        "OK"
    else
        let root = repoRoot.TrimEnd('/') + "/"

        let rel (path: string) =
            if path.StartsWith(root) then
                path.Substring(root.Length)
            else
                path

        withLineReader (fun getLines ->
            diags
            |> Array.groupBy (fun d -> d.Range.FileName)
            |> Array.collect (fun (f, ds) -> Array.append [| $"--- {rel f}" |] (ds |> Array.map (formatOne getLines)))
            |> String.concat "\n")
