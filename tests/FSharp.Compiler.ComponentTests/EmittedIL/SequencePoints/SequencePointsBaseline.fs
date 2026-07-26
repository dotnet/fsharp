namespace EmittedIL

open System.Diagnostics
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open FSharp.Test.Compiler

/// Snapshots sequence points and IL into `<baselineDirectory>/<test module>/<test name>.bsl`.
type SequencePointsBaseline(baselineDirectory: string) =
    member private _.path(moduleName, name) =
        Path.Combine(baselineDirectory, moduleName, name + ".bsl")

    member this.verify(source: string, [<CallerMemberName; Optional; DefaultParameterValue("")>] name: string) =
        let moduleName = StackTrace().GetFrame(1).GetMethod().DeclaringType.Name
        let source = source.Trim()
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> withNoOptimize
        |> compile
        |> shouldSucceed
        |> verifySequencePointsBaseline source (this.path(moduleName, name))
        |> ignore

    /// `frame` is the test method. Source is recovered from the result so its line numbers
    /// match the emitted sequence points.
    member this.verifyResult(frame: StackFrame, result: CompilationResult) =
        let m = frame.GetMethod()
        let source =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                match r.Compilation with
                | FS fs -> fs.Source.GetSourceText |> Option.defaultWith (fun () -> fs.Source.LoadSourceText())
                | _ -> failwith "Expected an F# compilation"
        result
        |> shouldSucceed
        |> verifySequencePointsBaseline source (this.path(m.DeclaringType.Name, m.Name))
