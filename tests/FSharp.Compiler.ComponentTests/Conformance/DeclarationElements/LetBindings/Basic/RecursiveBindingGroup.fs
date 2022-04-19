// #Conformance #DeclarationElements #LetBindings #Regression
// Regression test for bug 6372
let foo() = 
    let rec a() = 1
    let b() = 2
    let b() = 3
    b()

let bar() = 
    let rec a() = 1
    let b() = 2
    let b() = 3
    b()


let rec x y = 
    baz "hello"
    y
and baz (x:string) = 
    let baz(x:obj) = 
      printfn "string"
    baz("hey")
