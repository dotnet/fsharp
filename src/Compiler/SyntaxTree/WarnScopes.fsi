// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing

module internal WarnScopes =

    /// To be called from lex.fsl to register the line directives for warn scope processing
    val RegisterLineDirective: lexbuf: Lexbuf * fileIndex: int * line: int -> unit

    /// To be called from lex.fsl: #nowarn / #warnon directives are turned into warn scopes and saved.
    /// Warn scopes are always based on the original file index and line number, irrespective of any line directives.
    val ParseAndRegisterWarnDirective: lexbuf: Lexbuf -> unit

    /// Add the WarnScopes data of a lexed file into the diagnostics options
    val MergeInto: FSharpDiagnosticOptions -> range list -> Lexbuf -> unit

    /// Get the collected ranges of the warn directives
    val getDirectiveRanges: Lexbuf -> range list

    /// Get the ranges of any comments after warn directives
    val getCommentRanges: Lexbuf -> range list

    /// Clear the temporary warn scope related data in the Lexbuf
    val removeTemporaryData: Lexbuf -> unit

    /// Check if the range is inside a WarnScope.On scope
    val IsWarnon: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool

    /// Check if the range is inside a WarnScope.Off scope
    val IsNowarn: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool
