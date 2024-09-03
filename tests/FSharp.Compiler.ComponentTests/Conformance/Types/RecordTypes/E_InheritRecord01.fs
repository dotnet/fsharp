// #Regression #Conformance #TypesAndModules #Records 
// Verify error when inheriting from a record
// Regression of FSB 4662


type Record = {x:int; y:int}

type Sub<'t> = 
       inherit Record
