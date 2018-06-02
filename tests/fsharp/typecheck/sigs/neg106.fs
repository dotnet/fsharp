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

module FSharpDeclaredOutParamTest_Negaative1  = 
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

module FSharpDeclaredOutParamTest_Negative1  = 
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

