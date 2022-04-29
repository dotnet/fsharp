// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception labels must begin with an uppercase letter
//<Expects id="FS0053" span="(9,1-9,19)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
//<Expects id="FS0053" span="(10,1-10,19)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>

#light

exception ı of int     // err 
exception i of int     // err
