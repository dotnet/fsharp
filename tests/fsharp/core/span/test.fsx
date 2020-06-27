#load "refs.generated.fsx"

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
    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices
    open FSharp.NativeInterop

    [<AutoOpen>]
    module Helpers = 
        let failures = ref false
        let report_failure (s) = 
            stderr.WriteLine ("NO: " + s); 
            failures := true
        let test s b = if b then () else report_failure(s) 

        (* TEST SUITE FOR Int32 *)

        let out r (s:string) = r := !r @ [s]

        let check s actual expected = 
            if actual = expected then printfn "%s: OK" s
            else report_failure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)

        let check2 s expected actual = check s actual expected 


    [<IsReadOnly; Struct>]
    type ReadOnlyStruct(count1: int, count2: int) = 
        member x.Count1 = count1
        member x.Count2 = count2

    [<IsByRefLike; Struct>]
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
                let byte = bytes.[i]
                sum <- sum + int byte
            sum

        let SafeSum7(bytes: ReadOnlyMemory<byte>) =
            let bytes = bytes.Span
            let mutable sum = 0
            for i in 0 .. bytes.Length - 1 do 
                let byte = bytes.[i]
                sum <- sum + int byte
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
            let arraySpan = new Span<byte>(arrayMemory)
            for i in 0 .. 99 do arrayMemory.[i] <- byte i
            SafeSum(arraySpan)|> printfn "res = %d"

            // native memory
            let nativeMemory = Marshal.AllocHGlobal(100)
            let nativeSpan = new Span<byte>(nativeMemory.ToPointer(), 100)
            for i in 0 .. 99 do Marshal.WriteByte(nativeMemory, i, byte i)
            SafeSum(nativeSpan)|> printfn "res = %d"
            Marshal.FreeHGlobal(nativeMemory)

            // stack memory
            let mem = NativePtr.stackalloc<byte>(100)
            let mem2 = mem |> NativePtr.toVoidPtr
            for i in 0 .. 99 do NativePtr.set mem i (byte i)
            let stackSpan = Span<byte>(mem2, 100)
            SafeSum(stackSpan) |> printfn "res = %d"
        f6()



    [<Struct>]
    type AllowedEvilStruct = 
        [<DefaultValue>]
        val mutable v : int
        member x.Replace(y:AllowedEvilStruct) = x <- y

    module SubsumptionOnMember =
        type Doot() = class end

        [<Sealed>]
        type GiantDad() =

            let test (data: Span<byte>) (doot: Doot) =
                ()

            member __.Test(data: Span<byte>) =
                test data Unchecked.defaultof<Doot>

    module AssignToByrefReturn =
        type C() = 
            static let mutable v = System.DateTime.Now
            static member M() = &v

        let F1() = 
            C.M() <-  System.DateTime.Now        

    module AssignToSpanItem =
        let F2() = 
            let data = Span<byte>.Empty
            data.[0] <- 1uy    

    module AssignToSpanItemGeneric =
        let F2<'T>() = 
            let data = Span<'T>.Empty
            data.[0] <- Unchecked.defaultof<'T>    

    module CheckReturnOfSpan1 = 
        let test () =
            let s = Span<byte>.Empty
            s

    module CheckReturnOfSpan2 = 
                
        type Jopac() =

            member this.Create() =
                let mutable x = 1
                this.Create(&x)

            member __.Create(x: byref<int>) =
                Span<int>.Empty

    module CheckReturnOfSpan3 = 
        type Jopac_NotCompile_WhichIsMightBeIncorrect() =
            
            member __.Create(x: byref<int>) =
                Span<int>.Empty

            member this.Create() =
                let mutable x = 1
                let x = this.Create(&x)
                x

            member this.CreateAgain() =
                let mutable x = 1
                this.Create(&x)

    module ByRefSpanParam  = 
        type C() = 
            static member M(x: byref<Span<int>>) = x.[0] <- 5

        let Test() = 
            let mutable res = 9
            let mutable span = Span<int>(NativePtr.toVoidPtr &&res,1)
            let v =  C.M(&span)
            check "cwvereweoiwekl4" res 5

            let minfo = typeof<C>.GetMethod("M")
            check "cwnoreeker1" (minfo.GetParameters().[0].IsIn) false
            check "cwnoreeker2" (minfo.GetParameters().[0].IsOut) false
            check "cwnoreeker3" (minfo.ReturnParameter.IsIn) false
            check "cwnoreeker4" (minfo.ReturnParameter.IsOut) false

        Test()

    module SpanByRefReturn  = 
        type C() = 
            static member M(x: byref<Span<int>>) = x.[0] <- x.[0] + 1; &x

        let Test() = 
            let mutable res = 9
            let mutable span = Span<int>(NativePtr.toVoidPtr &&res,1)
            let v =  &C.M(&span)
            check "cwvereweoiwvw4" v.[0] 10

            let minfo = typeof<C>.GetMethod("M")
            check "cwnoreeker6d" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
            check "cwnoreekerr" (minfo.ReturnParameter.IsIn) false
            check "cwnoreekert" (minfo.ReturnParameter.IsOut) false

        Test()


    module SpanReturn  = 
        type C() = 
            static member M(x: byref<Span<int>>) = x.[0] <- x.[0] + 1; x

        let Test() = 
            let mutable res = 9
            let mutable span = Span<int>(NativePtr.toVoidPtr &&res,1)
            let v =  C.M(&span)
            check "cwvereweoiwvw4" v.[0] 10

            let minfo = typeof<C>.GetMethod("M")
            check "cwnoreeker6d" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
            check "cwnoreekerr" (minfo.ReturnParameter.IsIn) false
            check "cwnoreekert" (minfo.ReturnParameter.IsOut) false

        Test()

    module SpanSafetyTests0 = 

        type SpanLikeType = Span<int>

        let m1 (x: byref<Span<int>>) (y: Span<byte>) =
            // this is all valid, unconcerned with stack-referring stuff
            let local = SpanLikeType()
            x <- local
            x

    module SpanSafetyTests1 = 
        type SpanLikeType = Span<int>
        let m1 (x: byref<Span<int>>) (y: Span<byte>) =
            // this is all valid, unconcerned with stack-referring stuff
            let local = SpanLikeType()
            x <- local
            x
        let test1 (param1: byref<Span<int>>) (param2: Span<byte>) =
            let mutable stackReferringBecauseMutable1 = Span<byte>()

            //let stackReferringBecauseMutable1 = Span<byte>(NativePtr.toVoidPtr(&&x), 1)
            //let stackReferringBecauseMutable1 = Span<byte>(NativePtr.toVoidPtr(NativePtr.ofByRef(&&x)), 1)
            //let stackReferring1 = Span<byte>(NativePtr.toVoidPtr(NativePtr.stackalloc< byte>(100), 1)

            let mutable stackReferringBecauseMutable2 = Span<int>()

            // this is allowed
            stackReferringBecauseMutable2 <- m1 &stackReferringBecauseMutable2 stackReferringBecauseMutable1

