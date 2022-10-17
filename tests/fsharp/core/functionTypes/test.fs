namespace Fantomas.Core

module Context =
    type Context = { SourceCode: string }

namespace FSharp.Compiler

module Syntax =

    type SynExpr =
        | IfThenElse
        | While

module Text =
    type Range =
        struct
            val startLine: int
            val startColumn: int
            val endLine: int
            val endColumn: int
        end

namespace Fantomas.Core

module internal CodePrinter =

    open FSharp.Compiler
    open FSharp.Compiler.Syntax
    open FSharp.Compiler.Text
    open Fantomas.Core.Context

    type ASTContext =
        { Meh: bool }
        static member Default = { Meh = false }

    let rec genExpr (e: SynExpr) (ctx: Context) = ctx

    and genLambdaArrowWithTrivia
        (bodyExpr: SynExpr -> Context -> Context)
        (body: SynExpr)
        (arrowRange: Range option)
        : Context -> Context =
        id
