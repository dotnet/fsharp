// #Regression #Conformance #TypesAndModules #Records 
// Verify error when type inference cannot pin down record type


type Foo = { FieldA : int; FieldB : int               }
type Bar = {               FieldB : int; FieldC : int }

let x = { FieldA = 1; FieldB = 2; FieldC = 3 }
