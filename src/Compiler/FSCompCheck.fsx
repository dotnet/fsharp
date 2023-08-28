#r "nuget: FSharpPlus, 1.4.1"
let inline (|Matches|_|) format = FSharpPlus.Parsing.trySscanf format

__SOURCE_DIRECTORY__ + "/FSComp.txt"
|> System.IO.File.ReadAllLines
|> Seq.fold
    (fun counters line ->
        match line with
        | Matches "%d,%s" (errorCode, _)
        | Matches "#%d %s" (errorCode, _)
        | Matches "#%d,%s" (errorCode, _) ->
            match counters with
            | [] -> [ errorCode ] // Initialise a counter
            | head :: tail when errorCode >= head -> errorCode :: tail // Increment current counter
            | _ -> errorCode :: counters // Start a new counter
        | _ -> counters)
    []
|> function
    | [] -> failwith "FSComp.txt contained no error codes but expected at least one"
    | [ _finalErrorCode ] -> () // Expected: One counter counted to the end
    | _finalErrorCode :: counters ->
        failwith $"Error codes not sorted in FSComp.txt, breaks happened after {List.rev counters}"
