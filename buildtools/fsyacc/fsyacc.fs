(* (c) Microsoft Corporation 2005-2008.  *)

module FsLexYacc.FsYacc.Program

open Printf
open FSharp.Text
open FsLexYacc.FsYacc.AST
open FsLexYacc.FsYacc.Driver

//------------------------------------------------------------------
// This is the program proper

let mutable input = None
let mutable modname = None
let mutable internal_module = false
let mutable opens = []
let mutable out = None
let mutable tokenize = false
let mutable compat = false
let mutable log = false
let mutable light = None
let mutable inputCodePage = None
let mutable lexlib = "FSharp.Text.Lexing"
let mutable parslib = "FSharp.Text.Parsing"
let mutable bufferTypeArgument = "'cty"

let usage =
  [ ArgInfo("-o", ArgType.String (fun s -> out <- Some s), "Name the output file.");
    ArgInfo("-v", ArgType.Unit (fun () -> log <- true), "Produce a listing file."); 
    ArgInfo("--module", ArgType.String (fun s -> modname <- Some s), "Define the F# module name to host the generated parser."); 
    ArgInfo("--internal", ArgType.Unit (fun () -> internal_module <- true), "Generate an internal module");
    ArgInfo("--open", ArgType.String (fun s -> opens <- opens @ [s]), "Add the given module to the list of those to open in both the generated signature and implementation."); 
    ArgInfo("--light", ArgType.Unit (fun () ->  light <- Some true), "(ignored)");
    ArgInfo("--light-off", ArgType.Unit (fun () ->  light <- Some false), "Add #light \"off\" to the top of the generated file");
    ArgInfo("--ml-compatibility", ArgType.Unit (fun _ -> compat <- true), "Support the use of the global state from the 'Parsing' module in FSharp.PowerPack.dll."); 
    ArgInfo("--tokens", ArgType.Unit (fun _ -> tokenize <- true), "Simply tokenize the specification file itself."); 
    ArgInfo("--lexlib", ArgType.String (fun s ->  lexlib <- s), "Specify the namespace for the implementation of the lexer (default: FSharp.Text.Lexing)");
    ArgInfo("--parslib", ArgType.String (fun s ->  parslib <- s), "Specify the namespace for the implementation of the parser table interpreter (default: FSharp.Text.Parsing)");
    ArgInfo("--codepage", ArgType.Int (fun i -> inputCodePage <- Some i), "Assume input lexer specification file is encoded with the given codepage.")
    ArgInfo("--buffer-type-argument", ArgType.String (fun s -> bufferTypeArgument <- s), "Generic type argument of the LexBuffer type."); ]

let _ = ArgParser.Parse(usage,(fun x -> match input with Some _ -> failwith "more than one input given" | None -> input <- Some x),"fsyacc <filename>")

let main() = 
  let filename = (match input with Some x -> x | None -> failwith "no input given") in 
  if tokenize then printTokens filename inputCodePage
  
  let spec = 
    match readSpecFromFile filename inputCodePage with
    | Ok spec -> spec
    | Result.Error (e, line, col) -> 
        eprintf "%s(%d,%d): error: %s" filename line col e.Message
        exit 1
  
  use logger = match logFileName(filename, out, log) with
               | Some outputLogName -> new FileLogger(outputLogName) :> Logger
               | None -> new NullLogger() :> Logger
  let compiledSpec = compileSpec spec logger
  printfn "        building tables"
  printfn "        %d states" compiledSpec.states.Length; 
  printfn "        %d nonterminals" compiledSpec.gotoTable.[0].Length; 
  printfn "        %d terminals" compiledSpec.actionTable.[0].Length; 
  printfn "        %d productions" compiledSpec.prods.Length; 
  printfn "        #rows in action table: %d" compiledSpec.actionTable.Length; 
(*
  printfn "#unique rows in action table: %d" (List.length (Array.foldBack (fun row acc -> insert (Array.to_list row) acc) actionTable [])); 
  printfn "maximum #different actions per state: %d" (Array.foldBack (fun row acc ->max (List.length (List.foldBack insert (Array.to_list row) [])) acc) actionTable 0); 
  printfn "average #different actions per state: %d" ((Array.foldBack (fun row acc -> (List.length (List.foldBack insert (Array.to_list row) [])) + acc) actionTable 0) / (Array.length states)); 
*)
  
  let generatorState: GeneratorState =
    { GeneratorState.Default with
          input = filename
          output = out
          logger = logger
          light = light
          modname = modname
          internal_module = internal_module
          opens = opens
          lexlib = lexlib
          parslib = parslib
          compat = compat
          bufferTypeArgument = bufferTypeArgument }
  writeSpecToFile generatorState spec compiledSpec

let result = 
    try main()
    with e -> 
      eprintf "FSYACC: error FSY000: %s\n%s" (match e with Failure s -> s | e -> e.Message) e.StackTrace;
      exit 1

