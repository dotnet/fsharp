// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: quotation

#light

type BadType = <@ @> // -> int

exit 1
