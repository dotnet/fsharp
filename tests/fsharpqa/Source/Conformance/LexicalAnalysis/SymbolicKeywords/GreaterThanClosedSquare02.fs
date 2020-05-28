// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:2464
// closing square bracket following closing generic type angle bracket  is syntax error without whitespace
//<Expects status="success"></Expects>

#light

let id x = x
let r6a = [id<int> ]
let r6b = [id<int>]

