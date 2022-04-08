namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module Structure =

    //# This file is needed by the rest of the suite. It is not really a testcase...
    // SOURCE=CodeGenHelper.fs       SCFLAGS="-a -g"      # CodeGenHelper.fs
    let codeGenHelperLibrary =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__,  "CodeGenHelper.fs")))
        |> withName "CodeGenHelper"

    let setupCompilation compilation =
        compilation
        |> asFs
        |> asExe
        |> withReferences [codeGenHelperLibrary]

    let verifyCompilation compilation =
        compilation
        |> setupCompilation
        |> compile
        |> shouldSucceed

    let verifyExecution compilation =
        compilation
        |> setupCompilation
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=AttributesOnLet01.fs   SCFLAGS="-r:CodeGenHelper.dll" # AttributesOnLet01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnLet01.fs"|])>]
    let ``AttributesOnLet01_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=AttributesOnLet02.fs   SCFLAGS="-r:CodeGenHelper.dll" # AttributesOnLet02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnLet02.fs"|])>]
    let ``AttributesOnLet02_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=AttributesOnProperty.fs             SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnProperty.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnProperty.fs"|])>]
    let ``AttributesOnProperty_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertyGetter.fs       SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertyGetter.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnPropertyGetter.fs"|])>]
    let ``AttributesOnPropertyGetter_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertyGetterSetter.fs SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertyGetterSetter.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnPropertyGetterSetter.fs"|])>]
    let ``AttributesOnPropertyGetterSetter_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertySetter.fs       SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertySetter.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributesOnPropertySetter.fs"|])>]
    let ``AttributesOnPropertySetter_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=ClassArity01.fs        SCFLAGS="-r:CodeGenHelper.dll" # ClassArity01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ClassArity01.fs"|])>]
    let ``ClassArity01_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=Delegates01.fs         SCFLAGS="-r:CodeGenHelper.dll" # Delegates01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Delegates01.fs"|])>]
    let ``Delegates01_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=ReadOnlyStructFromLib.fs SCFLAGS="-r:ReadWriteLib.dll" PRECMD="\$CSC_PIPE /target:library /reference:System.Core.dll ReadWriteLib.cs"	# ReadOnlyStructFromLib.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReadOnlyStructFromLib.fs"|])>]
    let ``ReadOnlyStructFromLib_fs`` compilation =
        let readWriteLib =
            CSharpFromPath (Path.Combine(__SOURCE_DIRECTORY__, "ReadWriteLib.cs"))
            |> withName "ReadWriteLib"

        compilation
        |> withReferences([readWriteLib])
        |> verifyCompilation

    // SOURCE=DiscUnionCodeGen1.fs SCFLAGS="-r:CodeGenHelper.dll"       # DiscUnionCodeGen1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DiscUnionCodeGen1.fs"|])>]
    let ``DiscUnionCodeGen1_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Equality01.fs                                             # Equality01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Equality01.fs"|])>]
    let ``Equality01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Events01.fs            SCFLAGS="-r:CodeGenHelper.dll" # Events01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Events01.fs"|])>]
    let ``Events01_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=Events02.fs            SCFLAGS="-r:CodeGenHelper.dll"     # Events02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Events02.fs"|])>]
    let ``Events02_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=Extensions01.fs SCFLAGS="-r:CodeGenHelper.dll"            # Extensions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Extensions01.fs"|])>]
    let ``Extensions01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Extensions02.fs SCFLAGS="-r:CodeGenHelper.dll"            # Extensions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Extensions02.fs"|])>]
    let ``Extensions02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=FunctionArity01.fs     SCFLAGS="-r:CodeGenHelper.dll" # FunctionArity01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FunctionArity01.fs"|])>]
    let ``FunctionArity01_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=LocalTypeFunctionInIf.fs   # LocalTypeFunctionInIf.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LocalTypeFunctionInIf.fs"|])>]
    let ``LocalTypeFunctionInIf_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ModuleArity01.fs       SCFLAGS="-r:CodeGenHelper.dll" # ModuleArity01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleArity01.fs"|])>]
    let ``ModuleArity01_fs`` compilation =
        compilation
        |> verifyExecution

#if !TESTING_ON_LINUX
    // SOURCE=NativePtr01.fs         PEVER=/Exp_Fail SCFLAGS="-r:CodeGenHelper.dll" # NativePtr01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NativePtr01.fs"|])>]
    let ``NativePtr01_fs`` compilation =
        compilation
        |> setupCompilation
        |> PEVerifier.verifyPEFile
        |> PEVerifier.shouldFail
#endif

    // SOURCE=ObjectExpressions01.fs SCFLAGS="-r:CodeGenHelper.dll"     # ObjectExpressions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ObjectExpressions01.fs"|])>]
    let ``ObjectExpressions01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE="UnionTypeWithSignature01.fsi UnionTypeWithSignature01.fs" SCFLAGS="-r:CodeGenHelper.dll"	# UnionTypeWithSignature01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionTypeWithSignature01.fsi"|])>]
    let ``UnionTypeWithSignature01_fsi`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "UnionTypeWithSignature01.fs"))
        |> verifyExecution

    // SOURCE="UnionTypeWithSignature02.fsi UnionTypeWithSignature02.fs" SCFLAGS="-r:CodeGenHelper.dll"	# UnionTypeWithSignature02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionTypeWithSignature02.fsi"|])>]
    let ``UnionTypeWithSignature02_fsi`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "UnionTypeWithSignature02.fs"))
        |> verifyCompilation

    // SOURCE=UnitsOfMeasure01.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnitsOfMeasure01.fs"|])>]
    let ``UnitsOfMeasure01_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=UnitsOfMeasure02.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnitsOfMeasure02.fs"|])>]
    let ``UnitsOfMeasure02_fs`` compilation =
        compilation
        |> verifyExecution

    // SOURCE=UnitsOfMeasure03.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnitsOfMeasure03.fs"|])>]
    let ``UnitsOfMeasure03_fs`` compilation =
        compilation
        |> verifyExecution
