// #Conformance #ObjectOrientedTypes #Enums
// Regression test for https://github.com/dotnet/fsharp/issues/11785

module M =
    type E = A = 'A' | B = 'B'

[<AutoOpen>]
module Ext =
    type M.E with
        static member (|||) (a: M.E, b: M.E) = M.E.B

open M

[<EntryPoint>]
let main _ =
    let r = E.A ||| E.B
    if r <> E.B then
        failwith "expected the extension (|||) to be used"

    0
