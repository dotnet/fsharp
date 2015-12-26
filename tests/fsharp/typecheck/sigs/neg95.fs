namespace Neg95

(* Optimizer should not throw InternalError when it
   encounters an inline method that references `base`,
   a compilation error should be reported instead.
   See issue: https://github.com/Microsoft/visualfsharp/issues/740 *)

type Base<'T>() =
    
    member __.s() = ()

type Class<'T>() =
    inherit Base<'T>()

    member inline __.Bar x = x
    member inline __.s2() = base.s()
    member inline this.s3() = this.Bar 1 |> ignore
