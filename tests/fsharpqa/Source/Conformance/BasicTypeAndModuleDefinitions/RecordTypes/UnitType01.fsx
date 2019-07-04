// #Conformance #TypesAndModules #Records 
// Field has type 'unit' (which is kind of special)
//<Expects status="success"></Expects>
#light

type T1 = { u : unit;}

let y = { u = () }


