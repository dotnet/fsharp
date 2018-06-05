module Pos31

let x1 : obj list  = [ ""  ]
let x2 : obj [] = [| ""  |]
let x3 : obj list  = [ yield ""  ]
let x4 : obj[]  = [| yield "" |]
let x5 : seq<obj>  = seq { yield "" }
let x6 : seq<obj>  = seq { yield "" :> obj  }

let g2 () : System.Reflection.MemberInfo[] = 
    [| yield (Unchecked.defaultof<System.Type> :> _) |]

let g3 () : System.Reflection.MemberInfo[] = 
    [| yield Unchecked.defaultof<System.Type> |]

let g4 () : System.Reflection.MemberInfo[] = 
    [| yield! g2()
       yield Unchecked.defaultof<System.Type> |]

let g5 xs : System.Reflection.MemberInfo[] = 
    [| for x in xs do yield x |]
    
