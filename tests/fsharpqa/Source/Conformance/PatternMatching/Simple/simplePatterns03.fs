// #Conformance #PatternMatching 
#light

// Match pattern with 'named pattern'
type Foo() =
    static member StaticProperty = 42
    
let testNamedPattern x =
    match x with
    | x as someNewIdentifier when x = 42
        -> if (someNewIdentifier <> x) || (someNewIdentifier <> 42) then
                false
           else
                true
    | _ as someNewIdentifier when someNewIdentifier = 0 -> true
    | _ -> false

if testNamedPattern 42 <> true then exit 1
if testNamedPattern 0 <> true then exit 1
if testNamedPattern (-1) <> false then exit 1
    
exit 0
