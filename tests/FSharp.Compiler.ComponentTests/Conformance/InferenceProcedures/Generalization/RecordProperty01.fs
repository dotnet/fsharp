// #Regression #Conformance #TypeInference 
// Regression for Dev11:41009, this class definition used to generate an error. Make sure we can use it too.
//<Expects status="error" span="(11,16-11,29)" id="FS3068">The function or member 'Empty' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member Rope\.Empty: Rope<'T>'\.</Expects>

type Rope<'T> =
    | Sub of 'T[] 
    static member Empty  = 
        Sub [| |]
    static member Create(lhs:Rope<'T>) =
        match 0 with        
        | 0 -> Rope<_>.Empty
        | n1 -> lhs

let x = Sub([|1|])
let y = Rope<int>.Empty

exit 1