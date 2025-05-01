// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.GeneratedEqualityHashingComparison

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

    // SOURCE=Arrays.fsx                                                        # Arrays.fsx
    [<Theory; FileInlineData("Arrays.fsx")>]
    let``Arrays_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Comparison01.fs                                                   # Comparison01.fs
    [<Theory; FileInlineData("Comparison01.fs")>]
    let``Comparison01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CustomEquality01.fs                                               # CustomEquality01.fs
    [<Theory; FileInlineData("CustomEquality01.fs")>]
    let``CustomEquality01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 37, Col 1, Line 37, Col 1, "Main module of program is empty: nothing will happen when it is run")
        ]

    [<Theory; FileInlineData("E_CustomEqualityEquals01.fs")>]
    let``E_CustomEqualityEquals01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 344, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 346, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.Equals'. Consider implementing a matching override for 'Object.GetHashCode()'")
            (Error 344, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 346, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.Equals'. Consider implementing a matching override for 'Object.GetHashCode()'")
            (Error 344, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 346, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.Equals'. Consider implementing a matching override for 'Object.GetHashCode()'")
        ]

    // SOURCE=E_CustomEqualityGetHashCode01.fs SCFLAGS="--test:ErrorRanges"     # E_CustomEqualityGetHashCode01.fs
    [<Theory; FileInlineData("E_CustomEqualityGetHashCode01.fs")>]
    let``E_CustomEqualityGetHashCode01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 344, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 13, Col 6, Line 13, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
            (Error 344, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 17, Col 6, Line 17, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
            (Error 344, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 21, Col 6, Line 21, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
           ]

    // SOURCE=E_ExceptionsNoComparison.fs SCFLAGS="--test:ErrorRanges"          # E_ExceptionsNoComparison.fs
    [<Theory; FileInlineData("E_ExceptionsNoComparison.fs")>]
    let``E_ExceptionsNoComparison_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 9, Line 9, Col 15, "The type 'exn' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
            (Error 1, Line 10, Col 9, Line 10, Col 15, "The type 'exn' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]


    // SOURCE=EqualOnTuples01.fs                                                # EqualOnTuples01.fs
    [<Theory; FileInlineData("EqualOnTuples01.fs")>]
    let``EqualOnTuples01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Equality01.fs                                                     # Equality01.fs
    [<Theory; FileInlineData("Equality01.fs")>]
    let``Equality01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed


    // SOURCE=Exceptions.fsx                                                    # Exceptions.fsx
    [<Theory; FileInlineData("Exceptions.fsx")>]
    let``Exceptions_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Generated_Record.fsx                                              # Generated_Record.fsx
    [<Theory; FileInlineData("Generated_Record.fsx")>]
    let``Generated_Record_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Hashing01.fs                                                      # Hashing01.fs
    [<Theory; FileInlineData("Hashing01.fs")>]
    let``Hashing01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Lists.fsx                                                         # Lists.fsx
    [<Theory; FileInlineData("Lists.fsx")>]
    let``Lists_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Class.fsx                                          # NeverGenerated_Class.fsx
    [<Theory; FileInlineData("NeverGenerated_Class.fsx")>]
    let``NeverGenerated_Class_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Delegate.fsx                                       # NeverGenerated_Delegate.fsx
    [<Theory; FileInlineData("NeverGenerated_Delegate.fsx")>]
    let``NeverGenerated_Delegate_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Enum.fsx                                           # NeverGenerated_Enum.fsx
    [<Theory; FileInlineData("NeverGenerated_Enum.fsx")>]
    let``NeverGenerated_Enum_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Interface.fsx                                      # NeverGenerated_Interface.fsx
    [<Theory; FileInlineData("NeverGenerated_Interface.fsx")>]
    let``NeverGenerated_Interface_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Options.fsx                                                       # Options.fsx
    [<Theory; FileInlineData("Options.fsx")>]
    let``Options_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Sample_Records.fsx                                                # Sample_Records.fsx
    [<Theory; FileInlineData("Sample_Records.fsx")>]
    let``Sample_Records_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Sample_Tuples.fsx                                                 # Sample_Tuples.fsx
    [<Theory; FileInlineData("Sample_Tuples.fsx")>]
    let``Sample_Tuples_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Structs.fsx                                                       # Structs.fsx
    [<Theory; FileInlineData("Structs.fsx")>]
    let``Structs_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Unions.fsx                                                        # Unions.fsx
    [<Theory; FileInlineData("Unions.fsx")>]
    let``Unions_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_CustomEqualityEquals01.fs SCFLAGS="--test:ErrorRanges"          # E_CustomEqualityEquals01.fs
