// #Regression #Conformance #LexicalAnalysis #Operators 
// Regression test for FSHARP1.0:4994
// We could not define operator >.

let ( >. ) x y = x + y
let ( <. ) x y = x + y

let ( !>. ) x y = x + y
let ( !<. ) x y = x + y

let ( |>. ) x y = x + y
let ( |<. ) x y = x + y

let ( >>. ) x y = x + y
let ( <<. ) x y = x + y

let ( >.. ) x y = x + y
let ( |>.. ) x y = x + y

