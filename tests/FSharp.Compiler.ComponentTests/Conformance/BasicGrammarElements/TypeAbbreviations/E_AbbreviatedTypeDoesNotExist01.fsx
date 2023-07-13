// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Type tp abbreviate a type that does not exist
//<Expects id="FS0039" span="(7,10-7,11)" status="error">The namespace or module 'X' is not defined</Expects>
#light

type Z = X.Y.Z.W          // error
