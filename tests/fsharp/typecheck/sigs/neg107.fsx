#r @"..\..\..\..\packages\System.Memory.4.5.0-rc1\lib\netstandard2.0\System.Memory.dll"
#r @"..\..\..\..\packages\NETStandard.Library.NETFramework.2.0.0-preview2-25405-01\build\net461\ref\netstandard.dll"

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
