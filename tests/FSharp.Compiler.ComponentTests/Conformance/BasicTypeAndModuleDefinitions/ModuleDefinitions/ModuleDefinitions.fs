// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ModuleDefinitions =

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

    // SOURCE=AutoOpen01.fs                                                                                                # AutoOpen01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AutoOpen01.fs"|])>]
    let ``AutoOpen01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AutoOpen02.fs                                                                                                # AutoOpen02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AutoOpen02.fs"|])>]
    let ``AutoOpen02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 15, Col 4, Line 15, Col 5, "The value or constructor 'x' is not defined.")
        ]

    // SOURCE=AutoOpen03.fs                                                                                                # AutoOpen03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AutoOpen03.fs"|])>]
    let ``AutoOpen03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=DefineModule01.fs                                                                                            # DefineModule01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DefineModule01.fs"|])>]
    let ``DefineModule01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_CannotAccessPrivateMembersOfAnotherType.fs    SCFLAGS="--test:ErrorRanges" COMPILE_ONLY=1 	# E_CannotAccessPrivateMembersOfAnotherType.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CannotAccessPrivateMembersOfAnotherType.fs"|])>]
    let ``E_CannotAccessPrivateMembersOfAnotherType_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 12, Col 10, Line 12, Col 17, "The struct, record or union type 'SpecSet' is not structurally comparable because the type 'SpecMulti' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'SpecSet' to clarify that the type is not comparable")
            (Error 491, Line 22, Col 13, Line 22, Col 27, "The member or object constructor 'Impl' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    // SOURCE=E_ModuleSuffix01.fsx SCFLAGS="--test:ErrorRanges"                                                            # E_ModuleSuffix01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ModuleSuffix01.fsx"|])>]
    let ``E_ModuleSuffix01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 13, Col 8, Line 13, Col 18, "Duplicate definition of type or module 'module'")
        ]

    // SOURCE=E_ModuleSuffix_NameClash01.fsx SCFLAGS="--test:ErrorRanges"                                                  # E_ModuleSuffix_NameClash01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ModuleSuffix_NameClash01.fsx"|])>]
    let ``E_ModuleSuffix_NameClash01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 12, Col 1, Line 12, Col 15, "Duplicate definition of type, exception or module 'mModule'")
        ]

    // SOURCE=E_ModuleWithExpression02.fs COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"                                      # E_ModuleWithExpression02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ModuleWithExpression02.fs"|])>]
    let ``E_ModuleWithExpression02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 8, Col 12, Line 8, Col 20, "The namespace 'DateTime' is not defined.")
        ]

    // SOURCE=E_ModuleWithSameNameInNamespace01.fsx SCFLAGS="--test:ErrorRanges"                                           # E_ModuleWithSameNameInNamespace01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ModuleWithSameNameInNamespace01.fsx"|])>]
    let ``E_ModuleWithSameNameInNamespace01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 9, Col 1, Line 9, Col 18, "Duplicate definition of type, exception or module 'module'")
        ]

    // SOURCE="E_ModuleWithSameNameInNamespace02a.fsx E_ModuleWithSameNameInNamespace02b.fsx" SCFLAGS="--test:ErrorRanges" # E_ModuleWithSameNameInNamespace02 
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ModuleWithSameNameInNamespace02a.fsx"|])>]
    let ``E_ModuleWithSameNameInNamespace02a_fsx`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++"E_ModuleWithSameNameInNamespace02b.fsx"))
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 248, Line 7, Col 8, Line 7, Col 18, "Two modules named 'N.module' occur in two parts of this assembly")
        ]

    // SOURCE=E_ObsoleteAttribOnModules01.fs SCFLAGS="--test:ErrorRanges"                                                  # E_ObsoleteAttribOnModules01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ObsoleteAttribOnModules01.fs"|])>]
    let ``E_ObsoleteAttribOnModules01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 21, Col 10, Line 21, Col 24, "This construct is deprecated. Don't use this module.")
            (Error 101, Line 22, Col 25, Line 22, Col 39, "This construct is deprecated. Don't use this module.")
            (Error 101, Line 23, Col 14, Line 23, Col 28, "This construct is deprecated. Don't use this module.")
            (Warning 44, Line 26, Col 21, Line 26, Col 41, "This construct is deprecated. Don't use this nested module.")
        ]

    // SOURCE=FullyQualify01.fs                                                                                            # FullyQualify01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FullyQualify01.fs"|])>]
    let ``FullyQualify01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=LightSyntax01.fsx                                                                                            # LightSyntax01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LightSyntax01.fsx"|])>]
    let ``LightSyntax01_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ModuleAbbreviationWithModule01.fs COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"                                # ModuleAbbreviationWithModule01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleAbbreviationWithModule01.fs"|])>]
    let ``ModuleAbbreviationWithModule01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE="Module_internal01.fs Module_internalConsumer01.fs"                                                          # Module_internal01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Module_internal01.fs"|])>]
    let ``Module_internal01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ModuleSuffix02.fsx                                                                                           # ModuleSuffix02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleSuffix02.fsx"|])>]
    let ``ModuleSuffix02_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // NoMT SOURCE=ModuleSuffix03.fsx PRECMD="\$FSC_PIPE -a ModuleSuffix03Lib.fsx" SCFLAGS="-r:ModuleSuffix03Lib.dll"      # ModuleSuffix03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleSuffix03.fsx"|])>]
    let ``ModuleSuffix03_fsx`` compilation =
        let lib =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "ModuleSuffix03Lib.fsx"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ModuleSuffix04.fsx                                                                                           # ModuleSuffix04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleSuffix04.fsx"|])>]
    let ``ModuleSuffix04_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ModuleWithExpression01.fs COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"                                        # ModuleWithExpression01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleWithExpression01.fs"|])>]
    let ``ModuleWithExpression01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ModuleWithExpression02.fs COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"                                        # ModuleWithExpression02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleWithExpression02.fs"|])>]
    let ``ModuleWithExpression02_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_ExceptionDefinition.fsx                                                                           # Production_ExceptionDefinition.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_ExceptionDefinition.fsx"|])>]
    let ``Production_ExceptionDefinition_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_ImportDeclaration.fsx                                                                             # Production_ImportDeclaration.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_ImportDeclaration.fsx"|])>]
    let ``Production_ImportDeclaration_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_LetBindings_Binding.fsx                                                                           # Production_LetBindings_Binding.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_LetBindings_Binding.fsx"|])>]
    let ``Production_LetBindings_Binding_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_LetBindings_SideEff.fsx                                                                           # Production_LetBindings_SideEff.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_LetBindings_SideEff.fsx"|])>]
    let ``Production_LetBindings_SideEff_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_ModuleAbbreviation.fsx                                                                            # Production_ModuleAbbreviation.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_ModuleAbbreviation.fsx"|])>]
    let ``Production_ModuleAbbreviation_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_ModuleDefinition.fsx                                                                              # Production_ModuleDefinition.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_ModuleDefinition.fsx"|])>]
    let ``Production_ModuleDefinition_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_OCamlCompat.fsx                                                                                   # Production_OCamlCompat.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_OCamlCompat.fsx"|])>]
    let ``Production_OCamlCompat_fsx`` compilation =
        compilation
        |> withOcamlCompat
        |> withLangVersion50
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Production_TypeDefinitions.fsx                                                                               # Production_TypeDefinitions.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Production_TypeDefinitions.fsx"|])>]
    let ``Production_TypeDefinitions_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_ModuleAbbreviationWithNamespace01.fs COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"                           # W_ModuleAbbreviationWithNamespace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_ModuleAbbreviationWithNamespace01.fs"|])>]
    let ``W_ModuleAbbreviationWithNamespace01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 965, Line 8, Col 1, Line 9, Col 26, "The path 'Microsoft.FSharp.Core' is a namespace. A module abbreviation may not abbreviate a namespace.")
        ]

    // SOURCE=W_Production_OCamlCompat.fsx SCFLAGS="--test:ErrorRanges"                                                    # W_Production_OCamlCompat.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_Production_OCamlCompat.fsx"|])>]
    let ``W_Production_OCamlCompat_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 14, Col 13, Line 14, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 18, Col 13, Line 18, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 22, Col 13, Line 22, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 26, Col 13, Line 26, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 30, Col 13, Line 30, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 35, Col 13, Line 35, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
            (Error 62, Line 39, Col 13, Line 39, Col 19, "This construct is deprecated. The use of 'module M = struct ... end ' was deprecated in F# 2.0 and is no longer supported. Remove the 'struct' and 'end' and use indentation instead. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.")
        ]

    // 
    // #These 2 are not actual testcases, just test libraries for the next 2
    // SOURCE=LibFoo1.fs SCFLAGS="-a"
    // SOURCE=LibFOo2.fs SCFLAGS="-a"
    // SOURCE=SameTypeInTwoReferences01.fs SCFLAGS="-r:LibFoo1.dll -r:LibFoo2.dll"                                         # SameTypeInTwoReferences01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SameTypeInTwoReferences01.fs"|])>]
    let ``SameTypeInTwoReferences01_fs`` compilation =
        let libFoo1 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "LibFoo1.fs"))
            |> withOptimize
            |> asLibrary
        let libFoo2 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "LibFoo2.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [libFoo1; libFoo2]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SameTypeInTwoReferences02.fs SCFLAGS="-r:LibFoo2.dll -r:LibFoo1.dll"                                         # SameTypeInTwoReferences02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SameTypeInTwoReferences02.fs"|])>]
    let ``SameTypeInTwoReferences02_fs`` compilation =
        let libFoo1 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "LibFoo1.fs"))
            |> withOptimize
            |> asLibrary

        let libFoo2 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "LibFoo2.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [libFoo2; libFoo1]
        |> verifyCompileAndRun
        |> shouldSucceed
