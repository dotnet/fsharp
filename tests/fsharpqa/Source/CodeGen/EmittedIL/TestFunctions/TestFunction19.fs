// #NoMono #NoMT #CodeGen #EmittedIL   
#light

type C(x:int,y:int) = 
    member this.X = x
    member this.Y = y
    
let TestFunction19(inp) =
    let c1 = C(inp,inp)
    let c2 = C(inp,inp)
    printfn "c1 = %A, c2 = %A" c1 c2
