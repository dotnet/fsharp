module FsLexYacc.FsLex.Driver 

open FsLexYacc.FsLex.AST
open System
open System.IO
open FSharp.Text.Lexing
open System.Collections.Generic

type Domain = Unicode | ASCII

/// Wraps the inputs to the code generator
type GeneratorState =
    { inputFileName: string
      outputFileName: string
      inputCodePage: System.Text.Encoding
      generatedModuleName: string option
      disableLightMode: bool option
      generateInternalModule: bool
      opens: string list
      lexerLibraryName: string
      domain : Domain }

type PerRuleData = list<DfaNode * seq<Code>>
type DfaNodes = list<DfaNode>

type Writer(outputFileName, outputFileInterface) =
    let os = File.CreateText outputFileName :> TextWriter
    let mutable lineCount = 0
    let osi = File.CreateText outputFileInterface :> TextWriter
    let mutable interfaceLineCount = 0
    let incr () =
        lineCount <- lineCount + 1

    member x.WriteLine fmt =
        Printf.kfprintf (fun () -> incr(); os.WriteLine()) os fmt

    member x.Write fmt =
        Printf.fprintf os fmt

    member x.WriteCode (code, pos: Position) =
        if pos <> Position.Empty  // If bottom code is unspecified, then position is empty.
        then
            x.WriteLine "# %d \"%s\"" pos.Line pos.FileName
            x.WriteLine "%s" code
            let numLines = code.Replace("\r","").Split([| '\n' |]).Length
            lineCount  <- lineCount + numLines
            x.WriteLine "# %d \"%s\"" lineCount outputFileName

    member x.WriteUint16 (n: int) =
        os.Write n;
        os.Write "us;"

    member x.LineCount = lineCount
    
    member x.WriteInterface format = 
        fprintf osi format
    
    member x.WriteLineInterface format = 
        Printf.kfprintf (fun _ -> 
            interfaceLineCount <- interfaceLineCount + 1
            osi.WriteLine ()
        ) osi format
        
    member x.InterfaceLineCount = interfaceLineCount

    interface IDisposable with
        member x.Dispose() =
            os.Dispose()
            osi.Dispose()

let sentinel = 255 * 256 + 255

let readSpecFromFile fileName codePage =
  let stream,reader,lexbuf = UnicodeFileAsLexbuf(fileName, codePage)
  use stream = stream
  use reader = reader
  try
      let spec = Parser.spec Lexer.token lexbuf
      Ok spec
  with e ->
      (e, lexbuf.StartPos.Line, lexbuf.StartPos.Column)
      |> Error

let writeLightMode lightModeDisabled (fileName: string) (writer: Writer) =
    if lightModeDisabled = Some false || (lightModeDisabled = None && (Path.HasExtension(fileName) && Path.GetExtension(fileName) = ".ml"))
    then
        writer.Write "#light \"off\""

let writeModuleExpression genModuleName isInternal (writer: Writer) =
    match genModuleName with
    | None -> ()
    | Some s ->
        let internal_tag = if isInternal then "internal " else ""
        writer.WriteLine "module %s%s" internal_tag s
        writer.WriteLineInterface "module %s%s" internal_tag s

let writeOpens opens (writer: Writer) =
    writer.WriteLine ""
    writer.WriteLineInterface ""
    
    for s in opens do
        writer.WriteLine "open %s" s
        writer.WriteLineInterface "open %s" s

    if not (Seq.isEmpty opens) then
        writer.WriteLine ""
        writer.WriteLineInterface ""

let writeTopCode code (writer: Writer) = writer.WriteCode code

let writeUnicodeTranslationArray dfaNodes domain (writer: Writer) =
    let parseContext = 
        { unicode = match domain with | Unicode -> true | ASCII -> false
          caseInsensitive = false }
    writer.WriteLine "let trans : uint16[] array = "
    writer.WriteLine "    [| "
    match domain with
    | Unicode ->
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
            writer.WriteLine "    (* State %d *)" state.Id
            writer.Write  "     [| "
            let trans =
                let dict = Dictionary()
                state.Transitions |> List.iter dict.Add
                dict
            let emit n =
                if trans.ContainsKey(n) then
                  writer.WriteUint16 trans.[n].Id
                else
                  writer.WriteUint16 sentinel
            for i = 0 to numLowUnicodeChars-1 do
                let c = char i
                emit (EncodeChar c parseContext)
            for c in specificUnicodeChars do
                writer.WriteUint16 (int c)
                emit (EncodeChar c parseContext)
            for i = 0 to NumUnicodeCategories-1 do
                emit (EncodeUnicodeCategoryIndex i)
            emit Eof
            writer.WriteLine  "|];"
        done

    | ASCII ->
        // Each row for the ASCII table has format
        //      256 entries for ASCII characters
        //      1 entry for EOF
        //
        // Each entry is an encoded UInt16 value indicating the next state to transition to for this input.

        // This emits a (256+1) * #states array of encoded UInt16 values
        for state in dfaNodes do
            writer.WriteLine "   (* State %d *)" state.Id
            writer.Write " [|"
            let trans =
                let dict = Dictionary()
                state.Transitions |> List.iter dict.Add
                dict
            let emit n =
                if trans.ContainsKey(n) then
                  writer.WriteUint16 trans.[n].Id
                else
                  writer.WriteUint16 sentinel
            for i = 0 to 255 do
                let c = char i
                emit (EncodeChar c parseContext)
            emit Eof
            writer.WriteLine "|];"
        done

    writer.WriteLine "    |] "

