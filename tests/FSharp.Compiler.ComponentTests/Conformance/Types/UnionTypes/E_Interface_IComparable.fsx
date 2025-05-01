// #Regression #Conformance #TypesAndModules #Unions 
// By default, Union types implement IComparable
// Q: Is this a bug? I've reported it to fscore for now.


#light

type I = A | B

let p = A
match p with
| :? System.IComparable -> ()
| _ -> ()
