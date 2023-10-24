module M

//#r @"..\..\..\..\packages\System.Memory.4.5.0-rc1\lib\netstandard2.0\System.Memory.dll"
//#r @"..\..\..\..\packages\NETStandard.Library.NETFramework\2.0.0-preview2-25405-01\build\net461\ref\netstandard.dll"

module CompareExchangeTests_Negative1 = 
    let x = 3
    let v =  System.Threading.Interlocked.CompareExchange(&x, 3, 4) //   No overloads match for method 'CompareExchange'. 'inref<int>' is not compatible with type 'byref<int>'

module CompareExchangeTests_Negative1b = 
    let Test() = 
        let x = 3
        let v =  System.Threading.Interlocked.CompareExchange(&x, 3, 4) //   No overloads match for method 'CompareExchange'. 'inref<int>' is not compatible with type 'byref<int>'
        ()

module CompareExchangeTests_Negative2 = 
    let v =  System.Threading.Interlocked.CompareExchange(&3, 3, 4) //  'inref<int>' is not compatible with type 'byref<int>'

module TryGetValueTests_Negative1 = 
    let Test() = 
        let d = dict [ (3,4) ]
        let res = 9
        let v =  d.TryGetValue(3, &res) //  'inref<int,>' is not compatible with type 'byref<int>'
        ()

module FSharpDeclaredOutParamTest_Negative1  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let Test() = 
        let  res = 9
        let v =  C.M(&res) //'inref<int>' is not compatible with type 'byref<int>'
        ()

module FSharpDeclaredOverloadedOutParamTest_Negative1  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 7
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 8
    let Test() = 
        let  res = 9
        let v =  C.M("a", &res) //'inref<int>' is not compatible with type 'byref<int>'
        let v2 =  C.M(3, &res)  //'inref<int>' is not compatible with type 'byref<int>'
        ()

module FSharpDeclaredOutParamTest_Negative1b  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let Test() = 
        let res = 9
        let v =  C.M(&res) // 'inref<int>' is not compatible with type 'byref<int>'
        ()

module TestOneArgumentInRefThenMutate_Negative1 =

    type C() = 
        static member M (x:inref<int>) = &x

    let test() = 
        let mutable r1 = 1
        let addr = &C.M (&r1) //  Expecting a 'byref<int,ByRefKinds.Out>' but given a 'inref<int>'. The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'
        addr <- addr + 1 // "The byref pointer is readonly, so this write is not permitted"

module EvilStruct_Negative1 =
    [<Struct>]
    type EvilStruct(s: int) = 
        member x.Replace(y:EvilStruct) = x <- y

module MutateInRef1 =
    [<Struct>]
    type TestMut =

        val mutable x : int

    let testIn (m: inref<TestMut>) =
        m.x <- 1

module MatrixOfTests = 
    [<Struct>]
    type S = 
        [<DefaultValue(true)>]
        val mutable X : int

    module WriteToInRef = 
        let f1 (x: inref<int>) = x <- 1 // not allowed

    module WriteToInRefStructInner = 
        let f1 (x: inref<S>) = x.X <- 1 //not allowed

    module InRefToByRef = 
        let f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = f1 &x    // not allowed 

    module InRefToByRefStructInner = 
        let f1 (x: byref<'T>) = 1
        let f2 (x: inref<S>) = f1 &x.X    // not allowed 

    module InRefToOutRef = 
        let f1 (x: outref<'T>) = 1
        let f2 (x: inref<'T>) = f1 &x     // not allowed   

    module InRefToOutRefStructInner = 
        let f1 (x: outref<'T>) = 1
        let f2 (x: inref<'T>) = f1 &x.X     // not allowed   

    module InRefToByRefClassMethod = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1 &x // not allowed   

    module InRefToByRefClassMethodStructInner = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1 &x.X // not allowed   

    module InRefToOutRefClassMethod =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed
        let f2 (x: inref<'T>) = C.f1 &x        

    module InRefToOutRefClassMethodStructInner =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed
        let f2 (x: inref<'T>) = C.f1 &x.X        

    module InRefToByRefClassMethod2 = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1(&x) // not allowed   

    module InRefToByRefClassMethod2StructInner = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1(&x.X) // not allowed   

    module InRefToOutRefClassMethod2 =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed
        let f2 (x: inref<'T>) = C.f1(&x)        

    module InRefToOutRefClassMethod2StructInner =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed
        let f2 (x: inref<'T>) = C.f1(&x.X)        

    module UseOfLibraryOnly =
        type C() = 
            static member f1 (x: byref<'T, 'U>) = 1 // not allowed - library only
            static member f2 (x: ByRefKinds.In) = 1 // not allowed - library only
            static member f2 (x: ByRefKinds.InOut) = 1 // not allowed - library only
            static member f2 (x: ByRefKinds.Out) = 1 // not allowed - library only
