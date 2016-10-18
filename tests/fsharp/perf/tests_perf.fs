module ``FSharp-Tests-Perf``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest

let testConfig = FSharpTestSuite.testConfig

[<Test; FSharpSuiteScriptPermutations("perf/graph")>]
let graph p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("perf/nbody")>]
let nbody p = singleTestBuildAndRun p
