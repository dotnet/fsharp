// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System
open System.IO

module times =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--Times'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
    let ``times - error_01.fs - --Times`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--Times"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--Times'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--times-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_02.fs"|])>]
    let ``times - error_02.fs - --times-`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--times-"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--times-'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--times\+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_03.fs"|])>]
    let ``times - error_03.fs - --times+`` compilation =
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
        let oldConsole = Console.Out
        let sw = new StringWriter()
        Console.SetOut(sw)
        use _ = {new IDisposable with
                     member this.Dispose() = Console.SetOut(oldConsole) }

        compilation
        |> asFsx
        |> withOptions ["--times"]
        |> ignoreWarnings
        |> compile        
        |> shouldSucceed  
        |> ignore<CompilationResult>

        let consoleContents = sw.ToString()
        Assert.Contains("Parse inputs",consoleContents)
        Assert.Contains("Typecheck",consoleContents)
        Assert.Contains("GC0",consoleContents)
        Assert.Contains("Duration",consoleContents)


    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
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

