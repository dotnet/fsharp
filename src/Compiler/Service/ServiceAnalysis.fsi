// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

module public UnusedOpens =

    /// Get all unused open declarations in a file
    val getUnusedOpens:
        checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) -> Async<range list>

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

module public UnusedDeclarations =

    /// Get all unused declarations in a file
    val getUnusedDeclarations: checkFileResults: FSharpCheckFileResults * isScriptFile: bool -> Async<seq<range>>

module public UnnecessaryParentheses =

    /// Gets the ranges of all unnecessary pairs of parentheses in a file.
    ///
    /// Note that this may include pairs of nested ranges each of whose
    /// lack of necessity depends on the other's presence, such
    /// that it is valid to remove either set of parentheses but not both, e.g.:
    ///
    /// (x.M(y)).N → (x.M y).N ↮ x.M(y).N
    val getUnnecessaryParentheses: getSourceLineStr: (int -> string) -> parsedInput: ParsedInput -> Async<range seq>
