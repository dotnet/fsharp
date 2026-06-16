// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Basic syntax of anonymous union types
//<Expects status="success"></Expects>

let x: (int|string) = 42
let y: (int|string) = "hello"
let z: (int|float|string) = 3.14

let data: (string * (int|float|string)) list = 
    [ "name", "Joe"
      "age", 16
      "address", "here"
      "height", "5'10''" ]