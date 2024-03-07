open System
open System.IO

// this script is useful for tests using a .bsl file (baseline) containing expected compiler output
// which is matched against .vserr or .err file aside once the test has run
// the script replaces all the .bsl/.bslpp with either .err or .vserr

let diff path1 path2 =
    let result = System.Text.StringBuilder()
    let append (s: string) = result.AppendLine s |> ignore

    if not <| File.Exists(path1) then failwithf "Invalid path %s" path1
    if not <| File.Exists(path2) then failwithf "Invalid path %s" path2

    let lines1 = File.ReadAllLines(path1)
    let lines2 = File.ReadAllLines(path2)

    let minLines = min lines1.Length lines2.Length

    for i = 0 to (minLines - 1) do

        let normalizePath line = line
        let line1 = normalizePath lines1.[i]
        let line2 = normalizePath lines2.[i]

        if not (line1.Contains "// MVID") && 
           not (line1.Contains "// Image base:") && 
           not (line1.Contains ".line") && 
           not (line1.Contains " .ver ") && 
           not (line1.Contains "// Offset:") then 
          if line1 <> line2 then
            append <| sprintf "diff between [%s] and [%s]" path1 path2
            append <| sprintf "line %d" (i+1)
            append <| sprintf " - %s" line1
            append <| sprintf " + %s" line2

    if lines1.Length <> lines2.Length then
        append <| sprintf "diff between [%s] and [%s]" path1 path2
        append <| sprintf "diff at line %d" minLines
        lines1.[minLines .. (lines1.Length - 1)] |> Array.iter (append << sprintf "- %s")
        lines2.[minLines .. (lines2.Length - 1)] |> Array.iter (append << sprintf "+ %s")

    result.ToString()

let fsdiff actualFile expectedFile = 
    let errorText = System.IO.File.ReadAllText actualFile

    let result = diff expectedFile actualFile
    if result <> "" then
        printfn "%s" result

    result

let directories =
    [
      "fsharp/typecheck/sigs"
      "fsharp/typecheck/overloads"
      "fsharpqa/Source"
    ]
    |> List.map (fun d -> Path.Combine(__SOURCE_DIRECTORY__, ".." , d) |> DirectoryInfo)

let extensionPatterns = ["*.bsl"; "*.vsbsl"; "*.il.bsl"]
for d in directories do
    for p in extensionPatterns do 
        for bslFile in d.GetFiles (p, SearchOption.AllDirectories) do
         let baseLineFileName = bslFile.FullName
         if not (baseLineFileName.EndsWith("bslpp")) then
            let errFileName = 
                if baseLineFileName.EndsWith "vsbsl" then  Path.ChangeExtension(baseLineFileName, "vserr")
                elif baseLineFileName.EndsWith "il.bsl" then  baseLineFileName.Replace("il.bsl", "il")
                else Path.ChangeExtension(baseLineFileName, "err")
            let baseLineFile = FileInfo(baseLineFileName)
            let errFile = FileInfo(errFileName)
            //let baseLineFilePreProcess = FileInfo(Path.ChangeExtension(errFile.FullName, "bslpp"))

            if baseLineFile.Exists && errFile.Exists then

                //printfn "consider %s and %s" baseLineFile.FullName errFileName
                if fsdiff errFileName baseLineFileName <> "" then

                    printfn "%s not matching, replacing with %s" baseLineFileName errFileName
                    if not (fsi.CommandLineArgs |> Array.contains "-n") then 
                        errFile.CopyTo(baseLineFileName, true) |> ignore
