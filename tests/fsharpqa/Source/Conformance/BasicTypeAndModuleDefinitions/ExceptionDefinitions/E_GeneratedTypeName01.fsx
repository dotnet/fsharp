// #Regression #Conformance #TypesAndModules #Exceptions 
// An exception definition DOES NOT generate a type with name idException (used to be until Beta1)
// This really means: "id" that appears in the definition with string "Exception" appended

//<Expects status="error" span="(10,16-10,41)" id="FS0039">The type 'Crazy@name.pException' is not defined</Expects>


exception ``Crazy@name.p`` of string

let e = typeof<``Crazy@name.pException``>   // err

exit 1
