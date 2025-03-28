module private PM = 
    type PT = 
        abstract A : int
    let a = { new PT with member __.A = 1 }
    let b, c =
        { new PT with member __.A = 1 }
        , { new PT with member __.A = 1 }