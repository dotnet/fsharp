// #Regression #Conformance #TypesAndModules 
#light

// Verify error if creating a type abbreviation which results in an infinite type expression.


type Y = Y * Y

