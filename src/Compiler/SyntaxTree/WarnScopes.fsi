// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

module internal WarnScopes =

    /// For use in lex.fsl: #nowarn / #warnon directives are turned into warn scopes and saved in lexbuf.BufferLocalStore
    val ParseAndSaveWarnDirectiveLine: lexbuf: UnicodeLexing.Lexbuf -> unit

    /// Get the collected warn scopes out of the lexbuf.BufferLocalStore
    val FromLexbuf: lexbuf: UnicodeLexing.Lexbuf -> WarnScopeMap

    /// Add the warn scopes of a lexed file into the diagnostics options
    val MergeInto: FSharpDiagnosticOptions -> WarnScopeMap -> unit

    /// Check if the range is inside a WarnScope.On scope
    val IsWarnon: WarnScopeMap -> warningNumber: int -> mo: range option -> bool

    /// Check if the range is inside a WarnScope.Off scope
    val IsNowarn: WarnScopeMap -> warningNumber: int -> mo: range option -> bool -> bool
