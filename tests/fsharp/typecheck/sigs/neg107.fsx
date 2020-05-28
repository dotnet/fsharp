#load "neg107.generated.fsx"

namespace System.Runtime.CompilerServices

    open System
    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices
    [<AttributeUsage(AttributeTargets.All,AllowMultiple=false)>]
    [<Sealed>]
    type IsReadOnlyAttribute() =
        inherit System.Attribute()

    [<AttributeUsage(AttributeTargets.All,AllowMultiple=false)>]
    [<Sealed>]
    type IsByRefLikeAttribute() =
        inherit System.Attribute()

namespace Test

    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices
    open System
    
    module Span_Negative1 =
        let TestClosure1 (a: inref<int>) = id (fun () -> a)
        let TestClosure1b ([<In;  IsReadOnly>] a: byref<int>) = id (fun () -> a)
        let TestClosure2 (a: Span<int>) = id (fun () -> a)
        let TestClosure3 (a: ReadOnlySpan<int>) = id (fun () -> a)

        let TestAsyncClosure1 (a: inref<int>) = async { return a }
        let TestAsyncClosure1b ([<In;  IsReadOnly>] a: byref<int>) = async { return a }
        let TestAsyncClosure2 (a: Span<int>) = async { return a }
        let TestAsyncClosure3 (a: ReadOnlySpan<int>) = async { return a }

    module Span_Negative2 =
        let TestLocal1 () = let x = Span() in x

    module MutateInRef2 =
        [<Struct>]
        type TestMut =

            val mutable x : int

            member this.XAddr = &this.x // not allowed, Struct members cannot return the address of fields of the struct by reference", not entirely clear why C# disallowed this

    module MutateInRef3 =
        [<Struct>]
        type TestMut =

            val mutable x : int

            member this.XAddr = &this // not allowed, Struct members cannot return the address of fields of the struct by reference", not entirely clear why C# disallowed this


    module MutateInRef4 =
        [<Struct>]
        type TestMut1 =

            val mutable x : int

        [<Struct>]
        type TestMut2 =

            val mutable x : TestMut1

            member this.XAddr = &this.x.x // not allowed, Struct members cannot return the address of fields of the struct by reference", not entirely clear why C# disallowed this

    module DisallowIsByRefLikeWithByRefField =
        [<IsByRefLike;Struct>]
        type Beef(x: byref<int>) =

            member __.X = &x

