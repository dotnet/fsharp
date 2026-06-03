// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

[<AbstractClass; Sealed>]
type UnusedOpens =

    /// Get all unused open declarations in a file.
    /// Set <paramref name="includeFilesWithErrors"/> to <c>false</c> to skip analysis on files with any Error-severity diagnostic.
    static member getUnusedOpens:
        checkFileResults: FSharpCheckFileResults *
        getSourceLineStr: (int -> string) *
        ?includeFilesWithErrors: bool ->
            Async<range list>

module public SimplifyNames =

    /// Data for use in finding unnecessarily-qualified names and generating diagnostics to simplify them
    type SimplifiableRange =
        {
            /// The range of a name that can be simplified
            Range: range

            /// The relative name that can be applied to a simplifiable name
            RelativeName: string
        }

    /// Get all ranges that can be simplified in a file
    val getSimplifiableNames:
        checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) -> Async<seq<SimplifiableRange>>

[<AbstractClass; Sealed>]
type UnusedDeclarations =

    /// Get all unused declarations in a file.
    /// Set <paramref name="includeFilesWithErrors"/> to <c>false</c> to skip analysis on files with any Error-severity diagnostic.
    static member getUnusedDeclarations:
        checkFileResults: FSharpCheckFileResults * isScriptFile: bool * ?includeFilesWithErrors: bool ->
            Async<seq<range>>
