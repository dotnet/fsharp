namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module ExtensionMethodTests =

    [<Fact>]
    let ``Extension method with toplevel attribute on type`` () =
        Fsx
            """
open System.Runtime.CompilerServices

[<Extension>]
type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Extension method without toplevel attribute on type`` () =
        Fsx
            """
open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``F# CSharpStyleExtensionMethod consumed in C#`` () =
        let fsharp =
            FSharp
                """
module Hello

open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``F# CSharpStyleExtensionMethod defined in top level module with attribute consumed in C#`` () =
        let fsharp =
            FSharp
                """
namespace Hello

open System.Runtime.CompilerServices

[<Extension>]
module Foo =
    [<Extension>]
    let PlusOne (a:int) : int = a + 1
"""
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``F# CSharpStyleExtensionMethod defined in top level module without attribute consumed in C#`` () =
        let fsharp =
            FSharp
                """
namespace Hello

open System.Runtime.CompilerServices

module Foo =
    [<Extension>]
    let PlusOne (a:int) : int = a + 1
"""
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed

// TODO: verify nested module versus toplevel modules, recursive modules versus non recursive modules
// This may require some refactoring to extract some shared logic.