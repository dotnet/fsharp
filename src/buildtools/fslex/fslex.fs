// (c) Microsoft Corporation 2005-2009.  

module internal FsLexYacc.FsLex.Driver 

open FsLexYacc.FsLex
open FsLexYacc.FsLex.AST
open FsLexYacc.FsLex.Parser
open Printf
open Internal.Utilities
open Internal.Utilities.Text.Lexing
open System
open System.Collections.Generic
open System.IO 

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
    // It also presents the bytes read to the lexer in UTF8 decoded form
    let stream  = new FileStream(filename,FileMode.Open,FileAccess.Read,FileShare.Read) 
    let reader = 
        match codePage with 
        | None -> new  StreamReader(stream,true)
        | Some n -> new  StreamReader(stream,System.Text.Encoding.GetEncoding(n)) 
    let lexbuf = LexBuffer.FromFunction(reader.Read) 
    lexbuf.EndPos <- Position.FirstLine(filename)
    stream, reader, lexbuf
    
//------------------------------------------------------------------
// This is the program proper

let input = ref None
let out = ref None
let inputCodePage = ref None
let light = ref None

let mutable lexlib = "FSharp.Text.Lexing"

let usage =
  [ ArgInfo ("-o", ArgType.String (fun s -> out := Some s), "Name the output file.") 
    ArgInfo ("--codepage", ArgType.Int (fun i -> inputCodePage := Some i), "Assume input lexer specification file is encoded with the given codepage.") 
    ArgInfo ("--light", ArgType.Unit (fun () ->  light := Some true), "(ignored)")
    ArgInfo ("--light-off", ArgType.Unit (fun () ->  light := Some false), "Add #light \"off\" to the top of the generated file")
    ArgInfo ("--lexlib", ArgType.String (fun s ->  lexlib <- s), "Specify the namespace for the implementation of the lexer table interpreter (default FSharp.Text.Lexing)")
    ArgInfo ("--unicode", ArgType.Set unicode, "Produce a lexer for use with 16-bit unicode characters.")  
  ]

let _ = ArgParser.Parse(usage, (fun x -> match !input with Some _ -> failwith "more than one input given" | None -> input := Some x), "fslex <filename>")

let outputInt (os: TextWriter) (n:int) = os.Write(string n)

let outputCodedUInt16 (os: #TextWriter)  (n:int) = 
  os.Write n
  os.Write "us; "

let sentinel = 255 * 256 + 255 

let lineCount = ref 0
let cfprintfn (os: #TextWriter) fmt = Printf.kfprintf (fun () -> incr lineCount; os.WriteLine()) os fmt

let main() = 
  try 
    let filename = (match !input with Some x -> x | None -> failwith "no input given") 
    let domain = if !unicode then "Unicode" else "Ascii" 
    let spec = 
      let stream,reader,lexbuf = UnicodeFileAsLexbuf(filename, !inputCodePage) 
      use stream = stream
      use reader = reader
      try 
          Parser.spec Lexer.token lexbuf 
      with e -> 
          eprintf "%s(%d,%d): error: %s" filename lexbuf.StartPos.Line lexbuf.StartPos.Column 
              (match e with 
               | Failure s -> s 
               | _ -> e.Message)
          exit 1
    printfn "compiling to dfas (can take a while...)"
    let perRuleData, dfaNodes = AST.Compile spec
    let dfaNodes = dfaNodes |> List.sortBy (fun n -> n.Id) 

    printfn "%d states" dfaNodes.Length
    printfn "writing output" 
    
    let output = 
        match !out with 
        | Some x -> x 
        | _ -> 
            Path.Combine (Path.GetDirectoryName filename,Path.GetFileNameWithoutExtension(filename)) + ".fs"
    use os = System.IO.File.CreateText output

    if (!light = Some(false)) || (!light = None && (Path.HasExtension(output) && Path.GetExtension(output) = ".ml")) then
        cfprintfn os "#light \"off\""
    
    let printLinesIfCodeDefined (code,pos:Position) =
        if pos <> Position.Empty  // If bottom code is unspecified, then position is empty.        
        then 
            cfprintfn os "# %d \"%s\"" pos.Line pos.FileName
            cfprintfn os "%s" code

    printLinesIfCodeDefined spec.TopCode
    let code = fst spec.TopCode
    lineCount := !lineCount + code.Replace("\r","").Split([| '\n' |]).Length
    cfprintfn os "# %d \"%s\"" !lineCount output
    
    cfprintfn os "let trans : uint16[] array = "
    cfprintfn os "    [| "
    if !unicode then 
        let specificUnicodeChars = GetSpecificUnicodeChars()
        // This emits a (numLowUnicodeChars+NumUnicodeCategories+(2*#specificUnicodeChars)+1) * #states array of encoded UInt16 values
        
        // Each row for the Unicode table has format 
        //      128 entries for ASCII characters
        //      A variable number of 2*UInt16 entries for SpecificUnicodeChars 
        //      30 entries, one for each UnicodeCategory
        //      1 entry for EOF
        //
        // Each entry is an encoded UInt16 value indicating the next state to transition to for this input.
        //
        // For the SpecificUnicodeChars the entries are char/next-state pairs.
        for state in dfaNodes do
            cfprintfn os "    (* State %d *)" state.Id
            fprintf os "     [| "
            let trans = 
                let dict = new Dictionary<_,_>()
                state.Transitions |> List.iter dict.Add
                dict
            let emit n = 
                if trans.ContainsKey(n) then 
                  outputCodedUInt16 os trans.[n].Id 
                else
                  outputCodedUInt16 os sentinel
            for i = 0 to numLowUnicodeChars-1 do 
                let c = char i
                emit (EncodeChar c)
            for c in specificUnicodeChars do 
                outputCodedUInt16 os (int c) 
                emit (EncodeChar c)
            for i = 0 to NumUnicodeCategories-1 do 
                emit (EncodeUnicodeCategoryIndex i)
            emit Eof
            cfprintfn os "|];"
        done
    
    else
        // Each row for the ASCII table has format 
        //      256 entries for ASCII characters
        //      1 entry for EOF
        //
        // Each entry is an encoded UInt16 value indicating the next state to transition to for this input.

        // This emits a (256+1) * #states array of encoded UInt16 values
        for state in dfaNodes do
            cfprintfn os "   (* State %d *)" state.Id
            fprintf os " [|"
            let trans = 
                let dict = new Dictionary<_,_>()
                state.Transitions |> List.iter dict.Add
                dict
            let emit n = 
                if trans.ContainsKey(n) then 
                  outputCodedUInt16 os trans.[n].Id 
                else
                  outputCodedUInt16 os sentinel
            for i = 0 to 255 do 
                let c = char i
                emit (EncodeChar c)
            emit Eof
            cfprintfn os "|];"
        done
    
    cfprintfn os "    |] "
    
    fprintf os "let actions : uint16[] = [|"
    for state in dfaNodes do
        if state.Accepted.Length > 0 then 
          outputCodedUInt16 os (snd state.Accepted.Head)
        else
          outputCodedUInt16 os sentinel
    done
    cfprintfn os "|]"
    cfprintfn os "let _fslex_tables = %s.%sTables.Create(trans,actions)" lexlib domain
    
    cfprintfn os "let rec _fslex_dummy () = _fslex_dummy() "

    // These actions push the additional start state and come first, because they are then typically inlined into later
    // rules. This means more tailcalls are taken as direct branches, increasing efficiency and 
    // improving stack usage on platforms that do not take tailcalls.
    for ((startNode, actions),(ident,args,_)) in List.zip perRuleData spec.Rules do
        cfprintfn os "// Rule %s" ident
        cfprintfn os "and %s %s lexbuf =" ident (String.Join(" ",Array.ofList args))
        cfprintfn os "  match _fslex_tables.Interpret(%d,lexbuf) with" startNode.Id
        actions |> Seq.iteri (fun i (code:string, pos) -> 
            cfprintfn os "  | %d -> ( " i
            cfprintfn os "# %d \"%s\"" pos.Line pos.FileName
            let lines = code.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            for line in lines do
                cfprintfn os "               %s" line
            cfprintfn os "# %d \"%s\"" !lineCount output
            cfprintfn os "          )")
        cfprintfn os "  | _ -> failwith \"%s\"" ident
    

    cfprintfn os ""
        
    printLinesIfCodeDefined spec.BottomCode
    cfprintfn os "# 3000000 \"%s\"" output
    
  with e -> 
    eprintf "FSLEX: error FSL000: %s" (match e with Failure s -> s | e -> e.ToString())
    exit 1


let result = main()
