// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeAbbreviations =

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

    //SOURCE=AbbreviatedTypeSameAsValueId.fsx                                                                   # AbbreviatedTypeSameAsValueId.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AbbreviatedTypeSameAsValueId.fsx"|])>]
    let ``AbbreviatedTypeSameAsValueId_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Constraints_SampleFromSpec01.fsx                                                                   # Constraints_SampleFromSpec01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Constraints_SampleFromSpec01.fsx"|])>]
    let ``Constraints_SampleFromSpec01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    //SOURCE=E_AbbreviatedTypeAlreadyUsed01.fsx SCFLAGS="--test:ErrorRanges"                                    # E_AbbreviatedTypeAlreadyUsed01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbbreviatedTypeAlreadyUsed01.fsx"|])>]
    let ``E_AbbreviatedTypeAlreadyUsed01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 6, Col 32, Line 6, Col 38, "The type 'BigInt' is not defined in 'Microsoft.FSharp.Math'.")
            (Error 37, Line 7, Col 6, Line 7, Col 7, "Duplicate definition of type, exception or module 'T'")
        ]

    //SOURCE=E_AbbreviatedTypeDoesNotExist01.fsx SCFLAGS="--test:ErrorRanges"                                   # E_AbbreviatedTypeDoesNotExist01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbbreviatedTypeDoesNotExist01.fsx"|])>]
    let ``E_AbbreviatedTypeDoesNotExist01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 7, Col 10, Line 7, Col 11, "The namespace or module 'X' is not defined.")
        ]

    //SOURCE=E_InfiniteAbbreviation01.fs SCFLAGS="--test:ErrorRanges --flaterrors"                              # E_InfiniteAbbreviation01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InfiniteAbbreviation01.fs"|])>]
    let ``E_InfiniteAbbreviation01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 6, Col 6, Line 6, Col 7, "This type definition involves an immediate cyclic reference through an abbreviation")
            (Error 1, Line 8, Col 16, Line 8, Col 20, "This expression was expected to have type\n    'X'    \nbut here has type\n    ''a option'    ")
        ]

    //SOURCE=E_InfiniteAbbreviation02.fs                                                                        # E_InfiniteAbbreviation02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InfiniteAbbreviation02.fs"|])>]
    let ``E_InfiniteAbbreviation02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 7, Col 6, Line 7, Col 7, "This type definition involves an immediate cyclic reference through an abbreviation")
        ]

    //SOURCE=E_Constraints_SampleFromSpec02.fsx SCFLAGS="--test:ErrorRanges"                                    # E_Constraints_SampleFromSpec02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Constraints_SampleFromSpec02.fsx"|])>]
    let ``E_Constraints_SampleFromSpec02_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 16, Col 14, Line 16, Col 19, "A type parameter is missing a constraint 'when 'b :> IB'")
            (Error 35, Line 16, Col 6, Line 16, Col 7, "This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated. Type abbreviations must use all declared type parameters in the type being abbreviated. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of ...'.")
        ]

    //SOURCE=E_DroppedTypeVariable01.fsx SCFLAGS="--test:ErrorRanges -a"                                        # E_DroppedTypeVariable01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DroppedTypeVariable01.fsx"|])>]
    let ``E_DroppedTypeVariable01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 35, Line 6, Col 6, Line 6, Col 10, "This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated. Type abbreviations must use all declared type parameters in the type being abbreviated. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of ...'.")
        ]

    //SOURCE=E_FlexibleType01.fsx SCFLAGS="--test:ErrorRanges"                                                  # E_FlexibleType01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FlexibleType01.fsx"|])>]
    let ``E_FlexibleType01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 5, Col 1, Line 5, Col 2, "Unexpected infix operator in implementation file")
        ]

    //SOURCE="E_FlexibleTypeInSignature01.fsi E_FlexibleTypeInSignature01.fsx" SCFLAGS="--test:ErrorRanges"     # E_FlexibleTypeInSignature01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FlexibleTypeInSignature01.fsi"|])>]
    let ``E_FlexibleTypeInSignature01_fsi`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 715, Line 15, Col 16, Line 15, Col 18, "Anonymous type variables are not permitted in this declaration")
            (Error 35, Line 15, Col 6, Line 15, Col 13, "This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated. Type abbreviations must use all declared type parameters in the type being abbreviated. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of ...'.")
            (Error 240, Line 5, Col 1, Line 15, Col 18, "The signature file 'E_FlexibleTypeInSignature01' does not have a corresponding implementation file. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match.")
        ]

    //SOURCE=E_IncorrectRightSide_Hash.fsx SCFLAGS="--test:ErrorRanges"                                         # E_IncorrectRightSide_Hash.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IncorrectRightSide_Hash.fsx"|])>]
    let ``E_IncorrectRightSide_Hash_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 8, Col 1, Line 8, Col 5, "Incomplete structured construct at or before this point in type definition")
            (Error 547, Line 6, Col 6, Line 6, Col 15, "A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.")
        ]

    //SOURCE=E_IncorrectRightSide_Keyword.fsx SCFLAGS="--test:ErrorRanges"                                      # E_IncorrectRightSide_Keyword.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IncorrectRightSide_Keyword.fsx"|])>]
    let ``E_IncorrectRightSide_Keyword_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 6, Col 16, Line 6, Col 18, "Unexpected keyword 'of' in type definition")
        ]

    //SOURCE=E_IncorrectRightSide_Quotation.fsx SCFLAGS="--test:ErrorRanges"                                    # E_IncorrectRightSide_Quotation.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IncorrectRightSide_Quotation.fsx"|])>]
    let ``E_IncorrectRightSide_Quotation_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 6, Col 16, Line 6, Col 18, "Unexpected start of quotation in type definition")
        ]

    //SOURCE=E_InheritTypeAbrev.fs                                                                              # E_InheritTypeAbrev.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InheritTypeAbrev.fs"|])>]
    let ``E_InheritTypeAbrev_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 945, Line 9, Col 9, Line 9, Col 22, "Cannot inherit a sealed type")
        ]

    //SOURCE=E_PrivateTypeAbbreviation02.fs SCFLAGS="--test:ErrorRanges"                                        # E_PrivateTypeAbbreviation02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PrivateTypeAbbreviation02.fs"|])>]
    let ``E_PrivateTypeAbbreviation02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1092, Line 15, Col 10, Line 15, Col 13, "The type 'X' is not accessible from this code location")
        ]

    //SOURCE=E_Recursive01.fsx SCFLAGS="--test:ErrorRanges"                                                     # E_Recursive01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Recursive01.fsx"|])>]
    let ``E_Recursive01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 7, Col 18, Line 7, Col 20, "This type definition involves an immediate cyclic reference through an abbreviation")
        ]

    //SOURCE=E_Recursive02.fsx SCFLAGS="--test:ErrorRanges"                                                     # E_Recursive02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Recursive02.fsx"|])>]
    let ``E_Recursive02_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 7, Col 6, Line 7, Col 7, "This type definition involves an immediate cyclic reference through an abbreviation")
        ]

    //SOURCE=E_Recursive03.fsx SCFLAGS="--test:ErrorRanges"                                                     # E_Recursive03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Recursive03.fsx"|])>]
    let ``E_Recursive03_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 7, Col 6, Line 7, Col 7, "This type definition involves an immediate cyclic reference through an abbreviation")
        ]

    //SOURCE=E_UnexpectedCharInTypeName01.fs SCFLAGS="--test:ErrorRanges"                                       # E_UnexpectedCharInTypeName01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UnexpectedCharInTypeName01.fs"|])>]
    let ``E_UnexpectedCharInTypeName01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 5, Col 6, Line 5, Col 7, "Unexpected character '' in type name")
        ]

    //SOURCE=Identity01.fsx                                                                                     # Identity01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Identity01.fsx"|])>]
    let ``Identity01_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=PrivateTypeAbbreviation01.fs                                                                       # PrivateTypeAbbreviation01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PrivateTypeAbbreviation01.fs"|])>]
    let ``PrivateTypeAbbreviation01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
         |> shouldSucceed

    //SOURCE=ReorderingTypeVariables01.fsx SCFLAGS="--test:ErrorRanges --warnaserror+ -a"                       # ReorderingTypeVariables01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReorderingTypeVariables01.fsx"|])>]
    let ``ReorderingTypeVariables01_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=TypeNestedInModules01.fsx                                                                          # TypeNestedInModules01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeNestedInModules01.fsx"|])>]
    let ``TypeNestedInModules01_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=TypeAbbreviationAfterForwardRef.fs                                                                 # TypeAbbreviationAfterForwardRef.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeAbbreviationAfterForwardRef.fs"|])>]
    let ``TypeAbbreviationAfterForwardRef_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
