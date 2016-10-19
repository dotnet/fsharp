module ``FSharp-Tests-TypeProviders``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let buildTypeProviderTest (cfg: TestConfig) p = attempt {

    rm cfg "provided.dll"

    // "%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
    do! fsc cfg "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "providedJ.dll"

    // "%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
    do! fsc cfg "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    rm cfg "providedK.dll"

    // "%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
    do! fsc cfg "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    rm cfg "provider.dll"

    // "%FSC%" --out:provider.dll -a provider.fsx
    do! fsc cfg "--out:provider.dll -a" ["provider.fsx"]

    // call %~d0%~p0..\single-test-build.bat
    do! SingleTest.singleTestBuild cfg p ()

    }

[<Test; FSharpSuiteTest("typeProviders/diamondAssembly")>]
let diamondAssembly () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    rm cfg "provider.dll"

    // "%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    // "%FSC%" --out:provider.dll -a ..\helloWorld\provider.fsx
    do! fsc cfg "%s" "--out:provider.dll -a" [".."/"helloWorld"/"provider.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test1.dll -a test1.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test1.dll -a" cfg.fsc_flags ["test1.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll test2a.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll" cfg.fsc_flags ["test2a.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll test2b.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll" cfg.fsc_flags ["test2b.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll test3.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll" cfg.fsc_flags ["test3.fsx"]

    do! peverify cfg "test1.dll"

    do! peverify cfg "test2a.dll"

    do! peverify cfg "test2b.dll"

    do! peverify cfg "test3.exe"

    do! exec cfg ("."/"test3.exe") ""

    use testOkFile = fileguard cfg "test.ok"

    // "%FSI%" %fsi_flags% test3.fsx && (
    do! fsi cfg "%s" cfg.fsi_flags ["test3.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })



[<Test; FSharpSuiteTest("typeProviders/globalNamespace")>]
let globalNamespace () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // %CSC% /out:globalNamespaceTP.dll /debug+ /target:library /r:"%FSCOREDLLPATH%" globalNamespaceTP.cs
    do! csc cfg """/out:globalNamespaceTP.dll /debug+ /target:library /r:"%s" """ cfg.FSCOREDLLPATH ["globalNamespaceTP.cs"]

    // "%FSC%" %fsc_flags% /debug+ /r:globalNamespaceTP.dll /optimize- test.fsx
    do! fsc cfg "%s /debug+ /r:globalNamespaceTP.dll /optimize-" cfg.fsc_flags ["test.fsx"]
                
    })


[<Test; FSharpSuiteScriptPermutations("typeProviders/helloWorld")>]
let helloWorld p = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    rm cfg "provided.dll"

    rm cfg "provided1.dll"

    //"%FSC%" --out:provided1.dll -g -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided1.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "provided2.dll"

    //"%FSC%" --out:provided2.dll -g -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided2.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "provided3.dll"

    //"%FSC%" --out:provided3.dll -g -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided3.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "provided4.dll"

    //"%FSC%" --out:provided4.dll -g -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided4.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "providedJ.dll"

    //"%FSC%" --out:providedJ.dll -g -a ..\helloWorld\providedJ.fs
    do! fsc cfg "%s" "--out:providedJ.dll -g -a" [".."/"helloWorld"/"providedJ.fs"]

    rm cfg "providedK.dll"

    //"%FSC%" --out:providedK.dll -g -a ..\helloWorld\providedK.fs
    do! fsc cfg "%s" "--out:providedK.dll -g -a" [".."/"helloWorld"/"providedK.fs"]

    //"%FSC%" --out:providedNullAssemblyName.dll -g -a ..\helloWorld\providedNullAssemblyName.fsx
    do! fsc cfg "%s" "--out:providedNullAssemblyName.dll -g -a" [".."/"helloWorld"/"providedNullAssemblyName.fsx"]

    //call %~d0%~p0\..\build-typeprovider-test.bat
    do! buildTypeProviderTest cfg p

    rm cfg "provider_with_binary_compat_changes.dll"

    //mkdir bincompat1
    mkdir cfg "bincompat1"

    //pushd bincompat1
    log "pushd bincompat1"
    let bincompat1 = getfullpath cfg "bincompat1"

    //xcopy /y ..\*.dll .
    Directory.EnumerateFiles(bincompat1/"..", "*.dll")
    |> Seq.iter (fun from -> Commands.copy_y bincompat1 from ("."/Path.GetFileName(from)) |> ignore)

    //"%FSC%" -g -a -o:test_lib.dll -r:provider.dll ..\test.fsx
    do! fscIn cfg bincompat1 "%s" "-g -a -o:test_lib.dll -r:provider.dll" [".."/"test.fsx"]

    //"%FSC%" -r:test_lib.dll -r:provider.dll ..\testlib_client.fsx
    do! fscIn cfg bincompat1 "%s" "-r:test_lib.dll -r:provider.dll" [".."/"testlib_client.fsx"]

    //popd
    log "popd"

    //mkdir bincompat2
    mkdir cfg "bincompat2"
        
    //pushd bincompat2
    log "pushd bincompat2"
    let bincompat2 = getfullpath cfg "bincompat2"

    //xcopy /y ..\bincompat1\*.dll .
    Directory.EnumerateFiles(bincompat2/".."/"bincompat1", "*.dll")
    |> Seq.iter (fun from -> Commands.copy_y bincompat2 from ("."/Path.GetFileName(from)) |> ignore)


    //REM overwrite provider.dll
    //"%FSC%" --define:ADD_AN_OPTIONAL_STATIC_PARAMETER --define:USE_IMPLICIT_ITypeProvider2 --out:provider.dll -g -a ..\provider.fsx
    do! fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER --define:USE_IMPLICIT_ITypeProvider2 --out:provider.dll -g -a" [".."/"provider.fsx"]

    // "%FSC%" -g -a -o:test_lib_recompiled.dll -r:provider.dll ..\test.fsx
    do! fscIn cfg bincompat2 "-g -a -o:test_lib_recompiled.dll -r:provider.dll" [".."/"test.fsx"]

    //REM This is the important part of the binary compatibility part of the test: the new provider is being used, but 
    //REM with a binary that was generated w.r.t. the old provider. The new provider can still resolve the references
    //REM generated by the old provider which are stored in the F# metadata for test_lib.dll
    //"%FSC%" --define:ADD_AN_OPTIONAL_STATIC_PARAMETER -r:test_lib.dll -r:provider.dll ..\testlib_client.fsx
    do! fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER -r:test_lib.dll -r:provider.dll" [".."/"testlib_client.fsx"]

    //"%PEVERIFY%" provider.dll
    do! peverify cfg (bincompat2/"provider.dll")

    //"%PEVERIFY%" test_lib.dll
    do! peverify cfg (bincompat2/"test_lib.dll")

    // "%PEVERIFY%" test_lib_recompiled.dll
    do! peverify cfg (bincompat2/"test_lib_recompiled.dll")

    //"%PEVERIFY%" testlib_client.exe
    do! peverify cfg (bincompat2/"testlib_client.exe")

    do! SingleTest.singleTestRun cfg p
                
    })



[<Test; FSharpSuiteTest("typeProviders/helloWorldCSharp")>]
let helloWorldCSharp () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    rm cfg "magic.dll"

    // "%FSC%" --out:magic.dll -a magic.fs --keyfile:magic.snk
    do! fsc cfg "%s" "--out:magic.dll -a --keyfile:magic.snk" ["magic.fs "]

    rm cfg "provider.dll"

    // %CSC% /out:provider.dll /target:library "/r:%FSCOREDLLPATH%" /r:magic.dll provider.cs
    do! csc cfg """/out:provider.dll /target:library "/r:%s" /r:magic.dll""" cfg.FSCOREDLLPATH ["provider.cs"]

    // "%FSC%" %fsc_flags% /debug+ /r:provider.dll /optimize- test.fsx
    do! fsc cfg "%s /debug+ /r:provider.dll /optimize-" cfg.fsc_flags ["test.fsx"]

    // "%PEVERIFY%" magic.dll
    do! peverify cfg "magic.dll"

    // "%PEVERIFY%" provider.dll
    do! peverify cfg "provider.dll"

    // "%PEVERIFY%" test.exe
    do! peverify cfg "test.exe"

    // test.exe
    do! exec cfg ("."/"test.exe") ""
                
    })



