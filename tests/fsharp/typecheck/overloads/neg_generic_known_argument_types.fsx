open System.Runtime.InteropServices
  
type A<'zzz> =
  static member Foo(argA1: 'a, argB1: 'a -> 'b) : 'b = argB1 argA1
  static member Foo(argA1: 'a, argB1: 'a -> 'b, argC1: 'a -> 'b, argD: 'a -> 'b, [<Optional>] argZ1: 'zzz) : 'b = argB1 argA1
  static member Foo(argA2: 'a, argB2: 'a -> 'b, argC2: 'b -> 'c, argD: 'c -> 'd, [<Optional>] argZ2: 'zzz) : 'd = argD (argC2( argB2 argA2))
  
let inline f (aa: 'fa) (ab: 'fb) ac ad : 'e when (^fa) : (member X : 'b -> 'b) and (^b) : (member BBBB : unit -> unit) =
  let e : 'e = A<obj>.Foo(aa, ab, ac, argD = ad)
  e