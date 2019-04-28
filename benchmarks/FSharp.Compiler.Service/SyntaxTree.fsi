namespace FSharp.Compiler.Service

open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger

type FSharpErrorSeverity = FSharp.Compiler.SourceCodeServices.FSharpErrorSeverity

type SyntaxTree = 
    {
        FilePath: string
        ParseResult: ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) []
    }
