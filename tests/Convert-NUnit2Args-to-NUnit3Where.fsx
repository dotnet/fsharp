#r "System.Core.dll"

open System

type CmdArgs = { IncludeCategories: string option; ExcludeCategories: string option }

type Expr =
    | And of Expr list
    | Or of Expr list
    | Equal of Prop * string
    | NotEqual of Prop * string
and Prop =
    | Category
    | Property of string

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

    let il = includesList |> List.map (fun c -> Equal(Category, c)) |> Or

    let el = excludesList |> List.map (fun c -> NotEqual(Category, c)) |> And

    And([il; el])

let rec exprToString (w: Expr) =
    let addParens = sprintf "(%s)"
    let sanitize (s: string) = if s.Contains(" ") then sprintf "'%s'" s else s
    let propS p = match p with Category -> "cat" | Property name -> sanitize name

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
    | Equal (prop, v) -> Some (sprintf "%s == %s" (propS prop) (sanitize v))
    | NotEqual (prop, v) -> Some (sprintf "%s != %s" (propS prop) (sanitize v))

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
