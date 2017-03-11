#r "lib.dll"

open System.Reflection

type FooImpl() =
  interface IFoo
type BarImpl() =
  interface IBar
Foo.Do<FooImpl>("", (fun _ -> ()))
Foo.Do<BarImpl>("", (fun _ -> ())) // produced internal error: https://github.com/Microsoft/visualfsharp/issues/2411