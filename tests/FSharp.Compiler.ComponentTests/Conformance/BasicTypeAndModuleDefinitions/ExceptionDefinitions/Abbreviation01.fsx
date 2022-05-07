// #Conformance #TypesAndModules #Exceptions 
// Abbreviation - using short and long identifiers
//<Expects status="success"></Expects>


module M = 
    module N = 
       module O =
           exception E of string * int
           exception F = E                  // abbreviation

    module O = 
       exception E of string * int
       exception F = N.O.F                  // another abbreviation using a long identifier

