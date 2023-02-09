// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecordTypes =

    let verifyTypeCheckAsFsxAsLibrary compilation =
        compilation
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    let verifyTypeCheck compilation =
        compilation
        |> asLibrary
        |> typecheck

    let verifyCompileAndRunSucceeds compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    //# Fails due to stack overflow.
    //#ReqNOCov	SOURCE=BigRecord01.fs                                                           # BigRecord01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"BigRecord01.fs"|])>]
    let ``BigRecord01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    //# Renaming the .exe because for some weird reason on some OSes having 'DispatchSlot' in the .exe
    //# seems to trigger the UAC dialog... (e.g. Win7 x86)
    // SOURCE=DispatchSlot_Equals01.fsx SCFLAGS="-o dl_equals01.exe"                            # DispatchSlot_Equals01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DispatchSlot_Equals01.fsx"|])>]
    let ``DispatchSlot_Equals01_fsx`` compilation =
        compilation
        |> withName "dl_equals01"
        |> verifyCompileAndRunSucceeds

    // SOURCE=DispatchSlot_GetHashCode.fsx SCFLAGS="-o dl_gethashcode01.exe"                    # DispatchSlot_GetHashCode.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DispatchSlot_GetHashCode.fsx"|])>]
    let ``DispatchSlot_GetHashCode_fsx`` compilation =
        compilation
        |> withName "dl_gethashcode01"
        |> verifyCompileAndRunSucceeds

    // SOURCE=DuckTypingRecords01.fs                                                            # DuckTypingRecords01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DuckTypingRecords01.fs"|])>]
    let ``DuckTypingRecords01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=EqualAndBoxing01.fs                                                               # EqualAndBoxing01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualAndBoxing01.fs"|])>]
    let ``EqualAndBoxing01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=E_InheritRecord01.fs SCFLAGS="--test:ErrorRanges"                                # E_InheritRecord01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InheritRecord01.fs"|])>]
    let ``E_InheritRecord01_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 887, Line 9, Col 8, Line 9, Col 22, "The type 'Record' is not an interface type")
        ]

    // SOURCE=E_Interface_IComparable.fsx SCFLAGS="--test:ErrorRanges"                          # E_Interface_IComparable.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Interface_IComparable.fsx"|])>]
    let ``E_Interface_IComparable_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
                (Warning 67, Line 14, Col 3, Line 14, Col 24, "This type test or downcast will always hold")
                (Error 16, Line 14, Col 3, Line 14, Col 24, "The type 'I' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion.")
        ]


    // SOURCE=E_Interface_IStructuralHash.fsx                                                   # E_Interface_IStructuralHash.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Interface_IStructuralHash.fsx"|])>]
    let ``E_Interface_IStructuralHash_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 16, Col 15, Line 16, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 17, Col 1, Line 17, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Error 39, Line 20, Col 15, Line 20, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 21, Col 1, Line 21, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]

    // SOURCE=E_MutableFields01.fsx SCFLAGS="--test:ErrorRanges"                                # E_MutableFields01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MutableFields01.fsx"|])>]
    let ``E_MutableFields01_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 464, Line 15, Col 22, Line 15, Col 28, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 15, Col 35, Line 15, Col 42, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Error 5, Line 17, Col 1, Line 17, Col 5, "This field is not mutable")
            (Error 1, Line 17, Col 16, Line 17, Col 22, "The type 'decimal<Kg>' does not match the type 'float<Kg>'")
            (Error 5, Line 18, Col 1, Line 18, Col 5, "This field is not mutable")
            (Error 1, Line 18, Col 16, Line 18, Col 21, "This expression was expected to have type\n    'float'    \nbut here has type\n    'decimal'    ")
        ]

    // SOURCE=E_RecordCloning01.fs                                                              # E_RecordCloning01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RecordCloning01.fs"|])>]
    let ``E_RecordCloning01_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 7, Col 17, Line 7, Col 47, "This expression was expected to have type\n    'int array'    \nbut here has type\n    'RecType'    ")
        ]

    // SOURCE=E_RecordsNotNull01.fs                                                             # E_RecordsNotNull01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RecordsNotNull01.fs"|])>]
    let ``E_RecordsNotNull01_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 9, Col 22, Line 9, Col 26, "The type 'RecordType' does not have 'null' as a proper value")
        ]

    // SOURCE=E_RecordsNotNull02.fs                                                             # E_RecordsNotNull02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RecordsNotNull02.fs"|])>]
    let ``E_RecordsNotNull02_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=E_Scoping01.fsx SCFLAGS="--test:ErrorRanges"                                      # E_Scoping01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Scoping01.fsx"|])>]
    let ``E_Scoping01_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 9, Col 15, Line 9, Col 16, "The record label 'a' is not defined.")
        ]

    // SOURCE=E_Scoping02.fsx SCFLAGS=" --test:ErrorRanges --flaterrors"                        # E_Scoping02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Scoping02.fsx"|])>]
    let ``E_Scoping02_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3391, Line 9, Col 15, Line 9, Col 17, """This expression uses the implicit conversion 'System.Decimal.op_Implicit(value: int) : decimal' to convert type 'int' to type 'decimal'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".""")
        ]

    // SOURCE=E_TypeInference01.fs SCFLAGS="--test:ErrorRanges"                                 # E_TypeInference01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeInference01.fs"|])>]
    let ``E_TypeInference01_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
             (Error 764, Line 12, Col 15, Line 12, Col 24, "No assignment given for field 'Y' of type 'N.Blue'")
        ]

    // SOURCE=E_TypeInference01b.fs SCFLAGS="--test:ErrorRanges"                                # E_TypeInference01b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeInference01b.fs"|])>]
    let ``E_TypeInference01b_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 764, Line 13, Col 20, Line 13, Col 30, "No assignment given for field 'Y' of type 'N.M.Blue<_>'")
        ]

    // SOURCE=E_TypeInference02.fs SCFLAGS="--test:ErrorRanges"                                 # E_TypeInference02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeInference02.fs"|])>]
    let ``E_TypeInference02_fs`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 656, Line 8, Col 9, Line 8, Col 47, "This record contains fields from inconsistent types")
        ]

    // SOURCE=E_UnitType01.fsx SCFLAGS="-a --test:ErrorRanges"                                  # E_UnitType01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UnitType01.fsx"|])>]
    let ``E_UnitType01_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 10, Col 15, Line 10, Col 19, "The type 'unit' does not have 'null' as a proper value")
        ]

    // SOURCE=FieldBindingAfterWith01a.fs SCFLAGS=-a                                            # FieldBindingAfterWith01a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FieldBindingAfterWith01a.fs"|])>]
    let ``FieldBindingAfterWith01a_fs`` compilation =
        compilation
        |> verifyTypeCheckAsFsxAsLibrary

    // SOURCE=FieldBindingAfterWith01b.fs SCFLAGS=-a                                            # FieldBindingAfterWith01b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FieldBindingAfterWith01b.fs"|])>]
    let ``FieldBindingAfterWith01b_fs`` compilation =
        compilation
        |> verifyTypeCheckAsFsxAsLibrary

    // SOURCE=FullyQualify01.fs                                                                 # FullyQualify01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FullyQualify01.fs"|])>]
    let ``FullyQualify01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface01.fsx                                                                   # Interface01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Interface01.fsx"|])>]
    let ``Interface01_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface_Empty.fsx                                                               # Interface_Empty.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Interface_Empty.fsx"|])>]
    let ``Interface_Empty_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface_IComparable.fsx                                                         # Interface_IComparable.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Interface_IComparable.fsx"|])>]
    let ``Interface_IComparable_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=ImplicitEquals01.fs                                                              # ImplicitEquals001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ImplicitEquals01.fs"|])>]
    let ``ImplicitEquals01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=LongIdentifiers01.fsx                                                             # LongIdentifiers01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LongIdentifiers01.fsx"|])>]
    let ``LongIdentifiers01_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Member01.fsx                                                                      # Member01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Member01.fsx"|])>]
    let ``Member01_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=MutableFields01.fsx                                                               # MutableFields01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MutableFields01.fsx"|])>]
    let ``MutableFields01_fsx`` compilation =
        compilation
        |> withOptions ["--nowarn:464"]
        |> verifyCompileAndRunSucceeds

    // SOURCE=MutableFields_SampleFromSpec02.fsx                                                # MutableFields_SampleFromSpec02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MutableFields_SampleFromSpec02.fsx"|])>]
    let ``MutableFields_SampleFromSpec02_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=NoClashMemberIFaceMember.fs SCFLAGS="--warnaserror+"                              # NoClashMemberIFaceMember.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoClashMemberIFaceMember.fs"|])>]
    let ``NoClashMemberIFaceMember_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_Equals.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE        # Overload_Equals.fs - fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_Equals.fs"|])>]
    let ``Overload_Equals_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_GetHashCode.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE   # Overload_GetHashCode.fs - fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``Overload_GetHashCode_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_ToString.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE	    # Overload_ToString.fs - fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_ToString.fs"|])>]
    let ``Overload_ToString_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=SampleFromSpec01.fsx                                                              # SampleFromSpec01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SampleFromSpec01.fsx"|])>]
    let ``SampleFromSpec01_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=SampleFromSpec03.fsx                                                              # SampleFromSpec03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SampleFromSpec03.fsx"|])>]
    let ``SampleFromSpec03_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Scoping03.fsx                                                                     # Scoping03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Scoping03.fsx"|])>]
    let ``Scoping03_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Scoping04.fsx                                                                     # Scoping04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Scoping04.fsx"|])>]
    let ``Scoping04_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning01.fs                                                                # RecordCloning01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordCloning01.fs"|])>]
    let ``RecordCloning01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning02.fs                                                                # RecordCloning02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordCloning02.fs"|])>]
    let ``RecordCloning02_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning03.fs                                                                # RecordCloning03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordCloning03.fs"|])>]
    let ``RecordCloning03_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning01.fs                                                          # StructRecordCloning01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructRecordCloning01.fs"|])>]
    let ``StructRecordCloning01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning02.fs                                                          # StructRecordCloning02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructRecordCloning02.fs"|])>]
    let ``StructRecordCloning02_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning03.fs                                                          # StructRecordCloning03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructRecordCloning03.fs"|])>]
    let ``StructRecordCloning03_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Syntax01.fs                                                                       # Syntax01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Syntax01.fs"|])>]
    let ``Syntax01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=TypeInference01.fs                                                                # TypeInference01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeInference01.fs"|])>]
    let ``TypeInference01_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=TypeInference02.fs                                                                # TypeInference02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeInference02.fs"|])>]
    let ``TypeInference02_fs`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnitType01.fsx"|])>]
    let ``UnitType01_fsx`` compilation =
        compilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=W_Overrides01.fsx SCFLAGS="--test:ErrorRanges"                                    # W_Overrides01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_Overrides01.fsx"|])>]
    let ``W_Overrides01_fsx`` compilation =
        compilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 864, Line 11, Col 22, Line 11, Col 30, "This new member hides the abstract member 'System.Object.ToString() : string'. Rename the member or use 'override' instead.")
        ]
