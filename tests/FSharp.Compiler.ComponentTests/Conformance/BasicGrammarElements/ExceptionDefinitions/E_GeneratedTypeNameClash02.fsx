// #Regression #Conformance #TypesAndModules #Exceptions 
// An exception definition generates a type with name idException
// In this case we check and see what happens when such a type already exist


type EException = | E 

exception E of string
