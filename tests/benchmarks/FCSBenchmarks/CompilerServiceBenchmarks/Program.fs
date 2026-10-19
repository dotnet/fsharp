open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks
open BenchmarkDotNet.Configs

[<EntryPoint>]
let main args =
    match args with
    // Standalone retained-memory probe (not a BDN benchmark); see RetainedMemoryProbe for why.
    | [| "retained-memory" |] ->
        RetainedMemoryProbe.run ()
        0
    // Per-phase compile breakdown via the compiler's --times flag; see TimesProbe.
    | [| "times" |] ->
        TimesProbe.run ()
        0
    // Compile a real project from a captured fsc response file; see CompileProjectProbe.
    | [| "compile-project"; responseFile; projectDir |] ->
        CompileProjectProbe.run responseFile projectDir
        0
    // Deterministic retained memory of a real project's analysis held live; see RetainProjectProbe.
    | [| "retain-project"; responseFile; projectDir |] ->
        RetainProjectProbe.run responseFile projectDir
        0
    // Single-file check then hold alive for an external heap dump; see CheckFileProbe.
    | [| "check-file"; responseFile; projectDir; fileToCheck |] ->
        CheckFileProbe.run responseFile projectDir fileToCheck
        0
    | _ ->
        let cfg = ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator)
        BenchmarkSwitcher.FromAssembly(typeof<DecentlySizedStandAloneFileBenchmark>.Assembly).Run(args,cfg) |> ignore
        0
