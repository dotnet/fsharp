// #Regression #Conformance #TypesAndModules #Records 
// Verify error when inheriting from a record
// Regression of FSB 4662
//<Expects id="FS0887" span="(9,8-9,22)" status="error">is not an interface type</Expects>

type Record = {x:int; y:int}

type Sub<'t> = 
       inherit Record
