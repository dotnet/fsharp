// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
//<Expects id="FS0053" span="(9,12-9,13)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
//<Expects id="FS0053" span="(10,12-10,13)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>

#light

type a = | ı of int     // err: Case labels for union types must be uppercase identifiers
         | i of int     // err: Case labels for union types must be uppercase identifiers
