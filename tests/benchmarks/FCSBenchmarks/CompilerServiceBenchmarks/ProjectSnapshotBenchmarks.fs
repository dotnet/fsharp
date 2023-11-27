module FSharp.Benchmarks.ProjectSnapshotBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open FSharp.Test.ProjectGeneration
open BenchmarkDotNet.Engines


[<Literal>]
let FSharpCategory = "fsharp"
