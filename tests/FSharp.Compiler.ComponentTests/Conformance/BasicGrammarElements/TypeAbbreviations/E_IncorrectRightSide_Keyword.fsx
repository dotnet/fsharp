// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: quotation
//<Expects id="FS0010" span="(6,16-6,18)" status="error">Unexpected keyword 'of' in type definition</Expects>
#light

type BadType = of int

exit 1
