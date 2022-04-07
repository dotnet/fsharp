// #NoMono #NoMT #CodeGen #EmittedIL   
#light

let TestFunction1() =
    printfn "Hello";
    printfn "World";
    3+4
    
let TestFunction3() =
    try 
       let x = TestFunction1()
       printfn "Hello";
    with _ -> 
       printfn "World"        

