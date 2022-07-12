namespace FSharp.Compiler.ComponentTests.EmittedIL

open Microsoft.FSharp.Core
open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ReflectionHelper

module ``Struct getters readonly`` =

    let structRecord =
        FSharp
            """
            module TestStructRecord

            [<Struct>] type MyRecord = { MyField : int }
            """

    let nonStructRecord =
        FSharp
            """
            module TestNonStructRecord

            type MyRecord = { MyField : int }
            """

    let structAnonRecord =
        FSharp
            """
            module TestStructAnonRecord

            let myRecord = struct {| MyField = 3 |}
            """

    let nonStructAnonRecord =
        FSharp
            """
            module TestNonStructAnonRecord

            let myRecord = {| MyField = 3 |}
            """

    let structNonRecord =
        FSharp
            """
            module TestStructNonRecord

            [<Struct>]
            type MyStruct(myField: int) =
                member _.MyField = myField
            """

    let structNonRecordVal =
        FSharp
            """
            module TestStructNonRecordVal

            [<Struct>]
            type MyStruct =
                val MyField: int
            """

    let structWithCustomGetter =
        FSharp
            """
            module TestStructWithCustomGetter

            [<Struct>]
            type MyStruct =
                val mutable x: int
                member this.MyField with get () = this.x <- 4
            """

    [<Fact>]
    let ``Struct record has readonly attribute on getter`` () =
        structRecord
        |> compileAssembly
        |> getType "TestStructRecord+MyRecord"
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Struct anonymous record has readonly attribute on getter`` () =
        structAnonRecord
        |> compileAssembly
        |> getFirstAnonymousType
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Non-struct anonymous record doesn't have readonly attribute on getter`` () =
        nonStructAnonRecord
        |> compileAssembly
        |> getFirstAnonymousType
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Non-struct record doesn't have readonly getters`` () =
        nonStructRecord
        |> compileAssembly
        |> getType "TestNonStructRecord+MyRecord"
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Struct has readonly getters`` () =
        structNonRecord
        |> compileAssembly
        |> getType "TestStructNonRecord+MyStruct"
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Struct val has readonly getter`` () =
        structNonRecordVal
        |> compileAssembly
        |> getType "TestStructNonRecordVal+MyStruct"
        |> getMethod "get_MyField"
        |> should haveAttribute "IsReadOnlyAttribute"

    [<Fact>]
    let ``Custom getter on a struct doesn't have readonly attribute`` () =
        structWithCustomGetter
        |> compileAssembly
        |> getType "TestStructWithCustomGetter+MyStruct"
        |> getMethod "get_MyField"
        |> shouldn't haveAttribute "IsReadOnlyAttribute"

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
              IL_0001:  ldfld      int32 TestStructRecord/MyRecord::MyField@
              IL_0006:  ret
            }""" ]

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
              IL_0001:  ldfld      int32 TestNonStructRecord/MyRecord::MyField@
              IL_0006:  ret
            } """ ]
