// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
// This means it cannot start with a string
//<Expects id="FS0010" span="(9,12-9,13)" status="error">Unexpected reserved keyword in union case</Expects>
#light

type T = | (* *) A
         | ``null

