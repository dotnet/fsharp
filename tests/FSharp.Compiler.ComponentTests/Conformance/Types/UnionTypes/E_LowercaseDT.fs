// #Regression #Conformance #TypesAndModules #Unions 
#light 

// FS1: 325, Case sensitivity reported by Eugene


type list = cons of int | Conss of int

// Compile error
exit 1
