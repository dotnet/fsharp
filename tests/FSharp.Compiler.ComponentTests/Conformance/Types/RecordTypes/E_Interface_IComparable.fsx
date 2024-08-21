// #Regression #Conformance #TypesAndModules #Records 
// By default, record types implement IComparable
//<Expects id="FS0067" span="(14,3-14,24)" status="warning">This type test or downcast will always hold</Expects>
//<Expects id="FS0016" span="(14,3-14,24)" status="error">The type 'I' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion</Expects>
#light

[<Measure>] type Kg

type I = { A : float<Kg> ; B : decimal<Kg> }

let p = { A = 10.0<Kg>; B = 11.0M<Kg> }

match p with
| :? System.IComparable -> ()
| _ -> ()
