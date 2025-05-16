// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:1077
// closing brace following generic type bracket is syntax error without whitespace (lexed into symbolic token).
//<Expects status="success"></Expects>

#light

[<Measure>] type Kg
type Y = {mass_at_rest:float<Kg>}
let y = {mass_at_rest=10.0<Kg>}
