// Expected: No warnings - valid in verbose syntax
module Module

type A = A;;type B = A;;module C = ();;exception D;;module E = C;;let f () = ();;open System;;module G = module H = E;;
