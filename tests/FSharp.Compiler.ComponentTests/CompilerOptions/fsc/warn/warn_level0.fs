// #NoMT #CompilerOptions 

[<NoComparison>]
/// Meh - Informational diagnostics "XML comment is not placed on a valid language element."
type A = int

let x = 10
// Normally a The result of this equality expression has type 'bool' and is implicitly discarded.
x = 20

let mul x y = x * y
// Normally a warning
[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else (fact (n - 1) (mul n acc)) + 23

// Normally a 'Main module of program is empty: nothing will happen when it is run' warning
()
