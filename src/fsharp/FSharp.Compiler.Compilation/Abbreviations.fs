﻿namespace FSharp.Compiler.Compilation

open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger

type internal FSharpErrorSeverity = FSharp.Compiler.SourceCodeServices.FSharpErrorSeverity
type internal CompilationErrorLogger = FSharp.Compiler.SourceCodeServices.CompilationErrorLogger
type internal CompilationGlobalsScope = FSharp.Compiler.SourceCodeServices.CompilationGlobalsScope
type internal ParseResult = ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) []
type internal SymbolEnv = FSharp.Compiler.SourceCodeServices.SymbolEnv
type FSharpSymbol = FSharp.Compiler.SourceCodeServices.FSharpSymbol

