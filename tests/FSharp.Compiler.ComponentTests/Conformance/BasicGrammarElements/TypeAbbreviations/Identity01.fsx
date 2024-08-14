// #Conformance #TypesAndModules 
// Type abbreviation
// A type that redefine itself -- not very useful, but ok
//<Expects status="success"></Expects>
#light

type Z = Z          // ok (useless, but ok)
