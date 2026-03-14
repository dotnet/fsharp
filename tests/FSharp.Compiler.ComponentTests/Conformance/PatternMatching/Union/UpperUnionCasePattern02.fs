
let test1 x =
    match x with
    | UndefinedCase -> 1

let test2 x =
    match x with UndefinedCase -> 1

let test3 = function | UndefinedCase -> 1

let test4 = function UndefinedCase -> 1

let test5 () =
    try failwith "test"
    with | UndefinedException -> 1
    
let test6 () =
    try failwith "test"  
    with UndefinedException -> 1

let test7 () =
    try failwith "test"  
    with UndefinedException as Foo -> 1

let test8 () =
    try failwith "test"  
    with | UndefinedException as Foo -> 1