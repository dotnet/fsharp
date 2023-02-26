// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module EntryPoint =

    //	SOURCE=behavior001.fs							# noarguments001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"behavior001.fs"|])>]
    let ``behavior001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    //	SOURCE=noarguments001.fs							# noarguments001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"noarguments001.fs"|])>]
    let ``noarguments001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed


    //NoMT	SOURCE="twofiles_001a.fs twofiles_001b.fs"					# twofiles_001a/b
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"twofiles_001a.fs"|])>]
    let ``twofiles_001a_fs_twofiles_001b_fs`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "twofiles_001b.fs"))
        |> asExe
        |> compile
        |> shouldSucceed

    //NoMT	SOURCE=inamodule001.fs								# inamodule001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"inamodule001.fs"|])>]
    let ``inamodule001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    //	SOURCE=E_twoentrypoints001.fs                   SCFLAGS="--test:ErrorRanges"		# E_twoentrypoints001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_twoentrypoints001.fs"|])>]
    let ``E_twoentrypoints001_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 433, Line 18, Col 5, Line 19, Col 19, "A function labeled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence.")
        ]

    //	SOURCE="E_twofiles_002b.fs E_twofiles_002a.fs"  SCFLAGS="--test:ErrorRanges"		# E_twofiles_002b/a
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_twofiles_002b.fs"|])>]
    let ``E_twofiles_002b_fs_E_twofiles_002a_fs`` compilation =
        compilation
        |> asExe
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "E_twofiles_002a.fs"))
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 433, Line 10, Col 5, Line 10, Col 9, "A function labeled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence.")
        ]

    //	SOURCE=E_oninvalidlanguageelement001.fs         SCFLAGS="--test:ErrorRanges"		# E_oninvalidlanguageelement001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_oninvalidlanguageelement001.fs"|])>]
    let ``E_oninvalidlanguageelement001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 9, Col 3, Line 9, Col 13, """This attribute is not valid for use on this language element""")
        ]

    //	SOURCE=E_twoattributesonsamefunction001.fs      SCFLAGS="--test:ErrorRanges"	# E_twoattributesonsamefunction001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_twoattributesonsamefunction001.fs"|])>]
    let ``E_twoattributesonsamefunction001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 429, Line 12, Col 7, Line 12, Col 17, """The attribute type 'EntryPointAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.""")
        ]

    //	SOURCE=entrypointfunctionnotmain001.fs						# entrypointfunctionnotmain001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"entrypointfunctionnotmain001.fs"|])>]
    let ``entrypointfunctionnotmain001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    //	SOURCE=E_invalidsignature001.fs                 SCFLAGS="--test:ErrorRanges"	# E_invalidsignature001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_invalidsignature001.fs"|])>]
    let ``E_invalidsignature001_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 15, Col 23, Line 15, Col 27, """This expression was expected to have type
    'int list'    
but here has type
    'string array'    """)
        ]

    //	SOURCE=E_InvalidSignature02.fs                  SCFLAGS="--test:ErrorRanges"	# E_InvalidSignature02
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidSignature02.fs"|])>]
    let ``E_InvalidSignature02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 15, Col 4, Line 15, Col 6, """This expression was expected to have type
    'int'    
but here has type
    'unit'    """)
        ]

    //NoMT	SOURCE=entrypointandFSI.fs    SCFLAGS="--multiemit-" FSIMODE=PIPE COMPILE_ONLY=1				# entrypointandFSI.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"entrypointandFSI.fs"|])>]
    let ``entrypointandFSI.fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 1, Col 1, Line 1, Col 1, "Main module of program is empty: nothing will happen when it is run")
        ]

    //NoMT	SOURCE=entrypointandFSI02.fsx SCFLAGS="--multiemit-" FSIMODE=EXEC COMPILE_ONLY=1				# entrypointandFSI02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"entrypointandFSI02.fsx"|])>]
    let ``W_NoEntryPointInLastModuleInsideMultipleNamespace_fsentrypointandFSI02_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    //	SOURCE=E_CompilingToALibrary01.fs SCFLAGS="--test:ErrorRanges --target:library"	# E_CompilingToALibrary01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompilingToALibrary01.fs"|])>]
    let ``CompilingToALibrary01_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldSucceed

    //	SOURCE=E_CompilingToAModule01.fs  SCFLAGS="--test:ErrorRanges --target:module"	# E_CompilingToAModule01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompilingToAModule01.fs"|])>]
    let ``CompilingToAModule01_fs`` compilation =
        compilation
        |> asModule
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldSucceed

    //	SOURCE=EntryPointAndAssemblyCulture.fs								# EntryPointAndAssemblyCulture.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EntryPointAndAssemblyCulture.fs"|])>]
    let ``EntryPointAndAssemblyCulture_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    //	SOURCE=W_NoEntryPointInLastModuleInsideMultipleNamespace.fs SCFLAGS="--test:ErrorRanges"	# W_NoEntryPointInLastModuleInsideMultipleNamespace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_NoEntryPointInLastModuleInsideMultipleNamespace.fs"|])>]
    let ``W_NoEntryPointInLastModuleInsideMultipleNamespace_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 1, Col 1, Line 1, Col 1, "Main module of program is empty: nothing will happen when it is run")
        ]

    //	SOURCE=W_NoEntryPointModuleInNamespace.fs SCFLAGS="--test:ErrorRanges"	# W_NoEntryPointModuleInNamespace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_NoEntryPointModuleInNamespace.fs"|])>]
    let ``W_NoEntryPointModuleInNamespace_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 11, Col 24, Line 11, Col 24, "Main module of program is empty: nothing will happen when it is run")
        ]

    //	SOURCE=W_NoEntryPointMultipleModules.fs SCFLAGS="--test:ErrorRanges"	# W_NoEntryPointMultipleModules.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_NoEntryPointMultipleModules.fs"|])>]
    let ``W_NoEntryPointMultipleModules_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 13, Col 24, Line 13, Col 24, "Main module of program is empty: nothing will happen when it is run")
        ]

    //	SOURCE=W_NoEntryPointTypeInNamespace.fs SCFLAGS="--test:ErrorRanges"	# W_NoEntryPointTypeInNamespace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_NoEntryPointTypeInNamespace.fs"|])>]
    let ``W_NoEntryPointTypeInNamespace_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 10, Col 18, Line 10, Col 18, "Main module of program is empty: nothing will happen when it is run")
        ]