module NegTests = 

    let testData () = 
        // set TESTS_SIMPLE=neg2h neg4 neg1 neg1_a neg2 neg2c neg2e neg2g neg6
        let testsSimple = 
            ["neg2h"; "neg4"; "neg1"; "neg1_a"; "neg2"; "neg2c"; "neg2e"; "neg2g"; "neg6"]
        // REM neg7 - excluded 
        // set TESTS_SIMPLE=%TESTS_SIMPLE% InvalidInvokerExpression providerAttributeErrorConsume ProviderAttribute_EmptyConsume
            @ ["InvalidInvokerExpression"; "providerAttributeErrorConsume"; "ProviderAttribute_EmptyConsume"]
        
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNestedNamespaces_Exception
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_NamespaceName_Exception
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_NamespaceName_Empty
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetTypes_Exception
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ResolveTypeName_Exception
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNamespaces_Exception
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParameters_Exception 
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetInvokerExpression_Exception 
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetTypes_Null
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ResolveTypeName_Null
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNamespaces_Null
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParameters_Null
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetInvokerExpression_Null
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_DoesNotHaveConstructor
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ConstructorThrows
        // set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments
        let testsWithDefine = [
            "EVIL_PROVIDER_GetNestedNamespaces_Exception";
            "EVIL_PROVIDER_NamespaceName_Exception";
            "EVIL_PROVIDER_NamespaceName_Empty";
            "EVIL_PROVIDER_GetTypes_Exception";
            "EVIL_PROVIDER_ResolveTypeName_Exception";
            "EVIL_PROVIDER_GetNamespaces_Exception";
            "EVIL_PROVIDER_GetStaticParameters_Exception";
            "EVIL_PROVIDER_GetInvokerExpression_Exception";
            "EVIL_PROVIDER_GetTypes_Null";
            "EVIL_PROVIDER_ResolveTypeName_Null";
            "EVIL_PROVIDER_GetNamespaces_Null";
            "EVIL_PROVIDER_GetStaticParameters_Null";
            "EVIL_PROVIDER_GetInvokerExpression_Null";
            "EVIL_PROVIDER_DoesNotHaveConstructor";
            "EVIL_PROVIDER_ConstructorThrows";
            "EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments" ]
        
        (testsSimple @ testsWithDefine)
        |> List.map (fun t -> FSharpSuiteTestCaseData("typeProviders/negTests", t))

    [<Test; TestCaseSource("testData")>]
    let negTests name = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()
        let dir = cfg.Directory

        do! requireENCulture ()

        let fileExists = Commands.fileExists dir >> Option.isSome

        rm cfg "provided.dll"

        // "%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
        do! fsc cfg "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

        rm cfg "providedJ.dll"

        // "%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
        do! fsc cfg "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

        rm cfg "providedK.dll"

        // "%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
        do! fsc cfg "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

        rm cfg "provider.dll"

        // "%FSC%" --out:provider.dll -a  provider.fsx
        do! fsc cfg "--out:provider.dll -a" ["provider.fsx"]

        // "%FSC%" --out:provider_providerAttributeErrorConsume.dll -a  providerAttributeError.fsx
        do! fsc cfg "--out:provider_providerAttributeErrorConsume.dll -a" ["providerAttributeError.fsx"]

        // "%FSC%" --out:provider_ProviderAttribute_EmptyConsume.dll -a  providerAttribute_Empty.fsx
        do! fsc cfg "--out:provider_ProviderAttribute_EmptyConsume.dll -a" ["providerAttribute_Empty.fsx"]

        rm cfg "helloWorldProvider.dll"

        // "%FSC%" --out:helloWorldProvider.dll -a  ..\helloWorld\provider.fsx
        do! fsc cfg "--out:helloWorldProvider.dll -a" [".."/"helloWorld"/"provider.fsx"]

        rm cfg "MostBasicProvider.dll"

        // "%FSC%" --out:MostBasicProvider.dll -a  MostBasicProvider.fsx
        do! fsc cfg "--out:MostBasicProvider.dll -a" ["MostBasicProvider.fsx"]

        //REVIEW use testfixture setup to run this code ---^ only once?

        // if "%1"=="" goto :RunAllTests
        // if "%1"=="--withDefine" goto :RunSpecificWithDefine
        // call :RunTest %1
        // goto :ReportResults
        ignore "is a parametrized test, like --withDefine"

        // :Preprocess
        let preprocess name pref = 
          attempt {
           let dirp = (dir |> Commands.pathAddBackslash)
           do
            File.ReadAllText(sprintf "%s%s.%sbslpp" dirp name pref)
               .Replace("<ASSEMBLY>", getfullpath cfg (sprintf "provider_%s.dll" name))
               .Replace("<URIPATH>",sprintf "file:///%s" dirp)
               |> fun txt -> File.WriteAllText(sprintf "%s%s.%sbsl" dirp name pref,txt)
          }

        // :RunTestWithDefine
        let runTestWithDefine = attempt {
            // "%FSC%" --define:%1 --out:provider_%1.dll -a  provider.fsx

            do! if name = "ProviderAttribute_EmptyConsume" || name = "providerAttributeErrorConsume" then Success ()
                else  fsc cfg "--define:%s --out:provider_%s.dll -a" name name ["provider.fsx"]

            // :RunTest
            // if EXIST %1.bslpp   call :Preprocess "%1" ""
            do! if fileExists (sprintf "%s.bslpp" name) then preprocess name ""
                else Success

            // if EXIST %1.vsbslpp call :Preprocess "%1" "vs"
            do! if fileExists (sprintf "%s.vsbslpp" name) then preprocess name "vs"
                else Success

            // :DoRunTest
            // call ..\..\single-neg-test.bat %1
            //let cfg2 = {cfg with fsc_flags = sprintf "%s -r:provider_%s.dll" cfg.fsc_flags name }
            do! SingleTest.singleNegTest cfg name

            }

        // :RunSpecificWithDefine
        // call :RunTestWithDefine %2
        do! runTestWithDefine
        // goto :ReportResults
        ignore "useless, checked already"

        // :RunAllTests
        // for %%T in (%TESTS_SIMPLE%) do call :RunTest %%T
        // for %%T in (%TESTS_WITH_DEFINE%) do call :RunTestWithDefine %%T
        ignore "is a parametrized test"

                
        })


[<Test; FSharpSuiteScriptPermutations("typeProviders/splitAssembly")>]
let splitAssembly p = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" --out:provider.dll -a provider.fs
    do! fsc cfg "--out:provider.dll -a" ["provider.fs"]

    // "%FSC%" --out:providerDesigner.dll -a providerDesigner.fsx
    do! fsc cfg "--out:providerDesigner.dll -a" ["providerDesigner.fsx"]

    do! SingleTest.singleTestBuild cfg p
        
    do! SingleTest.singleTestRun cfg p
    })



[<Test; FSharpSuiteTest("typeProviders/wedgeAssembly")>]
let wedgeAssembly () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    rm cfg "provider.dll"

    rm cfg "provided.dll"

    // "%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
    do! fsc cfg "%s" "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "providedJ.dll"

    // "%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
    do! fsc cfg "%s" "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    rm cfg "providedK.dll"

    // "%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
    do! fsc cfg "%s" "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    // "%FSC%" --out:provider.dll -a ..\helloWorld\provider.fsx
    do! fsc cfg "%s" "--out:provider.dll -a" [".."/"helloWorld"/"provider.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a.dll -a test2a.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a" cfg.fsc_flags ["test2a.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b.dll -a test2b.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a" cfg.fsc_flags ["test2b.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3.exe test3.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe" cfg.fsc_flags ["test3.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a test2a.fsi test2a.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a" cfg.fsc_flags ["test2a.fsi"; "test2a.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a test2b.fsi test2b.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a" cfg.fsc_flags ["test2b.fsi"; "test2b.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS test3.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS" cfg.fsc_flags ["test3.fsx"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a test2a-restricted.fsi test2a.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a" cfg.fsc_flags ["test2a-restricted.fsi"; "test2a.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a test2b-restricted.fsi test2b.fs
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a"cfg.fsc_flags ["test2b-restricted.fsi"; "test2b.fs"]

    // "%FSC%" %fsc_flags% --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED test3.fsx
    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED" cfg.fsc_flags ["test3.fsx"]

    // "%PEVERIFY%" test2a.dll
    do! peverify cfg "test2a.dll"

    // "%PEVERIFY%" test2b.dll
    do! peverify cfg "test2b.dll"

    // "%PEVERIFY%" test3.exe
    do! peverify cfg "test3.exe"

    // test3.exe
    do! exec cfg ("."/"test3.exe") ""

                
    })
