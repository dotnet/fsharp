let digits = set { '0' .. '9' }

let rec intStringEndIndex i s =
    if i >= String.length s then
        String.length s - 1
    elif Set.contains s[i] digits then
        intStringEndIndex (i + 1) s
    else
        i - 1

__SOURCE_DIRECTORY__ + "/FSComp.txt"
|> System.IO.File.ReadAllLines
|> Seq.fold
    (fun counters line ->
        let line = line.TrimStart [| ' '; '#' |]
        let intStringEndIndex = intStringEndIndex 0 line

        if intStringEndIndex >= 0 then // Line starts with an integer (error code)
            let errorCode = line[..intStringEndIndex] |> int

            match counters with
            | [] -> [ errorCode ] // Initialise a counter
            | head :: tail when errorCode >= head -> errorCode :: tail // Increment current counter
            | _ -> errorCode :: counters // Start a new counter
        else
            counters)
    []
|> function
    | [] -> failwith "FSComp.txt contained no error codes but expected at least one"
    | [ _finalErrorCode ] -> () // Expected: One counter counted to the end
    | _finalErrorCode :: counters ->
        failwith $"Error codes not sorted in FSComp.txt, break(s) happened after {List.rev counters}"
