// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT
// Compiler should turn 'foreach' loops over lists into 'while' loops
module ForEachOnList01

// Some variations to make sure optimizer can detect the foreach properly

let test1(lst: int list) =
     let mutable z = 0
     for x in lst do
         z <- z + x

let test2() =
     let mutable z = 0
     for x in [1;2;3] do
         z <- z + x

let test3() =
     let xs = [1;2;3]
     let mutable z = 0
     for x in xs do
         z <- z + x

let test4() =
     let mutable z = 0
     let xs = [1;2;3]
     for x in xs do
         z <- z + x

let test5() =
     let xs = [1;2;3]
     for x in xs do
         printfn "%A" x

// more complex enumerable expression
let test6() = 
    for i in (
                [1;2;3;4]
                |> List.map (fun x ->
                    x + 1)
                ) do
        printfn "%O" i
 
// multiline body 
let test7() = 
    for i in (
                [1;2;3;4]
                |> List.map (fun x ->
                    x + 1)
                ) do
        let tmp = i + 1
        printfn "%O" tmp