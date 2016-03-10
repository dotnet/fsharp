#r "System.Core.dll"

open System

type CmdArgs = { IncludeCategories: string option; ExcludeCategories: string option }

type Expr =
    | And of Expr list
    | Or of Expr list
    | CatEqual of string
    | CatNotEqual of string

let toWhereExpr (cmdArgs: CmdArgs) =
    
    let split (line: string) = 
        line.Split([| "," |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> s.Trim())
        |> List.ofArray

    let includesList = 
        cmdArgs.IncludeCategories
        |> Option.map split
        |> function None -> [] | Some l -> l
    let excludesList = 
        cmdArgs.ExcludeCategories
        |> Option.map split
        |> function None -> [] | Some l -> l

    let il = includesList |> List.map (fun c -> CatEqual(c)) |> Or

    let el = excludesList |> List.map (fun c -> CatNotEqual(c)) |> And

    And([il; el])

let rec exprToString (w: Expr) =
    let addParens = sprintf "(%s)"
    let sanitize (s: string) = if s.Contains(" ") then sprintf "'%s'" s else s

    match w with
    | And [] -> None
    | Or [] -> None
    | And l ->
        match l |> List.map exprToString |> List.choose id with
        | [] -> None
        | [x] -> Some x
        | xs -> Some (xs |> List.map addParens |> String.concat " and ") 
    | Or l ->
        match l |> List.map exprToString |> List.choose id with
        | [] -> None
        | [x] -> Some x
        | xs -> Some (xs |> List.map addParens |> String.concat " or ")
    | CatEqual v -> Some (sprintf "cat==%s" (sanitize v))
    | CatNotEqual v -> Some (sprintf "cat != %s" (sanitize v))

let parseCmdArgs (args: string list) =
    match args with
    | [a; b] -> { IncludeCategories = Some a; ExcludeCategories = Some b }
    | xs -> failwithf "Invalid arguments %A" xs
    
let main args =
    args
    |> parseCmdArgs 
    |> toWhereExpr 
    |> exprToString
    |> function None -> "" | Some s -> s
    |> printfn "%s"

let rec getScriptArgs l = 
    match l with
    | [] -> []
    | "--" :: rest -> rest
    | _ :: tail -> getScriptArgs tail

try
    Environment.GetCommandLineArgs()
    |> List.ofArray
    |> getScriptArgs
    |> main
    exit 0
with e ->
    printfn "%s" e.Message
    exit 1
