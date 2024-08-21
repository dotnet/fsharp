// #Conformance #DeclarationElements #LetBindings 
#light

// Verify let bindings can function as patterns

// Wildcard
let _ = (1, '2')

// Tuple decomposition
let x, y = 1, '2'

// List decomposition
// Using 'OR' pattern

let _ =
    let _ :: x | x = [1 .. 2]
    if x <> [2] then failwith "Failed: 1"

let _ =
    let _ :: x | x = [] : int list
    if x <> [] then failwith "Failed: 2"

