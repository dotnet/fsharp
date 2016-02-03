module EnvLst

open System

open PlatformHelpers

type EnvLstFile = EnvLstLine list
and EnvLstLine =
    | Comment of string 
    | Data of EnvLstLineData
and EnvLstLineData = { 
    Tags: string list
    Vars: (string * string) list
    Comment: string option }

let private (|Comment|_|) (s: string) =
    match s with
    | s when s.StartsWith("#") -> Some (s.TrimStart([| '#' |]))
    | _ -> None

[<RequireQualifiedAccess>]
type private DataPart =
    | Var of string * string
    | Comment of string

let private parseDataParts (from: string) =
    let rec parseDataPartsHelper (v: string) xs =
        match (v.TrimStart()) with
        | s when s.Trim() = "" ->
            Choice1Of2 xs
        | Comment (comment) ->
            Choice1Of2 (DataPart.Comment comment :: xs)
        | s ->
            match s |> splitAtFirst ((=) '=') with
            | name, None ->
                Choice2Of2 (sprintf "Expected '=' after %s" name)
            | name, Some(v) ->
                match v.TrimStart() with
                | a when a.StartsWith("\"") -> //quoted (escape char \ ), like SOURCE="some value with spaces"
                    let rec innerQuote (alreadyParsed: string) (s: string) =
                        let current, rest = match s with "" -> "","" | x -> (x.Substring(0,1)), (x.Substring(1))
                        match alreadyParsed, current, rest with
                        | pre, "", _ ->
                            pre, ""
                        | pre, "\"", xs when pre.EndsWith("\\") -> //escaped "
                            innerQuote (pre + "\"") xs
                        | pre, "\"", xs -> //final "
                            pre, xs
                        | pre, x, xs ->
                            innerQuote (pre + x) xs
                    let value, rest = innerQuote "" (a.Substring(1))
                    parseDataPartsHelper rest (DataPart.Var(name, value) :: xs)
                | a ->  //unquoted, like SOURCE=avalue
                    let value, rest =
                        match a |> splitAtFirst Char.IsWhiteSpace with
                        | p0, None -> p0, ""
                        | p0, Some (rest) -> p0, rest
                    parseDataPartsHelper rest (DataPart.Var(name, value) :: xs)

    match parseDataPartsHelper from [] with
    | Choice1Of2 parts -> parts |> List.rev |> Choice1Of2
    | failure -> failure

let parseLine (line: string) =
    match line with
    | s when s.Trim() = "" -> Choice1Of2 None
    | Comment(comment) -> Comment (comment) |> Some |> Choice1Of2
    | s ->
        match s |> splitAtFirst ((=) '\t') with
        | s, None -> Choice2Of2 (sprintf "Expected '\\t' not found")
        | tagList, Some rest ->
            let tags = tagList.Split([| " " |], StringSplitOptions.RemoveEmptyEntries)
            match parseDataParts rest with
            | Choice1Of2 parts ->
                let vars = 
                    parts 
                    |> List.choose (function DataPart.Var (k,v) -> Some (k,v) | _ -> None)
                let comment = 
                    parts 
                    |> List.choose (function DataPart.Comment c -> Some c | _ -> None)
                    |> List.tryHead
                Data { Tags = tags |> List.ofArray; Vars = vars; Comment = comment }
                |> Some |> Choice1Of2
            | Choice2Of2 failure -> 
                Choice2Of2 failure
            

