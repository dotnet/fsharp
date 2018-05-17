module Pos31

let x1 : list<obj>  = [ yield "" ]
let x2 : seq<obj>  = seq { yield "" }
let x3 : seq<obj>  = seq { yield "" :> obj  }
