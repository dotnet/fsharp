// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compileAndRun

    // SOURCE=AsPat01.fs                                                       # AsPat01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsPat01.fs"|])>]
    let ``AsPat01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AsPat02.fs                                                       # AsPat02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsPat02.fs"|])>]
    let ``AsPat02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:25";]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AsPat01.fs                                                     # E_AsPat01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AsPat01.fs"|])>]
    let ``E_AsPat01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 5, Col 16, Line 5, Col 23, "Type mismatch. Expecting a\n    ''a * 'b'    \nbut given a\n    ''a * 'b * 'c'    \nThe tuples have differing lengths of 2 and 3")
        ]

    // SOURCE=E_AttributesOnLet01.fs SCFLAGS="--test:ErrorRanges"              # E_AttributesOnLet01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributesOnLet01.fs"|])>]
    let ``E_AttributesOnLet01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 683, Line 14, Col 6, Line 14, Col 27, "Attributes are not allowed within patterns")
            (Error 842, Line 14, Col 8, Line 14, Col 23, "This attribute is not valid for use on this language element")
            (Error 683, Line 14, Col 42, Line 14, Col 63, "Attributes are not allowed within patterns")
            (Error 842, Line 14, Col 44, Line 14, Col 59, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_ErrorsForInlineValue.fs SCFLAGS="--test:ErrorRanges"           # E_ErrorsForInlineValue.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ErrorsForInlineValue.fs"|])>]
    let ``E_ErrorsForInlineValue_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 832, Line 9, Col 12, Line 9, Col 15, "Only functions may be marked 'inline'")
            (Error 832, Line 11, Col 12, Line 11, Col 16, "Only functions may be marked 'inline'")
        ]

    // SOURCE=E_ErrorsforIncompleteTryWith.fs SCFLAGS="--test:ErrorRanges"     # E_ErrorsforIncompleteTryWith.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ErrorsforIncompleteTryWith.fs"|])>]
    let ``E_ErrorsforIncompleteTryWith_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 58, Line 10, Col 1, Line 10, Col 5, "Possible incorrect indentation: this token is offside of context started at position (8:1). Try indenting this token further or using standard formatting conventions.")
            (Error 10, Line 10, Col 6, Line 10, Col 7, "Unexpected start of structured construct in expression")
            (Error 583, Line 9, Col 5, Line 9, Col 6, "Unmatched '('")
            (Error 10, Line 10, Col 16, Line 10, Col 17, "Unexpected symbol ')' in implementation file")
        ]

    // SOURCE=E_GenericTypeAnnotations01.fs SCFLAGS="--test:ErrorRanges"       # E_GenericTypeAnnotations01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_GenericTypeAnnotations01.fs"|])>]
    let ``E_GenericTypeAnnotations01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 13, Line 7, Col 14, "Unexpected reserved keyword in pattern")
            (Error 583, Line 7, Col 8, Line 7, Col 9, "Unmatched '('")
            (Error 10, Line 7, Col 25, Line 7, Col 26, "Unexpected reserved keyword in binding")
            (Error 583, Line 7, Col 19, Line 7, Col 20, "Unmatched '('")
        ]

    // SOURCE=E_InvalidInnerRecursiveBinding.fs SCFLAGS="--test:ErrorRanges"   # E_InvalidInnerRecursiveBinding.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidInnerRecursiveBinding.fs"|])>]
    let ``E_InvalidInnerRecursiveBinding_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 10, Col 13, Line 10, Col 24, "Duplicate definition of value 'foo'")
        ]

    // SOURCE=E_InvalidInnerRecursiveBinding2.fs SCFLAGS="--test:ErrorRanges"  # E_InvalidInnerRecursiveBinding2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidInnerRecursiveBinding2.fs"|])>]
    let ``E_InvalidInnerRecursiveBinding2_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 8, Col 5, Line 8, Col 8, "Duplicate definition of value 'foo'")
        ]

    // SOURCE="E_Literals02.fsi E_Literals02.fs"                               # E_Literals02.fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Literals02.fsi"|])>]
    let ``E_Literals02_fsi`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++"E_Literals02.fs"))
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 876, Line 12, Col 1, Line 13, Col 18, "A declaration may only be the [<Literal>] attribute if a constant value is also given, e.g. 'val x: int = 1'")
            (Error 876, Line 15, Col 1, Line 16, Col 15, "A declaration may only be the [<Literal>] attribute if a constant value is also given, e.g. 'val x: int = 1'")
        ]

    // SOURCE="E_Literals03.fsi E_Literals03.fs"                               # E_Literals03.fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Literals03.fsi"|])>]
    let ``E_Literals03_fsi`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++"E_Literals03.fs"))
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 34, Line 9, Col 5, Line 9, Col 10, "Module 'M' contains\n    [<Literal>]\nval test2: int = 65    \nbut its signature specifies\n    [<Literal>]\nval test2: int = 12    \nThe literal constant values and/or attributes differ")
            (Error 34, Line 6, Col 5, Line 6, Col 10, "Module 'M' contains\n    [<Literal>]\nval test1: string = \"ab\"    \nbut its signature specifies\n    [<Literal>]\nval test1: string = \"xy\"    \nThe literal constant values and/or attributes differ")
        ]

    // // // SOURCE="E_Literals04.fs"                                                # E_Literals04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Literals04.fs"|])>]
    let ``E_Literals04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 267, Line 11, Col 18, Line 11, Col 19, "This is not a valid constant expression or custom attribute value")
            (Error 837, Line 11, Col 13, Line 11, Col 31, "This is not a valid constant expression")
            (Error 267, Line 14, Col 13, Line 14, Col 17, "This is not a valid constant expression or custom attribute value")
            (Error 267, Line 17, Col 13, Line 17, Col 15, "This is not a valid constant expression or custom attribute value")
            (Error 267, Line 20, Col 13, Line 20, Col 17, "This is not a valid constant expression or custom attribute value")
            (Error 267, Line 23, Col 13, Line 23, Col 18, "This is not a valid constant expression or custom attribute value")
            (Warning 3178, Line 26, Col 13, Line 26, Col 26, "This is not valid literal expression. The [<Literal>] attribute will be ignored.")
        ]

    // SOURCE=E_Pathological01.fs SCFLAGS=--test:ErrorRanges                   # E_Pathological01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Pathological01.fs"|])>]
    let ``E_Pathological01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1232, Line 8, Col 3, Line 8, Col 6, "End of file in triple-quote string begun at or before here")
        ]

    // SOURCE=E_Pathological03.fs SCFLAGS=--test:ErrorRanges                   # E_Pathological03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Pathological03.fs"|])>]
    let ``E_Pathological03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 514, Line 9, Col 21, Line 9, Col 22, "End of file in string begun at or before here")
        ]

    // SOURCE=E_Pathological05.fs SCFLAGS=--test:ErrorRanges                   # E_Pathological05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Pathological05.fs"|])>]
    let ``E_Pathological05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 515, Line 8, Col 33, Line 8, Col 35, "End of file in verbatim string begun at or before here")
        ]

    // SOURCE=E_Pathological06.fs SCFLAGS=--test:ErrorRanges                   # E_Pathological06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Pathological06.fs"|])>]
    let ``E_Pathological06_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1232, Line 8, Col 20, Line 8, Col 23, "End of file in triple-quote string begun at or before here")
        ]

    // SOURCE=Literals01.fs                                                    # Literals01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Literals01.fs"|])>]
    let ``Literals01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //// SOURCE=ManyLetBindings.fs SCFLAGS="--debug:full     --optimize-"        # Full ManyLetBindings.fs
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ManyLetBindings.fs"|])>]
    //let ``ManyLetBindings_fs_Full_pdbs`` compilation =
    //    compilation
    //    |> withFullPdb
    //    |> withNoOptimize
    //    |> verifyCompileAndRun
    //    |> shouldSucceed

    // SOURCE=ManyLetBindings.fs SCFLAGS="--debug:portable --optimize-"        # Portable ManyLetBindings.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ManyLetBindings.fs"|])>]
    let ``ManyLetBindings_fs_PortablePdbs`` compilation =
        compilation
        |> withPortablePdb
        |> withNoOptimize
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=nestedLetBindings.fs                                             # nestedLetBindings.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"nestedLetBindings.fs"|])>]
    let ``nestedLetBindings_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Pathological02.fs SCFLAGS=-a                                     # Pathological02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Pathological02.fs"|])>]
    let ``Pathological02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:193"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Pathological04.fs SCFLAGS=-a                                     # Pathological04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Pathological04.fs"|])>]
    let ``Pathological04_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:193"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck.fs                                                   # SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_DoBindingsNotUnit01.fs                                         # W_DoBindingsNotUnit01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_DoBindingsNotUnit01.fs"|])>]
    let ``W_DoBindingsNotUnit01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 9, Col 1, Line 9, Col 10, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // SOURCE=W_DoBindingsNotUnit02.fsx SCFLAGS="--warnaserror"                # W_DoBindingsNotUnit02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_DoBindingsNotUnit02.fsx"|])>]
    let ``W_DoBindingsNotUnit02_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=RecursiveBindingGroup.fs SCFLAGS=""                              # RecursiveBindingGroup.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveBindingGroup.fs"|])>]
    let ``RecursiveBindingGroup_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
