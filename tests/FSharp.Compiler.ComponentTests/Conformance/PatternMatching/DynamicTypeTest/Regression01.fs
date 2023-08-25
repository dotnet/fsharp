// #Conformance #PatternMatching #TypeTests 
//<Expects status="success"></Expects>

type Foo () = class end

type Bar () =
    inherit Foo()

let instOfBox = box <| Bar()

let test1() =
    match true, instOfBox with
    | false, :? Bar -> 1 // Shouldn't match, true <> false
    |     _, :? Foo -> 2 // Should return 2
    | _             -> 3 // Unexplored
    
let test2() =
    match instOfBox, true with
    | :? Bar, false -> 1 // Shouldn't match, false <> true
    | :? Foo,     _ -> 2 // Should return 2
    | _             -> 3 // Unexplored


if test1() <> 2 then exit 1
if test2() <> 2 then exit 1

exit 0
