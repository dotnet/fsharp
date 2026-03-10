// #NoMono #NoMT #CodeGen #EmittedIL   


type R = { x:int; y:int }

let TestFunction17(inp) =
    let x = {x=3;y=inp}
    x,x
