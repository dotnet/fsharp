// #NoMono #NoMT #CodeGen #EmittedIL #Unions #NETFX20Only #NETFX40Only 
module CCtorDUWithMember01a
type C = 
    | A 
    | B
    member x.P = 1

let e2 = C.A
