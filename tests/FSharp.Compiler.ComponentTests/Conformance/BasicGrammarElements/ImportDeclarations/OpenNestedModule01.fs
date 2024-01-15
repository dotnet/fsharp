// #Conformance #DeclarationElements #Import 
#light

module ABC =

    type Foo1 =
        | A
        | B
        
    module ABC =
    
        type Foo2 =
            | A
            | B
            

open ABC
open ABC.ABC

// Somewhat ambiguous, but since 'Foo2' was defined last, it wins.
let x1 = A

let test (x : ABC.ABC.Foo2) =
    match x with
    | A -> ()
    | B -> exit 1

test x1

