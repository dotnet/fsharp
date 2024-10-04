// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing

module internal WarnScopes =

    /// To be called from lex.fsl to register the line directives for warn scope processing
    val RegisterLineDirective: lexbuf: Lexbuf * previousFileIndex: int * fileIndex: int * line: int -> unit

    /// To be called from lex.fsl: #nowarn / #warnon directives are turned into warn scopes and saved
    val ParseAndRegisterWarnDirective: lexbuf: Lexbuf -> unit

    /// Add the WarnScopes data of a lexed file into the diagnostics options
    val MergeInto: FSharpDiagnosticOptions -> Lexbuf -> unit

    /// Check if the range is inside a WarnScope.On scope
    val IsWarnon: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool

    /// Check if the range is inside a WarnScope.Off scope
    val IsNowarn: FSharpDiagnosticOptions -> warningNumber: int -> mo: range option -> bool