let writeUnicodeActionsArray dfaNodes (writer: Writer) =
    writer.Write "let actions : uint16[] = [|"
    for state in dfaNodes do
        if state.Accepted.Length > 0 then
          writer.WriteUint16 (snd state.Accepted.Head)
        else
          writer.WriteUint16 sentinel
    done
    writer.WriteLine  "|]"

let writeUnicodeTables lexerLibraryName domain dfaNodes (writer: Writer) =
    writeUnicodeTranslationArray dfaNodes domain writer
    writeUnicodeActionsArray dfaNodes writer
    writer.WriteLine  "let _fslex_tables = %s.%sTables.Create(trans,actions)" lexerLibraryName (match domain with | Unicode -> "Unicode" | ASCII -> "Ascii")

let writeRules (rules: Rule list) (perRuleData: PerRuleData) outputFileName (writer: Writer) =
    writer.WriteLine  "let rec _fslex_dummy () = _fslex_dummy() "

    // These actions push the additional start state and come first, because they are then typically inlined into later
    // rules. This means more tailcalls are taken as direct branches, increasing efficiency and
    // improving stack usage on platforms that do not take tailcalls.
    for (startNode, actions),(ident,args,_) in List.zip perRuleData rules do
        writer.WriteLine "// Rule %s" ident
        writer.WriteLineInterface "/// Rule %s" ident
        let arguments =
            args
            |> List.map (function
                | RuleArgument.Ident ident -> ident
                | RuleArgument.Typed(ident, typ) -> sprintf "(%s: %s)" ident typ)
            |> String.concat " "

        writer.WriteLine "and %s %s lexbuf =" ident arguments
        
        let signature =
            if List.isEmpty args then
                sprintf "val %s: lexbuf: LexBuffer<char> -> token" ident
            else
                args
                |> List.map (function
                    | RuleArgument.Ident ident ->
                        // This is not going to lead to a valid signature file, the only workaround is that the caller will specify the type.
                        sprintf "%s: obj" ident
                    | RuleArgument.Typed(ident, typ) -> sprintf"%s: %s" ident typ)
                |> String.concat " -> "
                |> sprintf "val %s: %s -> lexbuf: LexBuffer<char> -> token" ident

        writer.WriteLineInterface "%s" signature
        
        writer.WriteLine "  match _fslex_tables.Interpret(%d,lexbuf) with" startNode.Id
        actions |> Seq.iteri (fun i (code:string, pos) ->
            writer.WriteLine "  | %d -> ( " i
            writer.WriteLine "# %d \"%s\"" pos.Line pos.FileName
            let lines = code.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            for line in lines do
                writer.WriteLine "               %s" line
            writer.WriteLine "# %d \"%s\"" writer.LineCount outputFileName
            writer.WriteLine "          )")
        writer.WriteLine "  | _ -> failwith \"%s\"" ident

    writer.WriteLine ""

let writeBottomCode code (writer: Writer) = writer.WriteCode code

let writeFooter outputFileName (writer: Writer) = writer.WriteLine "# 3000000 \"%s\"" outputFileName

let writeSpecToFile (state: GeneratorState) (spec: Spec) (perRuleData: PerRuleData) (dfaNodes: DfaNodes) =
    let output, outputi = state.outputFileName, String.Concat(state.outputFileName, "i")
    use writer = new Writer(output, outputi)
    writeLightMode state.disableLightMode state.outputFileName writer
    writeModuleExpression state.generatedModuleName state.generateInternalModule writer
    writeOpens state.opens writer
    writeTopCode spec.TopCode writer
    writeUnicodeTables state.lexerLibraryName state.domain dfaNodes writer
    writeRules spec.Rules perRuleData state.outputFileName writer
    writeBottomCode spec.BottomCode writer
    writeFooter state.outputFileName writer
    ()