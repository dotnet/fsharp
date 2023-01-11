// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Miscellaneous.FsharpSuiteMigrated

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
        scriptingEnv.Eval """let exit (code:int) = if code=0 then () else failwith $"Script called function 'exit' with code={code}" """ |> ignore
        scriptingEnv

    //let private getOrCreateEngine = createEngine//Tables.memoize createEngine

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
    type ExecutionMode = 
        | FSC_DEBUG 
        | FSC_OPTIMIZED 
        | FSI
        | COMPILED_EXE_APP

    let baseFolder = Path.Combine(__SOURCE_DIRECTORY__,"..","..","fsharp") |> Path.GetFullPath
    let inline testConfig (relativeFolder:string) = relativeFolder    

    let adjustVersion version bonusArgs = 
        match version with 
        | LangVersion.V47 -> "4.7",bonusArgs
        | LangVersion.V50 -> "5.0",bonusArgs
        | LangVersion.Preview -> "preview",bonusArgs
        | LangVersion.Latest  -> "latest", bonusArgs
        | LangVersion.SupportsMl -> "5.0",  "--mlcompatibility" :: bonusArgs       

    let supportedNames = set ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]

    let singleTestBuildAndRunAuxVersion (folder:string) bonusArgs mode langVersion = 
        let absFolder = Path.Combine(baseFolder,folder)
     
        let files = Directory.GetFiles(absFolder,"test*.fs*")
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
        |> ignoreWarnings
        |> withOptions (["--nowarn:0988;3370"] @ bonusArgs)    
        |> fun cu -> 
            match mode with
            | FSC_DEBUG -> cu |> withDebug |> withNoOptimize  |> ScriptRunner.runScriptFile langVersion
            | FSC_OPTIMIZED -> cu |> withOptimize |> withNoDebug |> ScriptRunner.runScriptFile langVersion
            | FSI -> cu |> ScriptRunner.runScriptFile langVersion    
            | COMPILED_EXE_APP -> cu |> withDefines ("TESTS_AS_APP" :: ScriptRunner.defaultDefines) |> compileExeAndRun
        |> shouldSucceed 
        |> ignore<CompilationResult>
    

    let singleTestBuildAndRunAux folder bonusArgs mode = singleTestBuildAndRunAuxVersion folder bonusArgs mode LangVersion.Latest
    let singleTestBuildAndRunVersion folder mode version = singleTestBuildAndRunAuxVersion folder [] mode version
    let singleTestBuildAndRun folder mode = singleTestBuildAndRunVersion folder mode LangVersion.Latest

    let singleVersionedNegTestAux (_folder:string) _bonusArgs (_version:LangVersion) (_testName:string) = ()
    let singleVersionedNegTest (folder:string)  (version:LangVersion) (testName:string) = 
        singleVersionedNegTestAux folder [] version testName



