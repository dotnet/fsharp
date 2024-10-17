// #Regression #Conformance #TypesAndModules 
// Abbreviation: it is not allowed to drop variable types
// Regression test for FSHARP1.0:3740


type Drop<'a,'b> = 'a * 'a

exit 1
