module Signatures.ModuleOrNamespaceTests

open Xunit
open FSharp.Test.Compiler
open Signatures.TestHelpers

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
    |> assertEqualIgnoreLineEnding
        """
namespace Foo.Types

  type Area = | Area of string * int
namespace Foo.Other

  [<Class>]
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
    |> assertEqualIgnoreLineEnding
        """
namespace Hey.There

  [<Class>]
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
    |> assertEqualIgnoreLineEnding
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
    |> assertEqualIgnoreLineEnding "namespace System"
        
[<Fact>]
let ``Empty module`` () =
    FSharp
        """
module Foobar

do ()
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding "module Foobar"

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
    |> assertEqualIgnoreLineEnding """
namespace Foo
namespace Bar"""

// https://github.com/dotnet/fsharp/issues/13832
[<Fact>]
let ``Recursive module with do binding`` () =
    FSharp
        """
module rec Foobar

do ()
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding "\nmodule Foobar"

[<Fact>]
let ``Empty namespace module`` () =
    FSharp
        """
namespace rec Foobar

do ()
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding "namespace Foobar"

[<Fact>]
let ``Attribute on nested module`` () =
    FSharp
        """
namespace MyApp.Types

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Area =
    type Meh =  class end
"""
    |> printSignatures
    |> prependNewline
    |> assertEqualIgnoreLineEnding """
namespace MyApp.Types

  [<RequireQualifiedAccess; CompilationRepresentation (enum<CompilationRepresentationFlags> (4))>]
  module Area =

    type Meh =
      class end"""

// https://github.com/dotnet/fsharp/issues/12067
[<Fact>]
let ``External FSharp namespace type prefix is preserved in signature`` () =
    let externalLib =
        FSharp
            """
namespace FSharp.Control

type AsyncSeq<'t> =
    member this.Length: int = 0

    static member BufferByCount: count: int -> source: AsyncSeq<'t> -> AsyncSeq<'t array> =
        failwith "todo"
"""

    FSharp
        """
namespace FSharp.MyStuff

module Library =
    open FSharp.Control

    let batch (s: AsyncSeq<'t>) = s
"""
    |> withReferences [ externalLib ]
    |> printSignatures
    |> prependNewline
    |> assertEqualIgnoreLineEnding
        """
namespace FSharp.MyStuff

  module Library =

    val batch: s: FSharp.Control.AsyncSeq<'t> -> FSharp.Control.AsyncSeq<'t>"""

// https://github.com/dotnet/fsharp/issues/13810
[<Fact>]
let ``Literal value in attribute uses literal name`` () =
    FSharp
        """
module Maybe

open System.ComponentModel

[<Literal>]
let A = "A"

module SubModule =
    type Foo() =
        [<Category(A)>]
        member this.Meh () = ()
"""
    |> printSignatures
    |> prependNewline
    |> assertEqualIgnoreLineEnding
        """

module Maybe

[<Literal>]
val A: string = "A"

module SubModule =

  type Foo =

    new: unit -> Foo

    [<System.ComponentModel.Category (A)>]
    member Meh: unit -> unit"""

// https://github.com/dotnet/fsharp/issues/13810
[<Fact>]
let ``Multiple literal values in attribute tuple args use literal names`` () =
    let actual =
        FSharp
            """
module TestMod

open System

[<AttributeUsage(AttributeTargets.All)>]
type TwoArgAttribute(a: string, b: string) =
    inherit Attribute()

[<Literal>]
let First = "first"

[<Literal>]
let Second = "second"

type Foo() =
    [<TwoArg(First, Second)>]
    member _.Bar() = ()
"""
        |> printSignatures

    // The attribute argument line should show literal names, not constant values
    let attrLine = actual.Split('\n') |> Array.find (fun l -> l.Contains("TwoArg") && l.Contains("[<"))
    Assert.Contains("First", attrLine)
    Assert.Contains("Second", attrLine)
    Assert.DoesNotContain("\"first\"", attrLine)
    Assert.DoesNotContain("\"second\"", attrLine)

// https://github.com/dotnet/fsharp/issues/13810 — known limitation:
// Qualified literal references (e.g. Module.Literal) are not recovered;
// the constant value is shown instead.
[<Fact>]
let ``Qualified literal in attribute shows constant value`` () =
    FSharp
        """
module TestMod

open System.ComponentModel

module Constants =
    [<Literal>]
    let A = "QualifiedValue"

type Foo() =
    [<Category(Constants.A)>]
    member this.Meh () = ()
"""
    |> printSignatures
    |> prependNewline
    |> fun actual ->
        // Qualified literal refs are not recovered — constant value is shown
        Assert.Contains("QualifiedValue", actual)

// https://github.com/dotnet/fsharp/issues/15389
[<Fact>]
let ``Backtick in identifier is properly escaped in signature`` () =
    FSharp
        """
module Foo

let ```a` b`` (a:int) (b:int) = ()
"""
    |> printSignatures
    |> prependNewline
    |> assertEqualIgnoreLineEnding
        """

module Foo

val ```a` b`` : a: int -> b: int -> unit"""

// Found by corpus-wide roundtrip sweep. Fixed: #19593
[<Fact>]
let ``Namespace global with class type roundtrips`` () =
    let implSource =
        """
namespace global

type Foo() = 
    member _.X = 1
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// Namespace global with nested module — fixed by moving ns global detection into NicePrint
[<Fact>]
let ``Namespace global with module roundtrips`` () =
    let implSource =
        """
namespace global

type Foo() = 
    member _.X = 1

module Utils = 
   let f (x:Foo) = x
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore
