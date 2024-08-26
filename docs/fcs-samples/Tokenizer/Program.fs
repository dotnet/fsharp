open FSharp.Compiler.SourceCodeServices

let sourceTok = FSharpSourceTokenizer([], Some "C:\\test.fsx")

let tokenizeLines (lines:string[]) =
  [ let state = ref FSharpTokenizerLexState.Initial
    for n, line in lines |> Seq.zip [ 0 .. lines.Length ] do
      let tokenizer = sourceTok.CreateLineTokenizer(line)
      let rec parseLine() = seq {
        match tokenizer.ScanToken(!state) with
        | Some(tok), nstate ->
            let str = line.Substring(tok.LeftColumn, tok.RightColumn - tok.LeftColumn + 1)
            yield str, tok
            state := nstate
            yield! parseLine()
        | None, nstate -> state := nstate }
      yield n, parseLine() |> List.ofSeq ]

let tokenizedLines = 
  tokenizeLines
    [| "// Sets the hello world variable"
       "let hello = \"Hello world\" " |]

for lineNo, lineToks in tokenizedLines do
  printfn "%d:  " lineNo
  for str, info in lineToks do printfn "       [%s:'%s']" info.TokenName str
