// #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
#light

type R = { x:int; y:int }

let TestFunction17(inp) =
    let x = {x=3;y=inp}
    x,x
