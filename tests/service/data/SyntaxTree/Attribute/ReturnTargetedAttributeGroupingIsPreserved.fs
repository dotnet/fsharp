module M

open System

[<AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)>]
type AAttribute() =
    inherit Attribute()

[<return: A>][<return: A>]
let f () = ()

[<return: A; return: A>]
let g () = ()
