// #Regression #Conformance #ObjectOrientedTypes #Enums
// Regression test for https://github.com/dotnet/fsharp/issues/11785
// Bitwise operators on an enum with a non-integral (char) underlying type have no runtime
// implementation and must be rejected at compile time.

type CharEnum = A = 'A' | B = 'B'

let bitwiseOr = CharEnum.A ||| CharEnum.B
let bitwiseAnd = CharEnum.A &&& CharEnum.B
let exclusiveOr = CharEnum.A ^^^ CharEnum.B
