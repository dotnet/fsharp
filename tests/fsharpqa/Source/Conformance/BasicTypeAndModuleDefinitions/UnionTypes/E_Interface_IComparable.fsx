// #Regression #Conformance #TypesAndModules #Unions 
// By default, Union types implement IComparable
// Q: Is this a bug? I've reported it to fscore for now.
//<Expects id="FS0067" span="(12,3-12,24)" status="warning">This type test or downcast will always hold</Expects>
//<Expects id="FS0016" span="(12,3-12,24)" status="error">The type 'I' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion</Expects>
#light

type I = A | B

let p = A
match p with
| :? System.IComparable -> ()
| _ -> ()
