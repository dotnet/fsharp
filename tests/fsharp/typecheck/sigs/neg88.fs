namespace neg88

open System
open System.Linq

type Cls<'t>() = 
    member this.M1<'a, 'b>(f : Func<'a, 'b>) : Cls<'b> = new Cls<'b>()
    member this.M2<'a, 'b>(f : System.Linq.Expressions.Expression<Func<'a, 'b>>) : Cls<'b> = new Cls<'b>()
    member this.M3<'a>(a : byref<'a>) = ()


module Test = 
    let inline map1  (data : ^a) (f: ^b -> ^c) : ^d = (^a : (member M1 : (^b -> ^c) -> ^d) (data,f))
    let inline map2  (data : ^a) (f: ^b -> ^c) : ^d = (^a : (member M2 : (^b -> ^c) -> ^d) (data,f))
    let inline map3 (data : ^a)(f: ^b ref)  : ^d = (^a : (member M3 : ^b ref -> ^d) (data,f))

    let c1 = Cls<int>()
    let _ = map1 c1 (fun x -> Math.Log(x))  // 1
    let _ = map2 c1 (fun x -> Math.Log(x))  // 2
    let r1 = ref 0
    map3 c1 r1  // 3
