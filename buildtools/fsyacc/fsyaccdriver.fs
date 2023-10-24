module FsLexYacc.FsYacc.Driver

open System
open System.IO
open FSharp.Text.Lexing
open FsLexYacc.FsYacc
open FsLexYacc.FsYacc.AST
open Printf
open System.Collections.Generic

let has_extension (s:string) = 
    (s.Length >= 1 && s.[s.Length - 1] = '.') 
    || Path.HasExtension(s)

let chop_extension (s:string) =
    if not (has_extension s) then invalidArg "s" "the file name does not have an extension"
    Path.Combine (Path.GetDirectoryName s,Path.GetFileNameWithoutExtension(s)) 

let checkSuffix (x:string) (y:string) = x.EndsWith(y)

let readSpecFromFile fileName codePage =
  let stream,reader,lexbuf = UnicodeFileAsLexbuf(fileName, codePage)
  use stream = stream
  use reader = reader
  try
      let spec = Parser.spec Lexer.token lexbuf
      Ok spec
  with e ->
      (e, lexbuf.StartPos.Line, lexbuf.StartPos.Column)
      |> Result.Error

let printTokens filename codePage = 
    let stream,reader,lexbuf = UnicodeFileAsLexbuf(filename, codePage) 
    use stream = stream
    use reader = reader

    try 
      while true do 
        printf "tokenize - getting one token";
        let t = Lexer.token lexbuf in 
        (*F# printf "tokenize - got %s" (Parser.token_to_string t); F#*)
        if t = Parser.EOF then exit 0
    with e -> 
         eprintf "%s(%d,%d): error: %s" filename lexbuf.StartPos.Line lexbuf.StartPos.Column e.Message;
         exit 1

let logFileName (input: string, out: string option, log: bool) = 
    if log then Some (match out with Some x -> chop_extension x + ".fsyacc.output" | _ -> chop_extension input + ".fsyacc.output") 
    else None

let deriveOutputFileNames (filename, out: string option) =
  let output = match out with Some x -> x | _ -> chop_extension filename + (if checkSuffix filename ".mly" then ".ml" else ".fs") in
  let outputi = match out with Some x -> chop_extension x + (if checkSuffix x ".ml" then ".mli" else ".fsi") | _ -> chop_extension filename + (if checkSuffix filename ".mly" then ".mli" else ".fsi") in
  output, outputi

type Logger = 
    inherit IDisposable

    abstract member LogStream: (TextWriter -> 'a) -> 'a
    abstract member Log: TextWriterFormat<'a> -> 'a
    abstract member LogString: string -> unit

type FileLogger (outputFileLog) =
    let osl = File.CreateText outputFileLog :> TextWriter

    interface Logger with
        member x.LogStream f = f osl

        member x.Log format = fprintfn osl format
        
        member x.LogString msg =  fprintfn osl "%s" msg

    interface IDisposable with
        member _.Dispose() = 
            osl.Dispose()

type NullLogger () =
    interface Logger with
        member x.LogStream f = 
            f  TextWriter.Null
        member x.Log f = 
            fprintfn TextWriter.Null f
        member x.LogString _ = ()

    interface IDisposable with member _.Dispose () = ()

type Writer(outputFileName, outputFileInterface) = 
    let os = File.CreateText outputFileName :> TextWriter
    let mutable outputLineCount = 0
    let osi = File.CreateText outputFileInterface :> TextWriter
    let mutable interfaceLineCount = 0

    member x.Write format = 
        fprintf os format
    
    member x.WriteLine format =
        kfprintf (fun _ -> 
            outputLineCount <- outputLineCount + 1
            os.WriteLine ()
        ) os format

    member x.WriteUInt16 (i: int) = fprintf os "%dus;" i
    
    member x.WriteCode (code, pos) = 
        x.WriteLine "# %d \"%s\"" pos.pos_lnum pos.pos_fname
        x.WriteLine "%s" code
        let codeLines = code.Replace("\r","").Split([| '\n' |]).Length
        outputLineCount <- outputLineCount + codeLines
        x.WriteLine "# %d \"%s\"" outputLineCount outputFileName

    member x.OutputLineCount = outputLineCount

    member x.WriteInterface format = 
        fprintf osi format
    
    member x.WriteLineInterface format = 
        kfprintf (fun _ -> 
            interfaceLineCount <- interfaceLineCount + 1
            osi.WriteLine ()
        ) osi format

    member x.InterfaceLineCount = interfaceLineCount
    
    

    interface IDisposable with
        member x.Dispose () =
            os.Dispose()
            osi.Dispose()


// This is to avoid name conflicts against keywords.
let generic_nt_name nt = "'gentype_" + nt
let anyMarker = 0xffff

let actionCoding   =
  let shiftFlag = 0x0000
  let reduceFlag = 0x4000
  let errorFlag = 0x8000
  let acceptFlag = 0xc000
  function
  | Accept -> acceptFlag
  | Shift n -> shiftFlag ||| n
  | Reduce n -> reduceFlag ||| n
  | Error -> errorFlag 

type GeneratorState = 
 { input: string
   output: string option
   logger: Logger
   light: bool option
   modname: string option
   internal_module: bool
   opens: string list
   lexlib: string
   parslib: string
   compat: bool
   generate_nonterminal_name: Identifier -> string
   map_action_to_int: Action -> int
   anyMarker: int
   bufferTypeArgument: string }
   with 
   static member Default = 
    {  input = ""
       output = None
       logger = new NullLogger()
       light = None
       modname = None
       internal_module = false
       opens = []
       lexlib = ""
       parslib = ""
       compat = false
       generate_nonterminal_name = generic_nt_name
       map_action_to_int = actionCoding
       anyMarker = anyMarker
       bufferTypeArgument = "'cty" }

let writeSpecToFile (generatorState: GeneratorState) (spec: ParserSpec) (compiledSpec: CompiledSpec) = 
      let output, outputi = deriveOutputFileNames (generatorState.input, generatorState.output)
      generatorState.logger.Log "        Output file describing compiled parser placed in %s and %s" output outputi
      use writer = new Writer(output, outputi)
      writer.WriteLine          "// Implementation file for parser generated by fsyacc";
      writer.WriteLineInterface "// Signature file for parser generated by fsyacc";

      if (generatorState.light = Some(false)) || (generatorState.light = None && checkSuffix output ".ml") then
          writer.WriteLine "#light \"off\"";
          writer.WriteLineInterface "#light \"off\"";

      match generatorState.modname with 
      | None -> ()
      | Some s -> 
          match generatorState.internal_module with
          | true ->
              writer.WriteLine          "module internal %s" s;
              writer.WriteLineInterface "module internal %s" s;
          | false ->
              writer.WriteLine "module %s" s;
              writer.WriteLineInterface "module %s" s;
      
      writer.WriteLine "#nowarn \"64\";; // turn off warnings that type variables used in production annotations are instantiated to concrete type";

      for s in generatorState.opens do
          writer.WriteLine          "open %s" s;
          writer.WriteLineInterface "open %s" s;

      writer.WriteLine "open %s" generatorState.lexlib;
      writer.WriteLine "open %s.ParseHelpers" generatorState.parslib;
      if generatorState.compat then 
          writer.WriteLine "open Microsoft.FSharp.Compatibility.OCaml.Parsing";

      writer.WriteCode spec.Header
      
      // Print the datatype for the tokens
      writer.WriteLine "// This type is the type of tokens accepted by the parser";
          
          writer.WriteLine "type token = ";
          writer.WriteLineInterface "type token = ";
          for id,typ in spec.Tokens do 
              match typ with
              | None -> 
                writer.WriteLine "  | %s" id
                writer.WriteLineInterface "  | %s" id
              | Some ty -> 
                writer.WriteLine "  | %s of (%s)" id ty
                writer.WriteLineInterface "  | %s of (%s)" id ty

      // Print the datatype for the token names
      writer.WriteLine "// This type is used to give symbolic names to token indexes, useful for error messages";
      writer.WriteLine          "type tokenId = ";
      writer.WriteLineInterface "type tokenId = ";
          for id,_ in spec.Tokens do 
               writer.WriteLine          "    | TOKEN_%s" id;
               writer.WriteLineInterface "    | TOKEN_%s" id;
      writer.WriteLine          "    | TOKEN_end_of_input";
      writer.WriteLineInterface "    | TOKEN_end_of_input";
      writer.WriteLine          "    | TOKEN_error";
      writer.WriteLineInterface "    | TOKEN_error";

      writer.WriteLine "// This type is used to give symbolic names to token indexes, useful for error messages";
      writer.WriteLine          "type nonTerminalId = ";
      writer.WriteLineInterface "type nonTerminalId = ";
      for nt in compiledSpec.nonTerminals do 
          writer.WriteLine          "    | NONTERM_%s" nt;
          writer.WriteLineInterface "    | NONTERM_%s" nt;


      writer.WriteLine "";
      writer.WriteLine "// This function maps tokens to integer indexes";
      writer.WriteLine "let tagOfToken (t:token) = ";
      writer.WriteLine "  match t with";
      spec.Tokens |> List.iteri (fun i (id,typ) -> 
          writer.WriteLine "  | %s %s -> %d " id (match typ with Some _ -> "_" | None -> "") i);
      writer.WriteLineInterface "/// This function maps tokens to integer indexes";
      writer.WriteLineInterface "val tagOfToken: token -> int";

      writer.WriteLine "";
      writer.WriteLine "// This function maps integer indexes to symbolic token ids";
      writer.WriteLine "let tokenTagToTokenId (tokenIdx:int) = ";
      writer.WriteLine "  match tokenIdx with";
      spec.Tokens |> List.iteri (fun i (id,_) ->  writer.WriteLine "  | %d -> TOKEN_%s " i id)
      writer.WriteLine "  | %d -> TOKEN_end_of_input" compiledSpec.endOfInputTerminalIdx;
      writer.WriteLine "  | %d -> TOKEN_error" compiledSpec.errorTerminalIdx;
      writer.WriteLine "  | _ -> failwith \"tokenTagToTokenId: bad token\""

      writer.WriteLineInterface "";
      writer.WriteLineInterface "/// This function maps integer indexes to symbolic token ids";
      writer.WriteLineInterface "val tokenTagToTokenId: int -> tokenId";

      writer.WriteLine "";
      writer.WriteLine "/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production";
      writer.WriteLine "let prodIdxToNonTerminal (prodIdx:int) = ";
      writer.WriteLine "  match prodIdx with";
      compiledSpec.prods |> Array.iteri (fun i (nt,_,_,_) ->  writer.WriteLine "    | %d -> NONTERM_%s " i nt);
      writer.WriteLine "    | _ -> failwith \"prodIdxToNonTerminal: bad production index\""

      writer.WriteLineInterface "";
      writer.WriteLineInterface "/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production";
      writer.WriteLineInterface "val prodIdxToNonTerminal: int -> nonTerminalId";

      writer.WriteLine "";
      writer.WriteLine "let _fsyacc_endOfInputTag = %d " compiledSpec.endOfInputTerminalIdx;
      writer.WriteLine "let _fsyacc_tagOfErrorTerminal = %d" compiledSpec.errorTerminalIdx;
      writer.WriteLine "";
      writer.WriteLine "// This function gets the name of a token as a string";
      writer.WriteLine "let token_to_string (t:token) = ";
      writer.WriteLine "  match t with ";
      spec.Tokens |> List.iteri (fun _ (id,typ) ->  writer.WriteLine "  | %s %s -> \"%s\" " id (match typ with Some _ -> "_" | None -> "") id);

      writer.WriteLineInterface "";
      writer.WriteLineInterface "/// This function gets the name of a token as a string";
      writer.WriteLineInterface "val token_to_string: token -> string";

      writer.WriteLine "";
      writer.WriteLine "// This function gets the data carried by a token as an object";
      writer.WriteLine "let _fsyacc_dataOfToken (t:token) = ";
      writer.WriteLine "  match t with ";

      for id,typ in spec.Tokens do
          writer.WriteLine "  | %s %s -> %s " 
            id
            (match typ with Some _ -> "_fsyacc_x" | None -> "")
            (match typ with Some _ -> "Microsoft.FSharp.Core.Operators.box _fsyacc_x" | None -> "(null : System.Object)")

      for key,_ in spec.Types |> Seq.countBy fst |> Seq.filter (fun (_,n) -> n > 1)  do
            failwithf "%s is given multiple %%type declarations" key;
        
      for key,_ in spec.Tokens |> Seq.countBy fst |> Seq.filter (fun (_,n) -> n > 1)  do
            failwithf "%s is given %%token declarations" key
        
      let types = Map.ofList spec.Types 
      let tokens = Map.ofList spec.Tokens 
      
      let nStates = compiledSpec.states.Length 
      begin 
          writer.Write "let _fsyacc_gotos = [| " ;
          let numGotoNonTerminals = compiledSpec.gotoTable.[0].Length 
          let gotoIndexes = Array.create numGotoNonTerminals 0 
          let mutable gotoTableCurrIndex = 0 
          for j = 0 to numGotoNonTerminals-1 do  
              gotoIndexes.[j] <- gotoTableCurrIndex

              (* Count the number of entries in the association table. *)
              let mutable count = 0
              for i = 0 to nStates - 1 do 
                let goto = compiledSpec.gotoTable.[i].[j] 
                match goto with 
                | None -> ()
                | Some _ -> count <- count + 1
       
              (* Write the head of the table (i.e. the number of entries and the default value) *)
              gotoTableCurrIndex <- gotoTableCurrIndex + 1
              writer.WriteUInt16 count
              writer.WriteUInt16 generatorState.anyMarker
              
              (* Write the pairs of entries in incremental order by key *)
              (* This lets us implement the lookup by a binary chop. *)
              for i = 0 to nStates - 1 do 
                let goto = compiledSpec.gotoTable.[i].[j] 
                match goto with 
                | None -> ()
                | Some n -> 
                    gotoTableCurrIndex <- gotoTableCurrIndex + 1
                    writer.WriteUInt16 i
                    writer.WriteUInt16 n
          writer.WriteLine "|]" ;
          (* Output offsets into gotos table where the gotos for a particular nonterminal begin *)
          writer.Write "let _fsyacc_sparseGotoTableRowOffsets = [|" ;
          for j = 0 to numGotoNonTerminals-1 do  
              writer.WriteUInt16 gotoIndexes.[j]
          writer.WriteLine "|]"
      end;

      begin 
          writer.Write "let _fsyacc_stateToProdIdxsTableElements = [| " ;
          let indexes = Array.create compiledSpec.states.Length 0 
          let mutable currIndex = 0 
          for j = 0 to compiledSpec.states.Length - 1 do
              let state = compiledSpec.states.[j]
              indexes.[j] <- currIndex;

              (* Write the head of the table (i.e. the number of entries) *)
              writer.WriteUInt16 state.Length;
              currIndex <- currIndex + state.Length + 1
              
              (* Write the pairs of entries in incremental order by key *)
              (* This lets us implement the lookup by a binary chop. *)
              for prodIdx in state do
                    writer.WriteUInt16 prodIdx
          writer.WriteLine "|]" ;
          (* Output offsets into gotos table where the gotos for a particular nonterminal begin *)
          writer.Write "let _fsyacc_stateToProdIdxsTableRowOffsets = [|" ;
          for idx in indexes do 
              writer.WriteUInt16 idx;
          writer.WriteLine "|]" ;
      end;

      begin 
        let numActionRows = (Array.length compiledSpec.actionTable) 
        let _ = Array.length compiledSpec.actionTable.[0] 
        writer.WriteLine "let _fsyacc_action_rows = %d" numActionRows;
        writer.Write "let _fsyacc_actionTableElements = [|" ;
        let actionIndexes = Array.create numActionRows 0 
        
        let mutable actionTableCurrIndex = 0 
        for i = 0 to nStates-1 do 
            actionIndexes.[i] <- actionTableCurrIndex;
            let actions = compiledSpec.actionTable.[i] 
            let terminalsByAction = Dictionary<_,int list>(10) 
            let countPerAction = Dictionary<_,_>(10) 
            for terminal = 0 to actions.Length - 1 do  
                  let action = snd actions.[terminal] 
                  if terminalsByAction.ContainsKey action then 
                      terminalsByAction.[action] <- terminal :: terminalsByAction.[action] ;
                  else
                      terminalsByAction.[action] <- [terminal];
                  if countPerAction.ContainsKey action then 
                    countPerAction.[action] <- countPerAction.[action]+1
                  else 
                    countPerAction.[action] <- 1

            let mostCommonAction = 
                let mostCommon = ref Error 
                let max = ref 0 
                for KeyValue(x,y) in countPerAction do 
                    if y > !max then (mostCommon := x; max := y)
                !mostCommon 

            (* Count the number of entries in the association table. *)
            let count = ref 0 
            for KeyValue(action,terminals) in terminalsByAction do 
                for terminal in terminals do 
                   if action <> mostCommonAction then  
                       incr count;
            
            (* Write the head of the table (i.e. the number of entries and the default value) *)
            actionTableCurrIndex <- actionTableCurrIndex + 1;
            writer.WriteUInt16 !count;
            writer.WriteUInt16 (generatorState.map_action_to_int mostCommonAction);
            
            (* Write the pairs of entries in incremental order by key *)
            (* This lets us implement the lookup by a binary chop. *)
            for terminal = 0 to Array.length actions-1 do  
                let action = snd actions.[terminal] in 
                if action <> mostCommonAction then  (
                    actionTableCurrIndex <- actionTableCurrIndex + 1;
                    writer.WriteUInt16 terminal;
                    writer.WriteUInt16 (generatorState.map_action_to_int action);
                );
        writer.WriteLine "|]" ;
        (* Output offsets into actions table where the actions for a particular nonterminal begin *)
        writer.Write "let _fsyacc_actionTableRowOffsets = [|" ;
        for j = 0 to numActionRows-1 do  
            writer.WriteUInt16 actionIndexes.[j];
        writer.WriteLine "|]" ;

      end;
      begin 
          writer.Write "let _fsyacc_reductionSymbolCounts = [|" ;
          for nt,ntIdx,syms,code in compiledSpec.prods do 
              writer.WriteUInt16 (List.length syms);
          writer.WriteLine "|]" ;
      end;
      begin 
          writer.Write "let _fsyacc_productionToNonTerminalTable = [|" ;
          for nt,ntIdx,syms,code in compiledSpec.prods do 
              writer.WriteUInt16 ntIdx;
          writer.WriteLine "|]" ;
      end;
      begin 
          writer.Write "let _fsyacc_immediateActions = [|" ;
          for prodIdx in compiledSpec.immediateActionTable do 
              match prodIdx with
                | None     -> writer.WriteUInt16 generatorState.anyMarker (* NONE REP *)
                | Some act -> writer.WriteUInt16 (generatorState.map_action_to_int act)
          writer.WriteLine "|]" ;
      end;
      
      let getType nt = if types.ContainsKey nt then  types.[nt] else generatorState.generate_nonterminal_name nt
      begin 
          writer.WriteLine "let _fsyacc_reductions = lazy [|"
          for nt,ntIdx,syms,code in compiledSpec.prods do 
              writer.WriteLine "# %d \"%s\"" writer.OutputLineCount output;
              writer.WriteLine "        (fun (parseState : %s.IParseState) ->"  generatorState.parslib
              if generatorState.compat then 
                  writer.WriteLine "            Parsing.set_parse_state parseState;"
              syms |> List.iteri (fun i sym -> 
                  let tyopt = 
                      match sym with
                      | Terminal t -> 
                          if tokens.ContainsKey t then 
                            tokens.[t]
                          else None
                      | NonTerminal nt -> Some (getType nt) 
                  match tyopt with 
                  | Some ty -> writer.WriteLine "            let _%d = parseState.GetInput(%d) :?> %s in" (i+1) (i+1) ty
                  | None -> ())
              writer.WriteLine "            Microsoft.FSharp.Core.Operators.box" 
              writer.WriteLine "                (";
              writer.WriteLine "                   (";
              match code with 
              | Some (_,pos) -> writer.WriteLine "# %d \"%s\"" pos.pos_lnum pos.pos_fname
              | None -> ()
              match code with 
              | Some (code,_) -> 
                  let dollar = ref false in 
                  let c = code |> String.collect (fun c -> 
                      if not !dollar && c = '$' then (dollar := true; "")
                      elif !dollar && c >= '0' && c <= '9' then (dollar := false; "_"+String(c,1))
                      elif !dollar then (dollar := false; "$"+String(c,1))
                      else String(c,1))
                  let lines = c.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries);
                  for line in lines do 
                      writer.WriteLine "                     %s" line;
                  if !dollar then writer.Write "$"
              | None -> 
                  writer.WriteLine "                      raise (%s.Accept(Microsoft.FSharp.Core.Operators.box _1))" generatorState.parslib
              writer.WriteLine "                   )";
              // Place the line count back for the type constraint
              match code with 
              | Some (_,pos) -> writer.WriteLine "# %d \"%s\"" pos.pos_lnum pos.pos_fname
              | None -> ()
              writer.WriteLine "                 : %s));" (if types.ContainsKey nt then  types.[nt] else generatorState.generate_nonterminal_name nt);
          done;
          writer.WriteLine "|]" ;
      end;
      writer.WriteLine "# %d \"%s\"" writer.OutputLineCount output;
      writer.WriteLine "let tables : %s.Tables<_> = " generatorState.parslib
      writer.WriteLine "  { reductions = _fsyacc_reductions.Value;"
      writer.WriteLine "    endOfInputTag = _fsyacc_endOfInputTag;"
      writer.WriteLine "    tagOfToken = tagOfToken;"
      writer.WriteLine "    dataOfToken = _fsyacc_dataOfToken; "
      writer.WriteLine "    actionTableElements = _fsyacc_actionTableElements;"
      writer.WriteLine "    actionTableRowOffsets = _fsyacc_actionTableRowOffsets;"
      writer.WriteLine "    stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements;"
      writer.WriteLine "    stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets;"
      writer.WriteLine "    reductionSymbolCounts = _fsyacc_reductionSymbolCounts;"
      writer.WriteLine "    immediateActions = _fsyacc_immediateActions;"
      writer.WriteLine "    gotos = _fsyacc_gotos;"
      writer.WriteLine "    sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets;"
      writer.WriteLine "    tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal;"
      writer.WriteLine "    parseError = (fun (ctxt:%s.ParseErrorContext<_>) -> " generatorState.parslib
      writer.WriteLine "                              match parse_error_rich with "
      writer.WriteLine "                              | Some f -> f ctxt"
      writer.WriteLine "                              | None -> parse_error ctxt.Message);"
      
      writer.WriteLine "    numTerminals = %d;" (Array.length compiledSpec.actionTable.[0]);
      writer.WriteLine "    productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable  }"
      writer.WriteLine "let engine lexer lexbuf startState = tables.Interpret(lexer, lexbuf, startState)"                                                                                                         

      for id,startState in List.zip spec.StartSymbols compiledSpec.startStates do
            if not (types.ContainsKey id) then 
              failwith ("a %type declaration is required for start token "+id);
            let ty = types.[id] in 
            writer.WriteLine "let %s lexer lexbuf : %s =" id ty;
            writer.WriteLine "    engine lexer lexbuf %d :?> _" startState

      for id in spec.StartSymbols do
          if not (types.ContainsKey id) then 
            failwith ("a %type declaration is required for start token "+id);
          let ty = types.[id] in 
          writer.WriteLineInterface "val %s : (%s.LexBuffer<%s> -> token) -> %s.LexBuffer<%s> -> (%s) " id generatorState.lexlib generatorState.bufferTypeArgument generatorState.lexlib generatorState.bufferTypeArgument ty;


let compileSpec (spec: ParserSpec) (logger: Logger) = 
  let spec1 = ProcessParserSpecAst spec 
  CompilerLalrParserSpec logger.LogStream spec1 
