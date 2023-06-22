// #Conformance #TypesAndModules #Unions 
// Union Types
// union-cases   :=  [ ‘|’ ] union-case ‘|’ ... ‘|’ union-case     
//<Expects status="success"></Expects>
#light

// With missing |
type MissingPipe =   A 
                   | B
                   | NaN
                   
// With |
type WithPipe =  |    A 
                  | B
                   |     NaN
