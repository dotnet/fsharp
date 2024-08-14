// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Recursive definition: using list of...
//<Expects id="FS0953" span="(7,6-7,7)" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>
#light

type X = X list                 // cyclic

exit 1
