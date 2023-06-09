module FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.StaticLet

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let verifyCompileAndRun compilation =
    compilation
    |> asExe
    |> compileAndRun


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionShowCase.fs"|])>]
let ``Static let - union showcase`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init U 1
init U 2
init end
Case2 1
Case2 2"""


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordShowCase.fs"|])>]
let ``Static let - record showcase`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init R 1
init R 2
1
2"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleEmptyType.fs"|])>]
let ``Static let in empty type`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleEmptyGenericType.fs"|])>]
let ``Static let in empty generic type`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleUnion.fs"|])>]
let ``Static let in simple union`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlainEnum.fs"|])>]
let ``Support in plain enums - typecheck should fail`` compilation =
    compilation
    |> typecheck
    |> shouldFail
    |> withDiagnosticMessage "Enumerations cannot have members"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternForUnion.fs"|])>]
let ``Static active pattern in  union`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """B 999"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructUnion.fs"|])>]
let ``Static let in struct union`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleRecord.fs"|])>]
let ``Static let in simple record`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructRecord.fs"|])>]
let ``Static let in struct record`` compilation =
    compilation
    |> typecheck
    |> shouldSucceed
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CreateUnionCases.fs"|])>]
let ``Static let creating DU cases`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "..."

    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionOrderOfExecution.fs"|])>]
let ``Static let union - order of execution`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordOrderOfExecution.fs"|])>]
let ``Static let record - order of execution`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"

    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveDUs.fs"|])>]
let ``Static let - recursive DU definitions calling each other`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveRecords.fs"|])>]
let ``Static let - recursive record definitions calling each other`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"QuotationsForStaticLetUnions.fs"|])>]
let ``Static let - quotations support for unions`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Let (s, Value ("A"), Call (None, ofString, [s]))"""
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"QuotationsForStaticLetRecords.fs"|])>]
let ``Static let - quotations support for records`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetInGenericUnion.fs"|])>]
let ``Static let union - executes per generic struct typar`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetInGenericRecords.fs"|])>]
let ``Static let record - executes per generic struct typar`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "TODO put anything meaningful here"