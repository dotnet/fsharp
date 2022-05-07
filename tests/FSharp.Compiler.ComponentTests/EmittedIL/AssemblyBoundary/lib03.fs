// #CodeGen #Optimizations #Assemblies 
namespace N

module L3 =
    type internal T1 = { rf1 : int }
    type internal T3 = | C3
    type internal T4 = | C4 of int    

    let internal x1 = { rf1 = 1 }       // type is is internal
    let f1 (x:obj) = unbox x = x1       // f1 rhs contains internals
    let internal x2 = { rf1 = 2 }       // rhs is internal
    let internal x3 = C3   : T3         // rhs is internal
    let internal x4 = C4 4 : T4         // rhs is internal
    let internal x5 = C3                // type is internal
    let f5 (x:obj) = unbox x = x5       // f5 rhs contains internals

    let mutable internal a6 = true
    let f6() = a6                       // f6 rhs contains internals
    
    exception internal E of int

    let k1() = typeof<T1>
    let k2() = typeof<T3>
    let k3() = typeof<T4>
    let s1() = sizeof<T1>
    let s2() = sizeof<T3>
    let s3() = sizeof<T4>
