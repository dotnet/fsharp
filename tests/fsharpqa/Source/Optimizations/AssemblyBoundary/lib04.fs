// #CodeGen #Optimizations #Assemblies 
namespace N

module L4 =
    type internal T1<'A> = { rf1 : 'A }
    type internal T3<'A> = | C3
    type internal T4<'A> = | C4 of 'A

    let internal x1 = { rf1 = 1 }       // type is is internal
    let f1 (x:obj) = unbox x = x1       // f1 rhs contains internals
    let internal x2 = { rf1 = 2 }       // rhs is internal
    let internal x3 = C3   : T3<_>      // rhs is internal
    let internal x4 = C4 4 : T4<_>      // rhs is internal
    let internal x5 = C3                // type is internal
    let f5 (x:obj) = unbox x = x5       // f5 rhs contains internals

    let k1() = typeof<T1<int>>
    let k2() = typeof<T3<int>>
    let k3() = typeof<T4<int>>
    let s1() = sizeof<T1<int>>
    let s2() = sizeof<T3<int>>
    let s3() = sizeof<T4<int>>
    
