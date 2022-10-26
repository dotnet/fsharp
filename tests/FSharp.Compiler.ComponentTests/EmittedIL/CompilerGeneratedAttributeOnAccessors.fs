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

    [<Fact>]
    let ``Class property has CompilerGenerated attributes in IL`` () =
        classProperty
        |> compile
        |> verifyIL [
            """
            .method public hidebysig specialname
                        instance int32  get_Age() cil managed
                {
                  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
                  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

                  .maxstack  8
                  IL_0000:  ldarg.0
                  IL_0001:  ldfld      int32 Test/User::Age@
                  IL_0006:  ret
                }

                .method public hidebysig specialname
                        instance void  set_Age(int32 v) cil managed
                {
                  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
                  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

                  .maxstack  8
                  IL_0000:  ldarg.0
                  IL_0001:  ldarg.1
                  IL_0002:  stfld      int32 Test/User::Age@
                  IL_0007:  ret
                }
            """
        ]

    [<Theory>]
    [<InlineData("get_Age")>]
    [<InlineData("set_Age")>]
    let ``Class static property has CompilerGenerated attribute`` method =
        classStaticProperty
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod method
        |> should haveAttribute "CompilerGeneratedAttribute"

    [<Fact>]
    let ``Class static property has CompilerGenerated attributes in IL`` () =
        classStaticProperty
        |> compile
        |> verifyIL [
            """
             .method public specialname static int32
                        get_Age() cil managed
                {
                  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
                  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

                  .maxstack  8
                  IL_0000:  volatile.
                  IL_0002:  ldsfld     int32 Test/User::init@4
                  IL_0007:  ldc.i4.1
                  IL_0008:  bge.s      IL_0011

                  IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
                  IL_000f:  br.s       IL_0011

                  IL_0011:  ldsfld     int32 Test/User::Age@
                  IL_0016:  ret
                }

                .method public specialname static void
                        set_Age(int32 v) cil managed
                {
                  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
                  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

                  .maxstack  8
                  IL_0000:  volatile.
                  IL_0002:  ldsfld     int32 Test/User::init@4
                  IL_0007:  ldc.i4.1
                  IL_0008:  bge.s      IL_0011

                  IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
                  IL_000f:  br.s       IL_0011

                  IL_0011:  ldarg.0
                  IL_0012:  stsfld     int32 Test/User::Age@
                  IL_0017:  ret
                }
            """
        ]

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
        |> should haveAttribute "DebuggerNonUserCodeAttribute"


    [<Fact>]
    let ``Record setters should have CompilerGenerated attribute`` () =
        FSharp
            """
            module Test

            type User = { mutable Age : int }
            """
        |> compileAssembly
        |> getType "Test+User"
        |> getMethod "set_Age"
        |> should haveAttribute "CompilerGeneratedAttribute"
        |> should haveAttribute "DebuggerNonUserCodeAttribute"

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
        |> should haveAttribute "DebuggerNonUserCodeAttribute"
