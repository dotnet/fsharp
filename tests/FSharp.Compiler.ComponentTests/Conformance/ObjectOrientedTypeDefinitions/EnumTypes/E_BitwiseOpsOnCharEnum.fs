// #Regression #Conformance #ObjectOrientedTypes #Enums
// Regression test for https://github.com/dotnet/fsharp/issues/11785

type CharEnum = A = 'A' | B = 'B'

let bitwiseOr = CharEnum.A ||| CharEnum.B
let bitwiseAnd = CharEnum.A &&& CharEnum.B
let exclusiveOr = CharEnum.A ^^^ CharEnum.B
