// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Regression test for FSHARP1.0:575 - Augmentations lead to unsoundnesses in the treatment of constraints.  Availalbe instances should be lexically scoped
//<Expects id="FS0430" span="(16,21-16,22)" status="error">The member '\(\+\)' is used in an invalid way\. A use of '\(\+\)' has been inferred prior to its definition.+</Expects>

type genmat = M of int
let x = Unchecked.defaultof<_> // generate an inference variable
let f() = x + x

f() = M 4 |> ignore

let v = M 4

type genmat with
    static member ( + ) (M x, M y) =  v

exit 1