#if NEGATIVE
            // this is NOT allowed
            stackReferringBecauseMutable2 <- m1 &param1 stackReferringBecauseMutable1

            // this is NOT allowed
            param1 <- m1 &stackReferringBecauseMutable2 stackReferringBecauseMutable1

            // this is NOT allowed
            param1 <- stackReferringBecauseMutable2.Slice(10)
#endif

            // this is allowed
            param1 <- Span<int>()

            // this is allowed
            stackReferringBecauseMutable2 <- param1

    module SpanSafetyTests2 = 
        let m2 (x: byref<Span<int>>) =
            // should compile
            &x

    module SpanSafetyTests3 = 
        type SpanLikeType = Span<int>
        let m1 (x: byref<Span<int>>) (y: Span<byte>) =
            // this is all valid, unconcerned with stack-referring stuff
            let local = SpanLikeType()
            x <- local
            x
        let m2 (x: byref<Span<int>>) =
            // should compile
            &x

        let test2 (param1: byref<Span<int>>) (param2: Span<byte>) =
            let mutable stackReferringBecauseMutable1 = Span<byte>()
            let mutable stackReferringBecauseMutable2 = Span<int>()

            let stackReferring3 = &(m2 &stackReferringBecauseMutable2)

            // this is allowed
            stackReferring3 <- m1 &stackReferringBecauseMutable2 stackReferringBecauseMutable1

            // this is allowed
            m2(&stackReferring3) <- stackReferringBecauseMutable2

#if NEGATIVE
            // this is NOT allowed
            m2(&param1) <- stackReferringBecauseMutable2

            // this is NOT allowed
            param1 <- stackReferring3

            // this is NOT allowed
            &stackReferring3
#endif

            // this is allowed
            //&param1 - uncomment to test return

    module SpanSafetyTests4 = 
        let test2 (param1: byref<Span<int>>) (param2: Span<byte>) =
            // this is allowed
            &param1 // uncomment to test return

#if NEGATIVE

    module CheckReturnOfSpan4 = 
        type Jopac_NotCompile_WhichIsCorrect() =

            member __.Create(x: byref<int>) =
                &x

            member this.Create() =
                let mutable x = 1
                let x = &this.Create(&x)
                &x

            member this.CreateAgain() =
                let mutable x = 1
                &this.Create(&x)

#endif

#if NOT_YET

#if NEGATIVE

    // Disallow this:
    [<IsReadOnly; Struct>]
    type DisallowedIsReadOnlyStruct  = 
        [<DefaultValue>]
        val mutable X : int

#endif

    // Allow this:
    [<IsReadOnly; IsByRefLike; Struct>]
    type ByRefLikeStructWithSpanField(count1: Span<int>, count2: int) = 
        member x.Count1 = count1
        member x.Count2 = count2

#endif
