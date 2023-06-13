// #Regression #Conformance #TypesAndModules #Unions 
#light 

// FS1: 325, Case sensitivity reported by Eugene
//<Expects id="FS0053" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>

type list = cons of int | Conss of int

// Compile error
exit 1
