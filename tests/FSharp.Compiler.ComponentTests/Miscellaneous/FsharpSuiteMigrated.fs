// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Miscellaneous.FsharpSuiteMigrated

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers 



module ScriptRunner = 
    open Internal.Utilities.Library
    
    let private createEngine(args,version) = 
        let scriptingEnv = getSessionForEval args version
        scriptingEnv.Eval """
let errorStringWriter = new System.IO.StringWriter()        
let oldConsoleError = System.Console.Error
System.Console.SetError(errorStringWriter)
let exit (code:int) = 
    System.Console.SetError(oldConsoleError)
    if code=0 then 
        () 
    else failwith $"Script called function 'exit' with code={code} and collected in stderr: {errorStringWriter.ToString()}" """ |> ignore
        scriptingEnv

    let defaultDefines = 
        [ 
#if NETCOREAPP
          "NETCOREAPP"
#endif
        ]

    let runScriptFile version (cu:CompilationUnit)  =
        let cu  = cu |> withDefines defaultDefines
        match cu with 
        | FS fsSource ->
            File.Delete("test.ok")           
            let engine = createEngine (fsSource.Options |> Array.ofList,version)
            let res = evalScriptFromDiskInSharedSession engine cu
            match res with
            | CompilationResult.Failure _ -> res
            | CompilationResult.Success s -> 
                if File.Exists("test.ok") then
                    res
                else
                    failwith $"Results looked correct, but 'test.ok' file was not created. Result: %A{s}"       

        | _ -> failwith $"Compilation unit other than fsharp is not supported, cannot process %A{cu}"

/// This test file was created by porting over (slower) FsharpSuite.Tests
/// In order to minimize human error, the test definitions have been copy-pasted and this adapter provides implementations of the test functions
module TestFrameworkAdapter = 
    open FSharp.Test.Compiler.Assertions.TextBasedDiagnosticAsserts

    type ExecutionMode = 
        | FSC_DEBUG 
        | FSC_OPTIMIZED 
        | FSI
        | COMPILED_EXE_APP
        | NEG_TEST_BUILD of testName:string

    let baseFolder = Path.Combine(__SOURCE_DIRECTORY__,"..","..","fsharp") |> Path.GetFullPath      

    let diffNegativeBaseline (cr:CompilationUnit) absFolder testName _version  =
        let expectedFiles = Directory.GetFiles(absFolder, testName + ".*")
        let baselines = 
            [ for f in expectedFiles do
                match Path.GetExtension(f) with
                | ".bsl" -> cr, f
                | ".vsbsl" -> cr |> withOptions ["--test:ContinueAfterParseFailure"], f
                | _ -> () ]
        [ for compilationUnit,baseline in baselines do
            compilationUnit       
            |> typecheck
            |> withResultsMatchingFile baseline ]
        |> List.head
            

    let adjustVersion version bonusArgs = 
        match version with 
        | LangVersion.V47 -> "4.7",bonusArgs
        | LangVersion.V50 -> "5.0",bonusArgs
        | LangVersion.V60 -> "6.0",bonusArgs
        | LangVersion.V70 -> "7.0",bonusArgs
        | LangVersion.Preview -> "preview",bonusArgs
        | LangVersion.Latest  -> "latest", bonusArgs
        | LangVersion.SupportsMl -> "5.0",  "--mlcompatibility" :: bonusArgs       

    let supportedNames = set ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]

    let singleTestBuildAndRunAuxVersion (folder:string) bonusArgs mode langVersion = 
        let absFolder = Path.Combine(baseFolder,folder)
        let supportedNames, files = 
            match mode with 
            | NEG_TEST_BUILD testName -> 
                let nameSet = 
                    supportedNames
                        .Add(testName+".fsx")
                        .Add(testName+".fs")
                        .Add(testName+".fsi")
                        .Add(testName+"-pre.fs")
                let files = Directory.GetFiles(absFolder,"*.fs*") |> Array.filter(fun n -> nameSet.Contains(Path.GetFileName(n)))
                nameSet, files
            | _ -> supportedNames, Directory.GetFiles(absFolder,"test*.fs*")
     
        let mainFile,otherFiles = 
            match files.Length with
            | 0 -> Directory.GetFiles(absFolder,"*.ml") |> Array.exactlyOne, [||]
            | 1 -> files |> Array.exactlyOne, [||]
            | _ -> 
                let mainFile,dependencies = 
                    files 
                    |> Array.filter (fun n -> supportedNames.Contains(Path.GetFileName(n))) 
                     // Convention in older FsharpSuite: test2 goes last, longer names like testlib before test, .fsi before .fs on equal filenames
                    |> Array.sortBy (fun n -> n.Contains("test2"), -n.IndexOf('.'), n.EndsWith(".fsi") |> not)                  
                    |> Array.splitAt 1        
                 
                mainFile[0],dependencies

        let version,bonusArgs = adjustVersion langVersion bonusArgs

        FsFromPath mainFile
        |> withAdditionalSourceFiles [for f in otherFiles -> SourceFromPath f]
        |> withLangVersion version
        |> fun cu -> 
            match mode with
            | FSC_DEBUG | FSC_OPTIMIZED | FSI | COMPILED_EXE_APP -> 
                cu 
                |> ignoreWarnings 
                |> withOptions (["--nowarn:0988;3370"] @ bonusArgs) 
            | NEG_TEST_BUILD _ -> 
                cu |> 
                withOptions (["--vserrors";"--maxerrors:10000";"--warnaserror";"--warn:3";"--nowarn:20;21;1178;52"] @ bonusArgs) 
        |> fun cu -> 
            match mode with
            | FSC_DEBUG -> 
                cu 
                |> withDebug 
                |> withNoOptimize  
                |> ScriptRunner.runScriptFile langVersion 
                |> shouldSucceed 
            | FSC_OPTIMIZED -> 
                cu 
                |> withOptimize 
                |> withNoDebug 
                |> ScriptRunner.runScriptFile langVersion 
                |> shouldSucceed 
            | FSI -> 
                cu 
                |> ScriptRunner.runScriptFile langVersion 
                |> shouldSucceed 
            | COMPILED_EXE_APP -> 
                cu 
                |> withDefines ("TESTS_AS_APP" :: ScriptRunner.defaultDefines) 
                |> compileExeAndRun 
                |> shouldSucceed 
            | NEG_TEST_BUILD testName -> diffNegativeBaseline (cu |> withName mainFile) absFolder testName langVersion
            
        |> ignore<CompilationResult>
    

    let singleTestBuildAndRunAux folder bonusArgs mode = singleTestBuildAndRunAuxVersion folder bonusArgs mode LangVersion.Latest
    let singleTestBuildAndRunVersion folder mode version = singleTestBuildAndRunAuxVersion folder [] mode version
    let singleTestBuildAndRun folder mode = singleTestBuildAndRunVersion folder mode LangVersion.Latest

    let singleVersionedNegTestAux folder bonusArgs version testName =
        singleTestBuildAndRunAuxVersion folder bonusArgs  (NEG_TEST_BUILD testName) version
    let singleVersionedNegTest (folder:string)  (version:LangVersion) (testName:string) = 
        singleVersionedNegTestAux folder [] version testName
    let singleNegTest folder testName = singleVersionedNegTest folder LangVersion.Latest testName