// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing

module internal WarnScopes =

    /// For use in lex.fsl: #nowarn / #warnon directives are turned into warn scopes and saved in lexbuf.BufferLocalStore
    val ParseAndSaveWarnDirectiveLine: lexbuf: Lexbuf -> unit
    
    /// Clear the warn scopes in lexbuf.BufferLocalStore for reuse of the lexbuf
    val ClearLexbufStore: Lexbuf -> unit

    /// Add the warn scopes of a lexed file into the diagnostics options
    val MergeInto: FSharpDiagnosticOptions -> Lexbuf -> unit

    /// Check if the range is inside a WarnScope.On scope
    val IsWarnon: WarnScopeMap -> warningNumber: int -> mo: range option -> bool

    /// Check if the range is inside a WarnScope.Off scope
    /// compatible = compatible with earlier (< F# 10.0) inconsistent interaction between #line and #nowarn
    val IsNowarn: WarnScopeMap -> warningNumber: int -> mo: range option -> compatible: bool -> bool
