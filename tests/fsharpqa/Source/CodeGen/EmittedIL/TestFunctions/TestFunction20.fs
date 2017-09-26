// #NoMono #NoMT #CodeGen #EmittedIL   
#light

type D(x:int,y:int) = 
    let z = x + y
    let f a = x + a
    let w = f z + z
    member this.X = x
    member this.Y = y
    
let TestFunction20(inp) =
    let d1 = D(inp,inp)
    let d2 = D(inp,inp)
    printfn "d1 = %A, d2 = %A" d1 d2
