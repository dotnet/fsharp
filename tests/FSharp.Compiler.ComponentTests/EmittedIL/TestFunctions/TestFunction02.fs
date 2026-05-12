// #NoMono #NoMT #CodeGen #EmittedIL 


let TestFunction1() =
    printfn "Hello";
    printfn "World";
    3+4
    
let TestFunction2() =
    let x = TestFunction1()
    printfn "Hello";
    printfn "World"    
    
