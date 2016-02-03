// test covering https://github.com/Microsoft/visualfsharp/issues/276

open FSharp.Data.UnitSystems.SI.UnitSymbols 

[<Struct>]
type S1 =
  val X: int

[<Struct>]
type S2 =
  val X: int<m> 

let f (x : 'T when 'T: unmanaged and 'T: struct) = printfn "%A" x

f (S1())
f (S2())  // previously had error FS0001: A generic construct requires that the type 'S2' is an unmanaged type
