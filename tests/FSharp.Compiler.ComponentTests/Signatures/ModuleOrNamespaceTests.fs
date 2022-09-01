module FSharp.Compiler.ComponentTests.Signatures.ModuleOrNamespaceTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

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

[<Fact>]
let ``Empty namespace`` () =
    FSharp
        """
namespace System

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Fantomas.Core.Tests")>]

do ()
"""
    |> printSignatures
    |> should equal "namespace System"
        
[<Fact>]
let ``Empty module`` () =
    FSharp
        """
module Foobar

do ()
"""
    |> printSignatures
    |> should equal "module Foobar"

[<Fact>]
let ``Two empty namespaces`` () =
    FSharp
        """
namespace Foo

do ()

namespace Bar

do ()
"""
    |> printSignatures
    |> prependNewline
    |> should equal """
namespace Foo
namespace Bar"""
