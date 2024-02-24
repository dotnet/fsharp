module TestProgram

type T =
    static member val P = 1 with get,set


let x = T.P
T.P <- 42