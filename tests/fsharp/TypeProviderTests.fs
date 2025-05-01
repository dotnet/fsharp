// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#if INTERACTIVE
//#r @"../../release/net40/bin/FSharp.Compiler.dll"
#r @"../../packages/xunit.assert/2.9.0/lib/net6.0/xunit.assert.dll"
#load "../../src/scripts/scriptlib.fsx"
#load "../FSharp.Test.Utilities/TestFramework.fs"
#load "single-test.fs"
#else
module FSharp.Test.FSharpSuite.TypeProviderTests
#endif

open System
open System.IO
open System.Reflection
open Xunit
open TestFramework
open Scripting
open SingleTest

open FSharp.Compiler.IO

#if !NETCOREAPP
// All tests which do a manual invoke of the F# compiler are disabled

#if NETCOREAPP
// Use these lines if you want to test CoreCLR
let FSC_OPTIMIZED = FSC_NETCORE (true, false)
let FSI = FSI_NETCORE
#else
let FSC_OPTIMIZED = FSC_NETFX (true, false)
let FSI = FSI_NETFX
#endif

let copyHelloWorld cfgDirectory =
    for helloDir in DirectoryInfo(__SOURCE_DIRECTORY__ + "/typeProviders").GetDirectories("hello*") do
        DirectoryInfo(cfgDirectory + "\\..").CreateSubdirectory(helloDir.Name).FullName
        |> copyFilesToDest helloDir.FullName

[<Fact>]
let diamondAssembly () =
    let cfg = testConfig "typeProviders/diamondAssembly"

    copyHelloWorld cfg.Directory

    rm cfg "provider.dll"

    // Add a version flag to make this generate native resources. The native resources aren't important and
    // can be dropped when the provided.dll is linked but we need to tolerate generated DLLs that have them
    fsc cfg "%s" "--out:provided.dll -a --version:0.0.0.1" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "%s" "--out:provider.dll -a" [".." ++ "helloWorld" ++ "provider.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test1.dll -a" cfg.fsc_flags ["test1.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll" cfg.fsc_flags ["test2a.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll" cfg.fsc_flags ["test2b.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll" cfg.fsc_flags ["test3.fsx"]

    peverify cfg "test1.dll"

    peverify cfg "test2a.dll"

    peverify cfg "test2b.dll"

    peverify cfg "test3.exe"

    exec cfg ("." ++ "test3.exe") ""

    

    fsiCheckPassed cfg "%s" cfg.fsi_flags ["test3.fsx"]

[<Fact>]
let globalNamespace () =
    let cfg = testConfig "typeProviders/globalNamespace"

    csc cfg """/out:globalNamespaceTP.dll /debug+ /target:library /r:netstandard.dll /r:"%s" """ cfg.FSCOREDLLPATH ["globalNamespaceTP.cs"]

    fsc cfg "%s /debug+ /r:globalNamespaceTP.dll /optimize-" cfg.fsc_flags ["test.fsx"]

let helloWorld p =
    let cfg = testConfig "typeProviders/helloWorld"

    fsc cfg "%s" "--out:provided1.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "%s" "--out:provided2.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "%s" "--out:provided3.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "%s" "--out:provided4.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "%s" "--out:providedJ.dll -g -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

    fsc cfg "%s" "--out:providedK.dll -g -a" [".." ++ "helloWorld" ++ "providedK.fs"]

    fsc cfg "%s" "--out:providedNullAssemblyName.dll -g -a" [".." ++ "helloWorld" ++ "providedNullAssemblyName.fsx"]

    fsc cfg "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

    fsc cfg "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

    fsc cfg "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

    fsc cfg "--out:provider.dll -a" ["provider.fsx"]

    SingleTest.singleTestBuildAndRunAux cfg p


    rm cfg "provider_with_binary_compat_changes.dll"

    mkdir cfg "bincompat1"

    log "pushd bincompat1"
    let bincompat1 = getfullpath cfg "bincompat1"

    Directory.EnumerateFiles(bincompat1 ++ "..", "*.dll")
    |> Seq.iter (fun from -> Commands.copy bincompat1 from ("." ++ Path.GetFileName(from)) |> ignore)

    fscIn cfg bincompat1 "%s" "-g -a -o:test_lib.dll -r:provider.dll" [".." ++ "test.fsx"]

    fscIn cfg bincompat1 "%s" "-r:test_lib.dll -r:provider.dll" [".." ++ "testlib_client.fsx"]

    log "popd"

    mkdir cfg "bincompat2"

    log "pushd bincompat2"
    let bincompat2 = getfullpath cfg "bincompat2"

    for fromFile in Directory.EnumerateFiles(bincompat2 ++ ".." ++ "bincompat1", "*.dll") do
        Commands.copy bincompat2 fromFile ("." ++ Path.GetFileName(fromFile)) |> ignore

    fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER --define:USE_IMPLICIT_ITypeProvider2 --out:provider.dll -g -a" [".." ++ "provider.fsx"]

    fscIn cfg bincompat2 "-g -a -o:test_lib_recompiled.dll -r:provider.dll" [".." ++ "test.fsx"]

    fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER -r:test_lib.dll -r:provider.dll" [".." ++ "testlib_client.fsx"]

    peverify cfg (bincompat2 ++ "provider.dll")

    peverify cfg (bincompat2 ++ "test_lib.dll")

    peverify cfg (bincompat2 ++ "test_lib_recompiled.dll")

    peverify cfg (bincompat2 ++ "testlib_client.exe")

[<Fact>]
let ``helloWorld fsc`` () = helloWorld FSC_OPTIMIZED

#if !NETCOREAPP
[<Fact>]
let ``helloWorld fsi`` () = helloWorld FSI_NETFX_STDIN
#endif

[<Fact>]
let helloWorldCSharp () =
    let cfg = testConfig "typeProviders/helloWorldCSharp"

    rm cfg "magic.dll"

    fsc cfg "%s" "--out:magic.dll -a --keyfile:magic.snk" ["magic.fs "]

    rm cfg "provider.dll"

    csc cfg """/out:provider.dll /target:library "/r:%s" /r:netstandard.dll /r:magic.dll""" cfg.FSCOREDLLPATH ["provider.cs"]

    fsc cfg "%s /debug+ /r:provider.dll /optimize-" cfg.fsc_flags ["test.fsx"]

    peverify cfg "magic.dll"

    peverify cfg "provider.dll"

    peverify cfg "test.exe"

    exec cfg ("." ++ "test.exe") ""

let singleNegTest name =
    let cfg = testConfig "typeProviders/negTests"

    copyHelloWorld cfg.Directory

    if requireENCulture () then

        let fileExists = Commands.fileExists cfg.Directory >> Option.isSome

        rm cfg "provided.dll"

        fsc cfg "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

        rm cfg "providedJ.dll"

        fsc cfg "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

        rm cfg "providedK.dll"

        fsc cfg "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

        rm cfg "provider.dll"

        fsc cfg "--out:provider.dll -g --optimize- -a" ["provider.fsx"]

        fsc cfg "--out:provider_providerAttributeErrorConsume.dll -a" ["providerAttributeError.fsx"]

        fsc cfg "--out:provider_ProviderAttribute_EmptyConsume.dll -a" ["providerAttribute_Empty.fsx"]

        rm cfg "helloWorldProvider.dll"

        fsc cfg "--out:helloWorldProvider.dll -g --optimize- -a" [".." ++ "helloWorld" ++ "provider.fsx"]

        rm cfg "MostBasicProvider.dll"

        fsc cfg "--out:MostBasicProvider.dll -g --optimize- -a" ["MostBasicProvider.fsx"]

        let preprocess name pref =
            let dirp = (cfg.Directory |> Commands.pathAddBackslash)
            do
            FileSystem.OpenFileForReadShim(sprintf "%s%s.%sbslpp" dirp name pref)
                      .ReadAllText()
                      .Replace("<ASSEMBLY>", getfullpath cfg (sprintf "provider_%s.dll" name))
                      .Replace("<FILEPATH>",dirp)
                      .Replace("<URIPATH>",sprintf "file:///%s" dirp)
                      |> fun txt -> FileSystem.OpenFileForWriteShim(sprintf "%s%s.%sbsl" dirp name pref).Write(txt)
                      
        if name = "ProviderAttribute_EmptyConsume" || name = "providerAttributeErrorConsume" then ()
        else fsc cfg "--define:%s --out:provider_%s.dll -a" name name ["provider.fsx"]

        if fileExists (sprintf "%s.bslpp" name) then preprocess name ""

        if fileExists (sprintf "%s.vsbslpp" name) then preprocess name "vs"

        SingleTest.singleNegTest cfg name

[<Theory>]
[<InlineData("neg1")>]
[<InlineData("neg2")>]
[<InlineData("neg2c")>]
[<InlineData("neg2e")>]
[<InlineData("neg2g")>]
[<InlineData("neg2h")>]
[<InlineData("neg4")>]
[<InlineData("neg6")>]
[<InlineData("InvalidInvokerExpression")>]
[<InlineData("providerAttributeErrorConsume")>]
[<InlineData("ProviderAttribute_EmptyConsume")>]
[<InlineData("EVIL_PROVIDER_GetNestedNamespaces_Exception")>]
[<InlineData("EVIL_PROVIDER_NamespaceName_Exception")>]
[<InlineData("EVIL_PROVIDER_NamespaceName_Empty")>]
[<InlineData("EVIL_PROVIDER_GetTypes_Exception")>]
[<InlineData("EVIL_PROVIDER_ResolveTypeName_Exception")>]
[<InlineData("EVIL_PROVIDER_GetNamespaces_Exception")>]
[<InlineData("EVIL_PROVIDER_GetStaticParameters_Exception")>]
[<InlineData("EVIL_PROVIDER_GetInvokerExpression_Exception")>]
[<InlineData("EVIL_PROVIDER_GetTypes_Null")>]
[<InlineData("EVIL_PROVIDER_ResolveTypeName_Null")>]
[<InlineData("EVIL_PROVIDER_GetNamespaces_Null")>]
[<InlineData("EVIL_PROVIDER_GetStaticParameters_Null")>]
[<InlineData("EVIL_PROVIDER_GetInvokerExpression_Null")>]
[<InlineData("EVIL_PROVIDER_DoesNotHaveConstructor")>]
[<InlineData("EVIL_PROVIDER_ConstructorThrows")>]
[<InlineData("EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments")>]
let ``negative type provider tests`` (name:string) = singleNegTest name

let splitAssembly subdir project =
    let cfg = testConfig project

    copyHelloWorld cfg.Directory

    let clean() =
        rm cfg "providerDesigner.dll"
        rmdir cfg "typeproviders"
        rmdir cfg "tools"
        rmdir cfg (".." ++ "typeproviders")
        rmdir cfg (".." ++ "tools")

    clean()

    fsc cfg "--out:provider.dll -a" ["provider.fs"]

    fsc cfg "--out:providerDesigner.dll -a" ["providerDesigner.fsx"]

    fsc cfg "--out:test.exe -r:provider.dll" ["test.fsx"]

    begin
        

        execAndCheckPassed cfg ("." ++ "test.exe") ""

    end

    begin
        

        fsiCheckPassed cfg "%s" cfg.fsi_flags ["test.fsx"]
    end

    // Do the same thing with different load locations for the type provider design-time component

    clean()

    // check a few load locations
    let someLoadPaths =
        [ subdir ++ "fsharp41" ++ "net48"
          subdir ++ "fsharp41" ++ "net472"
          subdir ++ "fsharp41" ++ "net461"
          subdir ++ "fsharp41" ++ "net45"
          // include up one directory
          ".." ++ subdir ++ "fsharp41" ++ "net45"
          subdir ++ "fsharp41" ++ "netstandard2.0" ]

    for dir in someLoadPaths do

        printfn ""
        printfn "Checking load path '%s'" dir
        clean()

        // put providerDesigner.dll into a different place
        mkdir cfg dir
        fsc cfg "--out:%s/providerDesigner.dll -a" dir ["providerDesigner.fsx"]

        fsc cfg "--out:test.exe -r:provider.dll" ["test.fsx"]

        begin        

            execAndCheckPassed cfg ("." ++ "test.exe") ""

        end

        begin
            

            fsiCheckPassed cfg "%s" cfg.fsi_flags ["test.fsx"]
        end

    clean()

[<Fact>]
let splitAssemblyTools () = splitAssembly "tools" "typeProviders/splitAssemblyTools"

[<Fact>]
let splitAssemblyTypeProviders () = splitAssembly "typeproviders" "typeProviders/splitAssemblyTypeproviders"

[<Fact>]
let wedgeAssembly () =
    let cfg = testConfig "typeProviders/wedgeAssembly"

    copyHelloWorld cfg.Directory

    rm cfg "provider.dll"

    rm cfg "provided.dll"

    fsc cfg "%s" "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

    rm cfg "providedJ.dll"

    fsc cfg "%s" "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

    rm cfg "providedK.dll"

    fsc cfg "%s" "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

    fsc cfg "%s" "--out:provider.dll -a" [".." ++ "helloWorld" ++ "provider.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a" cfg.fsc_flags ["test2a.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a" cfg.fsc_flags ["test2b.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe" cfg.fsc_flags ["test3.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a" cfg.fsc_flags ["test2a.fsi"; "test2a.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a" cfg.fsc_flags ["test2b.fsi"; "test2b.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS" cfg.fsc_flags ["test3.fsx"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a" cfg.fsc_flags ["test2a-restricted.fsi"; "test2a.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a"cfg.fsc_flags ["test2b-restricted.fsi"; "test2b.fs"]

    fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED" cfg.fsc_flags ["test3.fsx"]

    peverify cfg "test2a.dll"

    peverify cfg "test2b.dll"

    peverify cfg "test3.exe"

    exec cfg ("." ++ "test3.exe") ""
#endif
