// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IComparison =

    // SOURCE=DU.fs                                             # DU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DU.fs"|])>]
    let``DU_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:342"]
        |> compileAndRun
        |> shouldSucceed

    // SOURCE=Record.fs                                         # Record.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Record.fs"|])>]
    let``Record_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:342"]
        |> compileAndRun
        |> shouldSucceed

    // SOURCE=Struct.fs                                         # Struct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Struct.fs"|])>]
    let``Struct_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:342"]
        |> compileAndRun
        |> shouldSucceed

    // SOURCE=W_ImplIComparable.fs                              # W_ImplIComparable.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_ImplIComparable.fs"|])>]
    let``W_ImplIComparable_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 342, Line 10, Col 6, Line 10, Col 7, "The type 'S' implements 'System.IComparable'. Consider also adding an explicit override for 'Object.Equals'")
            (Warning 343, Line 15, Col 6, Line 15, Col 7, "The type 'C' implements 'System.IComparable' explicitly but provides no corresponding override for 'Object.Equals'. An implementation of 'Object.Equals' has been automatically provided, implemented via 'System.IComparable'. Consider implementing the override 'Object.Equals' explicitly")
            (Warning 988, Line 19, Col 1, Line 19, Col 1, "Main module of program is empty: nothing will happen when it is run")
        ]
