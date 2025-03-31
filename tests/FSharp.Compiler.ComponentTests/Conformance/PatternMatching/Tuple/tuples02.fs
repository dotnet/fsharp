module PM = 
    type PT = 
        abstract A : int
    let a = { new PT with member __.A = 1 }
    let b, c =
        { new PT with member __.A = 1 }
        , { new PT with member __.A = 1 }

module private PM2 = 
    type PT = 
        abstract A : int
    let a = { new PT with member __.A = 1 }
    let b, c =
        { new PT with member __.A = 1 }
        , { new PT with member __.A = 1 }