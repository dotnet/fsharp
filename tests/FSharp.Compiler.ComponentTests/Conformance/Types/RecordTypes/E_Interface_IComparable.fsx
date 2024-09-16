// #Regression #Conformance #TypesAndModules #Records 
// By default, record types implement IComparable


#light

[<Measure>] type Kg

type I = { A : float<Kg> ; B : decimal<Kg> }

let p = { A = 10.0<Kg>; B = 11.0M<Kg> }

match p with
| :? System.IComparable -> ()
| _ -> ()
