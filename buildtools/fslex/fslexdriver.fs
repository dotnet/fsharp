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
      lexerLibraryName: string
      domain : Domain }

type PerRuleData = list<DfaNode * seq<Code>>
type DfaNodes = list<DfaNode>

type Writer(fileName) =
    let os = File.CreateText fileName
    let mutable lineCount = 0
    let incr () =
        lineCount <- lineCount + 1

    member x.writeLine fmt =
        Printf.kfprintf (fun () -> incr(); os.WriteLine()) os fmt

    member x.write fmt =
        Printf.fprintf os fmt

    member x.writeCode (code, pos: Position) =
        if pos <> Position.Empty  // If bottom code is unspecified, then position is empty.
        then
            x.writeLine "# %d \"%s\"" pos.Line pos.FileName
            x.writeLine "%s" code
            let numLines = code.Replace("\r","").Split([| '\n' |]).Length
            lineCount  <- lineCount + numLines
            x.writeLine "# %d \"%s\"" lineCount fileName

    member x.LineCount = lineCount

    member x.WriteUint16 (n: int) =
        os.Write n;
        os.Write "us;"

    interface IDisposable with
        member x.Dispose() = os.Dispose()

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
        writer.write "#light \"off\""

let writeModuleExpression genModuleName isInternal (writer: Writer) =
    match genModuleName with
    | None -> ()
    | Some s ->
        let internal_tag = if isInternal then "internal " else ""
        writer.writeLine "module %s%s" internal_tag s

let writeTopCode code (writer: Writer) = writer.writeCode code

let writeUnicodeTranslationArray dfaNodes domain (writer: Writer) =
    let parseContext = 
        { unicode = match domain with | Unicode -> true | ASCII -> false
          caseInsensitive = false }
    writer.writeLine "let trans : uint16[] array = "
    writer.writeLine "    [| "
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
            writer.writeLine "    (* State %d *)" state.Id
            writer.write  "     [| "
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
            writer.writeLine  "|];"
        done

    | ASCII ->
        // Each row for the ASCII table has format
        //      256 entries for ASCII characters
        //      1 entry for EOF
        //
        // Each entry is an encoded UInt16 value indicating the next state to transition to for this input.

        // This emits a (256+1) * #states array of encoded UInt16 values
        for state in dfaNodes do
            writer.writeLine "   (* State %d *)" state.Id
            writer.write " [|"
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
            writer.writeLine "|];"
        done

    writer.writeLine "    |] "

let writeUnicodeActionsArray dfaNodes (writer: Writer) =
    writer.write "let actions : uint16[] = [|"
    for state in dfaNodes do
        if state.Accepted.Length > 0 then
          writer.WriteUint16 (snd state.Accepted.Head)
        else
          writer.WriteUint16 sentinel
    done
    writer.writeLine  "|]"

let writeUnicodeTables lexerLibraryName domain dfaNodes (writer: Writer) =
    writeUnicodeTranslationArray dfaNodes domain writer
    writeUnicodeActionsArray dfaNodes writer
    writer.writeLine  "let _fslex_tables = %s.%sTables.Create(trans,actions)" lexerLibraryName (match domain with | Unicode -> "Unicode" | ASCII -> "Ascii")

let writeRules (rules: Rule list) (perRuleData: PerRuleData) outputFileName (writer: Writer) =
    writer.writeLine  "let rec _fslex_dummy () = _fslex_dummy() "

    // These actions push the additional start state and come first, because they are then typically inlined into later
    // rules. This means more tailcalls are taken as direct branches, increasing efficiency and
    // improving stack usage on platforms that do not take tailcalls.
    for (startNode, actions),(ident,args,_) in List.zip perRuleData rules do
        writer.writeLine "// Rule %s" ident
        writer.writeLine "and %s %s lexbuf =" ident (String.Join(" ", Array.ofList args))
        writer.writeLine "  match _fslex_tables.Interpret(%d,lexbuf) with" startNode.Id
        actions |> Seq.iteri (fun i (code:string, pos) ->
            writer.writeLine "  | %d -> ( " i
            writer.writeLine "# %d \"%s\"" pos.Line pos.FileName
            let lines = code.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            for line in lines do
                writer.writeLine "               %s" line
            writer.writeLine "# %d \"%s\"" writer.LineCount outputFileName
            writer.writeLine "          )")
        writer.writeLine "  | _ -> failwith \"%s\"" ident

    writer.writeLine ""

let writeBottomCode code (writer: Writer) = writer.writeCode code

let writeFooter outputFileName (writer: Writer) = writer.writeLine "# 3000000 \"%s\"" outputFileName

let writeSpecToFile (state: GeneratorState) (spec: Spec) (perRuleData: PerRuleData) (dfaNodes: DfaNodes) =
    use writer = new Writer(state.outputFileName)
    writeLightMode state.disableLightMode state.outputFileName writer
    writeModuleExpression state.generatedModuleName state.generateInternalModule writer
    writeTopCode spec.TopCode writer
    writeUnicodeTables state.lexerLibraryName state.domain dfaNodes writer
    writeRules spec.Rules perRuleData state.outputFileName writer
    writeBottomCode spec.BottomCode writer
    writeFooter state.outputFileName writer
    ()