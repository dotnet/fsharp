// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:2464
// closing square bracket following closing generic type angle bracket  is syntax error without whitespace
//<Expects status="success"></Expects>

#light

[<Measure>] type Kg
type R4 = { I : list<float<Kg>> }

let r4 = { I = [1.0<Kg>]}   // ok
let r5 = { I = [1.0<Kg>] }  // ok (used to be bug 2464)
