// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
// This means it cannot start with a digit
//<Expects id="FS0010" span="(8,12-8,13)" status="error">Unexpected integer literal in union case</Expects>
#light

type a = | 1 of int         // err: can't use 
