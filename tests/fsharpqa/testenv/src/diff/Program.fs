open System
open System.IO
    
[<EntryPoint>]
let main args =
    let path1, path2 =
        match args with
        | [| arg1; arg2 |] ->
            if File.Exists(arg1) && File.Exists(arg2) then arg1, arg2
            else printfn "Invalid paths"; exit 1
        | _ ->
            printfn "Usage:"
            printfn "  diff.exe <file1> <file2>"
            exit 1

    let lines1 = File.ReadAllLines(path1)
    let lines2 = File.ReadAllLines(path2)

    let minLines = min lines1.Length lines2.Length

    for i = 0 to (minLines - 1) do
        if lines1.[i] <> lines2.[i] then
            printfn "diff between [%s] and [%s]" path1 path2
            printfn "line %d" (i+1)
            printfn " - %s" lines1.[i]
            printfn " + %s" lines2.[i]
            exit 1

    if lines1.Length <> lines2.Length then
        printfn "diff between [%s] and [%s]" path1 path2
        printfn "diff at line %d" minLines
        lines1.[minLines .. (lines1.Length - 1)] |> Array.iter (printfn "- %s")
        lines2.[minLines .. (lines2.Length - 1)] |> Array.iter (printfn "+ %s")
        exit 1

    0       