module CoreTests =
    open TestFrameworkAdapter
      

    // These tests are enabled for .NET Framework and .NET Core
    [<Fact>]
    let ``access-FSC_DEBUG``() = singleTestBuildAndRun "core/access" FSC_DEBUG

    [<Fact>]
    let ``access-FSC_OPTIMIZED``() = singleTestBuildAndRun "core/access" FSC_OPTIMIZED

    [<Fact>]
    let ``access-FSI``() = singleTestBuildAndRun "core/access" FSI

    [<Fact>]
    let ``apporder-FSC_DEBUG`` () = singleTestBuildAndRun "core/apporder" FSC_DEBUG

    [<Fact>]
    let ``apporder-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/apporder" FSC_OPTIMIZED

    [<Fact>]
    let ``apporder-FSI`` () = singleTestBuildAndRun "core/apporder" FSI

    [<Fact>]
    let ``array-FSC_DEBUG-5.0`` () = singleTestBuildAndRunVersion "core/array" FSC_DEBUG LangVersion.V50

    [<Fact>]
    let ``array-FSC_OPTIMIZED-5.0`` () = singleTestBuildAndRunVersion "core/array" FSC_OPTIMIZED LangVersion.V50

    [<Fact>]
    let ``array-FSI-5.0`` () = singleTestBuildAndRunVersion "core/array" FSI LangVersion.V50

    [<Fact>]
    let ``array-FSC_OPTIMIZED-preview`` () = singleTestBuildAndRunVersion "core/array" FSC_OPTIMIZED LangVersion.Preview

    [<Fact>]
    let ``array-no-dot-FSC_DEBUG`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSC_DEBUG LangVersion.Preview

    [<Fact>]
    let ``array-no-dot-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSC_OPTIMIZED LangVersion.Preview

    [<Fact>]
    let ``array-no-dot-FSI`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSI LangVersion.Preview

    [<Fact>]
    let ``array-no-dot-warnings-langversion-default`` () =
        let cfg = testConfig "core/array-no-dot-warnings"
        singleVersionedNegTest cfg LangVersion.Latest "test-langversion-default"

    [<Fact>]
    let ``array-no-dot-warnings-langversion-5_0`` () =
        let cfg = testConfig "core/array-no-dot-warnings"
        singleVersionedNegTest cfg LangVersion.V50 "test-langversion-5.0"

    [<Fact>]
    let ``ref-ops-deprecation-langversion-preview`` () =
        let cfg = testConfig "core/ref-ops-deprecation"
        singleVersionedNegTest cfg LangVersion.Preview "test-langversion-preview"

    [<Fact>]
    let ``auto-widen-version-5_0``() = 
        let cfg = testConfig "core/auto-widen/5.0"
        singleVersionedNegTest cfg LangVersion.V50 "test"

    [<Fact>]
    let ``auto-widen-version-FSC_DEBUG-preview``() =
        singleTestBuildAndRunVersion "core/auto-widen/preview" FSC_DEBUG LangVersion.Preview

    [<Fact>]
    let ``auto-widen-version-FSC_OPTIMIZED-preview``() =
        singleTestBuildAndRunVersion "core/auto-widen/preview" FSC_OPTIMIZED LangVersion.Preview

    [<Fact>]
    let ``auto-widen-version-preview-warns-on``() = 
        let cfg = testConfig "core/auto-widen/preview"   
        singleVersionedNegTestAux cfg " --warnon:3388 --warnon:3389 --warnon:3395 --warnaserror+ --define:NEGATIVE" LangVersion.Preview "test"

    [<Fact>]
    let ``auto-widen-version-preview-default-warns``() = 
        let cfg = testConfig "core/auto-widen/preview-default-warns"  
        singleVersionedNegTestAux cfg " --warnon:3388 --warnon:3389 --warnon:3395 --warnaserror+ --define:NEGATIVE" LangVersion.Preview "test"

    [<Fact>]
    let ``comprehensions-FSC_DEBUG`` () = singleTestBuildAndRun "core/comprehensions" FSC_DEBUG

    [<Fact>]
    let ``comprehensions-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/comprehensions" FSC_OPTIMIZED

    [<Fact>]
    let ``comprehensions-FSI`` () = singleTestBuildAndRun "core/comprehensions" FSI

    [<Fact>]
    let ``comprehensionshw-FSC_DEBUG`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_DEBUG

    [<Fact>]
    let ``comprehensionshw-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_OPTIMIZED

    [<Fact>]
    let ``comprehensionshw-FSI`` () = singleTestBuildAndRun "core/comprehensions-hw" FSI

    [<Fact>]
    let ``genericmeasures-FSC_DEBUG`` () = singleTestBuildAndRun "core/genericmeasures" FSC_DEBUG

    [<Fact>]
    let ``genericmeasures-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/genericmeasures" FSC_OPTIMIZED

    [<Fact>]
    let ``genericmeasures-FSI`` () = singleTestBuildAndRun "core/genericmeasures" FSI

    [<Fact>]
    let ``innerpoly-FSC_DEBUG`` () = singleTestBuildAndRun "core/innerpoly"  FSC_DEBUG

    [<Fact>]
    let ``innerpoly-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/innerpoly" FSC_OPTIMIZED

    [<Fact>]
    let ``innerpoly-FSI`` () = singleTestBuildAndRun "core/innerpoly" FSI

    [<Fact>]
    let ``namespaceAttributes-FSC_DEBUG`` () = singleTestBuildAndRun "core/namespaces" COMPILED_EXE_APP

    [<Fact>]
    let ``namespaceAttributes-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/namespaces" COMPILED_EXE_APP

    [<Fact>]
    let ``unicode2-FSC_DEBUG`` () = singleTestBuildAndRun "core/unicode" FSC_DEBUG // TODO: fails on coreclr

    [<Fact>]
    let ``unicode2-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/unicode" FSC_OPTIMIZED // TODO: fails on coreclr

    [<Fact>]
    let ``unicode2-FSI`` () = singleTestBuildAndRun "core/unicode" FSI

    [<Fact>]
    let ``lazy test-FSC_DEBUG`` () = singleTestBuildAndRun "core/lazy" FSC_DEBUG

    [<Fact>]
    let ``lazy test-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/lazy" FSC_OPTIMIZED

    [<Fact>]
    let ``lazy test-FSI`` () = singleTestBuildAndRun "core/lazy" FSI

    [<Fact>]
    let ``letrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec" FSC_DEBUG

    [<Fact>]
    let ``letrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec" FSC_OPTIMIZED

    [<Fact>]
    let ``letrec-FSI`` () = singleTestBuildAndRun "core/letrec" FSI

    [<Fact>]
    let ``letrec (mutrec variations part one) FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_DEBUG

    [<Fact>]
    let ``letrec (mutrec variations part one) FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_OPTIMIZED

    [<Fact>]
    let ``letrec (mutrec variations part one) FSI`` () = singleTestBuildAndRun "core/letrec-mutrec" FSI

    [<Fact>]
    let ``libtest-FSC_DEBUG`` () = singleTestBuildAndRun "core/libtest" FSC_DEBUG

    [<Fact>]
    let ``libtest-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/libtest" FSC_OPTIMIZED

    [<Fact>]
    let ``libtest-FSI`` () = singleTestBuildAndRun "core/libtest" FSI

    [<Fact>]
    let ``lift-FSC_DEBUG`` () = singleTestBuildAndRun "core/lift" FSC_DEBUG

    [<Fact>]
    let ``lift-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/lift" FSC_OPTIMIZED

    [<Fact>]
    let ``lift-FSI`` () = singleTestBuildAndRun "core/lift" FSI

    [<Fact>]
    let ``map-FSC_DEBUG`` () = singleTestBuildAndRun "core/map" FSC_DEBUG

    [<Fact>]
    let ``map-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/map" FSC_OPTIMIZED

    [<Fact>]
    let ``map-FSI`` () = singleTestBuildAndRun "core/map" FSI

    [<Fact>]
    let ``measures-FSC_DEBUG`` () = singleTestBuildAndRun "core/measures" FSC_DEBUG

    [<Fact>]
    let ``measures-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/measures" FSC_OPTIMIZED

    [<Fact>]
    let ``measures-FSI`` () = singleTestBuildAndRun "core/measures" FSI

    [<Fact>]
    let ``nested-FSC_DEBUG`` () = singleTestBuildAndRun "core/nested" FSC_DEBUG

    [<Fact>]
    let ``nested-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/nested" FSC_OPTIMIZED

    [<Fact>]
    let ``nested-FSI`` () = singleTestBuildAndRun "core/nested" FSI

    [<Fact>]
    let ``members-ops-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ops" FSC_OPTIMIZED

    [<Fact>]
    let ``members-ops-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ops" FSC_DEBUG

    [<Fact>]
    let ``members-ops-FSI`` () = singleTestBuildAndRun "core/members/ops" FSI

    [<Fact>]
    let ``members-ops-mutrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_DEBUG

    [<Fact>]
    let ``members-ops-mutrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_OPTIMIZED

    [<Fact>]
    let ``members-ops-mutrec-FSI`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSI

    [<Fact>]
    let ``seq-FSC_DEBUG`` () = singleTestBuildAndRun "core/seq" FSC_DEBUG

    [<Fact>]
    let ``seq-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/seq" FSC_OPTIMIZED

    [<Fact>]
    let ``seq-FSI`` () = singleTestBuildAndRun "core/seq" FSI

    [<Fact>]
    let ``math-numbers-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/math/numbers" FSC_OPTIMIZED

    [<Fact>]
    let ``math-numbers-FSI`` () = singleTestBuildAndRun "core/math/numbers" FSI

    [<Fact>]
    let ``members-ctree-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ctree" FSC_DEBUG

    [<Fact>]
    let ``members-ctree-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ctree" FSC_OPTIMIZED

    [<Fact>]
    let ``members-ctree-FSI`` () = singleTestBuildAndRun "core/members/ctree" FSI

    [<Fact>]
    let ``members-factors-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/factors" FSC_DEBUG

    [<Fact>]
    let ``members-factors-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/factors" FSC_OPTIMIZED

    [<Fact>]
    let ``members-factors-FSI`` () = singleTestBuildAndRun "core/members/factors" FSI

    [<Fact>]
    let ``members-factors-mutrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_DEBUG

    [<Fact>]
    let ``members-factors-mutrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_OPTIMIZED

    [<Fact>]
    let ``members-factors-mutrec-FSI`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSI

    [<Fact>]
    let ``graph-FSC_DEBUG`` () = singleTestBuildAndRunVersion "perf/graph" FSC_DEBUG LangVersion.SupportsMl

    [<Fact>]
    let ``graph-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "perf/graph" FSC_OPTIMIZED LangVersion.SupportsMl

    [<Fact>]
    let ``graph-FSI`` () = singleTestBuildAndRunVersion "perf/graph" FSI LangVersion.SupportsMl

    [<Fact>]
    let ``nbody-FSC_DEBUG`` () = singleTestBuildAndRunVersion "perf/nbody" FSC_DEBUG LangVersion.SupportsMl

    [<Fact>]
    let ``nbody-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "perf/nbody" FSC_OPTIMIZED LangVersion.SupportsMl

    [<Fact>]
    let ``nbody-FSI`` () = singleTestBuildAndRunVersion "perf/nbody" FSI LangVersion.SupportsMl

    [<Fact>]
    let ``forexpression-FSC_DEBUG`` () = singleTestBuildAndRun "core/forexpression" FSC_DEBUG

    [<Fact>]
    let ``forexpression-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/forexpression" FSC_OPTIMIZED

    [<Fact>]
    let ``forexpression-FSI`` () = singleTestBuildAndRun "core/forexpression" FSI

    [<Fact>]
    let ``letrec (mutrec variations part two) FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_DEBUG

    [<Fact>]
    let ``letrec (mutrec variations part two) FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_OPTIMIZED

    [<Fact>]
    let ``letrec (mutrec variations part two) FSI`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSI

    [<Fact>]
    let ``printf`` () = singleTestBuildAndRunVersion "core/printf" FSC_OPTIMIZED LangVersion.Preview

    [<Fact>]
    let ``printf-interpolated`` () = singleTestBuildAndRunVersion "core/printf-interpolated" FSC_OPTIMIZED LangVersion.Preview

    [<Fact>]
    let ``tlr-FSC_DEBUG`` () = singleTestBuildAndRun "core/tlr" FSC_DEBUG

    [<Fact>]
    let ``tlr-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/tlr" FSC_OPTIMIZED

    [<Fact>]
    let ``tlr-FSI`` () = singleTestBuildAndRun "core/tlr" FSI

    [<Fact>]
    let ``subtype-FSC_DEBUG`` () = singleTestBuildAndRun "core/subtype" FSC_DEBUG

    [<Fact>]
    let ``subtype-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/subtype" FSC_OPTIMIZED

    [<Fact>]
    let ``subtype-FSI`` () = singleTestBuildAndRun "core/subtype" FSI

    [<Fact>]
    let ``syntax-FSC_DEBUG`` () = singleTestBuildAndRun "core/syntax" FSC_DEBUG

    [<Fact>]
    let ``syntax-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/syntax" FSC_OPTIMIZED

    [<Fact>]
    let ``syntax-FSI`` () = singleTestBuildAndRun "core/syntax" FSI

    [<Fact>]
    let ``test int32-FSC_DEBUG`` () = singleTestBuildAndRun "core/int32" FSC_DEBUG

    [<Fact>]
    let ``test int32-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/int32" FSC_OPTIMIZED

    [<Fact>]
    let ``test int32-FSI`` () = singleTestBuildAndRun "core/int32" FSI

    // This test stays in FsharpSuite for a later migration phases, it uses hardcoded #r to a C# compiled cslib.dll inside
