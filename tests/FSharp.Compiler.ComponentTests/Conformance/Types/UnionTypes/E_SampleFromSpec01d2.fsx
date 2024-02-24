// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Sample from spec - using #light "off"
// The with/end tokens can be omitted when using the #light syntax option as long as the 
// type-defn-elements vertically aligns with the first ‘|’ in the  union-cases
// Regression test for FSHARP1.0:3707
//<Expects id="FS0010" span="(13,1-13,7)" status="error">.+'member'</Expects>
#light

type Message = 
| Result of string
| Request of int * string
member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm

