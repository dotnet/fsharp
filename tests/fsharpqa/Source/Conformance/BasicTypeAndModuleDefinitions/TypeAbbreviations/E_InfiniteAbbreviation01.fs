// #Regression #Conformance #TypesAndModules
// Verify error if creating a type abbreviation which results in an infinite type expression.
//<Expects id="FS0953" span="(6,6-6,7)" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>
//<Expects id="FS0001" span="(8,16-8,20)" status="error">This expression was expected to have type     'X'     but here has type     ''a option'</Expects>

type X = option<X>

let test : X = None

