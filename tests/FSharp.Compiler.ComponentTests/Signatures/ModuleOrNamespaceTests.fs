module FSharp.Compiler.ComponentTests.Signatures.ModuleOrNamespaceTests

open System
open Xunit
open FsUnit
open FSharp.Test.Compiler

let inline private prependNewline v = String.Concat("\n", v)

let equal x =
    let x =
        match box x with
        | :? String as s -> s.Replace("\r\n", "\n") |> box
        | x -> x

    equal x

[<Fact>]
let ``Type from shared namespace`` () =
    FSharp
        """
namespace Foo.Types

type Area = | Area of string * int

namespace Foo.Other

type Map<'t,'v> =
    member this.Calculate : Foo.Types.Area = failwith "todo"
"""
    |> printSignatures
    |> prependNewline
    |> should
        equal
        """
namespace Foo.Types

  type Area = | Area of string * int
namespace Foo.Other

  type Map<'t,'v> =

    member Calculate: Foo.Types.Area"""

[<Fact>]
let ``Return type used in own type definition`` () =
    FSharp
        """
namespace Hey.There

type Foo =
    static member Zero : Foo = failwith "todo"
"""
    |> printSignatures
    |> prependNewline
    |> should
        equal
        """
namespace Hey.There

  type Foo =

    static member Zero: Foo"""

[<Fact>]
let ``Function types`` () =
    FSharp
        """
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
        id"""
    |> printSignatures
    |> prependNewline
    |> should
        equal
        """
namespace Fantomas.Core

  module Context =

    type Context =
      { SourceCode: string }
namespace FSharp.Compiler

  module Syntax =

    type SynExpr =
      | IfThenElse
      | While

  module Text =

    [<Struct>]
    type Range =

      val startLine: int

      val startColumn: int

      val endLine: int

      val endColumn: int
namespace Fantomas.Core

  module internal CodePrinter =

    type ASTContext =
      { Meh: bool }

      static member Default: ASTContext

    val genExpr: e: FSharp.Compiler.Syntax.SynExpr -> ctx: Context.Context -> Context.Context

    val genLambdaArrowWithTrivia: bodyExpr: (FSharp.Compiler.Syntax.SynExpr -> Context.Context -> Context.Context) -> body: FSharp.Compiler.Syntax.SynExpr -> arrowRange: FSharp.Compiler.Text.Range option -> (Context.Context -> Context.Context)"""
