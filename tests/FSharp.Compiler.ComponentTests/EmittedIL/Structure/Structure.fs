namespace EmittedIL

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

    let verifyIl compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline


    // SOURCE=AttributesOnLet01.fs   SCFLAGS="-r:CodeGenHelper.dll" # AttributesOnLet01.fs
    [<Theory; FileInlineData("AttributesOnLet01.fs")>]
    let ``AttributesOnLet01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=AttributesOnLet02.fs   SCFLAGS="-r:CodeGenHelper.dll" # AttributesOnLet02.fs
    [<Theory; FileInlineData("AttributesOnLet02.fs")>]
    let ``AttributesOnLet02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=AttributesOnProperty.fs             SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnProperty.fs
    [<Theory; FileInlineData("AttributesOnProperty.fs")>]
    let ``AttributesOnProperty_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertyGetter.fs       SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertyGetter.fs
    [<Theory; FileInlineData("AttributesOnPropertyGetter.fs")>]
    let ``AttributesOnPropertyGetter_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertyGetterSetter.fs SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertyGetterSetter.fs
    [<Theory; FileInlineData("AttributesOnPropertyGetterSetter.fs")>]
    let ``AttributesOnPropertyGetterSetter_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=AttributesOnPropertySetter.fs       SCFLAGS="-r:CodeGenHelper.dll"    # AttributesOnPropertySetter.fs
    [<Theory; FileInlineData("AttributesOnPropertySetter.fs")>]
    let ``AttributesOnPropertySetter_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=ClassArity01.fs        SCFLAGS="-r:CodeGenHelper.dll" # ClassArity01.fs
    [<Theory; FileInlineData("ClassArity01.fs")>]
    let ``ClassArity01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=Delegates01.fs         SCFLAGS="-r:CodeGenHelper.dll" # Delegates01.fs
    [<Theory; FileInlineData("Delegates01.fs")>]
    let ``Delegates01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=ReadOnlyStructFromLib.fs SCFLAGS="-r:ReadWriteLib.dll" PRECMD="\$CSC_PIPE /target:library /reference:System.Core.dll ReadWriteLib.cs"	# ReadOnlyStructFromLib.fs
    [<Theory; FileInlineData("ReadOnlyStructFromLib.fs")>]
    let ``ReadOnlyStructFromLib_fs`` compilation =
        let readWriteLib =
            CSharpFromPath (Path.Combine(__SOURCE_DIRECTORY__, "ReadWriteLib.cs"))
            |> withName "ReadWriteLib"

        compilation
        |> getCompilation
        |> withReferences([readWriteLib])
        |> verifyCompilation

    // SOURCE=DiscUnionCodeGen1.fs SCFLAGS="-r:CodeGenHelper.dll"       # DiscUnionCodeGen1.fs
    [<Theory; FileInlineData("DiscUnionCodeGen1.fs")>]
    let ``DiscUnionCodeGen1_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Equality01.fs                                             # Equality01.fs
    [<Theory; FileInlineData("Equality01.fs")>]
    let ``Equality01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Events01.fs            SCFLAGS="-r:CodeGenHelper.dll" # Events01.fs
    [<Theory; FileInlineData("Events01.fs")>]
    let ``Events01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=Events02.fs            SCFLAGS="-r:CodeGenHelper.dll"     # Events02.fs
    [<Theory; FileInlineData("Events02.fs")>]
    let ``Events02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=Extensions01.fs SCFLAGS="-r:CodeGenHelper.dll"            # Extensions01.fs
    [<Theory; FileInlineData("Extensions01.fs")>]
    let ``Extensions01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Extensions02.fs SCFLAGS="-r:CodeGenHelper.dll"            # Extensions02.fs
    [<Theory; FileInlineData("Extensions02.fs")>]
    let ``Extensions02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=FloatsAndDoubles_1.fs  SCFLAGS="-g --out:FloatsAndDoubles.exe" COMPILE_ONLY=1 POSTCMD="comparebsl.cmd  FloatsAndDoubles.exe" # FloatsAndDoubles.fs
    [<Theory; FileInlineData("FloatsAndDoubles.fs", Realsig=BooleanOptions.Both)>]
    let ``FloatsAndDoubles_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyIl

    // SOURCE=FunctionArity01.fs     SCFLAGS="-r:CodeGenHelper.dll" # FunctionArity01.fs
    [<Theory; FileInlineData("FunctionArity01.fs")>]
    let ``FunctionArity01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=LocalTypeFunctionInIf.fs   # LocalTypeFunctionInIf.fs
    [<Theory; FileInlineData("LocalTypeFunctionInIf.fs")>]
    let ``LocalTypeFunctionInIf_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ModuleArity01.fs       SCFLAGS="-r:CodeGenHelper.dll" # ModuleArity01.fs
    [<Theory; FileInlineData("ModuleArity01.fs")>]
    let ``ModuleArity01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

#if false && !NETCOREAPP && !NETSTANDARD
    // SOURCE=NativePtr01.fs         PEVER=/Exp_Fail SCFLAGS="-r:CodeGenHelper.dll" # NativePtr01.fs
    [<Theory; FileInlineData("NativePtr01.fs")>]
    let ``NativePtr01_fs`` compilation =
        compilation
        |> getCompilation
        |> setupCompilation
        |> PEVerifier.verifyPEFile
        |> PEVerifier.shouldFail
#endif

    // SOURCE=ObjectExpressions01.fs SCFLAGS="-r:CodeGenHelper.dll"     # ObjectExpressions01.fs
    [<Theory; FileInlineData("ObjectExpressions01.fs")>]
    let ``ObjectExpressions01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE="UnionTypeWithSignature01.fsi UnionTypeWithSignature01.fs" SCFLAGS="-r:CodeGenHelper.dll"	# UnionTypeWithSignature01.fs
    [<Theory; FileInlineData("UnionTypeWithSignature01.fsi")>]
    let ``UnionTypeWithSignature01_fsi`` compilation =
        compilation
        |> getCompilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "UnionTypeWithSignature01.fs"))
        |> verifyExecution

    // SOURCE="UnionTypeWithSignature02.fsi UnionTypeWithSignature02.fs" SCFLAGS="-r:CodeGenHelper.dll"	# UnionTypeWithSignature02.fs
    [<Theory; FileInlineData("UnionTypeWithSignature02.fsi")>]
    let ``UnionTypeWithSignature02_fsi`` compilation =
        compilation
        |> getCompilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "UnionTypeWithSignature02.fs"))
        |> verifyCompilation

    // SOURCE=UnitsOfMeasure01.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure01.fs
    [<Theory; FileInlineData("UnitsOfMeasure01.fs")>]
    let ``UnitsOfMeasure01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=UnitsOfMeasure02.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure02.fs
    [<Theory; FileInlineData("UnitsOfMeasure02.fs")>]
    let ``UnitsOfMeasure02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution

    // SOURCE=UnitsOfMeasure03.fs    SCFLAGS="-r:CodeGenHelper.dll"     # UnitsOfMeasure03.fs
    [<Theory; FileInlineData("UnitsOfMeasure03.fs")>]
    let ``UnitsOfMeasure03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyExecution
