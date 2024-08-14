// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: hash
//<Expects id="FS0010" span="(8,1-8,5)" status="error">Incomplete structured construct at or before this point in type definition</Expects>


type BadType = #

exit 1
