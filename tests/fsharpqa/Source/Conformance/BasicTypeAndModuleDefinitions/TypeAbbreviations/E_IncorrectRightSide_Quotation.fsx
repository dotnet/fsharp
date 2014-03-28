// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: quotation
//<Expects id="FS0010" span="(6,16-6,18)" status="error">Unexpected start of quotation in type definition$</Expects>
#light

type BadType = <@ @> // -> int

exit 1