#if NETCOREAPP
    [<Fact>]
    let ``quotes-FSC-FSC_DEBUG`` () = singleTestBuildAndRun "core/quotes" FSC_DEBUG

    [<Fact>]
    let ``quotes-FSC-BASIC`` () = singleTestBuildAndRun "core/quotes" FSC_OPTIMIZED

    [<Fact>]
    let ``quotes-FSI-BASIC`` () = singleTestBuildAndRun "core/quotes" FSI
#endif

    [<Fact>]
    let ``recordResolution-FSC_DEBUG`` () = singleTestBuildAndRun "core/recordResolution" FSC_DEBUG

    [<Fact>]
    let ``recordResolution-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/recordResolution" FSC_OPTIMIZED

    [<Fact>]
    let ``recordResolution-FSI`` () = singleTestBuildAndRun "core/recordResolution" FSI

#if NETCOREAPP
// This test has hardcoded expectations about current synchronization context
// Will be moved out of FsharpSuite.Tests in a later phase
    [<Fact>]
    let ``control-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/control" FSC_OPTIMIZED

    [<Fact>]
    let ``control-FSI`` () = singleTestBuildAndRun "core/control" FSI

    [<Fact>]
    let ``control --tailcalls`` () =
        let cfg = testConfig "core/control"
        singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED
