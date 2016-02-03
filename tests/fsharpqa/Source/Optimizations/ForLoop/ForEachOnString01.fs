// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT
// Compiler should turn 'foreach' loops over strings into 'for' loops
module ForEachOnString01

open System

let test1(str: string) =
     let mutable z = 0
     for x in str do
         z <- z + (int x)

let test2() =
     let mutable z = 0
     for x in "123" do
         z <- z + (int x)

let test3() =
     let xs = "123"
     let mutable z = 0
     for x in xs do
         z <- z + (int x)

let test4() =
     let mutable z = 0
     let xs = "123"
     for x in xs do
         z <- z + (int x)

let test5() =
     let xs = "123"
     for x in xs do
         printfn "%A" x

// test6, test7 makes sure the optimization triggers
// for System.String and String as well.
// They are the same type as string but internally string
// is an alias which potentially could make the optimizer
// miss one case or the other
let test6(str: System.String) =
     let mutable z = 0
     for x in str do
         z <- z + (int x)

let test7() =
     let xs : String = "123"
     let mutable z = 0
     for x in xs do
         z <- z + (int x)

// more complex enumerable expression
let test8() =
    for i in (
                "1234"
                |> String.map (fun x ->
                    char ((int x) + 1))
                ) do
        printfn "%O" i

// multiline body
let test9() =
    for i in (
                "1234"
                |> String.map (fun x ->
                    char ((int x) + 1))
                ) do
        let tmp = System.String.Format("{0} foo", i)
        printfn "%O" tmp
