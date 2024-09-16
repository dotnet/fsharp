// #Regression #Conformance #TypesAndModules #Exceptions 
// Verify error if you try to start an exception definition with a lower case letter
// Regression test for FSHARP1.0:2817


#light

exception lowerCaseException of string

exit 1
