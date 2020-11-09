// #Regression #Conformance #TypesAndModules 
#light

// Verify error if creating a type abbreviation which results in an infinite type expression.
//<Expects id="FS0953" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>

type Y = Y * Y

