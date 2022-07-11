namespace FSharp.Compiler.ComponentTests.EmittedIL

open Microsoft.FSharp.Core
open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ReflectionHelper

module ``Auto-generated accessors have CompilerGenerated attribute`` =

    let classProperty =
        FSharp
            """
            module Test

            type User() =
                member val Age = 0 with get, set
            """

    let classStaticProperty =
        FSharp
            """
            module Test

            type User() =
                static member val Age = 0 with get, set
            """

    [<Theory>]
    [<InlineData("get_Age")>]
    [<InlineData("set_Age")>]
    let ``Class property has CompilerGenerated attribute`` method =
        classProperty
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod method
        |> should haveAttribute "CompilerGeneratedAttribute"

    [<Theory>]
    [<InlineData("get_Age")>]
    [<InlineData("set_Age")>]
    let ``Class static property has CompilerGenerated attribute`` method =
        classStaticProperty
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod method
        |> should haveAttribute "CompilerGeneratedAttribute"

    [<Theory>]
    [<InlineData("get_Age")>]
    [<InlineData("set_Age")>]
    let ``Custom accessor shouldn't have CompilerGenerated attribute`` method =
        FSharp
            """
            module Test

            type User() =
                member this.Age
                    with get() = 9000
                    and set (value: int) = ()
            """
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod method
        |> shouldn't haveAttribute "CompilerGeneratedAttribute"

    [<Fact>]
    let ``Record getters should have CompilerGenerated attribute`` () =
        FSharp
            """
            module Test

            type User = { Age : int }
            """
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod "get_Age"
        |> should haveAttribute "CompilerGeneratedAttribute"

    [<Fact>]
    let ``Anonymous record getters should have CompilerGenerated attribute`` () =
        FSharp
            """
            module Test

            let user = {| Age = 9000 |}
            """
        |> compileAssembly
        |> getFirstAnonymousType
        |> getMethod "get_Age"
        |> should haveAttribute "CompilerGeneratedAttribute"
