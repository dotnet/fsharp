// #Regression #Conformance #TypesAndModules 
// Abbreviation: it is ok to reorder type variables
// Used to be regression test for FSHARP1.0:3738
// It is now regression test for FSHARP1.0:4786
//<Expects status="success"></Expects>
module M
type Reverse<'a,'b> = 'b -> 'a

let f (g : Reverse<int,char>) = g('a')

