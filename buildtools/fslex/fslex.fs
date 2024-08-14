// (c) Microsoft Corporation 2005-2009.

module FsLexYacc.FsLex.Program

open FsLexYacc.FsLex.AST
open FsLexYacc.FsLex.Driver
open Printf
open FSharp.Text
open System.IO

//------------------------------------------------------------------
// This is the program proper

let mutable input = None
let mutable out = None
let mutable inputCodePage = None
let mutable light = None
let mutable modname = None
let mutable internal_module = false
let mutable opens = []
let mutable lexlib = "FSharp.Text.Lexing"
let mutable unicode = false
let mutable caseInsensitive = false

let usage =
  [ ArgInfo ("-o", ArgType.String (fun s -> out <- Some s), "Name the output file.")
    ArgInfo ("--module", ArgType.String (fun s -> modname <- Some s), "Define the F# module name to host the generated parser.");
    ArgInfo ("--internal", ArgType.Unit (fun () -> internal_module <- true), "Generate an internal module");
    ArgInfo("--open", ArgType.String (fun s -> opens <- opens @ [s]), "Add the given module to the list of those to open in both the generated signature and implementation."); 
    ArgInfo ("--codepage", ArgType.Int (fun i -> inputCodePage <- Some i), "Assume input lexer specification file is encoded with the given codepage.")
    ArgInfo ("--light", ArgType.Unit (fun () ->  light <- Some true), "(ignored)")
    ArgInfo ("--light-off", ArgType.Unit (fun () ->  light <- Some false), "Add #light \"off\" to the top of the generated file")
    ArgInfo ("--lexlib", ArgType.String (fun s ->  lexlib <- s), "Specify the namespace for the implementation of the lexer table interpreter (default FSharp.Text.Lexing)")
    ArgInfo ("--unicode", ArgType.Unit (fun () -> unicode <- true), "Produce a lexer for use with 16-bit unicode characters.")
    ArgInfo ("-i", ArgType.Unit (fun () -> caseInsensitive <- true), "Produce a case-insensitive lexer.")
  ]

let _ = ArgParser.Parse(usage, (fun x -> match input with Some _ -> failwith "more than one input given" | None -> input <- Some x), "fslex <filename>")

let compileSpec (spec: Spec) (ctx: ParseContext) =
    let perRuleData, dfaNodes = Compile ctx spec 
    let dfaNodes = dfaNodes |> List.sortBy (fun n -> n.Id)
    perRuleData, dfaNodes

let main() =
  try
    let filename = (match input with Some x -> x | None -> failwith "no input given")
    let parseContext = 
      { unicode = unicode 
        caseInsensitive = caseInsensitive }
    let spec =
        match readSpecFromFile filename inputCodePage with
        | Ok spec -> spec
        | Error (e, line, column) ->
            eprintf "%s(%d,%d): error: %s" filename line column
              (match e with
               | Failure s -> s
               | _ -> e.Message)
            exit 1

    printfn "compiling to dfas (can take a while...)"
    let perRuleData, dfaNodes = compileSpec spec parseContext
    printfn "%d states" dfaNodes.Length

    printfn "writing output"

    let output =
        match out with
        | Some x -> x
        | _ -> Path.ChangeExtension(filename, ".fs")

    let state : GeneratorState =
        { inputFileName = filename
          outputFileName = output
          inputCodePage = inputCodePage |> Option.map System.Text.Encoding.GetEncoding |> Option.defaultValue System.Text.Encoding.UTF8
          generatedModuleName = modname
          disableLightMode = light
          generateInternalModule = internal_module
          opens = opens
          lexerLibraryName = lexlib
          domain = if unicode then Unicode else ASCII }
    writeSpecToFile state spec perRuleData dfaNodes

  with e ->
    eprintf "FSLEX: error FSL000: %s" (match e with Failure s -> s | e -> e.ToString())
    exit 1


let result = main()
