module EmittedIL.TryCatch

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler


let setupCompilation compilation = 
    compilation
    |> withOptions [ "--test:EmitFeeFeeAs100001" ]
    |> withNoWarn 75 //The command-line option '--generate-filter-blocks' has been deprecated
    |> withNoWarn 52 //The value has been copied to ensure the original is not mutated
    |> asExe
    |> withNoOptimize
    |> withNoInterfaceData
    |> withNoOptimizationData
    |> withNoDebug
    |> ignoreWarnings

let verifyCompilation compilation =
    setupCompilation compilation
    |> verifyILBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternRecoverableException.fs"|])>]
let ``TryCatch with active pattern`` compilation =
    compilation
    |> verifyCompilation

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternRecoverableException.fs"|],BaselineSuffix = ".generateFilterBlocks")>]
let ``TryCatch with active pattern and filter blocks switch`` compilation =
    compilation
    |> withOptions ["--generate-filter-blocks"]
    |> verifyCompilation

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TryWithExplicitGuard.fs"|])>]
let ``TryCatch with explicit guard`` compilation =
    compilation
    |> verifyCompilation

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TryWithExplicitGuard.fs"|],BaselineSuffix = ".generateFilterBlocks")>]
let ``TryCatch with explicit guard and filter blocks switch`` compilation =
    compilation
    |> withOptions ["--generate-filter-blocks"]
    |> verifyCompilation
        
        
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StackOverflowRepro.fs"|])>]
let ``Stackoverflow reproduction`` compilation =
    let compilationResult = 
        compilation
        |> setupCompilation
        // I cannot just `compileAndRun` this in process now, because it will crash entire test host.
        |> compile
    match compilationResult with
    | CompilationResult.Success ({OutputPath = Some dllFile} as s) -> 
       let fsharpCoreFile = typeof<voption<_>>.Assembly.Location
       File.Copy(fsharpCoreFile, Path.Combine(Path.GetDirectoryName(dllFile), Path.GetFileName(fsharpCoreFile)), true)
       let exitCode, stdout, stderr =  CompilerAssert.ExecuteAndReturnResult (dllFile, isFsx=false, deps = s.Dependencies, newProcess=true)

       let expectedStrings,expectedCode =
#if NETCOREAPP
            [|"Stack overflow";"at StackOverflowRepro.viaActivePattern(Int32)"|],-1073741571
#else
            [|"Process is terminated due to StackOverflowException"|],-2147023895
#endif


       for s in expectedStrings do
           Assert.Contains(s,stderr)

       Assert.Equal("",stdout)
       exitCode |> Assert.shouldBe expectedCode

    | _ -> failwith (sprintf "%A" compilationResult)

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StackOverflowRepro.fs"|])>]
let ``Stackoverflow prevention`` compilation =
    compilation
    |> setupCompilation
    |> withOptions ["--generate-filter-blocks"]
    |> compileAndRun
    |> shouldSucceed
    |> verifyOutput "System.OperationCanceledException"

   