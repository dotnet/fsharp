// #Conformance #TypesAndModules #Exception
// Make sure we properly detect field naming collisions



exception AAA of Data1 : int * string

exception BBB of A : int * A : string

exception CCC of A : int * A : string * A : int