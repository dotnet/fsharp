// #NoMono #NoMT #CodeGen #EmittedIL 


let TestFunction1() =
    printfn "Hello";
    printfn "World";
    3+4
    
let TestFunction5() =
    let x = 
       let y = TestFunction1()
       printfn "Hello";
       y + y
    x + x

