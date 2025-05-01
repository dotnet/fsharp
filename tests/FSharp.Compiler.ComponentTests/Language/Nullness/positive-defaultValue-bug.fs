module EnabledPositive
open System

module NotNullConstraint =
    let f3 (x: 'T when 'T : not null) = 1
    let v5 = f3 (Some 1) // Expect to give a warning
    //let w5 = (Some 1) |> f3 // Expect to give a warning


[<Struct>]
type C4b =
    [<DefaultValue>]
    val mutable Whoops : FSharp.Collections.List<int> | null // expect no warning