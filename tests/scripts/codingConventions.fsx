// Print some stats about some very very basic code formatting conventions

open System.IO

let lines = 
    [| for dir in [ "src/fsharp"; "src/fsharp/symbols"; "src/fsharp/service"; "src/absil" ]do
          for file in Directory.EnumerateFiles(__SOURCE_DIRECTORY__ + "/../../" + dir,"*.fs") do
        // TcGlobals.fs gets an exception
            let lines = File.ReadAllLines file
            for (line, lineText) in Array.indexed lines do 

                // We hardwire some exceptions
                if not (Path.GetFileName(file) = "service.fs") &&  // churning
                   not (lineText.Contains("SuppressMessage")) && // old fxcop annotation
                   not (Path.GetFileName(file) = "TcGlobals.fs") &&
                   not (Path.GetFileName(file) = "tast.fs" && line > 2100 && line < 2400) then
                
                    yield file, (line+1, lineText) |]


printfn "------ LINE LENGTH ANALYSIS ----------"
let totalLines = lines.Length
let buckets = lines |> Array.groupBy (fun (_file, (_line, lineText)) -> lineText.Length / 10) |> Array.sortByDescending (fun (key, vs) -> key)

for (key, sz) in buckets do
    printfn "bucket %d-%d - %%%2.1f" (key*10) (key*10+9) (double sz.Length / double totalLines * 100.0)

printfn "top bucket: "

for (file, (line, text)) in snd buckets.[0] do   
   printfn "%s %d %s..." file line text.[0..50]

let numLong = lines |> Array.filter (fun (_, (line, lineText)) -> lineText.Length > 120) |> Array.length
let numHuge = lines |> Array.filter (fun (_, (line, lineText)) -> lineText.Length > 160) |> Array.length
let numHumungous = lines |> Array.filter (fun (_, (line, lineText)) -> lineText.Length > 200) |> Array.length

printfn "%d long lines = %2.2f%%" numLong (double numLong / double totalLines)
printfn "%d huge lines = %2.2f%%" numHuge (double numHuge / double totalLines)
printfn "%d humungous lines = %2.2f%%" numHumungous (double numHumungous / double totalLines)

printfn "------ SPACE AFTER COMMA ANALYSIS ----------"

let commas =
    lines
    |> Array.groupBy fst 
    |> Array.map (fun (file, lines) -> 
        file,
        lines 
        |> Array.sumBy (fun (_,(_,line)) ->
              line |> Seq.pairwise |> Seq.filter (fun (c1, c2) -> c1 = ',' && c2 <> ' ') |> Seq.length)) 
    |> Array.sortByDescending snd

printfn "Top files that have commas without spaces: %A" (Array.truncate 10 commas)


