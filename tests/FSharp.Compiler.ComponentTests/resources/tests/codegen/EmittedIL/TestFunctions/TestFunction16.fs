// #NoMono #NoMT #CodeGen #EmittedIL   
#light
              
type U = U of int * int

let TestFunction16(inp) =
    let x = U(inp,inp)
    x,x
