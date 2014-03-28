// Parsing: support fsyacc-generated parsers

[<CompilerMessage("This module is for ML compatibility. Consider using the Microsoft.FSharp.Text.Parsing namespace directly. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Parsing
open Microsoft.FSharp.Text.Lexing
open Microsoft.FSharp.Text.Parsing
open Microsoft.FSharp.Compatibility.OCaml

let err _  = failwith "You must generate your parser using the '--ml-compatibility' option or call 'Parsing.set_parse_state parseState' in each action before using functions from the Parsing module.  This is because the module uses global state which must be set up for use in each parsing action. Review the notes in the 'Microsoft.FSharp.Compatibility.OCaml.Parsing' module if you are using parsers on multiple threads."
let dummyProvider = 
    { new IParseState with 
        member x.InputRange(i) = err();
        member p.InputStartPosition(n) = err();
        member p.InputEndPosition(n) = err();
        member x.ResultRange = err();
        member x.GetInput(i) = err();
        member x.ParserLocalStore = err();
        member x.RaiseError() = err()  
      }

let mutable parse_information = dummyProvider
let set_parse_state (x:IParseState) = parse_information <- x

let enforce_nonnull_pos p = 
  match (box p) with 
  | null -> Position.Empty
  | _ -> p

let symbol_start_pos ()   = parse_information.ResultRange   |> fst |> enforce_nonnull_pos
let symbol_end_pos ()     = parse_information.ResultRange   |> snd |> enforce_nonnull_pos
let rhs_start_pos (n:int) = parse_information.InputRange(n) |> fst |> enforce_nonnull_pos
let rhs_end_pos (n:int)   = parse_information.InputRange(n) |> snd |> enforce_nonnull_pos

exception Parse_error  = RecoverableParseError
let parse_error s = parse_information.RaiseError()(failwith s : unit)

let symbol_start () = (symbol_start_pos()).pos_cnum
let symbol_end () = (symbol_end_pos()).pos_cnum
let rhs_start n = (rhs_start_pos n).pos_cnum
let rhs_end n = (rhs_end_pos n).pos_cnum

