// #NoMono #NoMT #CodeGen #EmittedIL 


let TestFunction1() =
    printfn "Hello";
    printfn "World";
    3+4
    
let TestFunction4() =
    try 
       let x = TestFunction1()
       printfn "Hello";
    finally 
       printfn "World"
