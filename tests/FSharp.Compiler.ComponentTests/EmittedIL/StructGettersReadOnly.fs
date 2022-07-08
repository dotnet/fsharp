namespace FSharp.Compiler.ComponentTests.EmittedIL

open Microsoft.FSharp.Core
open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ReflectionHelper

module ``Struct getters readonly`` =

    let structRecord =
        FSharp
            """
            module Test

            [<Struct>] type MyRecord = { MyField : int }
            """

    [<Fact>]
    let ``Struct record has readonly attribute on getter`` () =
        structRecord
        |> compileAssembly
        |> getType "Test+MyRecord"
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Struct record has readonly attribute on getter in IL`` () =
        structRecord
        |> compile
        |> shouldSucceed
        |> verifyIL [ """
            .method public hidebysig specialname
                    instance int32  get_MyField() cil managed
            {
              .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

              .maxstack  8
              IL_0000:  ldarg.0
              IL_0001:  ldfld      int32 Test/MyRecord::MyField@
              IL_0006:  ret
            }""" ]

    let nonStructRecord =
        FSharp
            """
            module Test

            type MyRecord = { MyField : int }
            """

    [<Fact>]
    let ``Non-struct record doesn't have readonly getters`` () =
        nonStructRecord
        |> compileAssembly
        |> getType "Test+MyRecord"
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Non-struct record doesn't have readonly getters in IL`` () =
        nonStructRecord
        |> compile
        |> shouldSucceed
        |> verifyIL [ """
            .method public hidebysig specialname
                    instance int32  get_MyField() cil managed
            {

              .maxstack  8
              IL_0000:  ldarg.0
              IL_0001:  ldfld      int32 Test/MyRecord::MyField@
              IL_0006:  ret
            } """ ]

    [<Fact>]
    let ``Struct anonymous record has readonly attribute on getter`` () =
        FSharp
            """
            module Test

            let myRecord = struct {| MyField = 3 |}
            """
        |> compileAssembly
        |> getFirstAnonymousType
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Non-struct anonymous record doesn't have readonly attribute on getter`` () =
        FSharp
            """
            module Test

            let myRecord = {| MyField = 3 |}
            """
        |> compileAssembly
        |> getFirstAnonymousType
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Struct has readonly getters`` () =
        FSharp
            """
            module Test

            [<Struct>]
            type MyStruct =
                val MyField: int
            """
        |> compileAssembly
        |> getType "Test+MyStruct"
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Custom getter on a struct doesn't have readonly attribute`` () =
        FSharp
            """
            module Test

            [<Struct>]
            type MyStruct =
                val mutable x: int
                member this.MyField with get () = this.x <- 4
            """
        |> compileAssembly
        |> getType "Test+MyStruct"
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"
