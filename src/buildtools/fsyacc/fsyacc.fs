(* (c) Microsoft Corporation 2005-2008.  *)

module internal FsLexYacc.FsYacc.Driver 

open System.IO 
open System.Collections.Generic
open Printf
open Internal.Utilities
open Internal.Utilities.Text.Lexing

open FsLexYacc.FsYacc
open FsLexYacc.FsYacc.AST

//------------------------------------------------------------------
// This code is duplicated from Microsoft.FSharp.Compiler.UnicodeLexing

type Lexbuf =  LexBuffer<char>

/// Standard utility to create a Unicode LexBuffer
///
/// One small annoyance is that LexBuffers and not IDisposable. This means 
/// we can't just return the LexBuffer object, since the file it wraps wouldn't
/// get closed when we're finished with the LexBuffer. Hence we return the stream,
/// the reader and the LexBuffer. The caller should dispose the first two when done.
let UnicodeFileAsLexbuf (filename,codePage : int option) : FileStream * StreamReader * Lexbuf =
    // Use the .NET functionality to auto-detect the unicode encoding
    // It also uses Lexing.from_text_reader to present the bytes read to the lexer in UTF8 decoded form
    let stream  = new FileStream(filename,FileMode.Open,FileAccess.Read,FileShare.Read) 
    let reader = 
        match codePage with 
        | None -> new  StreamReader(stream,true)
        | Some n -> new  StreamReader(stream,System.Text.Encoding.GetEncoding(n)) 
    let lexbuf = LexBuffer<char>.FromFunction(reader.Read) 
    lexbuf.EndPos <- Position.FirstLine(filename);
    stream, reader, lexbuf

//------------------------------------------------------------------
// This is the program proper

let input = ref None
let modname= ref None
let internal_module = ref false
let opens= ref []
let out = ref None
let tokenize = ref false
let compat = ref false
let log = ref false
let light = ref None
let inputCodePage = ref None
let mutable lexlib = "FSharp.Text.Lexing"
let mutable parslib = "FSharp.Text.Parsing"

let usage =
  [ ArgInfo("-o", ArgType.String (fun s -> out := Some s), "Name the output file.");
    ArgInfo("-v", ArgType.Unit (fun () -> log := true), "Produce a listing file."); 
    ArgInfo("--module", ArgType.String (fun s -> modname := Some s), "Define the F# module name to host the generated parser."); 
    ArgInfo("--internal", ArgType.Unit (fun () -> internal_module := true), "Generate an internal module");
    ArgInfo("--open", ArgType.String (fun s -> opens := !opens @ [s]), "Add the given module to the list of those to open in both the generated signature and implementation."); 
    ArgInfo("--light", ArgType.Unit (fun () ->  light := Some true), "(ignored)");
    ArgInfo("--light-off", ArgType.Unit (fun () ->  light := Some false), "Add #light \"off\" to the top of the generated file");
    ArgInfo("--ml-compatibility", ArgType.Set compat, "Support the use of the global state from the 'Parsing' module in FSharp.PowerPack.dll."); 
    ArgInfo("--tokens", ArgType.Set tokenize, "Simply tokenize the specification file itself."); 
    ArgInfo("--lexlib", ArgType.String (fun s ->  lexlib <- s), "Specify the namespace for the implementation of the lexer (default: FSharp.Text.Lexing)");
    ArgInfo("--parslib", ArgType.String (fun s ->  parslib <- s), "Specify the namespace for the implementation of the parser table interpreter (default: FSharp.Text.Parsing)");
    ArgInfo("--codepage", ArgType.Int (fun i -> inputCodePage := Some i), "Assume input lexer specification file is encoded with the given codepage.");  ]

let _ = ArgParser.Parse(usage,(fun x -> match !input with Some _ -> failwith "more than one input given" | None -> input := Some x),"fsyacc <filename>")

