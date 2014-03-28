// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
// This means it cannot start with a string
//<Expects id="FS0010" span="(9,12-9,15)" status="error">Unexpected string literal in union case</Expects>
#light

type T = | (* *) A  // ok
         | "A"      // err
