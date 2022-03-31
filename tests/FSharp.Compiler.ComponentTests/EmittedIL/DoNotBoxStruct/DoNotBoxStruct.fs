namespace FSharp.Compiler.ComponentTests.EmittedIL

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DoNotBoxStruct =

    let computationExprLibrary =
        FsxFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth.fs"))
        |> withName "DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth_fs"

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions ["--test:EmitFeeFeeAs100001"]
        |> asExe
        |> withReferences [computationExprLibrary]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    //SOURCE=DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth.exe"	# DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth.fs"|])>]
    let ``DoNotBoxStruct_ArrayOfArray_FSInterface_NoExtMeth_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_Array_FSInterface_NoExtMeth.fs          SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_Array_FSInterface_NoExtMeth.exe"		# DoNotBoxStruct_Array_FSInterface_NoExtMeth.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_Array_FSInterface_NoExtMeth.fs"|])>]
    let ``DoNotBoxStruct_Array_FSInterface_NoExtMeth_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_MDArray_FSInterface_NoExtMeth.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_MDArray_FSInterface_NoExtMeth.exe"	# DoNotBoxStruct_MDArray_FSInterface_NoExtMeth.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_MDArray_FSInterface_NoExtMeth.fs"|])>]
    let ``DoNotBoxStruct_MDArray_FSInterface_NoExtMeth_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_NoArray_FSInterface_NoExtMeth.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_NoArray_FSInterface_NoExtMeth.exe"	# DoNotBoxStruct_NoArray_FSInterface_NoExtMeth.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_NoArray_FSInterface_NoExtMeth.fs"|])>]
    let ``DoNotBoxStruct_NoArray_FSInterface_NoExtMeth_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_ArrayOfArray_CSInterface.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_ArrayOfArray_CSInterface.exe"			# DoNotBoxStruct_ArrayOfArray_CSInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_ArrayOfArray_CSInterface.fs"|])>]
    let ``DoNotBoxStruct_ArrayOfArray_CSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_Array_CSInterface.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_Array_CSInterface.exe"			# DoNotBoxStruct_Array_CSInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_Array_CSInterface.fs"|])>]
    let ``DoNotBoxStruct_Array_CSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_Array_FSInterface.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_Array_FSInterface.exe"		# DoNotBoxStruct_Array_FSInterface.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_Array_FSInterface.fs"|])>]
    let ``DoNotBoxStruct_Array_FSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_MDArray_CSInterface.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_MDArray_CSInterface.exe"		# DoNotBoxStruct_MDArray_CSInterface.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_MDArray_CSInterface.fs"|])>]
    let ``DoNotBoxStruct_MDArray_CSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_MDArray_FSInterface.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_MDArray_FSInterface.exe"		# DoNotBoxStruct_MDArray_FSInterface.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_MDArray_FSInterface.fs"|])>]
    let ``DoNotBoxStruct_MDArray_FSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_NoArray_CSInterface.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_NoArray_CSInterface.exe"					# DoNotBoxStruct_NoArray_CSInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_NoArray_CSInterface.fs"|])>]
    let ``DoNotBoxStruct_NoArray_CSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_NoArray_FSInterface.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_NoArray_FSInterface.exe"		# DoNotBoxStruct_NoArray_FSInterface.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_NoArray_FSInterface.fs"|])>]
    let ``DoNotBoxStruct_NoArray_FSInterface_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=DoNotBoxStruct_ToString.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd DoNotBoxStruct_ToString.exe"				# DoNotBoxStruct_ToString.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DoNotBoxStruct_ToString.fs"|])>]
    let ``DoNotBoxStruct_ToString_fs`` compilation =
        compilation
        |> verifyCompilation
