// #Regression #Conformance #TypesAndModules #Exceptions 
// An exception definition generates a type with name idException
// In this case we check and see what happens when such a type already exist
//<Expects id="FS0037" span="(8,11-8,12)" status="error">Duplicate definition of type, exception or module 'EException'</Expects>

type EException = | E 

exception E of string
