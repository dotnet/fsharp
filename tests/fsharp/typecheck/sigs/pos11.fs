// verify type inference can determine proper type for lambda args when there are multiple overloads containing lambdas
module LambdaOverloadTypeInference

open System.Linq

let xs = ["1"]
let qs = ["1"].AsQueryable()

let inline f1 (x : ^a) =
    ( ^a : (member Length : int) (x))

let f2 (x : 'a) =
    (2, x)

let f3 x i =
    i

let _ = xs.Sum(fun x -> x.Length)
let _ = xs.Select(fun x -> x.Length)
let _ = xs.Where(fun x -> x.Length % 3 = 0).Select(fun x -> x.Length)
let _ = xs.Select(fun x i -> (x.Length, i))
let _ = xs.Sum(fun x -> 64L)
let _ = xs.Sum(fun x -> 1.)
let _ = xs.Sum(fun x -> decimal 1.0)
let _ = xs.Select(fun x _ -> x.Length)
let _ = xs.Select(fun _ i -> i)
let _ = xs.Where(fun x i -> x.Length % 3 = 0)
let _ = xs.Where(fun x _ -> x.Length % 3 = 0)
let _ = xs.Select(f1)
let _ = xs.Sum(f1)
let _ = xs.Select(f2)
let _ = xs.Select(f3)
let _ = xs.SelectMany(fun x -> seq { yield x.Length })
// let _ = xs.SelectMany(fun x -> [ x.Length ])
let _ = xs.SelectMany(fun x i -> seq { yield (float i, x.Length) })
// let _ = xs.SelectMany((fun x -> seq { yield x.Length }), (fun x i -> (i, x.Length)))
// let _ = xs.SelectMany((fun x i -> seq { yield (i, x.Length) }), (fun x tup -> (tup, x.Length)))
let _ = xs.GroupBy(fun src -> src.Length)
//let _ = xs.GroupBy((fun src -> src.Length), (fun key grp -> grp.Count()) )
let _ = xs.Join([2], (fun x -> x.Length), (fun i -> i), (fun x i -> x.Length = i))

let _ = qs.Sum(fun x -> x.Length)
let _ = qs.Select(fun x -> x.Length)
let _ = qs.Where(fun x -> x.Length % 3 = 0).Select(fun x -> x.Length)
let _ = qs.Select(fun x i -> (x.Length, i))
let _ = qs.Sum(fun x -> 64L)
let _ = qs.Sum(fun x -> 1.)
let _ = qs.Sum(fun x -> decimal 1.0)
let _ = qs.Select(fun x _ -> x.Length)
let _ = qs.Select(fun _ i -> i)
let _ = qs.Where(fun x i -> x.Length % 3 = 0)
let _ = qs.Where(fun x _ -> x.Length % 3 = 0)
let _ = qs.Select(f1)
let _ = qs.Sum(f1)
let _ = qs.Select(f2)
let _ = qs.Select(f3)
let _ = qs.SelectMany(fun x -> seq { yield x.Length })
// let _ = qs.SelectMany(fun x -> [ x.Length ])
let _ = qs.SelectMany(fun x i -> seq { yield (float i, x.Length) })
// let _ = qs.SelectMany((fun x -> seq { yield x.Length }), (fun x i -> (i, x.Length)))
// let _ = qs.SelectMany((fun x i -> seq { yield (i, x.Length) }), (fun x tup -> (tup, x.Length)))
let _ = qs.GroupBy(fun src -> src.Length)
//let _ = qs.GroupBy((fun src -> src.Length), (fun key grp -> grp.Count()) )
let _ = qs.Join([2], (fun x -> x.Length), (fun i -> i), (fun x i -> x.Length = i))


type Repro<'a>(x : 'a) =
    member this.Meth( f1 : 'a -> 'b ) = ()
    member this.Meth( f1 : int) = ()
    member this.Meth( f1 : 'a) = ()
    member this.Meth( f1 : 'a, f2 : 'a) = ()
    member this.Meth( f1 : 'a -> 'b, f2 : 'a -> 'b) = ()
    member this.Meth( f1x : 'a -> 'b -> 'c) = ()
    member this.Meth( f1 : int, f2 : int) = ()

let z = Repro("foo")

z.Meth(fun a -> a.Length)
z.Meth(f1 = (fun a -> a.Length))

z.Meth(1)
z.Meth(f1 = 1)

z.Meth("str")
z.Meth(f1 = "str")

z.Meth("str", "str")
z.Meth(f1 = "str", f2 = "str")

z.Meth(1, 2)
z.Meth(f1 = 1, f2 = 2)

//z.Meth(fun a b -> a.Length)
z.Meth(f1x = (fun a b -> a.Length))

//z.Meth(fun a -> fun b -> a.Length)
z.Meth(f1x = (fun a -> fun b -> a.Length))

//z.Meth((fun a -> a.Length), (fun a2 -> a2.Length))
//z.Meth(f1 = (fun a -> a.Length), f2 = (fun a2 -> a2.Length))

let z2 = Repro(id)

z2.Meth(fun a -> ())
z2.Meth(f1 = (fun a -> ()))

z2.Meth(1)
z2.Meth(f1 = 1)

z2.Meth(fun () -> ())
z2.Meth(f1 = (fun () -> ()))

z2.Meth((fun () -> ()), (fun () -> ()))

z2.Meth(1, 2)
z2.Meth(f1 = 1, f2 = 2)

type TestTy() = class end

type Repro2<'a, 'b, 'c>(x : 'a, y : 'b, z : 'c) =
    member this.Meth( f1 : 'b -> 'c ) = ()
    member this.Meth( f1 : 'a) = ()

let z3 = Repro2((fun (s : string) -> s), "", 10)
z3.Meth(fun s  -> s.Length)
z3.Meth(fun s  -> s)

let z4 = Repro2((fun (s : float) -> s), "", 10)
//z4.Meth(fun s  -> s.Length)
z4.Meth(fun s  -> s)