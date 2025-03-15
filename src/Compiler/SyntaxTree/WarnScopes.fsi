// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing

module internal WarnScopes =

    /// To be called during lexing to register the line directives for warn scope processing.
    val internal RegisterLineDirective: lexbuf: Lexbuf * fileIndex: int * line: int -> unit

    /// To be called during lexing to save #nowarn / #warnon directives.
    val ParseAndRegisterWarnDirective: lexbuf: Lexbuf -> unit

    /// To be called after lexing a file to create warn scopes from the stored line and
    /// warn directives and to add them to the warn scopes from other files in the diagnostics options.
    /// Note that isScript and subModuleRanges are needed only to avoid breaking changes for previous language versions.
    val MergeInto: FSharpDiagnosticOptions -> isScript: bool -> subModuleRanges: range list -> Lexbuf -> unit

    /// Get the collected ranges of the warn directives
    val getDirectiveTrivia: Lexbuf -> WarnDirectiveTrivia list

    /// Get the ranges of comments after warn directives
    val getCommentTrivia: Lexbuf -> CommentTrivia list

    /// Check if the range is inside a "warnon" scope for the given warning number.
    val IsWarnon: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool

    /// Check if the range is inside a "nowarn" scope for the given warning number.
    val IsNowarn: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool
