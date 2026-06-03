// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

/// Controls whether analysis (unused opens, unused declarations) runs on files that contain errors.
[<RequireQualifiedAccess>]
type AnalysisScope =
    /// Run analysis on all files regardless of error diagnostics (historical FCS default).
    | AllFiles
    /// Skip analysis on files that contain any Error-severity diagnostic, avoiding false positives.
    | FilesWithoutErrors

[<AbstractClass; Sealed>]
type UnusedOpens =

    /// Get all unused open declarations in a file
    static member getUnusedOpens:
        checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) * ?analysisScope: AnalysisScope ->
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

    /// Get all unused declarations in a file
    static member getUnusedDeclarations:
        checkFileResults: FSharpCheckFileResults * isScriptFile: bool * ?analysisScope: AnalysisScope ->
            Async<seq<range>>
