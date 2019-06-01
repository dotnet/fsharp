// #Regression #Conformance #DataExpressions 
// Regression test for FSHARP1.0:4183
// Assert failed in code that take the address of a local of type unit
//<Expects status="success"></Expects>

let f () = 
    let dict = System.Collections.Generic.Dictionary<int,unit>()
    let mutable v = ()
    dict.TryGetValue(3,&v)

