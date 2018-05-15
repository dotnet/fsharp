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

    open System
    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices


    [<IsReadOnly>]
    type ReadOnlyStruct(count1: int, count2: int) = 
        member x.Count1 = count1
        member x.Count2 = count2

    [<IsByRefLike>]
    type ByRefLikeStruct(count1: int, count2: int) = 
        member x.Count1 = count1
        member x.Count2 = count2

    module TypeRefTests = 
    
        let f1 (x: ByRefLikeStruct) = ()
        let f2 (x: ReadOnlyStruct) = x.Count1
        let f3 (x: ReadOnlyStruct) = x
        let f4 (x: Span<int>) = ()
        let f5 (x: Memory<int>) = ()
        let f6 (x: ReadOnlySpan<int>) = ()
        let f7 (x: ReadOnlyMemory<int>) = ()

    module Sample1 = 
        open FSharp.NativeInterop
        open System.Runtime.InteropServices
    // this method does not care what kind of memory it works on
        let SafeSum(bytes: Span<byte>) =
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                sum <- sum + int bytes.[i]
            sum

        let SafeSum2(bytes: Span<byte>) =
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                let byteAddr = &bytes.[i]
                sum <- sum + int byteAddr
            sum

        let SafeSum3(bytes: Memory<byte>) =
            let bytes = bytes.Span
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                let byteAddr = &bytes.[i]
                sum <- sum + int byteAddr
            sum

        let SafeSum4(bytes: Memory<byte>) =
            let bytes = bytes.Span
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                sum <- sum + int bytes.[i]
            sum

        let SafeSum5(bytes: ReadOnlySpan<byte>) =
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                sum <- sum + int bytes.[i]
            sum

        let SafeSum6(bytes: ReadOnlySpan<byte>) =
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                let byteAddr = &bytes.[i]
                sum <- sum + int byteAddr
            sum

        let SafeSum7(bytes: ReadOnlyMemory<byte>) =
            let bytes = bytes.Span
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                let byteAddr = &bytes.[i]
                sum <- sum + int byteAddr
            sum

        let SafeSum8(bytes: ReadOnlyMemory<byte>) =
            let bytes = bytes.Span
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                sum <- sum + int bytes.[i]
            sum


        let f6() = 
            // managed memory
            let arrayMemory = Array.zeroCreate<byte>(100)
            let arraySpan = new Span<byte>(arrayMemory);
            SafeSum(arraySpan)|> printfn "res = %d"

            // native memory
            let nativeMemory = Marshal.AllocHGlobal(100);
            let nativeSpan = new Span<byte>(nativeMemory.ToPointer(), 100);
            SafeSum(nativeSpan)|> printfn "res = %d"
            Marshal.FreeHGlobal(nativeMemory);

            // stack memory
            let mem = NativePtr.stackalloc<byte>(100)
            let mem2 = mem |> NativePtr.toVoidPtr
            let stackSpan = Span<byte>(mem2, 100)
            SafeSum(stackSpan) |> printfn "res = %d"
        f6()



    [<Struct>]
    type AllowedEvilStruct = 
        [<DefaultValue>]
        val mutable v : int
        member x.Replace(y:AllowedEvilStruct) = x <- y


//Since in parameters are read-only ref parameters, all ref parameter limitations apply.
//
//Cannot apply with an iterator method (I.e. method that has yield statements.)
//Cannot apply with an async method


    #if NOT_YET
    // Allow this:
    [<IsByRefLike>]
    type ByRefLikeStructWithByrefField(count1: byref<int>, count2: int) = 
        member x.Count1 = count1
        member x.Count2 = count2


    // Disallow this:
    [<IsReadOnly>]
    type ReadOnlyStruct  = 
        [<DefaultValue>]
        val mutable X : int
    #endif