let output_int (os: #TextWriter) (n:int) = os.Write(string n)

let outputCodedUInt16 (os: #TextWriter)  (n:int) = 
  os.Write n;
  os.Write "us; ";

let shiftFlag = 0x0000
let reduceFlag = 0x4000
let errorFlag = 0x8000
let acceptFlag = 0xc000
let actionMask = 0xc000

let anyMarker = 0xffff

let actionCoding action  =
  match action with 
  | Accept -> acceptFlag
  | Shift n -> shiftFlag ||| n
  | Reduce n -> reduceFlag ||| n
  | Error -> errorFlag 

let main() = 
  let filename = (match !input with Some x -> x | None -> failwith "no input given") in 
  let spec = 
      let stream,reader,lexbuf = UnicodeFileAsLexbuf(filename, !inputCodePage) 
      use stream = stream
      use reader = reader

      try 
        if !tokenize then begin 
          while true do 
            printf "tokenize - getting one token";
            let t = Lexer.token lexbuf in 
            (*F# printf "tokenize - got %s" (Parser.token_to_string t); F#*)
            if t = Parser.EOF then exit 0;
          done;
        end;
    
        Parser.spec Lexer.token lexbuf 
      with e -> 
         eprintf "%s(%d,%d): error: %s" filename lexbuf.StartPos.Line lexbuf.StartPos.Column e.Message;
         exit 1  in

  let has_extension (s:string) = 
    (s.Length >= 1 && s.[s.Length - 1] = '.') 
    || Path.HasExtension(s)

  let chop_extension (s:string) =
    if not (has_extension s) then invalidArg "s" "the file name does not have an extension"
    Path.Combine (Path.GetDirectoryName s,Path.GetFileNameWithoutExtension(s)) 
  
  let checkSuffix (x:string) (y:string) = x.EndsWith(y)

  let output = match !out with Some x -> x | _ -> chop_extension filename + (if checkSuffix filename ".mly" then ".ml" else ".fs") in
  let outputi = match !out with Some x -> chop_extension x + (if checkSuffix x ".ml" then ".mli" else ".fsi") | _ -> chop_extension filename + (if checkSuffix filename ".mly" then ".mli" else ".fsi") in
  let outputo = 
      if !log then Some (match !out with Some x -> chop_extension x + ".fsyacc.output" | _ -> chop_extension filename + ".fsyacc.output") 
      else None 

  use os = (File.CreateText output :> TextWriter)
  use osi = (File.CreateText outputi :> TextWriter)

  let lineCountOutput = ref 0
  let lineCountSignature = ref 0
  let cos = (os,lineCountOutput)
  let cosi = (osi,lineCountSignature)
  let cprintf (os:TextWriter,lineCount) fmt = Printf.fprintf os fmt
  let cprintfn (os:TextWriter,lineCount) fmt = Printf.kfprintf (fun () -> incr lineCount; os.WriteLine()) os fmt

  let logf = 
      match outputo with 
      | None -> (fun f -> ())
      | Some filename -> 
          let oso = (File.CreateText filename :> TextWriter) 
          (fun f -> f oso) 

  logf (fun oso -> fprintfn oso "        Output file describing compiled parser placed in %s and %s" output outputi);

  printfn "        building tables"; 
  let spec1 = ProcessParserSpecAst spec 
  let (prods,states, startStates,actionTable,immediateActionTable,gotoTable,endOfInputTerminalIdx,errorTerminalIdx,nonTerminals) = 
      CompilerLalrParserSpec logf spec1 

  let (code,pos) = spec.Header 
  printfn "        %d states" states.Length; 
  printfn "        %d nonterminals" gotoTable.[0].Length; 
  printfn "        %d terminals" actionTable.[0].Length; 
  printfn "        %d productions" prods.Length; 
  printfn "        #rows in action table: %d" actionTable.Length; 
(*
  printfn "#unique rows in action table: %d" (List.length (Array.foldBack (fun row acc -> insert (Array.to_list row) acc) actionTable [])); 
  printfn "maximum #different actions per state: %d" (Array.foldBack (fun row acc ->max (List.length (List.foldBack insert (Array.to_list row) [])) acc) actionTable 0); 
  printfn "average #different actions per state: %d" ((Array.foldBack (fun row acc -> (List.length (List.foldBack insert (Array.to_list row) [])) + acc) actionTable 0) / (Array.length states)); 
*)

  cprintfn cos "// Implementation file for parser generated by fsyacc";
  cprintfn cosi "// Signature file for parser generated by fsyacc";

  if (!light = Some(false)) || (!light = None && checkSuffix output ".ml") then
      cprintfn cos "#light \"off\"";
      cprintfn cosi "#light \"off\"";

  match !modname with 
  | None -> ()
  | Some s -> 
      match !internal_module with
      | true ->
          cprintfn cos "module internal %s" s;
          cprintfn cosi "module internal %s" s;
      | false ->
          cprintfn cos "module %s" s;
          cprintfn cosi "module %s" s;
  
  cprintfn cos "#nowarn \"64\";; // turn off warnings that type variables used in production annotations are instantiated to concrete type";

  for s in !opens do
      cprintfn cos "open %s" s;
      cprintfn cosi "open %s" s;

  cprintfn cos "open %s" lexlib;
  cprintfn cos "open %s.ParseHelpers" parslib;
  if !compat then 
      cprintfn cos "open Microsoft.FSharp.Compatibility.OCaml.Parsing";

  cprintfn cos "# %d \"%s\"" pos.pos_lnum pos.pos_fname;
  cprintfn cos "%s" code;
  lineCountOutput := !lineCountOutput + code.Replace("\r","").Split([| '\n' |]).Length;

  cprintfn cos "# %d \"%s\"" !lineCountOutput output;
  // Print the datatype for the tokens
  cprintfn cos "// This type is the type of tokens accepted by the parser";
  for out in [cos;cosi] do
      cprintfn out "type token = ";
      for id,typ in spec.Tokens do 
          match typ with
          | None -> cprintfn out "  | %s" id
          | Some ty -> cprintfn out "  | %s of (%s)" id ty; 

  // Print the datatype for the token names
  cprintfn cos "// This type is used to give symbolic names to token indexes, useful for error messages";
  for out in [cos;cosi] do
      cprintfn out "type tokenId = ";
      for id,typ in spec.Tokens do 
          cprintfn out "    | TOKEN_%s" id;
      cprintfn out "    | TOKEN_end_of_input";
      cprintfn out "    | TOKEN_error";

  cprintfn cos "// This type is used to give symbolic names to token indexes, useful for error messages";
  for out in [cos;cosi] do
      cprintfn out "type nonTerminalId = ";
      for nt in nonTerminals do 
          cprintfn out "    | NONTERM_%s" nt;

  cprintfn cos "";
  cprintfn cos "// This function maps tokens to integer indexes";
  cprintfn cos "let tagOfToken (t:token) = ";
  cprintfn cos "  match t with";
  spec.Tokens |> List.iteri (fun i (id,typ) -> 
      cprintfn cos "  | %s %s -> %d " id (match typ with Some _ -> "_" | None -> "") i);
  cprintfn cosi "/// This function maps tokens to integer indexes";
  cprintfn cosi "val tagOfToken: token -> int";

  cprintfn cos "";
  cprintfn cos "// This function maps integer indexes to symbolic token ids";
  cprintfn cos "let tokenTagToTokenId (tokenIdx:int) = ";
  cprintfn cos "  match tokenIdx with";
  spec.Tokens |> List.iteri (fun i (id,typ) -> 
      cprintfn cos "  | %d -> TOKEN_%s " i id)
  cprintfn cos "  | %d -> TOKEN_end_of_input" endOfInputTerminalIdx;
  cprintfn cos "  | %d -> TOKEN_error" errorTerminalIdx;
  cprintfn cos "  | _ -> failwith \"tokenTagToTokenId: bad token\""

  cprintfn cosi "";
  cprintfn cosi "/// This function maps integer indexes to symbolic token ids";
  cprintfn cosi "val tokenTagToTokenId: int -> tokenId";

  cprintfn cos "";
  cprintfn cos "/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production";
  cprintfn cos "let prodIdxToNonTerminal (prodIdx:int) = ";
  cprintfn cos "  match prodIdx with";
  prods |> Array.iteri (fun i (nt,ntIdx,syms,code) -> 
      cprintfn cos "    | %d -> NONTERM_%s " i nt);
  cprintfn cos "    | _ -> failwith \"prodIdxToNonTerminal: bad production index\""

  cprintfn cosi "";
  cprintfn cosi "/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production";
  cprintfn cosi "val prodIdxToNonTerminal: int -> nonTerminalId";

  cprintfn cos "";
  cprintfn cos "let _fsyacc_endOfInputTag = %d " endOfInputTerminalIdx;
  cprintfn cos "let _fsyacc_tagOfErrorTerminal = %d" errorTerminalIdx;
  cprintfn cos "";
  cprintfn cos "// This function gets the name of a token as a string";
  cprintfn cos "let token_to_string (t:token) = ";
  cprintfn cos "  match t with ";
  spec.Tokens |> List.iteri (fun i (id,typ) -> 
      cprintfn cos "  | %s %s -> \"%s\" " id (match typ with Some _ -> "_" | None -> "") id);

  cprintfn cosi "";
  cprintfn cosi "/// This function gets the name of a token as a string";
  cprintfn cosi "val token_to_string: token -> string";

  cprintfn cos "";
  cprintfn cos "// This function gets the data carried by a token as an object";
  cprintfn cos "let _fsyacc_dataOfToken (t:token) = ";
  cprintfn cos "  match t with ";

  for (id,typ) in spec.Tokens do
      cprintfn cos "  | %s %s -> %s " 
        id
        (match typ with Some _ -> "_fsyacc_x" | None -> "")
        (match typ with Some _ -> "Microsoft.FSharp.Core.Operators.box _fsyacc_x" | None -> "(null : System.Object)")

  let tychar = "'cty" 

  for (key,_) in spec.Types |> Seq.countBy fst |> Seq.filter (fun (_,n) -> n > 1)  do
        failwithf "%s is given multiple %%type declarations" key;
    
  for (key,_) in spec.Tokens |> Seq.countBy fst |> Seq.filter (fun (_,n) -> n > 1)  do
        failwithf "%s is given %%token declarations" key
    
  let types = Map.ofList spec.Types 
  let tokens = Map.ofList spec.Tokens 
  
  let nStates = states.Length 
  begin 
      cprintf cos "let _fsyacc_gotos = [| " ;
      let numGotoNonTerminals = gotoTable.[0].Length 
      let gotoIndexes = Array.create numGotoNonTerminals 0 
      let gotoTableCurrIndex = ref 0 in 
      for j = 0 to numGotoNonTerminals-1 do  
          gotoIndexes.[j] <- !gotoTableCurrIndex;

          (* Count the number of entries in the association table. *)
          let count = ref 0 in 
          for i = 0 to nStates - 1 do 
            let goto = gotoTable.[i].[j] 
            match goto with 
            | None -> ()
            | Some _ -> incr count
   
          (* Write the head of the table (i.e. the number of entries and the default value) *)
          gotoTableCurrIndex := !gotoTableCurrIndex + 1;
          outputCodedUInt16 os !count;
          outputCodedUInt16 os anyMarker;
          
          (* Write the pairs of entries in incremental order by key *)
          (* This lets us implement the lookup by a binary chop. *)
          for i = 0 to nStates - 1 do 
            let goto = gotoTable.[i].[j] 
            match goto with 
            | None -> ()
            | Some n -> 
                gotoTableCurrIndex := !gotoTableCurrIndex + 1;
                outputCodedUInt16 os i;
                outputCodedUInt16 os n;
      cprintfn cos "|]" ;
      (* Output offsets into gotos table where the gotos for a particular nonterminal begin *)
      cprintf cos "let _fsyacc_sparseGotoTableRowOffsets = [|" ;
      for j = 0 to numGotoNonTerminals-1 do  
          outputCodedUInt16 os gotoIndexes.[j];
      cprintfn cos "|]" ;
  end;

  begin 
      cprintf cos "let _fsyacc_stateToProdIdxsTableElements = [| " ;
      let indexes = Array.create states.Length 0 
      let currIndex = ref 0 
      for j = 0 to states.Length - 1 do
          let state = states.[j]
          indexes.[j] <- !currIndex;

          (* Write the head of the table (i.e. the number of entries) *)
          outputCodedUInt16 os state.Length;
          currIndex := !currIndex + state.Length + 1;
          
          (* Write the pairs of entries in incremental order by key *)
          (* This lets us implement the lookup by a binary chop. *)
          for prodIdx in state do
                outputCodedUInt16 os prodIdx;
      cprintfn cos "|]" ;
      (* Output offsets into gotos table where the gotos for a particular nonterminal begin *)
      cprintf cos "let _fsyacc_stateToProdIdxsTableRowOffsets = [|" ;
      for idx in indexes do 
          outputCodedUInt16 os idx;
      cprintfn cos "|]" ;
  end;

  begin 
    let numActionRows = (Array.length actionTable) 
    let maxActionColumns = Array.length actionTable.[0] 
    cprintfn cos "let _fsyacc_action_rows = %d" numActionRows;
    cprintf cos "let _fsyacc_actionTableElements = [|" ;
    let actionIndexes = Array.create numActionRows 0 
    
    let actionTableCurrIndex = ref 0 
    for i = 0 to nStates-1 do 
        actionIndexes.[i] <- !actionTableCurrIndex;
        let actions = actionTable.[i] 
        let terminalsByAction = new Dictionary<_,int list>(10) 
        let countPerAction = new Dictionary<_,_>(10) 
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
            for (KeyValue(x,y)) in countPerAction do 
                if y > !max then (mostCommon := x; max := y)
            !mostCommon 

        (* Count the number of entries in the association table. *)
        let count = ref 0 
        for (KeyValue(action,terminals)) in terminalsByAction do 
            for terminals  in terminals do 
               if action <> mostCommonAction then  
                   incr count;
        
        (* Write the head of the table (i.e. the number of entries and the default value) *)
        actionTableCurrIndex := !actionTableCurrIndex + 1;
        outputCodedUInt16 os !count;
        outputCodedUInt16 os (actionCoding mostCommonAction);
        
        (* Write the pairs of entries in incremental order by key *)
        (* This lets us implement the lookup by a binary chop. *)
        for terminal = 0 to Array.length actions-1 do  
            let action = snd actions.[terminal] in 
            if action <> mostCommonAction then  (
                actionTableCurrIndex := !actionTableCurrIndex + 1;
                outputCodedUInt16 os terminal;
                outputCodedUInt16 os (actionCoding action);
            );
    cprintfn cos "|]" ;
    (* Output offsets into actions table where the actions for a particular nonterminal begin *)
    cprintf cos "let _fsyacc_actionTableRowOffsets = [|" ;
    for j = 0 to numActionRows-1 do  
        cprintf cos "%a" outputCodedUInt16 actionIndexes.[j];
    cprintfn cos "|]" ;

  end;
  begin 
      cprintf cos "let _fsyacc_reductionSymbolCounts = [|" ;
      for nt,ntIdx,syms,code in prods do 
          cprintf cos "%a" outputCodedUInt16 (List.length syms);
      cprintfn cos "|]" ;
  end;
  begin 
      cprintf cos "let _fsyacc_productionToNonTerminalTable = [|" ;
      for nt,ntIdx,syms,code in prods do 
          cprintf cos "%a" outputCodedUInt16 ntIdx;
      cprintfn cos "|]" ;
  end;
  begin 
      cprintf cos "let _fsyacc_immediateActions = [|" ;
      for prodIdx in immediateActionTable do 
          match prodIdx with
            | None     -> cprintf cos "%a" outputCodedUInt16 anyMarker (* NONE REP *)
            | Some act -> cprintf cos "%a" outputCodedUInt16 (actionCoding act)
      cprintfn cos "|]" ;
  end;
  
  let getType nt = if types.ContainsKey nt then  types.[nt] else "'"+nt 
  begin 
      cprintf cos "let _fsyacc_reductions ()  =" ;
      cprintfn cos "    [| " ;
      for nt,ntIdx,syms,code in prods do 
          cprintfn cos "# %d \"%s\"" !lineCountOutput output;
          cprintfn cos "        (fun (parseState : %s.IParseState) ->"  parslib
          if !compat then 
              cprintfn cos "            Parsing.set_parse_state parseState;"
          syms |> List.iteri (fun i sym -> 
              let tyopt = 
                  match sym with
                  | Terminal t -> 
                      if tokens.ContainsKey t then 
                        tokens.[t]
                      else None
                  | NonTerminal nt -> Some (getType nt) 
              match tyopt with 
              | Some ty -> cprintfn cos "            let _%d = (let data = parseState.GetInput(%d) in (Microsoft.FSharp.Core.Operators.unbox data : %s)) in" (i+1) (i+1) ty
              | None -> ())
          cprintfn cos "            Microsoft.FSharp.Core.Operators.box" 
          cprintfn cos "                (";
          cprintfn cos "                   (";
          match code with 
          | Some (_,pos) -> cprintfn cos "# %d \"%s\"" pos.pos_lnum pos.pos_fname
          | None -> ()
          match code with 
          | Some (code,_) -> 
              let dollar = ref false in 
              let c = code |> String.collect (fun c -> 
                  if not !dollar && c = '$' then (dollar := true; "")
                  elif !dollar && c >= '0' && c <= '9' then (dollar := false; "_"+new System.String(c,1))
                  elif !dollar then (dollar := false; "$"+new System.String(c,1))
                  else new System.String(c,1))
              let lines = c.Split([| '\r'; '\n' |], System.StringSplitOptions.RemoveEmptyEntries);
              for line in lines do 
                  cprintfn cos "                     %s" line;
              if !dollar then os.Write '$'
          | None -> 
              cprintfn cos "                      raise (%s.Accept(Microsoft.FSharp.Core.Operators.box _1))" parslib
          cprintfn cos "                   )";
          // Place the line count back for the type constraint
          match code with 
          | Some (_,pos) -> cprintfn cos "# %d \"%s\"" pos.pos_lnum pos.pos_fname
          | None -> ()
          cprintfn cos "                 : %s));" (if types.ContainsKey nt then  types.[nt] else "'"+nt);
      done;
      cprintfn cos "|]" ;
  end;
  cprintfn cos "# %d \"%s\"" !lineCountOutput output;
  cprintfn cos "let tables () : %s.Tables<_> = " parslib
  cprintfn cos "  { reductions= _fsyacc_reductions ();"
  cprintfn cos "    endOfInputTag = _fsyacc_endOfInputTag;"
  cprintfn cos "    tagOfToken = tagOfToken;"
  cprintfn cos "    dataOfToken = _fsyacc_dataOfToken; "
  cprintfn cos "    actionTableElements = _fsyacc_actionTableElements;"
  cprintfn cos "    actionTableRowOffsets = _fsyacc_actionTableRowOffsets;"
  cprintfn cos "    stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements;"
  cprintfn cos "    stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets;"
  cprintfn cos "    reductionSymbolCounts = _fsyacc_reductionSymbolCounts;"
  cprintfn cos "    immediateActions = _fsyacc_immediateActions;"
  cprintfn cos "    gotos = _fsyacc_gotos;"
  cprintfn cos "    sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets;"
  cprintfn cos "    tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal;"
  cprintfn cos "    parseError = (fun (ctxt:%s.ParseErrorContext<_>) -> " parslib
  cprintfn cos "                              match parse_error_rich with "
  cprintfn cos "                              | Some f -> f ctxt"
  cprintfn cos "                              | None -> parse_error ctxt.Message);"
  
  cprintfn cos "    numTerminals = %d;" (Array.length actionTable.[0]);
  cprintfn cos "    productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable  }"
  cprintfn cos "let engine lexer lexbuf startState = (tables ()).Interpret(lexer, lexbuf, startState)"                                                                                                         

  for (id,startState) in List.zip spec.StartSymbols startStates do
        if not (types.ContainsKey id) then 
          failwith ("a %type declaration is required for for start token "+id);
        let ty = types.[id] in 
        cprintfn cos "let %s lexer lexbuf : %s =" id ty;
        cprintfn cos "    Microsoft.FSharp.Core.Operators.unbox ((tables ()).Interpret(lexer, lexbuf, %d))" startState

  for id in spec.StartSymbols do
      if not (types.ContainsKey id) then 
        failwith ("a %type declaration is required for start token "+id);
      let ty = types.[id] in 
      cprintfn cosi "val %s : (%s.LexBuffer<%s> -> token) -> %s.LexBuffer<%s> -> (%s) " id lexlib tychar lexlib tychar ty;

  logf (fun oso -> oso.Close())

let result = 
    try main()
    with e -> 
      eprintf "FSYACC: error FSY000: %s" (match e with Failure s -> s | e -> e.Message);
      exit 1

