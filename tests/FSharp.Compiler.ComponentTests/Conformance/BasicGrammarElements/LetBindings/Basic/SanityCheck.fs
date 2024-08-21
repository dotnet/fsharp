// #Conformance #DeclarationElements #LetBindings 
#light

let f x y = x * y

let result = f 1 10 + f 2 2
if result <> 14 then exit 1


let rec factorial x = if x <= 1 then 1 else x * factorial (x - 1)

let _ = factorial 2 * factorial 2
let (f1,f2,f3,f4) = factorial 1, factorial 2, factorial 3, factorial 4
if f4 <> 4 * 3 * 2 then failwith "Failed: 1"

do factorial 10 |> ignore; printfn "Do statements"; printfn "Seperated by semicolons"

let mutable x = 10
x <- f x x
if x <> 100 then failwith "Failed: 2"

// function which takes one arg of unit
// returning a function taking and returning an int
let somefuntion (arg : unit) : int -> int =
    let nestedfunction1 x y z =
        let nestedfunction2 x y =
            let nestedfunction3 x y z =
                x + y + z
            x + y
        x + (nestedfunction2 y z)
    let curriedNF3 = nestedfunction1 10
    let curriedNF3_2 = curriedNF3 20
    curriedNF3_2

if (somefuntion ()) 30 <> 60 then failwith "Failed: 3"

