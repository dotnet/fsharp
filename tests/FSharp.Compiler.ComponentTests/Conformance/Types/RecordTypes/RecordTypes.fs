// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecordTypes =

    let verifyTypeCheckAsFsxAsLibrary compilation =
        compilation
        |> asLibrary
        |> withOptions ["--nowarn:3560"]
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
    [<Theory; FileInlineData("BigRecord01.fs")>]
    let ``BigRecord01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    //# Renaming the .exe because for some weird reason on some OSes having 'DispatchSlot' in the .exe
    //# seems to trigger the UAC dialog... (e.g. Win7 x86)
    // SOURCE=DispatchSlot_Equals01.fsx SCFLAGS="-o dl_equals01.exe"                            # DispatchSlot_Equals01.fsx
    [<Theory; FileInlineData("DispatchSlot_Equals01.fsx")>]
    let ``DispatchSlot_Equals01_fsx`` compilation =
        compilation
        |> getCompilation
        |> withName "dl_equals01"
        |> verifyCompileAndRunSucceeds

    // SOURCE=DispatchSlot_GetHashCode.fsx SCFLAGS="-o dl_gethashcode01.exe"                    # DispatchSlot_GetHashCode.fsx
    [<Theory; FileInlineData("DispatchSlot_GetHashCode.fsx")>]
    let ``DispatchSlot_GetHashCode_fsx`` compilation =
        compilation
        |> getCompilation
        |> withName "dl_gethashcode01"
        |> verifyCompileAndRunSucceeds

    // SOURCE=DuckTypingRecords01.fs                                                            # DuckTypingRecords01.fs
    [<Theory; FileInlineData("DuckTypingRecords01.fs")>]
    let ``DuckTypingRecords01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=EqualAndBoxing01.fs                                                               # EqualAndBoxing01.fs
    [<Theory; FileInlineData("EqualAndBoxing01.fs")>]
    let ``EqualAndBoxing01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=E_InheritRecord01.fs SCFLAGS="--test:ErrorRanges"                                # E_InheritRecord01.fs
    [<Theory; FileInlineData("E_InheritRecord01.fs")>]
    let ``E_InheritRecord01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 887, Line 9, Col 8, Line 9, Col 22, "The type 'Record' is not an interface type")
        ]

    // SOURCE=E_Interface_IComparable.fsx SCFLAGS="--test:ErrorRanges"                          # E_Interface_IComparable.fsx
    [<Theory; FileInlineData("E_Interface_IComparable.fsx")>]
    let ``E_Interface_IComparable_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
                (Warning 67, Line 14, Col 3, Line 14, Col 24, "This type test or downcast will always hold")
                (Error 16, Line 14, Col 3, Line 14, Col 24, "The type 'I' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion.")
        ]


    // SOURCE=E_Interface_IStructuralHash.fsx                                                   # E_Interface_IStructuralHash.fsx
    [<Theory; FileInlineData("E_Interface_IStructuralHash.fsx")>]
    let ``E_Interface_IStructuralHash_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 16, Col 15, Line 16, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 17, Col 1, Line 17, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Error 39, Line 20, Col 15, Line 20, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 21, Col 1, Line 21, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]

    // SOURCE=E_MutableFields01.fsx SCFLAGS="--test:ErrorRanges"                                # E_MutableFields01.fsx
    [<Theory; FileInlineData("E_MutableFields01.fsx")>]
    let ``E_MutableFields01_fsx`` compilation =
        compilation
        |> getCompilation
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
    [<Theory; FileInlineData("E_RecordCloning01.fs")>]
    let ``E_RecordCloning01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 7, Col 17, Line 7, Col 47, "This expression was expected to have type\n    'int array'    \nbut here has type\n    'RecType'    ")
        ]

    // SOURCE=E_RecordsNotNull01.fs                                                             # E_RecordsNotNull01.fs
    [<Theory; FileInlineData("E_RecordsNotNull01.fs")>]
    let ``E_RecordsNotNull01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 9, Col 22, Line 9, Col 26, "The type 'RecordType' does not have 'null' as a proper value")
        ]

    // SOURCE=E_RecordsNotNull02.fs                                                             # E_RecordsNotNull02.fs
    [<Theory; FileInlineData("E_RecordsNotNull02.fs")>]
    let ``E_RecordsNotNull02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=E_Scoping01.fsx SCFLAGS="--test:ErrorRanges"                                      # E_Scoping01.fsx
    [<Theory; FileInlineData("E_Scoping01.fsx")>]
    let ``E_Scoping01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 9, Col 15, Line 9, Col 16, "The record label 'a' is not defined.")
        ]

    // SOURCE=E_Scoping02.fsx SCFLAGS=" --test:ErrorRanges --flaterrors"                        # E_Scoping02.fsx
    [<Theory; FileInlineData("E_Scoping02.fsx")>]
    let ``E_Scoping02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3391, Line 9, Col 15, Line 9, Col 17, """This expression uses the implicit conversion 'System.Decimal.op_Implicit(value: int) : decimal' to convert type 'int' to type 'decimal'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".""")
        ]

    // SOURCE=E_TypeInference01.fs SCFLAGS="--test:ErrorRanges"                                 # E_TypeInference01.fs
    [<Theory; FileInlineData("E_TypeInference01.fs")>]
    let ``E_TypeInference01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
             (Error 764, Line 12, Col 15, Line 12, Col 24, "No assignment given for field 'Y' of type 'N.Blue'")
        ]

    // SOURCE=E_TypeInference01b.fs SCFLAGS="--test:ErrorRanges"                                # E_TypeInference01b.fs
    [<Theory; FileInlineData("E_TypeInference01b.fs")>]
    let ``E_TypeInference01b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 764, Line 13, Col 20, Line 13, Col 30, "No assignment given for field 'Y' of type 'N.M.Blue<_>'")
        ]

    // SOURCE=E_TypeInference02.fs SCFLAGS="--test:ErrorRanges"                                 # E_TypeInference02.fs
    [<Theory; FileInlineData("E_TypeInference02.fs")>]
    let ``E_TypeInference02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 656, Line 8, Col 9, Line 8, Col 47, "This record contains fields from inconsistent types")
        ]

    // SOURCE=E_UnitType01.fsx SCFLAGS="-a --test:ErrorRanges"                                  # E_UnitType01.fsx
    [<Theory; FileInlineData("E_UnitType01.fsx")>]
    let ``E_UnitType01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 10, Col 15, Line 10, Col 19, "The type 'unit' does not have 'null' as a proper value")
        ]

    // SOURCE=FieldBindingAfterWith01a.fs SCFLAGS=-a                                            # FieldBindingAfterWith01a.fs
    [<Theory; FileInlineData("FieldBindingAfterWith01a.fs")>]
    let ``FieldBindingAfterWith01a_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheckAsFsxAsLibrary

    // SOURCE=FieldBindingAfterWith01b.fs SCFLAGS=-a                                            # FieldBindingAfterWith01b.fs
    [<Theory; FileInlineData("FieldBindingAfterWith01b.fs")>]
    let ``FieldBindingAfterWith01b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheckAsFsxAsLibrary

    // SOURCE=FullyQualify01.fs                                                                 # FullyQualify01.fs
    [<Theory; FileInlineData("FullyQualify01.fs")>]
    let ``FullyQualify01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface01.fsx                                                                   # Interface01.fsx
    [<Theory; FileInlineData("Interface01.fsx")>]
    let ``Interface01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface_Empty.fsx                                                               # Interface_Empty.fsx
    [<Theory; FileInlineData("Interface_Empty.fsx")>]
    let ``Interface_Empty_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Interface_IComparable.fsx                                                         # Interface_IComparable.fsx
    [<Theory; FileInlineData("Interface_IComparable.fsx")>]
    let ``Interface_IComparable_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=ImplicitEquals01.fs                                                              # ImplicitEquals001.fs
    [<Theory; FileInlineData("ImplicitEquals01.fs")>]
    let ``ImplicitEquals01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=LongIdentifiers01.fsx                                                             # LongIdentifiers01.fsx
    [<Theory; FileInlineData("LongIdentifiers01.fsx")>]
    let ``LongIdentifiers01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Member01.fsx                                                                      # Member01.fsx
    [<Theory; FileInlineData("Member01.fsx")>]
    let ``Member01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=MutableFields01.fsx                                                               # MutableFields01.fsx
    [<Theory; FileInlineData("MutableFields01.fsx")>]
    let ``MutableFields01_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--nowarn:464"]
        |> verifyCompileAndRunSucceeds

    // SOURCE=MutableFields_SampleFromSpec02.fsx                                                # MutableFields_SampleFromSpec02.fsx
    [<Theory; FileInlineData("MutableFields_SampleFromSpec02.fsx")>]
    let ``MutableFields_SampleFromSpec02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=NoClashMemberIFaceMember.fs SCFLAGS="--warnaserror+"                              # NoClashMemberIFaceMember.fs
    [<Theory; FileInlineData("NoClashMemberIFaceMember.fs")>]
    let ``NoClashMemberIFaceMember_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_Equals.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE        # Overload_Equals.fs - fsi
    [<Theory; FileInlineData("Overload_Equals.fs")>]
    let ``Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_GetHashCode.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE   # Overload_GetHashCode.fs - fsi
    [<Theory; FileInlineData("Overload_GetHashCode.fs")>]
    let ``Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // NoMT SOURCE=Overload_ToString.fs COMPILE_ONLY=1 SCFLAGS=--warnaserror+ FSIMODE=PIPE	    # Overload_ToString.fs - fsi
    [<Theory; FileInlineData("Overload_ToString.fs")>]
    let ``Overload_ToString_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=SampleFromSpec01.fsx                                                              # SampleFromSpec01.fsx
    [<Theory; FileInlineData("SampleFromSpec01.fsx")>]
    let ``SampleFromSpec01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=SampleFromSpec03.fsx                                                              # SampleFromSpec03.fsx
    [<Theory; FileInlineData("SampleFromSpec03.fsx")>]
    let ``SampleFromSpec03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Scoping03.fsx                                                                     # Scoping03.fsx
    [<Theory; FileInlineData("Scoping03.fsx")>]
    let ``Scoping03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Scoping04.fsx                                                                     # Scoping04.fsx
    [<Theory; FileInlineData("Scoping04.fsx")>]
    let ``Scoping04_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning01.fs                                                                # RecordCloning01.fs
    [<Theory; FileInlineData("RecordCloning01.fs")>]
    let ``RecordCloning01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning02.fs                                                                # RecordCloning02.fs
    [<Theory; FileInlineData("RecordCloning02.fs")>]
    let ``RecordCloning02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=RecordCloning03.fs                                                                # RecordCloning03.fs
    [<Theory; FileInlineData("RecordCloning03.fs")>]
    let ``RecordCloning03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning01.fs                                                          # StructRecordCloning01.fs
    [<Theory; FileInlineData("StructRecordCloning01.fs")>]
    let ``StructRecordCloning01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning02.fs                                                          # StructRecordCloning02.fs
    [<Theory; FileInlineData("StructRecordCloning02.fs")>]
    let ``StructRecordCloning02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=StructRecordCloning03.fs                                                          # StructRecordCloning03.fs
    [<Theory; FileInlineData("StructRecordCloning03.fs")>]
    let ``StructRecordCloning03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=Syntax01.fs                                                                       # Syntax01.fs
    [<Theory; FileInlineData("Syntax01.fs")>]
    let ``Syntax01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=TypeInference01.fs                                                                # TypeInference01.fs
    [<Theory; FileInlineData("TypeInference01.fs")>]
    let ``TypeInference01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=TypeInference02.fs                                                                # TypeInference02.fs
    [<Theory; FileInlineData("TypeInference02.fs")>]
    let ``TypeInference02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; FileInlineData("UnitType01.fsx")>]
    let ``UnitType01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunSucceeds

    // SOURCE=W_Overrides01.fsx SCFLAGS="--test:ErrorRanges"                                    # W_Overrides01.fsx
    [<Theory; FileInlineData("W_Overrides01.fsx")>]
    let ``W_Overrides01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyTypeCheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 864, Line 11, Col 22, Line 11, Col 30, "This new member hides the abstract member 'System.Object.ToString() : string'. Rename the member or use 'override' instead.")
        ]

    [<Fact>]
    let ``Records field appears multiple times in this record expressions``() =
        FSharp """
    type RecTy = { B: string }

    let t1 = { B = "a"; B = "b" }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 17, "The field 'B' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 2``() =
        FSharp """
    type RecTy = { B: string }

    let t1 = { B = "a"; B = "b"; B = "c" }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 17, "The field 'B' appears multiple times in this record expression or pattern")
            (Error 668, Line 4, Col 25, Line 4, Col 26, "The field 'B' appears multiple times in this record expression or pattern")
        ]
    
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 3``() =
        FSharp """
    type RecTy = { A: int; B: int }

    let t1 = { A = 1; B = 4; A = 5; B = 4 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 17, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 4, Col 23, Line 4, Col 24, "The field 'B' appears multiple times in this record expression or pattern")
        ]
    
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 4``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }

    let t1 = { A = 1; C = ""; A = 0; B = 5 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 17, "The field 'A' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 5``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }

    let t1 = { A = 4; C = ""; A = 8; B = 4; A = 9 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 17, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 4, Col 31, Line 4, Col 32, "The field 'A' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 6``() =
        FSharp """
    type RecTy = { ``A``: int; B: int; C: string }

    let t1 = { ``A`` = 5; B = 6; A = 6; B = 6; C = "" }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 4, Col 16, Line 4, Col 21, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 4, Col 27, Line 4, Col 28, "The field 'B' appears multiple times in this record expression or pattern")
        ]
    
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 7``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }
    let t1 = { A = 5;  B = 6; C = "" }
    match t1 with
    | { A = 5; A = 6; C = "" } -> ()
    | _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 5, Col 9, Line 5, Col 10, "The field 'A' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 8``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }
    let t1 = { A = 5;  B = 6; C = "" }
    match t1 with
    | { A = 5; A = 6; A = 8 } -> ()
    | _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 5, Col 9, Line 5, Col 10, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 5, Col 16, Line 5, Col 17, "The field 'A' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 9``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }
    let t1 = { A = 5;  B = 6; C = "" }
    match t1 with
    | { A = 5; B = 6; A = 8; B = 9; C = "" } -> ()
    | _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 5, Col 9, Line 5, Col 10, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 5, Col 16, Line 5, Col 17, "The field 'B' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 10``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }
    let t1 = { A = 5;  B = 6; C = "" }
    match t1 with
    | { A = 4; C = ""; A = 8; B = 4; A = 9 } -> ()
    | _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 5, Col 9, Line 5, Col 10, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 5, Col 24, Line 5, Col 25, "The field 'A' appears multiple times in this record expression or pattern")
        ]
        
    [<Fact>]
    let ``Records field appears multiple times in this record expressions 11``() =
        FSharp """
    type RecTy = { A: int; B: int; C: string }
    let t1 = { A = 5;  B = 6; C = "" }
    match t1 with
    | { A = 4; C = ""; A = 8; B = 4; A = 9; B = 4 } -> ()
    | _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 668, Line 5, Col 9, Line 5, Col 10, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 5, Col 24, Line 5, Col 25, "The field 'A' appears multiple times in this record expression or pattern")
            (Error 668, Line 5, Col 31, Line 5, Col 32, "The field 'B' appears multiple times in this record expression or pattern")
        ]