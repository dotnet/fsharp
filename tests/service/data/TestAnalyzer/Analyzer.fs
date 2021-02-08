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

    override _.OnCheckFile(fileCtxt, ct) =
        
        let m = Range.mkRange fileCtxt.ParseFileResults.FileName (Position.mkPos 3 0) (Position.mkPos 3 80)
        let m2 = Range.mkRange fileCtxt.ParseFileResults.FileName (Position.mkPos 6 0) (Position.mkPos 6 80)
        let source = fileCtxt.TryGetFileSource(fileCtxt.ParseFileResults.FileName).Value
        [| if source.ToString().Contains("WIBBLE") |> not then 
              FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "this diagnostic is always on line 6 until the magic word WIBBLE appears", 45, m2, "FA")
           if source.ToString().Contains("WAZZAM") |> not then 
               FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "this diagnostic is always on line 3 until the magic word WAZZAM appears", 45, m, "FA") |]
