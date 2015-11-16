module ``FSharp-Tests-Perf``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let testContext = FSharpTestSuite.testContext


module Graph = 

    [<Test; FSharpSuitePermutations("perf/graph")>]
    let graph p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module Nbody = 

    [<Test; FSharpSuitePermutations("perf/nbody")>]
    let nbody p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })
