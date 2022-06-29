module HistoricalBenchmark.Helpers

open System.IO
open System.Threading.Tasks
#if SERVICE_13_0_0
open Microsoft.FSharp.Compiler.SourceCodeServices
#else
#if SERVICE_30_0_0
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
#else
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
#endif
#endif

let getFileSourceText (filePath : string) =
    let text = File.ReadAllText(filePath)
#if SERVICE_13_0_0
    text
#else
    SourceText.ofString text
#endif

let failOnErrors (results : FSharpCheckFileResults) =
#if SERVICE_13_0_0 || SERVICE_30_0_0 
    if results.Errors.Length > 0 then failwithf "had errors: %A" results.Errors
#else
    if results.Diagnostics.Length > 0 then failwithf $"had errors: %A{results.Diagnostics}"
#endif

let makeCmdlineArgsWithSystemReferences (filePath : string) =
    let assemblies =
        let mainAssemblyLocation = typeof<System.Object>.Assembly.Location
        let frameworkDirectory = Path.GetDirectoryName(mainAssemblyLocation)
        Directory.EnumerateFiles(frameworkDirectory)
        |> Seq.filter (fun x ->
            let name = Path.GetFileName(x)
            (name.StartsWith("System.") && name.EndsWith(".dll") && not(name.Contains("Native"))) ||
            name.Contains("netstandard") ||
            name.Contains("mscorlib")
        )
        |> Array.ofSeq
        |> Array.append [|typeof<Async>.Assembly.Location|]
        
    let refs =
        assemblies
        |> Array.map (fun x ->
            $"-r:{x}"
        )
    [|"--simpleresolution";"--targetprofile:netcore";"--noframework"|]
    |> Array.append refs
    |> Array.append [|filePath|]
