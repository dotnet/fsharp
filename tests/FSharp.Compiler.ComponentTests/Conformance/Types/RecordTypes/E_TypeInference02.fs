// #Regression #Conformance #TypesAndModules #Records 
// Verify error when type inference cannot pin down record type
//<Expects id="FS0656" span="(8,9-8,47)" status="error">This record contains fields from inconsistent types</Expects>

type Foo = { FieldA : int; FieldB : int               }
type Bar = {               FieldB : int; FieldC : int }

let x = { FieldA = 1; FieldB = 2; FieldC = 3 }
