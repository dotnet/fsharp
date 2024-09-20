// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System
open System.IO

// reportTime uses global state.
[<Collection(nameof DoNotRunInParallel)>]
module times =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
    let ``times - error_01_fs - --Times`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--Times"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--Times'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_02.fs"|])>]
    let ``times - error_02_fs - --times-`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--times-"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--times-'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_03.fs"|])>]
    let ``times - error_03_fs - --times+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--times+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--times\+'"
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
    let ``times - to console`` compilation =
        compilation
        |> asFsx
        |> withBufferWidth 120
        |> withOptions ["--times"]
        |> ignoreWarnings
        |> compile        
        |> verifyOutputContains [|
            "Parse inputs"
            "Typecheck"
            "GC0"
            "Duration"|]
        |> ignore


    [<Theory(Skip="Flaky in CI due to file being locked, disabling for now until file closure is resolved."); Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
    let ``times - to csv file`` compilation =
        let tempPath = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString() + ".csv")
        use _ = {new IDisposable with
                     member this.Dispose() = File.Delete(tempPath) }

        compilation
        |> asFsx
        |> withOptions ["--times:"+tempPath]
        |> ignoreWarnings     
        |> compile
        |> shouldSucceed
        |> ignore<CompilationResult>
        
        let csvContents = File.ReadAllLines(tempPath)

        Assert.Contains("Name,StartTime,EndTime,Duration(s),Id,ParentId,RootId",csvContents[0])
        Assert.Contains(csvContents, fun row -> row.Contains("Typecheck"))
        Assert.Contains(csvContents, fun row -> row.Contains("Parse inputs"))
