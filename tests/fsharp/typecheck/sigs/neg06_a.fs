module Test

module NameClashesWithDefaultAugmentation =
    type T = | C of int * int
             | D of (int * int)

             // Expect error here:
             member x.IsC(a) = match a with
                               | C(_,_) -> true
                               | D(_) -> false
                               
             // Expect error here:
             static member IsD(b) = match b with
                                    | C(_,_) -> true
                                    | D(_) -> false

             // Expect no error here:
             member x.GetC1(a) = match a with
                                 | C(_,_) -> true
                                 | D(_) -> false
                               
             // Expect no error here:
             static member GetC2(b) = match b with
                                      | C(_,_) -> true
                                      | D(_) -> false

             // Expect no error here:
             static member GetC3(b) = match b with
                                      | C(_,_) -> true
                                      | D(_) -> false

             // Expect NO NO NO error here:
             member x.GetC(a) = match a with
                                 | C(_,_) -> true
                                 | D(_) -> false
                               
             // Expect NO NO NO error here:
             static member getD(b) = match b with
                                      | C(_,_) -> true
                                      | D(_) -> false

