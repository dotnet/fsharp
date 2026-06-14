// Regression test for https://github.com/dotnet/fsharp/issues/18125
module StructUnionLayout18125

[<Struct>]
type ABC = A | B | C

let verifySize () =
    // Struct DU has a compiler-generated _tag field; sizeof must not be 1.
    if sizeof<ABC> <> 4 then
        failwith $"Expected sizeof<ABC> = 4, got {sizeof<ABC>}"

verifySize ()
