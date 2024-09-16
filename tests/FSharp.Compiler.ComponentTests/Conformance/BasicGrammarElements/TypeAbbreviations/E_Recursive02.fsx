// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Recursive definition: using list of...

#light

type X = X list                 // cyclic

exit 1
