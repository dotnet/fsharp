module Pos31

let x1 : obj list  = [ ""  ]
let x2 : obj [] = [| ""  |]
let x3 : obj list  = [ yield ""  ]
let x4 : obj[]  = [| yield "" |]
let x5 : seq<obj>  = seq { yield "" }
let x6 : seq<obj>  = seq { yield "" :> obj  }
