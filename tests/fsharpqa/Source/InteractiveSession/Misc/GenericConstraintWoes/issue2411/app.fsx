#r "lib.dll"

open System.Reflection

type FezImpl() =
  interface IFez
type BarImpl() =
  interface IBar
Fez.Do<FezImpl>("", (fun _ -> ()))
Fez.Do<BarImpl>("", (fun _ -> ())) // produced internal error: https://github.com/Microsoft/visualfsharp/issues/2411