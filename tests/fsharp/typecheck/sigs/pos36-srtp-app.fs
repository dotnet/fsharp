module Pos36

open Lib

let check msg x y = if x = y then printfn "passed %s" msg else failwithf "failed '%s'" msg

let tbind ()  =
    check "vwknvewoiwvren1" (StaticMethods.M(C(3))) "M(C), x = 3"
    check "vwknvewoiwvren2" (StaticMethods.M(3L)) "M(int64), x = 3"

tbind()
