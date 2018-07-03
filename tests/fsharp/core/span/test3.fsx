#r @"..\..\..\..\packages\System.Memory.4.5.0-rc1\lib\netstandard2.0\System.Memory.dll"
#r @"..\..\..\..\packages\NETStandard.Library.NETFramework.2.0.0-preview2-25405-01\build\net461\ref\netstandard.dll"

#nowarn "9"
#nowarn "51"

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

namespace Tests

open System

// NOT POST INFERENCE CHECKS
#if NEGATIVE
    module ForEachDeferenceSpanElementCheck =
        let doSomething (s: Span<int>) =
            for x in s do
                x <- 5
                ()

    module ForEachDereferenceSpanElementSeqCheck =
        let doSomething () =
            seq {
                let s = Span<int>.Empty
                for x in s do
                    x <- 5
                    yield x
            }

    module ForEachDereferenceSpanElementAsyncCheck =
        let doSomething () =
            async {
                let s = Span<int>.Empty
                for x in s do
                    x <- 5
                    ()
            }
#endif