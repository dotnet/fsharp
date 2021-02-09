namespace TestAnalyzer

open FSharp.Core.CompilerServices
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

[<assembly: AnalyzerAssemblyAttribute>]
do()

[<AnalyzerAttribute>]
type MyAnalyzer(ctxt) = 
    inherit FSharpAnalyzer(ctxt)

    override this.OnCheckFile(fileCtxt, ct) =
        
        let m = Range.mkRange fileCtxt.ParseFileResults.FileName (Position.mkPos 3 0) (Position.mkPos 3 80)
        let m2 = Range.mkRange fileCtxt.ParseFileResults.FileName (Position.mkPos 6 0) (Position.mkPos 6 80)
        let source = fileCtxt.GetFileSource(fileCtxt.ParseFileResults.FileName)
        let text = source.GetSubTextString(0,source.Length)
        [| if text.Contains("WIBBLE") |> not then 
              FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "this diagnostic is always on line 6 until the magic word WIBBLE appears", 45, m2, "FA")
           if text.Contains("WAZZAM") |> not then 
               FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "this diagnostic is always on line 3 until the magic word WAZZAM appears", 45, m, "FA") |]

    override _.TryAdditionalToolTip(fileCtxt, pos, ct) =
        Some [| TaggedText.tagText $"This thing is on line {pos.Line} in file {fileCtxt.ParseFileResults.FileName}" |]