#endif

    [<Fact>]
    let ``controlChamenos-FSC_OPTIMIZED`` () =
        let cfg = testConfig "core/controlChamenos"
        singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED

    [<Fact>]
    let ``controlChamenos-FSI`` () =
        let cfg = testConfig "core/controlChamenos"
        singleTestBuildAndRunAux cfg  ["--tailcalls"] FSI

    [<Fact>]
    let ``controlMailbox-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/controlMailbox" FSC_OPTIMIZED

    [<Fact>]
    let ``controlMailbox-FSI`` () = singleTestBuildAndRun "core/controlMailbox" FSI

    [<Fact>]
    let ``controlMailbox --tailcalls`` () =
        let cfg = testConfig "core/controlMailbox"
        singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED

    [<Fact>]
    let ``csext-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/csext" FSC_OPTIMIZED

    [<Fact>]
    let ``csext-FSI`` () = singleTestBuildAndRun "core/csext" FSI

    [<Fact>]
    let ``enum-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/enum" FSC_OPTIMIZED

    [<Fact>]
    let ``enum-FSI`` () = singleTestBuildAndRun "core/enum" FSI

    [<Fact>]
    let ``longnames-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/longnames" FSC_OPTIMIZED

    [<Fact>]
    let ``longnames-FSI`` () = singleTestBuildAndRun "core/longnames" FSI

    [<Fact>]
    let ``math-numbersVS2008-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_OPTIMIZED

    [<Fact>]
    let ``math-numbersVS2008-FSI`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSI

    [<Fact>]
    let ``patterns-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "core/patterns" FSC_OPTIMIZED LangVersion.Preview

// This requires --multiemit on by default, which is not the case for .NET Framework
#if NETCOREAPP
    [<Fact>]
    let ``patterns-FSI`` () = singleTestBuildAndRun "core/patterns" FSI
#endif

    [<Fact>]
    let ``pinvoke-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/pinvoke" FSC_OPTIMIZED

    [<Fact>]
    let ``pinvoke-FSI`` () =
        singleTestBuildAndRun "core/pinvoke" FSI

    [<Fact>]
    let ``fsi_load-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/fsi-load" FSC_OPTIMIZED

    [<Fact>]
    let ``fsi_load-FSI`` () = singleTestBuildAndRun "core/fsi-load" FSI

    [<Fact>]
    let ``reflect-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/reflect" FSC_OPTIMIZED

    [<Fact>]
    let ``reflect-FSI`` () = singleTestBuildAndRun "core/reflect" FSI