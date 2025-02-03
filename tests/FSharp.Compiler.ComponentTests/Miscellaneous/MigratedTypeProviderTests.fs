module Miscellaneous.MigratedTypeProviderTests

open Xunit
open FSharp.Test
open FSharp.Test.ScriptHelpers 
open System.Runtime.InteropServices
open Miscellaneous.FsharpSuiteMigrated.TestFrameworkAdapter



[<Fact>]
let ``13219-bug-FSI`` () = singleTestBuildAndRun "regression/13219" FSI

[<Fact>]
let ``multi-package-type-provider-test-FSI`` () = singleTestBuildAndRun "regression/13710" FSI