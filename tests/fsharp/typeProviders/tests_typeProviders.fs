module ``FSharp-Tests-TypeProviders``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

[<Test>]
let diamondAssembly () = check (attempt {
    let cfg = testConfig "typeProviders/diamondAssembly"

    rm cfg "provider.dll"

    do! fsc cfg "%s" "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "%s" "--out:provider.dll -a" [".."/"helloWorld"/"provider.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test1.dll -a" cfg.fsc_flags ["test1.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll" cfg.fsc_flags ["test2a.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll" cfg.fsc_flags ["test2b.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll" cfg.fsc_flags ["test3.fsx"]

    do! peverify cfg "test1.dll"

    do! peverify cfg "test2a.dll"

    do! peverify cfg "test2b.dll"

    do! peverify cfg "test3.exe"

    do! exec cfg ("."/"test3.exe") ""

    use testOkFile = fileguard cfg "test.ok"

    do! fsi cfg "%s" cfg.fsi_flags ["test3.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })



[<Test>]
let globalNamespace () = check (attempt {
    let cfg = testConfig "typeProviders/globalNamespace"

    do! csc cfg """/out:globalNamespaceTP.dll /debug+ /target:library /r:"%s" """ cfg.FSCOREDLLPATH ["globalNamespaceTP.cs"]

    do! fsc cfg "%s /debug+ /r:globalNamespaceTP.dll /optimize-" cfg.fsc_flags ["test.fsx"]
                
    })


let helloWorld p = check (attempt {
    let cfg = testConfig "typeProviders/helloWorld"

    do! fsc cfg "%s" "--out:provided1.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "%s" "--out:provided2.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "%s" "--out:provided3.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "%s" "--out:provided4.dll -g -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "%s" "--out:providedJ.dll -g -a" [".."/"helloWorld"/"providedJ.fs"]

    do! fsc cfg "%s" "--out:providedK.dll -g -a" [".."/"helloWorld"/"providedK.fs"]

    do! fsc cfg "%s" "--out:providedNullAssemblyName.dll -g -a" [".."/"helloWorld"/"providedNullAssemblyName.fsx"]

    do! fsc cfg "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    do! fsc cfg "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    do! fsc cfg "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    do! fsc cfg "--out:provider.dll -a" ["provider.fsx"]

    do! SingleTest.singleTestBuildAndRunAux cfg p 


    rm cfg "provider_with_binary_compat_changes.dll"

    mkdir cfg "bincompat1"

    log "pushd bincompat1"
    let bincompat1 = getfullpath cfg "bincompat1"

    Directory.EnumerateFiles(bincompat1/"..", "*.dll")
    |> Seq.iter (fun from -> Commands.copy_y bincompat1 from ("."/Path.GetFileName(from)) |> ignore)

    do! fscIn cfg bincompat1 "%s" "-g -a -o:test_lib.dll -r:provider.dll" [".."/"test.fsx"]

    do! fscIn cfg bincompat1 "%s" "-r:test_lib.dll -r:provider.dll" [".."/"testlib_client.fsx"]

    log "popd"

    mkdir cfg "bincompat2"
        
    log "pushd bincompat2"
    let bincompat2 = getfullpath cfg "bincompat2"

    Directory.EnumerateFiles(bincompat2/".."/"bincompat1", "*.dll")
    |> Seq.iter (fun from -> Commands.copy_y bincompat2 from ("."/Path.GetFileName(from)) |> ignore)

    do! fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER --define:USE_IMPLICIT_ITypeProvider2 --out:provider.dll -g -a" [".."/"provider.fsx"]

    do! fscIn cfg bincompat2 "-g -a -o:test_lib_recompiled.dll -r:provider.dll" [".."/"test.fsx"]

    do! fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER -r:test_lib.dll -r:provider.dll" [".."/"testlib_client.fsx"]

    do! peverify cfg (bincompat2/"provider.dll")

    do! peverify cfg (bincompat2/"test_lib.dll")

    do! peverify cfg (bincompat2/"test_lib_recompiled.dll")

    do! peverify cfg (bincompat2/"testlib_client.exe")

    })


[<Test>]
let ``helloWorld fsc`` () = helloWorld FSC_OPT_PLUS_DEBUG

[<Test>]
let ``helloWorld fsi`` () = helloWorld FSI_STDIN


[<Test>]
let helloWorldCSharp () = check (attempt {
    let cfg = testConfig "typeProviders/helloWorldCSharp"

    rm cfg "magic.dll"

    do! fsc cfg "%s" "--out:magic.dll -a --keyfile:magic.snk" ["magic.fs "]

    rm cfg "provider.dll"

    do! csc cfg """/out:provider.dll /target:library "/r:%s" /r:magic.dll""" cfg.FSCOREDLLPATH ["provider.cs"]

    do! fsc cfg "%s /debug+ /r:provider.dll /optimize-" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "magic.dll"

    do! peverify cfg "provider.dll"

    do! peverify cfg "test.exe"

    do! exec cfg ("."/"test.exe") ""
                
    })


let testsSimple = 
    ["neg2h"; "neg4"; "neg1"; "neg1_a"; "neg2"; "neg2c"; "neg2e"; "neg2g"; "neg6"]
    @ ["InvalidInvokerExpression"; "providerAttributeErrorConsume"; "ProviderAttribute_EmptyConsume"]
        
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


[<Test>]
let negTests () = check (attempt {
  for name in (testsSimple  @ testsWithDefine) do
    let cfg = testConfig "typeProviders/negTests"
    let dir = cfg.Directory

    do! requireENCulture ()

    let fileExists = Commands.fileExists dir >> Option.isSome

    rm cfg "provided.dll"

    do! fsc cfg "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "providedJ.dll"

    do! fsc cfg "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    rm cfg "providedK.dll"

    do! fsc cfg "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    rm cfg "provider.dll"

    do! fsc cfg "--out:provider.dll -a" ["provider.fsx"]

    do! fsc cfg "--out:provider_providerAttributeErrorConsume.dll -a" ["providerAttributeError.fsx"]

    do! fsc cfg "--out:provider_ProviderAttribute_EmptyConsume.dll -a" ["providerAttribute_Empty.fsx"]

    rm cfg "helloWorldProvider.dll"

    do! fsc cfg "--out:helloWorldProvider.dll -a" [".."/"helloWorld"/"provider.fsx"]

    rm cfg "MostBasicProvider.dll"

    do! fsc cfg "--out:MostBasicProvider.dll -a" ["MostBasicProvider.fsx"]

    let preprocess name pref = 
        attempt {
        let dirp = (dir |> Commands.pathAddBackslash)
        do
        File.ReadAllText(sprintf "%s%s.%sbslpp" dirp name pref)
            .Replace("<ASSEMBLY>", getfullpath cfg (sprintf "provider_%s.dll" name))
            .Replace("<URIPATH>",sprintf "file:///%s" dirp)
            |> fun txt -> File.WriteAllText(sprintf "%s%s.%sbsl" dirp name pref,txt)
        }

    do! if name = "ProviderAttribute_EmptyConsume" || name = "providerAttributeErrorConsume" then Success ()
        else  fsc cfg "--define:%s --out:provider_%s.dll -a" name name ["provider.fsx"]

    do! if fileExists (sprintf "%s.bslpp" name) then preprocess name ""
        else Success

    do! if fileExists (sprintf "%s.vsbslpp" name) then preprocess name "vs"
        else Success

    do! SingleTest.singleNegTest cfg name

    })


[<Test>]
let splitAssembly () = check (attempt {
    let cfg = testConfig "typeProviders/splitAssembly"

    do! fsc cfg "--out:provider.dll -a" ["provider.fs"]

    do! fsc cfg "--out:providerDesigner.dll -a" ["providerDesigner.fsx"]

    do! SingleTest.singleTestBuildAndRunAux cfg FSC_OPT_PLUS_DEBUG
        
    })



[<Test>]
let wedgeAssembly () = check (attempt {
    let cfg = testConfig "typeProviders/wedgeAssembly"

    rm cfg "provider.dll"

    rm cfg "provided.dll"

    do! fsc cfg "%s" "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    rm cfg "providedJ.dll"

    do! fsc cfg "%s" "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    rm cfg "providedK.dll"

    do! fsc cfg "%s" "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    do! fsc cfg "%s" "--out:provider.dll -a" [".."/"helloWorld"/"provider.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a" cfg.fsc_flags ["test2a.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a" cfg.fsc_flags ["test2b.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe" cfg.fsc_flags ["test3.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a" cfg.fsc_flags ["test2a.fsi"; "test2a.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a" cfg.fsc_flags ["test2b.fsi"; "test2b.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS" cfg.fsc_flags ["test3.fsx"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a" cfg.fsc_flags ["test2a-restricted.fsi"; "test2a.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a"cfg.fsc_flags ["test2b-restricted.fsi"; "test2b.fs"]

    do! fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED" cfg.fsc_flags ["test3.fsx"]

    do! peverify cfg "test2a.dll"

    do! peverify cfg "test2b.dll"

    do! peverify cfg "test3.exe"

    do! exec cfg ("."/"test3.exe") ""

                
    })
