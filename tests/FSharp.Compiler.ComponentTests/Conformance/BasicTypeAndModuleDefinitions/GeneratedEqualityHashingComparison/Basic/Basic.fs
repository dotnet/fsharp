// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

    // SOURCE=Arrays.fsx                                                        # Arrays.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Arrays.fsx"|])>]
    let``Arrays_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Comparison01.fs                                                   # Comparison01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Comparison01.fs"|])>]
    let``Comparison01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CustomEquality01.fs                                               # CustomEquality01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CustomEquality01.fs"|])>]
    let``CustomEquality01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 988, Line 37, Col 1, Line 37, Col 1, "Main module of program is empty: nothing will happen when it is run")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CustomEqualityEquals01.fs"|])>]
    let``E_CustomEqualityEquals01_fs`` compilation =
        compilation
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CustomEqualityGetHashCode01.fs"|])>]
    let``E_CustomEqualityGetHashCode01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 344, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 13, Col 6, Line 13, Col 7, "The struct, record or union type 'R' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 13, Col 6, Line 13, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
            (Error 54, Line 13, Col 6, Line 13, Col 7, "This type is 'abstract' since some abstract members have not been given an implementation. If this is intentional then add the '[<AbstractClass>]' attribute to your type.")
            (Error 344, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 17, Col 6, Line 17, Col 7, "The struct, record or union type 'U' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 17, Col 6, Line 17, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
            (Error 54, Line 17, Col 6, Line 17, Col 7, "This type is 'abstract' since some abstract members have not been given an implementation. If this is intentional then add the '[<AbstractClass>]' attribute to your type.")
            (Error 344, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.GetHashCode' or 'Object.Equals'. You must apply the 'CustomEquality' attribute to the type")
            (Warning 345, Line 21, Col 6, Line 21, Col 7, "The struct, record or union type 'S' has an explicit implementation of 'Object.GetHashCode'. Consider implementing a matching override for 'Object.Equals(obj)'")
            (Error 359, Line 21, Col 6, Line 21, Col 7, "More than one override implements 'GetHashCode: unit -> int'")
            (Error 54, Line 21, Col 6, Line 21, Col 7, "This type is 'abstract' since some abstract members have not been given an implementation. If this is intentional then add the '[<AbstractClass>]' attribute to your type.")
           ]

    // SOURCE=E_ExceptionsNoComparison.fs SCFLAGS="--test:ErrorRanges"          # E_ExceptionsNoComparison.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExceptionsNoComparison.fs"|])>]
    let``E_ExceptionsNoComparison_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 9, Line 9, Col 15, "The type 'exn' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
            (Error 1, Line 10, Col 9, Line 10, Col 15, "The type 'exn' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]


    // SOURCE=EqualOnTuples01.fs                                                # EqualOnTuples01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualOnTuples01.fs"|])>]
    let``EqualOnTuples01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Equality01.fs                                                     # Equality01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Equality01.fs"|])>]
    let``Equality01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed


    // SOURCE=Exceptions.fsx                                                    # Exceptions.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Exceptions.fsx"|])>]
    let``Exceptions_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Generated_Record.fsx                                              # Generated_Record.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Generated_Record.fsx"|])>]
    let``Generated_Record_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Hashing01.fs                                                      # Hashing01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Hashing01.fs"|])>]
    let``Hashing01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Lists.fsx                                                         # Lists.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Lists.fsx"|])>]
    let``Lists_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Class.fsx                                          # NeverGenerated_Class.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NeverGenerated_Class.fsx"|])>]
    let``NeverGenerated_Class_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Delegate.fsx                                       # NeverGenerated_Delegate.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NeverGenerated_Delegate.fsx"|])>]
    let``NeverGenerated_Delegate_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Enum.fsx                                           # NeverGenerated_Enum.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NeverGenerated_Enum.fsx"|])>]
    let``NeverGenerated_Enum_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NeverGenerated_Interface.fsx                                      # NeverGenerated_Interface.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NeverGenerated_Interface.fsx"|])>]
    let``NeverGenerated_Interface_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Options.fsx                                                       # Options.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Options.fsx"|])>]
    let``Options_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Sample_Records.fsx                                                # Sample_Records.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Sample_Records.fsx"|])>]
    let``Sample_Records_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Sample_Tuples.fsx                                                 # Sample_Tuples.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Sample_Tuples.fsx"|])>]
    let``Sample_Tuples_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Structs.fsx                                                       # Structs.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs.fsx"|])>]
    let``Structs_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Unions.fsx                                                        # Unions.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Unions.fsx"|])>]
    let``Unions_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_CustomEqualityEquals01.fs SCFLAGS="--test:ErrorRanges"          # E_CustomEqualityEquals01.fs
